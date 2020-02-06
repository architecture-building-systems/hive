using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
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
            List<object> input_objects = new List<object>();
            if ((!DA.GetDataList(0, input_objects))) return;

            Dictionary<Type, int> typeDict = new Dictionary<Type, int>
            {
                {typeof(Hive.IO.EnergySystem.PV), 0}
            };


            //Rhino.RhinoApp.WriteLine(input_objects[0].RefEfficiencyElectric.ToString());

            List<Hive.IO.EnergySystem.PV> pv = new List<EnergySystem.PV>();

            //// can't cast GH_ObjectWrapper to Hive.IO.EnergySystem.PV...
            
            //foreach (object hive_input in input_objects)
            //{
            //    string data_type = hive_input.ToString();
            //    if (data_type == "Hive.IO.EnergySystem.PV")
            //    {
                    
            //        Hive.IO.EnergySystem.PV _pv = (Hive.IO.EnergySystem.PV)hive_input;
            //        pv.Add(_pv);
            //        Rhino.RhinoApp.WriteLine(_pv.RefEfficiencyElectric.ToString());
            //    }



            //        Rhino.RhinoApp.WriteLine(hive_input.GetType().ToString());
            //    //Rhino.RhinoApp.WriteLine(hive_input.RefEfficiencyElectric.ToString());
            //    switch (typeDict[hive_input.GetType()])
            //    {
            //        case 0:
            //            Rhino.RhinoApp.WriteLine("I am a PV object");
            //            break;
            //        default:
            //            break;
            //    }
            //}

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