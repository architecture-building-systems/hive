using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
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

            // 0, 1, 2, 3
            pManager.AddNumberParameter("ClgMonthly", "ClgMonthly", "ClgMonthly", GH_ParamAccess.list);
            pManager.AddNumberParameter("HtgMonthly", "HtgMonthly", "HtgMonthly", GH_ParamAccess.list);
            pManager.AddNumberParameter("ElecMonthly", "ElecMonthly", "ElecMonthly", GH_ParamAccess.list);
            pManager.AddNumberParameter("DHWMonthly", "DHWMonthly", "DHWMonthly", GH_ParamAccess.list);

            // 4, 5, 6, 7
            pManager.AddNumberParameter("CoolingHourly", "CoolingHourly", "CoolingHourly", GH_ParamAccess.list);
            pManager.AddNumberParameter("HeatingHourly", "HeatingHourly", "HeatingHourly", GH_ParamAccess.list);
            pManager.AddNumberParameter("ElectricityHourly", "ElectricityHourly", "ElectricityHourly", GH_ParamAccess.list);
            pManager.AddNumberParameter("DHWHourly", "DHWHourly", "DHWHourly", GH_ParamAccess.list);

            // 8, 9, 10, 11
            pManager.AddNumberParameter("SupplyCapacities", "SupplyCapacities", "SupplyCapacities", GH_ParamAccess.list);
            pManager.AddTextParameter("SupplyNames", "SupplyNames", "SupplyNames", GH_ParamAccess.list);
            pManager.AddNumberParameter("SupplyOpMonthly", "SupplyOpMonthly", "SupplyOpMonthly", GH_ParamAccess.tree);
            pManager.AddNumberParameter("SupplyOpHourly", "SupplyOpHourly", "SupplyOpHourly", GH_ParamAccess.tree);

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
            List<double> coolingMonthly = new List<double>();
            List<double> heatingMonthly = new List<double>();
            List<double> electricityMonthly = new List<double>();
            List<double> domesticHotWaterMonthly = new List<double>();
            DA.GetDataList(0, coolingMonthly);
            DA.GetDataList(1, heatingMonthly);
            DA.GetDataList(2, electricityMonthly);
            DA.GetDataList(3, domesticHotWaterMonthly);

            List<double> coolingHourly = new List<double>();
            List<double> heatingHourly = new List<double>();
            List<double> electricityHourly = new List<double>();
            List<double> domesticHotWaterHourly = new List<double>();
            DA.GetDataList(4, coolingHourly);
            DA.GetDataList(5, heatingHourly);
            DA.GetDataList(6, electricityHourly);
            DA.GetDataList(7, domesticHotWaterHourly);

            List<double> supplyCap = new List<double>();
            List<string> supplyNames = new List<string>();
            GH_Structure<GH_Number> supplyOpMonthly;
            GH_Structure<GH_Number> supplyOpHourly;
            DA.GetDataList(8, supplyCap);
            DA.GetDataList(9, supplyNames);
            DA.GetDataTree(10, out supplyOpMonthly);
            DA.GetDataTree(11, out supplyOpHourly);

            int techtypes = 3;
            bool[,] supplyTypes = new bool[supplyCap.Count, techtypes];
            string[] supplyUnits = new string[supplyCap.Count];
            for (int i = 0; i < supplyUnits.Length; i++)
            {
                supplyUnits[i] = "dummy";
                for (int u = 0; u < techtypes; u++)
                    supplyTypes[i, u] = true;
            }

            double[][] operationMonthly = new double[supplyOpMonthly.PathCount][];
            for (int i = 0; i < supplyOpMonthly.PathCount; i++)
            {
                GH_Path path = supplyOpMonthly.Paths[i];
                List<GH_Number> operation = supplyOpMonthly[path];
                operationMonthly[i] = new double[operation.Count];
                for (int u = 0; u < operation.Count; u++)
                {
                    operationMonthly[i][u] = operation[u].Value;
                }
            }

            double[][] operationHourly = new double[supplyOpHourly.PathCount][];
            for (int i = 0; i < supplyOpHourly.PathCount; i++)
            {
                GH_Path path = supplyOpHourly.Paths[i];
                List<GH_Number> operation = supplyOpHourly[path];
                operationHourly[i] = new double[operation.Count];
                for (int u = 0; u < operation.Count; u++)
                {
                    operationHourly[i][u] = operation[u].Value;
                }
            }

            //// Long's example about Grasshopper Trees
            //GH_Structure<GH_Point> iPoints;
            //DA.GetDataTree(0, out iPoints);

            //GH_Structure<GH_Circle> oCircles = new GH_Structure<GH_Circle>();
            //foreach (GH_Path path in iPoints.Paths)
            //{
            //    List<GH_Point> ghPoints = iPoints[path];
            //    List<GH_Circle> ghCircles = new List<GH_Circle>();
            //    foreach (GH_Point ghPoint in ghPoints)
            //    {
            //        Circle circle = new Circle(ghPoint.Value, 1.0);
            //        GH_Circle ghCircle = new GH_Circle(circle);
            //        ghCircles.Add(ghCircle);
            //    }
            //    oCircles.AppendRange(ghCircles, path);
            //}

            //DA.SetDataTree(0, oCircles);


            Results results = new Results();
            // these methods should handle nulls or wrong list lengths themselves
            results.SetTotalDemandMonthly(coolingMonthly.ToArray(), heatingMonthly.ToArray(), electricityMonthly.ToArray(), domesticHotWaterMonthly.ToArray());
            results.SetTotalDemandHourly(coolingHourly.ToArray(), heatingHourly.ToArray(), electricityHourly.ToArray(), domesticHotWaterHourly.ToArray());

            results.SetSupplySystemsCapacity(supplyNames.ToArray(), supplyTypes, supplyCap.ToArray(), supplyUnits);

            results.SetSupplySystemsGenerationMonthly(operationMonthly);
            results.SetSupplySystemsGenerationHourly(operationHourly);


            DA.SetData(0, results);
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("23cb5778-5a53-4dcc-ba2c-56d51c9c06b3"); }
        }
    }
}