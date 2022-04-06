using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;
using Hive.IO.Forms;
using Hive.IO.GhParametricInputs;
using Hive.IO.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rhino.Geometry;

//using Newtonsoft.Json;

namespace Hive.IO.GhInputOutput
{
    public class GhEnergySystem : GH_Component
    {
        private EnergySystemsInputViewModel _viewModel;

        public GhEnergySystem()
            : base("Input EnergySystems Hive", "HiveInputEnergySystems",
                "Hive Energy Systems input component and form to define solar energy systems, other conversion technologies, as well as emitters. The form opens with a double click onto the component.",
                "[hive]", "IO")
        {
        }


        protected override Bitmap Icon => Resources.IO_Energysytems;


        public override Guid ComponentGuid => new Guid("59389231-9a1b-4732-99dd-2bdda6b78bb8");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Mesh / Solar Technology Properties", "Mesh/SolarTechProperties",
                "(Optional input) Either mesh objects that represent photovoltaic panels, or SolarTechProperties from the Hive Parametric Input SolarTech component. If mesh objects are provided, they need to be assigned to a PV technology via the form (double click onto this component).",
                GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddGenericParameter("Conversion Technology Properties", "ConversionTechProperties",
                "(Optional input) ConversionTechProperties from the Hive Parametric Input ConversionTech component. If no input is provided, energy systems need to be defined via the form (double click onto this component).",
                GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddGenericParameter("Emitter Properties", "EmitterProperties",
                "(Optional input) EmitterProperties from the Hive Parametric Input Emitter component. If no input is provided, emitter systems need to be defined via the form (double click onto this component).",
                GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Energy Systems", "EnergySystems",
                "A List of Hive Energy Systems objects of type <Hive.IO.EnergySystems.>, such as Emitters, ConversionTech, SolarTech, etc.",
                GH_ParamAccess.list);
        }


        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Show UI", (sender, Exposure) => ShowForm(), null, true, false);
        }

        private void ShowForm()
        {
            
            var form = new EnergySystemsInputForm();
            form.ShowDialog(_viewModel);
            ExpireSolution(true);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
           
            if(_viewModel.FreeSurfaces.Count() != 0)
            {
                var w = GH_RuntimeMessageLevel.Warning;
                List<string> list = new List<string>();
                foreach (var srf in _viewModel.FreeSurfaces)
                    list.Add(srf.Name);
                string[] surfaces = list.ToArray();
                AddRuntimeMessage(w, String.Format("Attention! There are surface geometries in the Hizard that have not been assigned to any solar energy technology yet:\n- {0}", string.Join("\n- ", surfaces)));
            }
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
                            solarProperties.EmbodiedEmissions, solarProperties.Lifetime, solarProperties.MeshSurface, solarProperties.Technology,
                            solarProperties.ElectricEfficiency, solarProperties.PerformanceRatio, solarProperties.SurfaceTransmittance));
                    else if (solarProperties.Type == "BIPV")
                        conversionTech.Add(new BuildingIntegratedPV(solarProperties.InvestmentCost,
                            solarProperties.EmbodiedEmissions, solarProperties.Lifetime, solarProperties.MeshSurface, solarProperties.Technology,
                            solarProperties.ElectricEfficiency, solarProperties.PerformanceRatio, solarProperties.SurfaceTransmittance));
                    else if (solarProperties.Type == "PVT")
                        conversionTech.Add(new PVT(solarProperties.InvestmentCost, solarProperties.EmbodiedEmissions, solarProperties.Lifetime,
                            solarProperties.MeshSurface, solarProperties.Technology, solarProperties.ElectricEfficiency,
                            solarProperties.ThermalEfficiency, solarProperties.PerformanceRatio, solarProperties.SurfaceTransmittance));
                    else if (solarProperties.Type == "ST")
                        conversionTech.Add(new SolarThermal(solarProperties.InvestmentCost,
                            solarProperties.EmbodiedEmissions, solarProperties.Lifetime, solarProperties.MeshSurface, solarProperties.Technology,
                            solarProperties.ThermalEfficiency, solarProperties.PerformanceRatio, solarProperties.SurfaceTransmittance));
                    else
                        conversionTech.Add(new GroundCollector(solarProperties.InvestmentCost,
                            solarProperties.EmbodiedEmissions, solarProperties.Lifetime, solarProperties.MeshSurface,
                            solarProperties.Technology, solarProperties.PerformanceRatio, solarProperties.SurfaceTransmittance));

            if (conversionTechProperties != null)
            {
                if (conversionTechProperties.ASHPCapacity > 0.0)
                    conversionTech.Add(new AirSourceHeatPump(conversionTechProperties.ASHPCost,
                        conversionTechProperties.ASHPEmissions, conversionTechProperties.ASHPLifetime, conversionTechProperties.ASHPCapacity,
                        conversionTechProperties.ASHPEtaRef));
                if (conversionTechProperties.GasBoilerCapacity > 0.0)
                    conversionTech.Add(new GasBoiler(conversionTechProperties.GasBoilerCost,
                        conversionTechProperties.GasBoilerEmissions, conversionTechProperties.GasBoilerLifetime,
                        conversionTechProperties.GasBoilerCapacity,
                        conversionTechProperties.GasBoilerEfficiency));
                if (conversionTechProperties.CHPCapacity > 0.0)
                    conversionTech.Add(new CombinedHeatPower(conversionTechProperties.CHPCost,
                        conversionTechProperties.CHPEmissions, conversionTechProperties.CHPLifetime, conversionTechProperties.CHPCapacity,
                        conversionTechProperties.CHPHTP, conversionTechProperties.CHPEffElec));
                if (conversionTechProperties.ChillerCapacity > 0.0)
                    conversionTech.Add(new Chiller(conversionTechProperties.ChillerCost,
                        conversionTechProperties.ChillerEmissions, conversionTechProperties.ChillerLifetime, 
                        conversionTechProperties.ChillerCapacity, conversionTechProperties.ChillerEtaRef));
                if (conversionTechProperties.HeatExchangerCapacity > 0.0)
                    conversionTech.Add(new HeatCoolingExchanger(conversionTechProperties.HeatExchangerCost,
                        conversionTechProperties.HeatExchangerEmissions, conversionTechProperties.HeatExchangerLifetime, 
                        conversionTechProperties.HeatExchangerCapacity,
                        conversionTechProperties.HeatExchangerLosses));
                if (conversionTechProperties.CoolExchangerCapacity > 0.0)
                    conversionTech.Add(new HeatCoolingExchanger(conversionTechProperties.CoolExchangerCost,
                        conversionTechProperties.CoolExchangerEmissions, conversionTechProperties.CoolExchangerLifetime, 
                        conversionTechProperties.CoolExchangerCapacity,
                        conversionTechProperties.CoolExchangerLosses, false, true));
            }

            var emitters = new List<Emitter>();
            foreach (var emProp in emitterProperties)
            {
                Emitter emitter;
                if (emProp.IsRadiation)
                    emitter = new Radiator(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.Lifetime, emProp.IsHeating,
                        emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature);
                else
                    emitter = new AirDiffuser(emProp.InvestmentCost, emProp.EmbodiedEmissions, emProp.Lifetime, emProp.IsHeating,
                        emProp.IsCooling, emProp.SupplyTemperature, emProp.ReturnTemperature);
                emitter.SetEmitterName(emProp.Name);
                emitters.Add(emitter);
            }

            // create a viewmodel for the form (note, it get's set to null when the input values change...)
            CreateViewModel(conversionTech, meshList, emitters);

            // the result might be changed by opening the form,
            // so we need to create it based on the view model (the result of the form)
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

            // figure out which of the surfaces are from meshes... at the same time, if they came from Read(), hook them 
            // up to a mesh in meshes, in the same order
            var meshSurfaces = _viewModel.MeshSurfaces.ToArray();
            if (meshSurfaces.Length > 0 && meshSurfaces[0].Mesh == null)
                // last operation was Read(), we need to hook up the meshes properly
                for (var meshIndex = 0; meshIndex < meshSurfaces.Length; meshIndex++)
                {
                    var surface = meshSurfaces[meshIndex];
                    surface.Mesh = meshes.ElementAt(meshIndex);
                }

            // remove parametrically defined conversion technologies and emitters - they'll be added below anyway
            var formDefinedConversionTech =
                _viewModel.ConversionTechnologies.Where(ct => !ct.IsParametricDefined).ToArray();
            var formDefinedEmitters = _viewModel.Emitters.Where(e => !e.IsParametricDefined).ToArray();
            _viewModel.ConversionTechnologies.Clear();
            _viewModel.Emitters.Clear();
            _viewModel.Surfaces.Clear();


            var surfaceIndex = 0;

            // was the list of meshes changed since the last SolveInstance?
            foreach (var m in meshes)
                if (meshSurfaces.Any(svm => svm.Mesh == m))
                {
                    // mesh was input in last SolveInstance too, just keep it
                    var surface = meshSurfaces.First(svm => svm.Mesh == m);
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


            foreach (var ct in conversionTechnologies)
            {
                var ctvm = new ConversionTechPropertiesViewModel();
                switch (ct)
                {
                    case GasBoiler gasBoiler:
                        ctvm.Name = "Boiler (Gas)";
                        ctvm.SetProperties(gasBoiler);
                        break;

                    case BuildingIntegratedPV buildingIntegratedPV:
                        ctvm.Name = "Building Integrated Photovoltaic (BIPV)";
                        ctvm.SetProperties(buildingIntegratedPV);
                        var bipvSurface = new SurfaceViewModel
                        {
                            Area = AreaMassProperties.Compute(buildingIntegratedPV.SurfaceGeometry).Area,
                            Name = $"srf{surfaceIndex++}",
                            Mesh = buildingIntegratedPV.SurfaceGeometry
                        };
                        bipvSurface.Connection = ctvm;
                        _viewModel.Surfaces.Add(bipvSurface);
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
                // add user (form) defined conversion technologies back to the list
                _viewModel.ConversionTechnologies.Add(ctvm);


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
                // add user (form) defined emitters back to the list
                _viewModel.Emitters.Add(evm);
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
                if (ct.IsParametricDefined)
                {
                    result.Add(ct.ConversionTech);
                }
                else
                {
                    var specificCapitalCost = double.Parse(ct.SpecificCapitalCost);
                    var specificEmbodiedEmissions = double.Parse(ct.SpecificEmbodiedEmissions);
                    var lifetime = double.Parse(ct.Lifetime);
                    var efficiency = double.Parse(ct.Efficiency);
                    var performanceRatio = double.Parse(ct.PerformanceRatio);
                    var surfaceTransmittance = double.Parse(ct.SurfaceTransmittance); 
                    var capacity = double.Parse(ct.Capacity);
                    var heatToPowerRatio = double.Parse(ct.HeatToPowerRatio);
                    var distributionLosses = double.Parse(ct.DistributionLosses);
                    switch (ct.Name)
                    {
                        case "Photovoltaic (PV)":
                            ct.AvailableSurfaces = _viewModel.SurfacesForConversion(ct);
                            foreach (var sm in ct.SelectedSurfaces)
                                result.Add(new Photovoltaic(specificCapitalCost, specificEmbodiedEmissions, lifetime, sm.Mesh,
                                    "FIXME: PV",
                                    efficiency, performanceRatio, surfaceTransmittance));
                            break;
                        case "Building Integrated Photovoltaic (BIPV)":
                            ct.AvailableSurfaces = _viewModel.SurfacesForConversion(ct);
                            foreach (var sm in ct.SelectedSurfaces)
                                result.Add(new BuildingIntegratedPV(specificCapitalCost, specificEmbodiedEmissions, lifetime, sm.Mesh,
                                    "FIXME: BIPV",
                                    efficiency, performanceRatio, surfaceTransmittance));
                            break;
                        case "Solar Thermal (ST)":
                            ct.AvailableSurfaces = _viewModel.SurfacesForConversion(ct);
                            foreach (var sm in ct.SelectedSurfaces)
                                result.Add(new SolarThermal(specificCapitalCost, specificEmbodiedEmissions, lifetime, sm.Mesh,
                                    "FIXME: ST",
                                    efficiency, performanceRatio, surfaceTransmittance));
                            break;
                        case "Boiler (Gas)":
                            result.Add(new GasBoiler(specificCapitalCost, specificEmbodiedEmissions, lifetime, capacity,
                                efficiency));
                            break;
                        case "CHP":
                            result.Add(new CombinedHeatPower(specificCapitalCost, specificEmbodiedEmissions, lifetime, capacity,
                                heatToPowerRatio,
                                efficiency));
                            break;
                        case "Chiller (Electricity)":
                            result.Add(
                                new Chiller(specificCapitalCost, specificEmbodiedEmissions, lifetime, capacity, efficiency));
                            break;
                        case "ASHP (Electricity)":
                            result.Add(new AirSourceHeatPump(specificCapitalCost, specificEmbodiedEmissions, lifetime, capacity,
                                efficiency));
                            break;
                        case "Heat Exchanger":
                            result.Add(new HeatCoolingExchanger(specificCapitalCost, specificEmbodiedEmissions, lifetime,
                                capacity, distributionLosses));
                            break;
                        case "Cooling Exchanger":
                            result.Add(new HeatCoolingExchanger(specificCapitalCost, specificEmbodiedEmissions, lifetime,
                                capacity, distributionLosses,
                                false, true));
                            break;
                        default:
                            throw new Exception($"Don't know how to read {ct.Name}");
                    }
                }

            foreach (var emitter in _viewModel.Emitters)
            {
                var specificInvestmentCost = double.Parse(emitter.SpecificCapitalCost);
                var specificEmbodiedEmissions = double.Parse(emitter.SpecificEmbodiedEmissions);
                var lifetime = double.Parse(emitter.Lifetime);
                var inletTemperature = double.Parse(emitter.SupplyTemperature);
                var returnTemperature = double.Parse(emitter.ReturnTemperature);

                switch (emitter.Name)
                {
                    case "Radiator":
                        result.Add(new Radiator(specificInvestmentCost, specificEmbodiedEmissions, lifetime, emitter.IsHeating,
                            emitter.IsCooling,
                            inletTemperature, returnTemperature));
                        break;
                    case "Air diffuser":
                        result.Add(new AirDiffuser(specificInvestmentCost, specificEmbodiedEmissions, lifetime, emitter.IsHeating,
                            emitter.IsCooling,
                            inletTemperature, returnTemperature));
                        break;
                }
            }

            return result;
        }

        #region reading / writing state to the document

        private static readonly ITraceWriter TraceWriter = new MemoryTraceWriter();

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.All,
            TraceWriter = TraceWriter
        };

        /// <summary>
        ///     When writing state to the document, we have to take special care of the
        ///     following properties of the EnergySystemsInputViewModel:
        ///     - some of the conversion technologies and emitters are parametrically defined. we don't want to save these, as
        ///     they can't be changed by the form anyway...
        ///     - meshes. how to handle meshes? note, also, that they end up as surfaces with names.
        ///     I think, instead of saving a whole EnergySystemsInputViewModel, we're going to save:
        ///     - a list of form-defined ConversionTechPropertiesViewModel
        ///     - a list of form-defined emitters
        ///     - a list of surfaces, with .Mesh set to null but the name showing the index in the mesh list.
        ///     CreateViewModel and ReadViewModel need to be aware of this.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public override bool Write(GH_IWriter writer)
        {
            if (_viewModel == null)
                // CreateViewModel has never been called - no need to write anything, as user has never made any changes
                return base.Write(writer);

            // collect the data we want to store
            var meshes = _viewModel.MeshSurfaces.ToArray();
            var formDefinedConversionTech =
                _viewModel.ConversionTechnologies.Where(ct => !ct.IsParametricDefined).ToArray();
            var formDefinedEmitters = _viewModel.Emitters.Where(e => !e.IsParametricDefined).ToArray();


            // write out data - but don't write out the mesh objects themselves... (this is handled by JsonConvert - we use the OptIn 
            // method for serializing properties and just leave out the mesh objects)
            writer.SetString("meshes",
                JsonConvert.SerializeObject(meshes, Formatting.Indented, JsonSerializerSettings));
            writer.SetString("conversions",
                JsonConvert.SerializeObject(formDefinedConversionTech, Formatting.Indented, JsonSerializerSettings));
            writer.SetString("emitters",
                JsonConvert.SerializeObject(formDefinedEmitters, Formatting.Indented, JsonSerializerSettings));

            // write out connection between meshes and conversion tech - needs to be re-assembled later in Read()
            var meshConnections = new Dictionary<int, int>();
            for (var meshIndex = 0; meshIndex < meshes.Length; meshIndex++)
            {
                var mesh = meshes[meshIndex];
                if (mesh.Connection != null)
                {
                    // NOTE: mesh.Connection is guaranteed to be connected to an object in formDefinedConversionTech
                    var ctIndex = Array.IndexOf(formDefinedConversionTech, mesh.Connection);
                    meshConnections[meshIndex] = ctIndex;
                }
            }

            writer.SetString("meshConnections",
                JsonConvert.SerializeObject(meshConnections, Formatting.Indented, JsonSerializerSettings));

            return base.Write(writer);
        }

        /// <summary>
        ///     Read in the form-defined conversion technologies and emitters.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override bool Read(GH_IReader reader)
        {
            // the _viewModel is for null, so let's create a new one - note that it will
            // be updated in CreateViewModel - we're just going to set up some stuff to be ready...
            _viewModel = new EnergySystemsInputViewModel();
            _viewModel.ConversionTechnologies.Clear();
            _viewModel.Surfaces.Clear();
            _viewModel.Emitters.Clear();

            // read in the form-defined stuff
            try
            {
                var meshesJson = reader.GetString("meshes");
                var meshes = JsonConvert.DeserializeObject<SurfaceViewModel[]>(meshesJson, JsonSerializerSettings);

                var conversionsJson = reader.GetString("conversions");
                var conversions =
                    JsonConvert.DeserializeObject<ConversionTechPropertiesViewModel[]>(conversionsJson,
                        JsonSerializerSettings);

                var emittersJson = reader.GetString("emitters");
                var emitters =
                    JsonConvert.DeserializeObject<EmitterPropertiesViewModel[]>(emittersJson, JsonSerializerSettings);

                var meshConnectionsJson = reader.GetString("meshConnections");
                var meshConnections =
                    JsonConvert.DeserializeObject<Dictionary<int, int>>(meshConnectionsJson, JsonSerializerSettings);

                // connect the meshes to the conversion technologies
                foreach (var meshIndex in meshConnections.Keys)
                    meshes[meshIndex].Connection = conversions[meshConnections[meshIndex]];

                // add everything to the _viewModel
                foreach (var mesh in meshes) _viewModel.Surfaces.Add(mesh);

                foreach (var conversion in conversions) _viewModel.ConversionTechnologies.Add(conversion);

                foreach (var emitter in emitters) _viewModel.Emitters.Add(emitter);
            }
            catch (Exception ex)
            {
                // let's not worry too much about not being able to read the state...
                Message = $"Failed to read state from Document: {ex.Message}";
            }

            return base.Read(reader);
        }

        #endregion reading / writing state to the document


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