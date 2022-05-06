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
    public class GhResultsCreator : GH_Component
    {

        public GhResultsCreator()
          : base("Results Creator Hive", "HiveResultsCreator",
              "Merges everything from the Simulation Core into a Hive Results of type <Hive.IO.Results.Results>, containing all simulation results.",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Hive 0.1 supports only one zone
            // TO DO: list of lists? each zone has demand and you could have multiple zones here

            // 0, 1, 2, 3
            pManager.AddNumberParameter("Ideal Cooling Loads", "IdealCooling", "Ideal cooling loads in [kWh], from a thermal simulation (e.g., SIA380.1). List with either montly [12] or hourly [8760].", GH_ParamAccess.list);
            pManager.AddNumberParameter("Ideal Space Heating Loads", "IdealHeating", "Ideal space heating loads in [kWh], from a thermal simulation (e.g., SIA380.1). List with either montly [12] or hourly [8760].", GH_ParamAccess.list);
            pManager.AddNumberParameter("Ideal Electricity Loads", "IdealElec", "Ideal electricity loads in [kWh] from appliances and lighting. List with either montly [12] or hourly [8760].", GH_ParamAccess.list);
            pManager.AddNumberParameter("Ideal Domestic Hot Water Loads", "IdealDHW", "Domestic hot water load in [kWh]. List with either montly [12] or hourly [8760].", GH_ParamAccess.list);

            // 4
            pManager.AddGenericParameter("Hive Building", "HiveBuilding", "Hive Building <Hive.IO.Building.Building>", GH_ParamAccess.item);
            // 5
            pManager.AddGenericParameter("Hive ConversionTech", "HiveConversionTech", "Hive Conversion technologies <Hive.IO.EnergySystems.ConversionTech>", GH_ParamAccess.list);
            // 6
            pManager.AddGenericParameter("Hive Emitters", "HiveEmitters", "Hive emitters <Hive.IO.EnergySystems.Emitter>", GH_ParamAccess.list);

            // 7, 8, 9, 10, 11
            pManager.AddNumberParameter("Q_T_opaque", "Q_T_opaque", "transmission heat losses from opaque construction", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_T_transparent", "Q_T_transparent", "transmission heat losses from transparent construction", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_V", "Q_V", "ventilation heat losses", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_i", "Q_i", "Internal gains", GH_ParamAccess.list);
            pManager.AddNumberParameter("Q_s", "Q_s", "Solar heat gains", GH_ParamAccess.list);

            // 12, 13
            pManager.AddNumberParameter("consumedElec", "consumedElec", "consumedElec. pManager[2] also has generatedElec from e.g. PV. This here is consumed Elec from occupancy (equip & lighting) and systems (HP)", GH_ParamAccess.list);
            pManager.AddNumberParameter("consumedHeat", "consumedHeat", "consumedHeat. same as consumedElec... we could have negative heating loads from solar thermal or CHP", GH_ParamAccess.list);

            // 14
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
        

            Building.Building building = null;
            var conversionTechs = new List<ConversionTech>();
            var emitters = new List<Emitter>();
            var outputEnergies = new List<Carrier>();
            var inputEnergies = new List<Carrier>();
            DA.GetData(4, ref building);
            DA.GetDataList(5, conversionTechs);
            DA.GetDataList(6, emitters);

            var Qt_opaque = new List<double>();
            var Qt_transparent = new List<double>();
            var Qv = new List<double>();
            var Qi = new List<double>();
            var Qs = new List<double>();
            DA.GetDataList(7, Qt_opaque);
            DA.GetDataList(8, Qt_transparent);
            DA.GetDataList(9, Qv);
            DA.GetDataList(10, Qi);
            DA.GetDataList(11, Qs);

            var consumedElec = new List<double>();
            var consumedHeat = new List<double>();
            DA.GetDataList(12, consumedElec);
            DA.GetDataList(13, consumedHeat);


            GH_Structure<GH_Number> iQsPerWin;
            DA.GetDataTree(14, out iQsPerWin);
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


            building.Zones[0].ConsumedElectricityMonthly = consumedElec.ToArray();
            building.Zones[0].ConsumedHeatingMonthly = consumedHeat.ToArray();

            building.Zones[0].SetEnergyDemands(heating.ToArray(), domesticHotWater.ToArray(), cooling.ToArray(), electricity.ToArray());
            building.Zones[0].SetLossesAndGains(Qt_opaque.ToArray(), Qt_transparent.ToArray(), Qv.ToArray(), Qi.ToArray(), Qs.ToArray());
            building.Zones[0].SetWindowIrradiance(QsPerWindow);

            // writing data into results object
            Results.Results results = new Results.Results(building, conversionTechs, emitters);
            
            // these methods should handle nulls or wrong list lengths themselves
            ////results.SetTotalDemandMonthly(coolingMonthly.ToArray(), heatingMonthly.ToArray(), electricityMonthly.ToArray(), domesticHotWaterMonthly.ToArray());
            ////results.SetTotalDemandHourly(coolingHourly.ToArray(), heatingHourly.ToArray(), electricityHourly.ToArray(), domesticHotWaterHourly.ToArray());
            //results.SetSupplySystemsCapacity(supplyNames.ToArray(), supplyTypes, supplyCap.ToArray(), supplyUnits.ToArray());
            //results.SetSupplySystemsGenerationMonthly(operationMonthly);
            //results.SetSupplySystemsGenerationHourly(operationHourly);
            //results.SetIrradiationMesh(irradMesh.ToArray());
            //results.SetSkyDome(viewFactor, sunPathCrvs.ToArray());

            DA.SetData(0, results);
        }


        protected override System.Drawing.Bitmap Icon => Resources.IO_CoreMerger;


        public override Guid ComponentGuid
        {
            get { return new Guid("23cb5778-5a53-4dcc-ba2c-56d51c9c06b3"); }
        }
    }
}
