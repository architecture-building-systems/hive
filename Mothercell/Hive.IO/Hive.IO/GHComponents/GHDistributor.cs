using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHDistributor : GH_Component
    {
        public GHDistributor()
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
            pManager.AddGenericParameter("Hive.IO.EnergySystem.PV", "HiveIOEnSysPV", "Hive.IO.EnergySystem.PV from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.ST", "HiveIOEnSysST", "Hive.IO.EnergySystem.ST from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystem.PVT", "HiveIOEnSysPVT", "Hive.IO.EnergySystem.PVT from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.Environment", "HiveIOEnv", "Hive.IO.Environment from outside the Mothercell, ready to be deployed into the core.", GH_ParamAccess.item);
        }


        /// <summary>
        /// Manages all the incoming Hive.IO objects, and splits it into required output data
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_ObjectWrapper> input_objects = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, input_objects)) return;
            
            List<EnergySystem.PV> pv = new List<EnergySystem.PV>();
            List<EnergySystem.PVT> pvt = new List<EnergySystem.PVT>();
            List<EnergySystem.ST> st = new List<EnergySystem.ST>();
            Building building = null;
            Environment environment = null;

            foreach (GH_ObjectWrapper hive_input in input_objects)
            {
                switch (hive_input.Value.ToString())
                {
                    case "Hive.IO.EnergySystem.PV":
                        pv.Add(hive_input.Value as EnergySystem.PV);
                        break;
                    case "Hive.IO.EnergySystem.ST":
                        st.Add(hive_input.Value as EnergySystem.ST);
                        break;
                    case "Hive.IO.EnergySystem.PVT":
                        pvt.Add(hive_input.Value as EnergySystem.PVT);
                        break;
                    case "Hive.IO.Building":
                        building = hive_input.Value as Building;
                        break;
                    case "Hive.IO.Environment":
                        environment = hive_input.Value as Environment;
                        break;
                }
            }

            if (building != null) Rhino.RhinoApp.WriteLine("Building '{0}' read successfully", building.Type.ToString());
            Rhino.RhinoApp.WriteLine("Surface Energy Systems read in: \n PV: {0}; ST: {1}; PVT: {2}", pv.Count, st.Count, pvt.Count);

            DA.SetData(0, building);
            DA.SetDataList(1, pv);
            DA.SetDataList(2, st);
            DA.SetDataList(3, pvt);
            DA.SetData(4, environment);
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