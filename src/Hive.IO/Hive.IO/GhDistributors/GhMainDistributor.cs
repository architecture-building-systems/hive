﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhDistributors
{
    public class GhMainDistributor : GH_Component
    {
        public GhMainDistributor()
          : base("Main Distributor Hive", "HiveDistributor",
              "The Hive.IO.Distributor collects all Hive Inputs from outside the Mothercell (the simulation core) and outputs them individually according to their class type, ready for deployment.",
              "[hive]", "IO")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        /// <summary>
        /// Takes ALL Hive Input objects (e.g. Hive.IO.PV, Hive.IO.Building, etc.)
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Input Objects", "Hive Input Objects", "Hive Input Objects, all comes in here", GH_ParamAccess.list);
        }

        /// <summary>
        /// Output data that needs to be distributed within the mothercell to each respective simulation/calculation component
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Building", "Building", "Hive.IO.Building from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.Environment", "Environment", "Hive.IO.Environment from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.SurfaceBasedTech", "SurfaceBasedTech", "Hive.IO.EnergySystem. Photovoltaic; .SolarThermal; .PVT; .GroundCollector objects from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.ConversionTech", "ConversionTech", "Hive.IO.EnergySystem.ConversionTech; AirSourceHeatPump, Chillers, CHP, Boilers, etc.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.Emitter", "Emitter", "Hive.IO.EnergySystems.Emitter; Floor heating, radiator, etc.", GH_ParamAccess.list); // should be a list, even for a single zone (different cooling and heating emitter, e.g.). but how to know, which emitter goes to which simulation model?
        }


        /// <summary>
        /// Manages all the incoming Hive.IO objects, and splits it into required output data
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var inputObjects = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, inputObjects)) return;
            
            var srfBasedTech = new List<SurfaceBasedTech>();
            var conversionTech = new List<ConversionTech>();
            Building.Building building = null;
            Environment.Environment environment = null;
            var emitters = new List<Emitter>();

            foreach (GH_ObjectWrapper hiveInput in inputObjects)
            {
                if (hiveInput.Value is Photovoltaic)
                    srfBasedTech.Add(hiveInput.Value as Photovoltaic);
                else if (hiveInput.Value is SolarThermal)
                    srfBasedTech.Add(hiveInput.Value as SolarThermal);
                else if (hiveInput.Value is PVT)
                    srfBasedTech.Add(hiveInput.Value as PVT);
                else if (hiveInput.Value is GroundCollector)
                    srfBasedTech.Add(hiveInput.Value as GroundCollector);
                else if (hiveInput.Value is AirSourceHeatPump)
                    conversionTech.Add(hiveInput.Value as AirSourceHeatPump);
                else if (hiveInput.Value is Chiller)
                    conversionTech.Add(hiveInput.Value as Chiller);
                else if (hiveInput.Value is GasBoiler)
                    conversionTech.Add(hiveInput.Value as GasBoiler);
                else if (hiveInput.Value is CombinedHeatPower)
                    conversionTech.Add(hiveInput.Value as CombinedHeatPower);
                else if (hiveInput.Value is Building.Building)
                    building = hiveInput.Value as Building.Building;
                else if (hiveInput.Value is Environment.Environment)
                    environment = hiveInput.Value as Environment.Environment;
                else if (hiveInput.Value is Emitter)
                    emitters.Add(hiveInput.Value as Emitter);
            }

            //if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            //Rhino.RhinoApp.WriteLine("Surface Energy Systems read in: \n PV: {0}; ST: {1}; PVT: {2}", srfBasedTech.Count, st.Count, pvt.Count);

            DA.SetData(0, building);
            DA.SetData(1, environment); 
            DA.SetDataList(2, srfBasedTech);
            DA.SetDataList(3, conversionTech);
            DA.SetDataList(4, emitters);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8757ee6f-03c4-4f5e-ac6d-db04b4d20297"); }
        }
    }
}