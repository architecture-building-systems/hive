using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHDistributor : GH_Component
    {
        public GHDistributor()
          : base("Distributor", "Distributor",
              "Distributor",
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
                }
            }

            Rhino.RhinoApp.WriteLine("pv: {0}; st: {1}; pvt: {2}", pv.Count, st.Count, pvt.Count);
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