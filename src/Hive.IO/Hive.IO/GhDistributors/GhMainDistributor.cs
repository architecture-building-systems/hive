using System;
using System.CodeDom;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;
using Hive.IO.Util;
using Rhino;
using System.Reflection;

namespace Hive.IO.GhDistributors
{
    public class GhMainDistributor : GH_Component
    {
        public GhMainDistributor()
          : base("Main Distributor Hive", "HiveDistributor",
              "The Distributor collects all Hive inputs from outside the the simulation core and outputs them individually according to their class type. " +
              "The function of this component is that the user can simply input all hive input objects into the same node without bothering about their formats.",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;


        /// <summary>
        /// Takes ALL Hive Input objects (e.g. Hive.IO.PV, Hive.IO.Building, etc.)
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Inputs", "HiveInputs", "Hive Input Objects (Building, Energy Systems, Environment)", GH_ParamAccess.list);
        }

        /// <summary>
        /// Output data that needs to be distributed within the simulation core to each respective simulation/calculation component
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Building", "Building", 
                "Hive Building from outside the simulation core, ready to be deployed into the core.", 
                GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive Environment", "Environment", 
                "Hive Environment from outside the simulation core, ready to be deployed into the core.", 
                GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive EnergySystems SurfaceBasedTech", 
                "SurfaceBasedTech", "Hive surface based technologies (Photovoltaic, Solar Thermal, hybrid PVT, solar ground collector) from outside the simulation core, ready to be deployed into the core.", 
                GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive EnergySystems ConversionTech", 
                "ConversionTech", "Hive conversion technologies (air source heat pump, chillers, combined heat and power, boiler, etc.) from outside the simulation core, ready to be deployed into the core.", 
                GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive EnergySystems Emitter", "Emitter", 
                "Hive emitters (floor heating, radiator, air diffuser, etc.) from outside the simulation core, ready to be deployed into the core.", 
                GH_ParamAccess.list); // should be a list, even for a single zone (different cooling and heating emitter, e.g.). FIXME: but how to know, which emitter goes to which simulation model?
        }


        /// <summary>
        /// Manages all the incoming Hive.IO objects, and splits it into required output data
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //read InformationalVersion attribute from AssemblyInfo.cs to display as message under the Main Hive Distributor
            Message = Misc.GetInformationalVersionAttribute();

            var inputObjects = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, inputObjects)) return;
            
            var srfBasedTech = new List<SurfaceBasedTech>();
            var conversionTech = new List<ConversionTech>();
            Building.Building building = null;
            Environment.Environment environment = null;
            var emitters = new List<Emitter>();

            foreach (GH_ObjectWrapper hiveInput in inputObjects)
            {
                switch (hiveInput.Value)
                {
                    case GH_Boolean bln:
                        if (bln.Value == false)
                        {
                            RhinoApp.WriteLine("Toggle Set to \"False\": Hive.Core not simulating...");
                            return;
                        }
                        break;
                }
            }


            foreach (GH_ObjectWrapper hiveInput in inputObjects)
            {
                switch (hiveInput.Value)
                {
                    case BuildingIntegratedPV valueBIPV:
                        srfBasedTech.Add(valueBIPV); break;
                    case Photovoltaic valuePV:                      
                        srfBasedTech.Add(valuePV); break;
                    case SolarThermal valueST:                      
                        srfBasedTech.Add(valueST); break;
                    case PVT valuePVT:                              
                        srfBasedTech.Add(valuePVT); break;
                    case GroundCollector valueGC:                   
                        srfBasedTech.Add(valueGC); break;
                    case AirSourceHeatPump valueASHP:               
                        conversionTech.Add(valueASHP); break;
                    case Chiller valueChiller:                       
                        conversionTech.Add(valueChiller); break;
                    case GasBoiler valueGasBoiler:                  
                        conversionTech.Add(valueGasBoiler); break;
                    case CombinedHeatPower valueCHP:                
                        conversionTech.Add(valueCHP); break;
                    case HeatCoolingExchanger valueHCE:             
                        conversionTech.Add(valueHCE); break;
                    case Building.Building valueBuilding:          
                        building = valueBuilding; break;
                    case Environment.Environment valueEnvironment:  
                        environment = valueEnvironment; break;
                    case Emitter valueEmitter:                      
                        emitters.Add(valueEmitter); break;
                }
            }

            //if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            //Rhino.RhinoApp.WriteLine("Surface Energy Systems read in: \n PV: {0}; ST: {1}; PVT: {2}", srfBasedTech.Count, st.Count, pvt.Count);

            DA.SetData(0, building);
            DA.SetData(1, environment); 
            DA.SetDataList(2, srfBasedTech);
            DA.SetDataList(3, conversionTech);
            DA.SetDataList(4, emitters);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_CoreDistributor;

        public override Guid ComponentGuid => new Guid("8757ee6f-03c4-4f5e-ac6d-db04b4d20297");
    }
}