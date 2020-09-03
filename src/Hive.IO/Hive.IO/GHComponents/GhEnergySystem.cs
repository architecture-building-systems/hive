using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;
using Hive.IO.Forms;
using Hive.IO.GhParametricInputs;
using Hive.IO.Properties;
using Rhino.Geometry;

//using Newtonsoft.Json;

namespace Hive.IO.GHComponents
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


        protected override Bitmap Icon => Resources.IO_Solartech;


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


            // To do Daren: coding
            // To do Amr: UI/UX design
            // when opening the form, the user can select certain surfaces and change them to ST, GC, PVT, or PV individually


            var conversionTech = new List<ConversionTech>();


            //if (meshList.Count > 0)
            //{
            //    foreach (Mesh mesh in meshList)
            //    {
            //        if (Form_SystemType == "pv") 
            //            conversionTechnologies.Add(new Photovoltaic(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name, Form_pv_eff));
            //        else if (Form_SystemType == "pvt") 
            //            conversionTechnologies.Add(new PVT(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name, Form_pv_eff, Form_thermal_eff));
            //        else if (Form_SystemType == "st") 
            //            conversionTechnologies.Add(new SolarThermal(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name, Form_thermal_eff));
            //        else 
            //            conversionTechnologies.Add(new GroundCollector(Form_pv_cost, Form_pv_co2, mesh, Form_pv_name)); // Form_thermal_eff, 
            //    }
            //}
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

            if (conversionTechProperties == null)
            {
                conversionTech.Add(new GasBoiler(100.0, 100.0, 10.0, 0.9));
            }
            else
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
            if (emitterProperties.Count == 0)
            {
                emitters.Add(new Radiator(100.0, 100.0, true, false, 65.0, 55.0));
                emitters[0].SetEmitterName("ConventionalRadiator");
                emitters.Add(new AirDiffuser(100.0, 100.0, false, true, 20.0, 25.0));
                emitters[1].SetEmitterName("AirDiffuser");
            }
            else
            {
                var counter = 0;
                foreach (var emProp in emitterProperties)
                {
                    if (emProp.IsRadiation)
                        emitters.Add(new Radiator(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.IsHeating,
                            emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature));
                    else
                        emitters.Add(new AirDiffuser(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.IsHeating,
                            emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature));
                    emitters[counter].SetEmitterName(emProp.Name);
                    counter++;
                }
            }

            // create a viewmodel for the form (note, it get's set to null when the input values change...)
            if (_viewModel == null) _viewModel = CreateViewModel(conversionTech, meshList, emitters);

            // the result might be changed by opening the form,
            // so we need to create it based on the view model (the result of the form)
            //var result = ComputeResults(_viewModel);


            // compute the results
            var result = new List<object>();
            result.AddRange(conversionTech);
            result.AddRange(emitters);
            DA.SetDataList(0, result);
        }

        /// <summary>
        ///     Read in the conversion technologies and the emitters and build up a view model for
        ///     the form.
        /// </summary>
        /// <param name="conversionTechnologies"></param>
        /// <param name="emitters"></param>
        /// <returns></returns>
        private EnergySystemsInputViewModel CreateViewModel(
            IEnumerable<ConversionTech> conversionTechnologies, IEnumerable<Mesh> meshes, IEnumerable<Emitter> emitters)
        {
            var vm = new EnergySystemsInputViewModel();
            vm.ConversionTechnologies.Clear();
            vm.Surfaces.Clear();

            var surfaceIndex = 0;
            foreach (var ct in conversionTechnologies)
            {
                var ctvm = new ConversionTechPropertiesViewModel {IsParametricDefined = true};
                switch (ct)
                {
                    case GasBoiler gasBoiler:
                        ctvm.Name = "Boiler(Gas)";
                        ctvm.SetProperties(gasBoiler);
                        break;

                    case Photovoltaic photovoltaic:
                        ctvm.Name = "Photovoltaic (PV)";
                        ctvm.SetProperties(photovoltaic);
                        var pvSurface = new SurfaceViewModel
                        {
                            Area = AreaMassProperties.Compute(photovoltaic.SurfaceGeometry).Area,
                            Name = $"srf{surfaceIndex++}"
                        };
                        pvSurface.Connection = ctvm;
                        vm.Surfaces.Add(pvSurface);
                        break;

                    case SolarThermal solarThermal:
                        ctvm.Name = "Solar Thermal (ST)";
                        ctvm.SetProperties(solarThermal);
                        var stSurface = new SurfaceViewModel
                        {
                            Area = AreaMassProperties.Compute(solarThermal.SurfaceGeometry).Area,
                            Name = $"srf{surfaceIndex++}"
                        };
                        stSurface.Connection = ctvm;
                        vm.Surfaces.Add(stSurface);
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

                vm.ConversionTechnologies.Add(ctvm);
            }

            foreach (var m in meshes)
            {
                var surface = new SurfaceViewModel
                {
                    Area = AreaMassProperties.Compute(m).Area,
                    Name = $"srf{surfaceIndex++}"
                };
                vm.Surfaces.Add(surface);
            }

            return vm;
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