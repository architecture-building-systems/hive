using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO
{
    public class GHResults : GH_Component
    {

        public GHResults()
          : base("GHResults", "GHResults",
              "GHResults",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Hive 0.1 supports only one zone
            // TO DO: list of lists? each zone has demand and you could have multiple zones here

            pManager.AddNumberParameter("ClgMonthly", "ClgMonthly", "ClgMonthly", GH_ParamAccess.list);
            pManager.AddNumberParameter("HtgMonthly", "HtgMonthly", "HtgMonthly", GH_ParamAccess.list);
            pManager.AddNumberParameter("ElecMonthly", "ElecMonthly", "ElecMonthly", GH_ParamAccess.list);

            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ResultsObj", "ResultsObj", "ResultsObj", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // also sub-GHResults components, like distributor? Or just one massive Results reader?

            // input: all kinds of results from the Core 

            // output: jsons for Darens Visualizer, and GHREsults object for an updated Visualizer component


            List<double> clgMonthly = new List<double>();
            List<double> htgMonthly = new List<double>();
            List<double> elecMonthly = new List<double>();
            DA.GetDataList(0, clgMonthly);
            DA.GetDataList(1, htgMonthly);
            DA.GetDataList(2, elecMonthly);

            Results results = new Results();
            results.SetTotalDemandMonthly(clgMonthly.ToArray(), htgMonthly.ToArray(), elecMonthly.ToArray());

            DA.SetData(0, results);
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
            get { return new Guid("23cb5778-5a53-4dcc-ba2c-56d51c9c06b3"); }
        }
    }
}