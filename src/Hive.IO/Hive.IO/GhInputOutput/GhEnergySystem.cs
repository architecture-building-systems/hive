using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;
using Hive.IO.Forms;
using Hive.IO.GhParametricInputs;
using Rhino.Geometry;

//using Newtonsoft.Json;

namespace Hive.IO.GhInputOutput
{
    public class GhEnergySystem : GH_Component
    {
        private EnergySystemsInputViewModel _viewModel;

        public GhEnergySystem()
            : base("Input EnergySystems Hive", "HiveInputEnergySystems",
                "Hive.IO.EnergySystems input component (solar energy systems, other conversion technologies, emitters)." +
                "\nThe component opens a Form upon double click, where details of the solar energy system can be specified." +
                "\nPossible technologies are Photovoltaic (PV), Solar Thermal (ST), hybrid PVT, or Ground Collector (GC).",
                "[hive]", "IO")
        {
        }


        protected override Bitmap Icon => Properties.Resources.IO_Solartech;


        public override Guid ComponentGuid => new Guid("59389231-9a1b-4732-99dd-2bdda6b78bb8");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("SolarTechProperties", "SolarTechProperties",
                "List of SolarTechProperties describing solar technologies. Could also be just a mesh, in which case technology properties will be set via the form",
                GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddGenericParameter("ConversionTechProperties", "ConversionTechProperties",
                "ConversionTechProperties describing all other used conversion technologies (ASHP, boiler, CHP, etc",
                GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddGenericParameter("EmitterProperties", "EmitterProperties",
                "EmitterProperties describing emitter properties", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Energy Systems", "EnergySystems",
                "Building Energy Systems of type <Hive.IO.EnergySystems.>, such as Emitters, ConversionTech, SolarTech, etc.",
                GH_ParamAccess.list);
        }


        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Show UI", (sender, Exposure) => ShowForm(), null, true, false);
        }

        private void ShowForm()
        {
            var form = new EnergySystemsInput();
            form.DataContext = _viewModel;
            form.ShowDialog();
            ExpireSolution(true);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var solarObjects = new List<GH_ObjectWrapper>();
            DA.GetDataList(0, solarObjects);

            var meshList = new List<Mesh>();
            var solarTechProperties = new List<SolarTechProperties>();

            foreach (var ghObj in solarObjects)
                if (ghObj.Value is GH_Mesh ghMesh)
                    meshList.Add(ghMesh.Value);
                else if (ghObj.Value is Mesh mesh)
                    meshList.Add(mesh);
                else if (ghObj.Value is SolarTechProperties stp) solarTechProperties.Add(stp);

            ConversionTechProperties conversionTechProperties = null;
            DA.GetData(1, ref conversionTechProperties);

            var emitterProperties = new List<EmitterProperties>();
            DA.GetDataList(2, emitterProperties);

            var conversionTech = new List<ConversionTech>();

            if (solarTechProperties.Count > 0)
                foreach (var solarProperties in solarTechProperties)
                    if (solarProperties.Type == "PV")
                        conversionTech.Add(new Photovoltaic(solarProperties.InvestmentCost,
                            solarProperties.EmbodiedEmissions, solarProperties.MeshSurface, solarProperties.Technology,
                            solarProperties.ElectricEfficiency));
                    else if (solarProperties.Type == "PVT")
                        conversionTech.Add(new PVT(solarProperties.InvestmentCost, solarProperties.EmbodiedEmissions,
                            solarProperties.MeshSurface, solarProperties.Technology, solarProperties.ElectricEfficiency,
                            solarProperties.ThermalEfficiency));
                    else if (solarProperties.Type == "ST")
                        conversionTech.Add(new SolarThermal(solarProperties.InvestmentCost,
                            solarProperties.EmbodiedEmissions, solarProperties.MeshSurface, solarProperties.Technology,
                            solarProperties.ThermalEfficiency));
                    else
                        conversionTech.Add(new GroundCollector(solarProperties.InvestmentCost,
                            solarProperties.EmbodiedEmissions, solarProperties.MeshSurface,
                            solarProperties.Technology));

            if (conversionTechProperties != null)
            {
                if (conversionTechProperties.ASHPCapacity > 0.0)
                    conversionTech.Add(new AirSourceHeatPump(conversionTechProperties.ASHPCost,
                        conversionTechProperties.ASHPEmissions, conversionTechProperties.ASHPCapacity,
                        conversionTechProperties.ASHPEtaRef));
                if (conversionTechProperties.GasBoilerCapacity > 0.0)
                    conversionTech.Add(new GasBoiler(conversionTechProperties.GasBoilerCost,
                        conversionTechProperties.GasBoilerEmissions, conversionTechProperties.GasBoilerCapacity,
                        conversionTechProperties.GasBoilerEfficiency));
                if (conversionTechProperties.CHPCapacity > 0.0)
                    conversionTech.Add(new CombinedHeatPower(conversionTechProperties.CHPCost,
                        conversionTechProperties.CHPEmissions, conversionTechProperties.CHPCapacity,
                        conversionTechProperties.CHPHTP, conversionTechProperties.CHPEffElec));
                if (conversionTechProperties.ChillerCapacity > 0.0)
                    conversionTech.Add(new Chiller(conversionTechProperties.ChillerCost,
                        conversionTechProperties.ChillerEmissions, conversionTechProperties.ChillerCapacity,
                        conversionTechProperties.ChillerEtaRef));
                if (conversionTechProperties.HeatExchangerCapacity > 0.0)
                    conversionTech.Add(new HeatCoolingExchanger(conversionTechProperties.HeatExchangerCost,
                        conversionTechProperties.HeatExchangerEmissions, conversionTechProperties.HeatExchangerCapacity,
                        conversionTechProperties.HeatExchangerLosses));
                if (conversionTechProperties.CoolExchangerCapacity > 0.0)
                    conversionTech.Add(new HeatCoolingExchanger(conversionTechProperties.CoolExchangerCost,
                        conversionTechProperties.CoolExchangerEmissions, conversionTechProperties.CoolExchangerCapacity,
                        conversionTechProperties.CoolExchangerLosses, false, true));
            }

            var emitters = new List<Emitter>();
            foreach (var emProp in emitterProperties)
            {
                Emitter emitter;
                if (emProp.IsRadiation)
                {
                    emitter = new Radiator(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.IsHeating,
                        emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature);
                }
                else
                {
                    emitter = new AirDiffuser(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.IsHeating,
                        emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature);
                }
                emitter.SetEmitterName(emProp.Name);
                emitters.Add(emitter);
            }

            // create a viewmodel for the form (note, it get's set to null when the input values change...)
            CreateViewModel(conversionTech, meshList, emitters);

            // the result might be changed by opening the form,
            // so we need to create it based on the view model (the result of the form)
            //var result = ComputeResults(_viewModel);


            // compute the results
            var result = ReadViewModel();
            DA.SetDataList(0, result);
        }

        /// <summary>
        ///     Read in the conversion technologies and the emitters and build up a view model for
        ///     the form.
        /// </summary>
        /// <param name="conversionTechnologies"></param>
        /// <param name="emitters"></param>
        private void CreateViewModel(
            IEnumerable<ConversionTech> conversionTechnologies, IEnumerable<Mesh> meshes, IEnumerable<Emitter> emitters)
        {
            if (_viewModel == null)
            {
                // first time we run CreateViewModel, _viewModel is not set yet...
                _viewModel = new EnergySystemsInputViewModel();
                _viewModel.ConversionTechnologies.Clear();
                _viewModel.Surfaces.Clear();
                _viewModel.Emitters.Clear();
            }

            // remove parametrically defined conversion technologies and emitters - they'll be added below anyway
            var oldMeshes = _viewModel.Surfaces.Where(s => s.Connection == null || !s.Connection.IsParametricDefined).ToArray();
            var formDefinedConversionTech = _viewModel.ConversionTechnologies.Where(ct => !ct.IsParametricDefined).ToArray();
            var formDefinedEmitters = _viewModel.Emitters.Where(e => !e.IsParametricDefined).ToArray();
            _viewModel.ConversionTechnologies.Clear();
            _viewModel.Emitters.Clear();
            _viewModel.Surfaces.Clear();
            

            var surfaceIndex = 0;
            foreach (var ct in conversionTechnologies)
            {
                var ctvm = new ConversionTechPropertiesViewModel();
                switch (ct)
                {
                    case GasBoiler gasBoiler:
                        ctvm.Name = "Boiler (Gas)";
                        ctvm.SetProperties(gasBoiler);
                        break;

                    case Photovoltaic photovoltaic:
                        ctvm.Name = "Photovoltaic (PV)";
                        ctvm.SetProperties(photovoltaic);
                        var pvSurface = new SurfaceViewModel
                        {
                            Area = AreaMassProperties.Compute(photovoltaic.SurfaceGeometry).Area,
                            Name = $"srf{surfaceIndex++}",
                            Mesh = photovoltaic.SurfaceGeometry
                        };
                        pvSurface.Connection = ctvm;
                        _viewModel.Surfaces.Add(pvSurface);
                        break;

                    case SolarThermal solarThermal:
                        ctvm.Name = "Solar Thermal (ST)";
                        ctvm.SetProperties(solarThermal);
                        var stSurface = new SurfaceViewModel
                        {
                            Area = AreaMassProperties.Compute(solarThermal.SurfaceGeometry).Area,
                            Name = $"srf{surfaceIndex++}",
                            Mesh = solarThermal.SurfaceGeometry
                        };
                        stSurface.Connection = ctvm;
                        _viewModel.Surfaces.Add(stSurface);
                        break;

                    case AirSourceHeatPump ashp:
                        ctvm.Name = "ASHP (Electricity)";
                        ctvm.SetProperties(ashp);
                        break;

                    case CombinedHeatPower chp:
                        ctvm.Name = "CHP";
                        ctvm.SetProperties(chp);
                        break;

                    case Chiller chiller:
                        ctvm.Name = "Chiller (Electricity)";
                        ctvm.SetProperties(chiller);
                        break;

                    case HeatCoolingExchanger exchanger:
                        ctvm.Name = exchanger.IsHeating ? "Heat Exchanger" : "Cooling Exchanger";
                        ctvm.SetProperties(exchanger);
                        break;
                }
                _viewModel.ConversionTechnologies.Add(ctvm);
            }

            foreach (var ctvm in formDefinedConversionTech)
            {
                // add user (form) defined conversion technologies back to the list
                _viewModel.ConversionTechnologies.Add(ctvm);
            }

            // was the list of meshes changed since the last SolveInstance?
            foreach (var m in meshes)
            {
                if (oldMeshes.Any(svm => svm.Mesh == m))
                {
                    // mesh was input in last SolveInstance too, just keep it
                    var surface = oldMeshes.First(svm => svm.Mesh == m);
                    surface.Name = $"srf{surfaceIndex++}";
                    _viewModel.Surfaces.Add(surface);
                }
                else
                {
                    // mesh is a newly added mesh
                    var surface = new SurfaceViewModel
                    {
                        Area = AreaMassProperties.Compute(m).Area,
                        Name = $"srf{surfaceIndex++}",
                        Mesh = m
                    };
                    _viewModel.Surfaces.Add(surface);
                }
            }
            

            foreach (var emitter in emitters)
            {
                var epvm = new EmitterPropertiesViewModel();
                switch (emitter)
                {
                    case AirDiffuser airDiffuser:
                        epvm.Name = "Air diffuser";
                        epvm.SetProperties(airDiffuser);
                        break;
                    case Radiator radiator:
                        epvm.Name = "Radiator";
                        epvm.SetProperties(radiator);
                        break;
                }
                _viewModel.Emitters.Add(epvm);
            }

            foreach (var evm in formDefinedEmitters)
            {
                // add user (form) defined emitters back to the list
                _viewModel.Emitters.Add(evm);
            }
        }

        /// <summary>
        ///     Read out the results from the ViewModel. These include the originally (unmodified)
        ///     parametric inputs as created in SolveInstance.
        /// </summary>
        /// <returns></returns>
        private List<object> ReadViewModel()
        {
            var result = new List<object>();
            foreach (var ct in _viewModel.ConversionTechnologies)
            {
                if (ct.IsParametricDefined)
                {
                    result.Add(ct.ConversionTech);
                }
                else
                {
                    var specificCapitalCost = double.Parse(ct.SpecificCapitalCost);
                    var specificEmbodiedEmissions = double.Parse(ct.SpecificEmbodiedEmissions);
                    var efficiency = double.Parse(ct.Efficiency);
                    var capacity = double.Parse(ct.Capacity);
                    var heatToPowerRatio = double.Parse(ct.HeatToPowerRatio);
                    var distributionLosses = double.Parse(ct.DistributionLosses);
                    switch (ct.Name)
                    {
                        case "Photovoltaic (PV)":
                            foreach (var sm in ct.SelectedSurfaces)
                                result.Add(new Photovoltaic(specificCapitalCost, specificEmbodiedEmissions, sm.Mesh, "FIXME: PV",
                                    efficiency));
                            break;
                        case "Solar Thermal (ST)":
                            foreach (var sm in ct.SelectedSurfaces)
                                result.Add(new SolarThermal(specificCapitalCost, specificEmbodiedEmissions, sm.Mesh, "FIXME: ST",
                                    efficiency));
                            break;
                        case "Boiler (Gas)":
                            result.Add(new GasBoiler(specificCapitalCost, specificEmbodiedEmissions, capacity, efficiency));
                            break;
                        case "CHP":
                            result.Add(new CombinedHeatPower(specificCapitalCost, specificEmbodiedEmissions, capacity, heatToPowerRatio,
                                efficiency));
                            break;
                        case "Chiller (Electricity)":
                            result.Add(new Chiller(specificCapitalCost, specificEmbodiedEmissions, capacity, efficiency));
                            break;
                        case "ASHP (Electricity)":
                            result.Add(new AirSourceHeatPump(specificCapitalCost, specificEmbodiedEmissions, capacity, efficiency));
                            break;
                        case "Heat Exchanger":
                            result.Add(new HeatCoolingExchanger(specificCapitalCost, specificEmbodiedEmissions, capacity, distributionLosses));
                            break;
                        case "Cooling Exchanger":
                            result.Add(new HeatCoolingExchanger(specificCapitalCost, specificEmbodiedEmissions, capacity, distributionLosses,
                                false, true));
                            break;
                        default:
                            throw new Exception($"Don't know how to read {ct.Name}");
                    }
                }
            }

            foreach (var emitter in _viewModel.Emitters)
            {
                var specificInvestmentCost = double.Parse(emitter.SpecificCapitalCost);
                var specificEmbodiedEmissions = double.Parse(emitter.SpecificEmbodiedEmissions);
                var inletTemperature = double.Parse(emitter.SupplyTemperature);
                var returnTemperature = double.Parse(emitter.ReturnTemperature);
                
                switch (emitter.Name)
                {
                    case "Radiator":
                        result.Add(new Radiator(specificInvestmentCost, specificEmbodiedEmissions, emitter.IsHeating, emitter.IsCooling,
                            inletTemperature, returnTemperature));
                        break;
                    case "Air diffuser":
                        result.Add(new AirDiffuser(specificInvestmentCost, specificEmbodiedEmissions, emitter.IsHeating, emitter.IsCooling,
                            inletTemperature, returnTemperature));
                        break;
                }
            }
            return result;
        }

        /// <summary>
        ///     Input changed, delete the viewModel.
        /// </summary>
        protected override void ValuesChanged()
        {
            _viewModel = null;
            base.ValuesChanged();
        }


        #region GhEnergySystemAttributes

        public override void CreateAttributes()
        {
            m_attributes = new GhEnergySystemAttributes(this);
        }

        private class GhEnergySystemAttributes : GH_ComponentAttributes
        {
            public GhEnergySystemAttributes(IGH_Component component)
                : base(component)
            {
            }

            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                (Owner as GhEnergySystem)?.ShowForm();
                return GH_ObjectResponse.Handled;
            }
        }

        #endregion GhEnergySystemAttributes
    }
}