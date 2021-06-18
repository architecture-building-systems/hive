using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Hive.IO.EnergySystems;
using Hive.IO.Properties;
using Rhino.Geometry;

namespace Hive.IO.GhInputOutput
{
    public class GhResults : GH_Component
    {

        public GhResults()
          : base("Results Hive", "HiveResults",
              "Hive Results of type <Hive.IO.Results.Results>, containing all simulation results from within the simulation core.",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Hive 0.1 supports only one zone
            // TO DO: list of lists? each zone has demand and you could have multiple zones here

            // 0, 1, 2, 3
            pManager.AddNumberParameter("Cooling Load", "Clg", "Cooling load in [kWh]. List with either montly [12] or hourly [8760].", GH_ParamAccess.list);
            pManager.AddNumberParameter("Space Heating Load", "Htg", "Space Heating load in [kWh]. List with either montly [12] or hourly [8760].", GH_ParamAccess.list);
            pManager.AddNumberParameter("Electricity Load", "Elec", "Electricity load in [kWh]. List with either montly [12] or hourly [8760].", GH_ParamAccess.list);
            pManager.AddNumberParameter("Domestic Hot Water Load", "DHW", "Domestic hot water load in [kWh]. List with either montly [12] or hourly [8760].", GH_ParamAccess.list);

            // 4, 5, 6, 7
            pManager.AddNumberParameter("Supply System Capacities", "SupSysCap", "Capacities of the energy supply system technologies.", GH_ParamAccess.list);
            pManager.AddTextParameter("Supply System Names", "SupSysNames", "Names of the energy supply system technologies.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Operation Monthly", "OpMonthly", "Monthly operation schedules of the energy supply systems in [kWh/month].", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Operation Hourly", "OpHourly", "Hourly operation schedules of the energy supply systems in [kWh].", GH_ParamAccess.tree);

            // 8, 9
            pManager.AddBooleanParameter("Supply System Suitability", "SupSysSuit", "Suitability of supply system for electricity, heating and cooling generation.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Supply System Units", "SupSysUnits", "Unit of the supply system capacity / operation.", GH_ParamAccess.list);

            // 10, 11, 12
            pManager.AddMeshParameter("Solar Irradiated Surfaces", "SolSrfs", "Mesh surfaces whose vertices are colored according to their solar exposure in [kWh/year]", GH_ParamAccess.list);
            pManager.AddMeshParameter("Sky View Factor", "ViewFactor", "Sky dome showing view factors.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Sunpath Diagram", "SunPath", "Sunpath diagram for the geographic location of the building.", GH_ParamAccess.list);

            // 13
            pManager.AddGenericParameter("Hive Building", "HiveBuilding", "Hive Building <Hive.IO.Building.Building>", GH_ParamAccess.item);
            // 14
            pManager.AddGenericParameter("Hive ConversionTech", "HiveConversionTech", "Hive Conversion technologies <Hive.IO.EnergySystems.ConversionTech>", GH_ParamAccess.list);
            // 15
            pManager.AddGenericParameter("Hive Emitters", "HiveEmitters", "Hive emitters <Hive.IO.EnergySystems.Emitter>", GH_ParamAccess.list);

            // 16, 17, 18, 19, 20
            pManager.AddNumberParameter("Q_T_opaque", "Q_T_opaque", "transmission heat losses from opaque construction", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_T_transparent", "Q_T_transparent", "transmission heat losses from transparent construction", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_V", "Q_V", "ventilation heat losses", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_i", "Q_i", "Internal gains", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_s", "Q_s", "Solar heat gains", GH_ParamAccess.list);

            // 21, 22
            pManager.AddNumberParameter("consumedElec", "consumedElec", "consumedElec. pManager[2] also has generatedElec from e.g. PV. This here is consumed Elec from occupancy (equip & lighting) and systems (HP)", GH_ParamAccess.list);
            pManager.AddNumberParameter("consumedHeat", "consumedHeat", "consumedHeat. same as consumedElec... we could have negative heating loads from solar thermal or CHP", GH_ParamAccess.list);

            // 23
            pManager.AddNumberParameter("Q_s_per_win", "Q_s_per_win", "Solar heat gains per window", GH_ParamAccess.tree);

            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Results", "Results", "Writes a Hive Results object. Can be plugged into the Hive Visualizer and Hive Results Parameter.", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // also sub-GHResults components, like distributor? Or just one massive Results reader?

            // input: all kinds of results from the Core 
            List<double> cooling = new List<double>();
            List<double> heating = new List<double>();
            List<double> electricity = new List<double>();
            List<double> domesticHotWater = new List<double>();
            DA.GetDataList(0, cooling);
            DA.GetDataList(1, heating);
            DA.GetDataList(2, electricity);
            DA.GetDataList(3, domesticHotWater);

            List<double> supplyCap = new List<double>();
            List<string> supplyNames = new List<string>();
            DA.GetDataList(4, supplyCap);
            DA.GetDataList(5, supplyNames);
            DA.GetDataTree(6, out GH_Structure<GH_Number> supplyOpMonthly);
            DA.GetDataTree(7, out GH_Structure<GH_Number> supplyOpHourly);
            DA.GetDataTree(8, out GH_Structure<GH_Boolean> supplySysSuitability);

            List<string> supplyUnits = new List<string>();
            DA.GetDataList(9, supplyUnits);


            List<Mesh> irradMesh = new List<Mesh>();
            Mesh viewFactor = null;
            List<Curve> sunPathCrvs = new List<Curve>();
            DA.GetDataList(10, irradMesh);
            DA.GetData(11, ref viewFactor);
            DA.GetDataList(12, sunPathCrvs);


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
            

            Building.Building building = null;
            var conversionTechs = new List<ConversionTech>();
            var emitters = new List<Emitter>();
            var outputEnergies = new List<Carrier>();
            var inputEnergies = new List<Carrier>();
            DA.GetData(13, ref building);
            DA.GetDataList(14, conversionTechs);
            DA.GetDataList(15, emitters);

            var Qt_opaque = new List<double>();
            var Qt_transparent = new List<double>();
            var Qv = new List<double>();
            var Qi = new List<double>();
            var Qs = new List<double>();
            DA.GetDataList(16, Qt_opaque);
            DA.GetDataList(17, Qt_transparent);
            DA.GetDataList(18, Qv);
            DA.GetDataList(19, Qi);
            DA.GetDataList(20, Qs);

            var consumedElec = new List<double>();
            var consumedHeat = new List<double>();
            DA.GetDataList(21, consumedElec);
            DA.GetDataList(22, consumedHeat);


            GH_Structure<GH_Number> iQsPerWin;
            DA.GetDataTree(23, out iQsPerWin);
            double[][] QsPerWindow = new double[iQsPerWin.PathCount][];
            for (int i = 0; i < iQsPerWin.PathCount; i++)
            {
                GH_Path path = iQsPerWin.Paths[i];
                List<GH_Number> numbers = iQsPerWin[path];
                QsPerWindow[i] = new double[numbers.Count];
                for (int u = 0; u < numbers.Count; u++)
                {
                    try
                    {
                        QsPerWindow[i][u] = (double)numbers[u].Value / 1000.0;
                    }
                    catch
                    {
                        QsPerWindow[i][u] = 0.0;
                    }
                }
            }


            building.Zones[0].ConsumedElectricity = consumedElec.ToArray();
            building.Zones[0].ConsumedHeating = consumedHeat.ToArray();

            building.Zones[0].SetEnergyDemands(heating.ToArray(), domesticHotWater.ToArray(), cooling.ToArray(), electricity.ToArray());
            building.Zones[0].SetLossesAndGains(Qt_opaque.ToArray(), Qt_transparent.ToArray(), Qv.ToArray(), Qi.ToArray(), Qs.ToArray());
            building.Zones[0].SetWindowIrradiance(QsPerWindow);

            // writing data into results object
            Results.Results results = new Results.Results(building, conversionTechs, emitters);
            
            // these methods should handle nulls or wrong list lengths themselves
            //results.SetTotalDemandMonthly(coolingMonthly.ToArray(), heatingMonthly.ToArray(), electricityMonthly.ToArray(), domesticHotWaterMonthly.ToArray());
            //results.SetTotalDemandHourly(coolingHourly.ToArray(), heatingHourly.ToArray(), electricityHourly.ToArray(), domesticHotWaterHourly.ToArray());
            results.SetSupplySystemsCapacity(supplyNames.ToArray(), supplyTypes, supplyCap.ToArray(), supplyUnits.ToArray());
            results.SetSupplySystemsGenerationMonthly(operationMonthly);
            results.SetSupplySystemsGenerationHourly(operationHourly);
            results.SetIrradiationMesh(irradMesh.ToArray());
            results.SetSkyDome(viewFactor, sunPathCrvs.ToArray());

            DA.SetData(0, results);
        }




        protected override System.Drawing.Bitmap Icon => Resources.IO_CoreMerger;


        public override Guid ComponentGuid
        {
            get { return new Guid("23cb5778-5a53-4dcc-ba2c-56d51c9c06b3"); }
        }
    }
}
