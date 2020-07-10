using System;
using System.CodeDom;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;

namespace Hive.IO.GHComponents
{
    public class GhDistributor : GH_Component
    {
        public GhDistributor()
          : base("Hive.IO.Distributor", "HiveIODistr",
              "The Hive.IO.Distributor collects all Hive Inputs from outside the Mothercell (the simulation core) and outputs them individually according to their class type, ready for deployment.",
              "[hive]", "Mothercell")
        {
        }

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
            pManager.AddGenericParameter("Hive.IO.Building", "HiveIOBldg", "Hive.IO.Building from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.SurfaceBased", "HiveIOEnSysSrf", "Hive.IO.EnergySystem. Photovoltaic; .SolarThermal; .PVT; .GroundCollector objects from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.Environment", "HiveIOEnv", "Hive.IO.Environment from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.item);
        }


        /// <summary>
        /// Manages all the incoming Hive.IO objects, and splits it into required output data
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_ObjectWrapper> inputObjects = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, inputObjects)) return;
            
            List<SurfaceBasedTech> srfBasedTech = new List<SurfaceBasedTech>();
            Building building = null;
            Environment environment = null;

            foreach (GH_ObjectWrapper hiveInput in inputObjects)
            {
                switch (hiveInput.Value.ToString())
                {
                    case  "Hive.IO.EnergySystems.Photovoltaic":
                        srfBasedTech.Add(hiveInput.Value as Photovoltaic);
                        break;
                    case "Hive.IO.EnergySystems.SolarThermal":
                        srfBasedTech.Add(hiveInput.Value as SolarThermal);
                        break;
                    case "Hive.IO.EnergySystems.PVT":
                        srfBasedTech.Add(hiveInput.Value as PVT);
                        break;
                    case "Hive.IO.EnergySystems.GroundCollector":
                        srfBasedTech.Add(hiveInput.Value as GroundCollector);
                        break;
                    case "Hive.IO.Building":
                        building = hiveInput.Value as Building;
                        break;
                    case "Hive.IO.Environment":
                        environment = hiveInput.Value as Environment;
                        break;
                }
            }

            //if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            //Rhino.RhinoApp.WriteLine("Surface Energy Systems read in: \n PV: {0}; ST: {1}; PVT: {2}", srfBasedTech.Count, st.Count, pvt.Count);

            DA.SetData(0, building);
            DA.SetDataList(1, srfBasedTech);
            DA.SetData(2, environment);
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