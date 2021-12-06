using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace Hive.IO.GhDistributors
{
    public class GhDistResultsFiltered : GH_Component
    {
        // Results
        static Results.Results HiveResults { get; set; }

        // Key Names
        static readonly string kMonthly = "monthly";
        static readonly string kYearly = "yearly";
        static readonly string kLifetime = "lifetime";

        static readonly string kEnergy = "energy";
        static readonly string kEmissions = "emissions";
        static readonly string kCost = "cost";

        static readonly string kEmbodied = "embodied";
        static readonly string kOperational = "operational";

        static readonly string kConstruction = "embodied";
        static readonly string kSystems = "operational";


        // Key Arrays
        static readonly string[] keysTimeResolution = new string[] { kMonthly, kYearly, kLifetime };
        static readonly string[] keysKpi = new string[] { kEnergy, kEmissions, kCost };
        static readonly string[] keysEmbodiedOperational = new string[] { kEmbodied, kOperational };

        // Defaults
        string DefaultTimeResolution = keysTimeResolution[0];
        string DefaultKpi = keysKpi[0];
        string DefaultEmbodiedOperational = keysEmbodiedOperational[0];



        // TODO cheeeck this, values aint quite right
        Dictionary<(string, string, string), double?> filterMap = new Dictionary<(string, string, string), double?>()
        {
            { (kMonthly, kEnergy, kEmbodied), null },
            { (kMonthly, kEnergy, kOperational), null },
            { (kMonthly, kEmissions, kEmbodied), HiveResults.TotalEmissionsEmbodiedConstructionMonthlyLevelised },
            { (kMonthly, kEmissions, kOperational), HiveResults.TotalEmissionsOperationalConstructionMonthlyLevelised },
            { (kMonthly, kCost, kEmbodied), HiveResults.TotalCostEmbodiedConstructionMonthlyLevelised },
            { (kMonthly, kCost, kOperational), HiveResults.TotalCostOperationalConstructionMonthlyLevelized },

            { (kYearly, kEnergy, kEmbodied), null },
            { (kYearly, kEnergy, kOperational), null },
            { (kYearly, kEmissions, kEmbodied), HiveResults.TotalEmissionsEmbodiedConstructionYearlyLevelised },
            { (kYearly, kEmissions, kOperational), HiveResults.TotalEmissionsOperationalConstructionYearlyLevelised },
            { (kYearly, kCost, kEmbodied), HiveResults.TotalCostEmbodiedConstructionYearlyLevelised },
            { (kYearly, kCost, kOperational), HiveResults.TotalCostOperationalConstructionYearlyLevelised },

            { (kLifetime, kEnergy, kEmbodied), null },
            { (kLifetime, kEnergy, kOperational), null },
            { (kLifetime, kEmissions, kEmbodied), HiveResults.TotalEmissionsEmbodied },
            { (kLifetime, kEmissions, kOperational), HiveResults.TotalEmissionsOperational },
            { (kLifetime, kCost, kEmbodied), HiveResults.TotalCostEmbodied },
            { (kLifetime, kCost, kOperational), HiveResults.TotalCostOperational }
        };

        public GhDistResultsFiltered()
          : base("Distributor Results Hive", "HiveDistResults",
              "Distributor for Hive Results",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Results", "Results", "Hive results of type <Hive.IO.Results>", GH_ParamAccess.item);

            // Filters
            pManager.AddTextParameter("Time Resolution", "Time Resolution", "Defines at what time resolution to return results. Possible values are 'lifetime', 'yearly' and 'monthly'. Default is monthly. Note that yearly will provide a single annualized value.", GH_ParamAccess.item);
            pManager.AddTextParameter("KPI", "KPI", "Defines which KPI to return results for. Allowed values are 'energy', 'emissions', and 'cost'.", GH_ParamAccess.item);
            pManager.AddTextParameter("Results", "Results", "Hive results of type <Hive.IO.Results>", GH_ParamAccess.item);

        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("FilteredResult", "FilteredResult", "The result based on the given filters or default avlues", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var results = new Results.Results();
            if (!DA.GetData(0, ref results)) return;

            // Get the Filters
            string filterTimeResolution = DefaultTimeResolution;
            string filterKpi = DefaultKpi;
            string filterEmbodiedOperational = DefaultEmbodiedOperational;

            DA.GetData(1, ref filterTimeResolution);
            DA.GetData(2, ref filterKpi);
            DA.GetData(3, ref filterEmbodiedOperational);


            // Time Resolution
            filterTimeResolution = CheckFilterInput(filterTimeResolution, DefaultTimeResolution, keysTimeResolution);
            filterKpi = CheckFilterInput(filterTimeResolution, DefaultKpi, keysKpi);
            filterEmbodiedOperational = CheckFilterInput(filterTimeResolution, DefaultEmbodiedOperational, keysEmbodiedOperational);

            var filteredResult = QueryResults((filterTimeResolution, filterKpi, filterEmbodiedOperational));

            DA.SetData(0, filteredResult);
        }


        // this is somewhat horrendous... but didnt want to introduce SQL yet as not sure how simple this is with Grasshopper.
        double? QueryResults((string, string, string) query)
        {
            if (HiveResults == null) return null;
            return filterMap[query];
        }


        private string CheckFilterInput(string candidateKey, string defaultKey, string[] keys)
        {
            candidateKey = candidateKey.Trim().ToLower();
            return Array.Exists(keys, k => k == candidateKey) ? candidateKey : defaultKey;
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_CoreMerger; // FIXME should create dedicated icon


        public override Guid ComponentGuid => new Guid("4C5EFA32-D74E-4F09-96FB-BD98A8A13B68");
    }
}
