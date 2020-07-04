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
          : base("Hive.IO.Results", "HiveIOResults",
              "Hive.IO.Results object, containing results of all simulations run within the Mothercell.",
              "[hive]", "Mothercell")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Hive 0.1 supports only one zone
            // TO DO: list of lists? each zone has demand and you could have multiple zones here

            // 0, 1, 2, 3
            pManager.AddNumberParameter("Cooling Monthly", "ClgMonthly", "Monthly cooling energy demand in [kWh]. List with 12 elements.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Heating Monthly", "HtgMonthly", "Monthly heating energy in [kWh]. List with 12 elements.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Electricity Monthly", "ElecMonthly", "Monthly electricity demand in [kWh]. List with 12 elements.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Domestic Hot Water Monthly", "DHWMonthly", "Monthly domestic hot water demand in [kWh]. List with 12 elements.", GH_ParamAccess.list);

            // 4, 5, 6, 7
            pManager.AddNumberParameter("Cooling Hourly", "ClgHourly", "Hourly cooling energy demand in [kWh]. List with 8760 elements.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Heating Hourly", "HtgHourly", "Hourly heating energy demand in [kWh]. List with 8760 elements.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Electricity Hourly", "ElecHourly", "Hourly electricity demand in [kWh]. List with 8760 elements.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Domestic Hot Water Hourly", "DHWHourly", "Hourly domestic hot water demand in [kWh]. List with 8760 elements.", GH_ParamAccess.list);

            // 8, 9, 10, 11
            pManager.AddNumberParameter("Supply System Capacities", "SupSysCap", "Capacities of the energy supply system technologies.", GH_ParamAccess.list);
            pManager.AddTextParameter("Supply System Names", "SupSysNames", "Names of the energy supply system technologies.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Operation Monthly", "OpMonthly", "Monthly operation schedules of the energy supply systems in [kWh/month].", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Operation Hourly", "OpHourly", "Hourly operation schedules of the energy supply systems in [kWh].", GH_ParamAccess.tree);

            // 12, 13
            pManager.AddBooleanParameter("Supply System Suitability", "SupSysSuit", "Suitability of supply sytem for electricity, heating and cooling generation.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Supply System Units", "SupSysUnits", "Unit of the supply system capacity / operation.", GH_ParamAccess.list);

            // 14, 15
            pManager.AddMeshParameter("Solar Irradiated Surfaces", "SolSrfs", "Mesh surfaces whose vertices are coloured accordint to their solar exposure in [kWh/year]", GH_ParamAccess.list);
            pManager.AddMeshParameter("Sky View Factor", "ViewFactor", "Sky dome showing view factors.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Sunpath Diagram", "SunPath", "Sunpath diagram for the geographic location of the building.", GH_ParamAccess.list);


            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Results", "HiveIORes", "Writes an Hive.IO.Results object, ready to be shipped into the world, outside the Mothercell.", GH_ParamAccess.item);
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
            DA.GetDataList(8, supplyCap);
            DA.GetDataList(9, supplyNames);
            DA.GetDataTree(10, out GH_Structure<GH_Number> supplyOpMonthly);
            DA.GetDataTree(11, out GH_Structure<GH_Number> supplyOpHourly);
            DA.GetDataTree(12, out GH_Structure<GH_Boolean> supplySysSuitability);

            List<string> supplyUnits = new List<string>();
            DA.GetDataList(13, supplyUnits);


            List<Mesh> irradMesh = new List<Mesh>();
            Mesh viewFactor = null;
            List<Curve> sunPathCrvs = new List<Curve>();
            DA.GetDataList(14, irradMesh);
            DA.GetData(15, ref viewFactor);
            DA.GetDataList(16, sunPathCrvs);


            // tech types need to be read from the input. how?
            // PV will be read in here as just another supply technology, but with unit m², or kW_peak?
            int techtypes = 3;
            bool[,] supplyTypes = new bool[supplyCap.Count, techtypes];
            for (int i=0; i<supplySysSuitability.PathCount; i++)
            {
                GH_Path path = supplySysSuitability.Paths[i];
                List<GH_Boolean> suitability = supplySysSuitability[path];
                for(int u=0; u<suitability.Count; u++)
                {
                    supplyTypes[i, u] = suitability[u].Value;
                }
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


            // writing data into results object
            Results results = new Results();
            
            // these methods should handle nulls or wrong list lengths themselves
            results.SetTotalDemandMonthly(coolingMonthly.ToArray(), heatingMonthly.ToArray(), electricityMonthly.ToArray(), domesticHotWaterMonthly.ToArray());
            results.SetTotalDemandHourly(coolingHourly.ToArray(), heatingHourly.ToArray(), electricityHourly.ToArray(), domesticHotWaterHourly.ToArray());
            results.SetSupplySystemsCapacity(supplyNames.ToArray(), supplyTypes, supplyCap.ToArray(), supplyUnits.ToArray());
            results.SetSupplySystemsGenerationMonthly(operationMonthly);
            results.SetSupplySystemsGenerationHourly(operationHourly);
            results.SetIrradiationMesh(irradMesh.ToArray());
            results.SetSkyDome(viewFactor, sunPathCrvs.ToArray());

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
