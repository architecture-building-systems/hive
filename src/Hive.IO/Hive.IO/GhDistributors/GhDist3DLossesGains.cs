using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace Hive.IO.GhDistributors
{
    public class GhDist3DLossesGains : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GhDist3DLossesGains()
          : base("Distributor 3D Losses Gains", "Dist3DLossesGains",
              "Distributor for the 3D Losses and Gains arrows, getting data from the SIA380 simulator",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Transparent Surface Areas", "TranspAreas", "Transparent surface areas, in m²", GH_ParamAccess.list);
            pManager.AddNumberParameter("Opaque Surface Areas", "OpaqueAreas", "Opaque surface areas, in m²", GH_ParamAccess.list);
            pManager.AddNumberParameter("Monthly Solar Gains Per Window", "MonthlySolPerWin", "Monthly solar gains per window surface, in kWh", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Monthly Transmission Losses All Windows", "MonthlyTransLossAllWin", "Monthly transmission losses for all windows, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Monthly Transmission Losses All Walls", "MonthlyTransLossAllWalls", "Monthly transmission losses for all walls, in kWh", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Total Losses Per Window", "TotLossPerWin", "Total transmission losses per window surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Losses Per Opaque", "TotLossPerOpaq", "Total transmission losses per opaque surface, in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Total Gains Per Window", "TotGainsPerWin", "Total solar gains per window surface, in kWh", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var transparentSurfaceAreas = new List<double>();
            if (!DA.GetDataList(0, transparentSurfaceAreas)) return;
            var opaqueSurfaceAreas = new List<double>();
            if (!DA.GetDataList(1, opaqueSurfaceAreas)) return;

            if (!DA.GetDataTree(2, out GH_Structure<GH_Number> monthlySolarPerWin)) return;

            var monthlyTransmWindows = new List<double>();
            var monthlyTransmWalls = new List<double>();
            if (!DA.GetDataList(3, monthlyTransmWindows)) return;
            if (!DA.GetDataList(4, monthlyTransmWalls)) return;

            const int months = 12;

            double totWinArea = transparentSurfaceAreas.Sum();
            double totWallArea = opaqueSurfaceAreas.Sum();

            var totalGainsPerWindow = new List<double>();
            var totalLossesPerWindow = new List<double>();
            var totalLossesPerOpaque = new List<double>();

            for (int i = 0; i < transparentSurfaceAreas.Count; i++)
            {
                totalGainsPerWindow.Add(0.0);
                totalLossesPerWindow.Add(0.0);
                for (int m = 0; m < months; m++)
                {
                    totalLossesPerWindow[i] += ((monthlyTransmWindows[m] / totWinArea) * transparentSurfaceAreas[i]);
                    var branch = monthlySolarPerWin.Branches[i];
                    totalGainsPerWindow[i] += branch[m].Value;
                }
            }

            for (int i = 0; i < opaqueSurfaceAreas.Count; i++)
            {
                totalLossesPerOpaque.Add(0.0);
                for (int m = 0; m < months; m++)
                {
                    totalLossesPerOpaque[i] += ((monthlyTransmWalls[m] / totWallArea) * opaqueSurfaceAreas[i]);
                }

            }


            DA.SetDataList(0, totalLossesPerWindow);
            DA.SetDataList(1, totalLossesPerOpaque);
            DA.SetDataList(2, totalGainsPerWindow);
        }

        // TO DO: change with own icon
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_DistLossesGains;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("17511b5b-3799-42eb-a91f-323c6341683f"); }
        }
    }
}
