using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhDistributors
{
    public class GhDistResults : GH_Component
    {
        public GhDistResults()
          : base("Distributor Results Hive", "HiveDistResults",
              "Distributor for Hive results",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Results", "Results", "Hive Results of type <Hive.IO.Results.Results>", GH_ParamAccess.item);
            pManager.AddGenericParameter("ResultType", "ResultType", "Which result to extract. Choose from a dropdwon menu.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "Result", "The selected result to extract.", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var results = new Results.Results();
            if (!DA.GetData(0, ref results)) return;

            string resultsType = "";
            if (!DA.GetData(1, ref resultsType)) return;

            var result = typeof(Results.Results).GetProperty(resultsType)?.GetValue(results);
            if (result is double resultAsDouble)
            {
                DA.SetData(0, resultAsDouble);
            }
            else if (result is double[] resultAsDoubleArray)
            {
                DA.SetDataList(0, resultAsDoubleArray);
            }
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_CoreMerger; // FIXME


        public override Guid ComponentGuid => new Guid("F2D51DD0-3C0A-435F-8C0B-D6F78F6014C1");

    }
}