using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;


namespace Hive.IO.GhParametricInputs
{
    public class GhEnergyPotentials : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhParametricInputEnergyPotentials class.
        /// </summary>
        public GhEnergyPotentials()
          : base("Parametric Input Energy Potentials Hive", "HiveParaInPotentials",
              "Parametric inputs for Hive Energy Potentials for, e.g., optimization work flows or sensitivity analysis. This component will overwrite the settings in the Hive Environment Input Component when connected.",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("NaturalGasName", "NaturalGasName", "NaturalGasName", GH_ParamAccess.item);
            pManager.AddNumberParameter("NaturalGasPrice", "NaturalGasPrice", "Natural Gas Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("NaturalGasEmissions", "NaturalGasEmissions", "Natural Gas Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("NaturalGasAvailability", "NaturalGasAvailability", "Natural Gas availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddTextParameter("BioGasName", "BioGasName", "BioGasName", GH_ParamAccess.item);
            pManager.AddNumberParameter("BioGasPrice", "BioGasPrice", "Bio Gas Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("BioGasEmissions", "BioGasEmissions", "Bio Gas Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("BioGasAvailability", "BioGasAvailability", "Bio Gas Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddTextParameter("WoodPelletsName", "WoodPelletsName", "WoodPelletsName", GH_ParamAccess.item);
            pManager.AddNumberParameter("WoodPelletsPrice", "WoodPelletsPrice", "Wood Pellets Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("WoodPelletsEmissions", "WoodPelletsEmissions", "Wood Pellets Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("WoodPelletsAvailability", "WoodPelletsAvailability", "Wood Pellets Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddTextParameter("DistrictHeatingName", "DistrictHeatingName", "DistrictHeatingName", GH_ParamAccess.item);
            pManager.AddNumberParameter("DistrictHeatingPrice", "DistrictHeatingPrice", "District Heating Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictHeatingEmissions", "DistrictHeatingEmissions", "District Heating Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictHeatingAvailability", "DistrictHeatingAvailability", "District Heating Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictHeatingSupplyTemp", "DistrictHeatingSupplyTemp", "DistrictHeating Supply Temperature, in deg C. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddTextParameter("DistrictCoolingName", "DistrictCoolingName", "DistrictCoolingName", GH_ParamAccess.item);
            pManager.AddNumberParameter("DistrictCoolingPrice", "DistrictCoolingPrice", "District Cooling Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictCoolingEmissions", "DistrictCoolingEmissions", "District Cooling Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictCoolingAvailability", "DistrictCoolingAvailability", "District Cooling Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictCoolingSupplyTemp", "DistrictCoolingSupplyTemp", "District Cooling Supply Temperature, in deg C. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddTextParameter("GridElectricityName", "GridElectricityName", "GridElectricityName", GH_ParamAccess.item);
            pManager.AddNumberParameter("GridElectricityPrice", "GridElectricityPrice", "Grid Electricity Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("GridElectricityEmissions", "GridElectricityEmissions", "Grid Electricity Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("GridElectricityAvailability", "GridElectricityAvailability", "Grid Electricity Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Potentials", "Potentials", "Energy potentials of the site as custom class .", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var potentials = new EnergyCarrierTimeseries();

            var naturalGasPriceList = new List<double>();
            var naturalGasEmissionsList = new List<double>();
            var naturalGasAvailabilityList = new List<double>();
            DA.GetData(0, ref potentials.NaturalGasName);
            DA.GetDataList(1, naturalGasPriceList);
            DA.GetDataList(2, naturalGasEmissionsList);
            DA.GetDataList(3, naturalGasAvailabilityList);

            var bioGasPriceList = new List<double>();
            var bioGasEmissionsList = new List<double>();
            var bioGasAvailabilityList = new List<double>();
            DA.GetData(4, ref potentials.BioGasName);
            DA.GetDataList(5, bioGasPriceList);
            DA.GetDataList(6, bioGasEmissionsList);
            DA.GetDataList(7, bioGasAvailabilityList);

            var woodPriceList = new List<double>();
            var woodEmissionsList = new List<double>();
            var woodAvailabilityList = new List<double>();
            DA.GetData(8, ref potentials.WoodPelletsName);
            DA.GetDataList(9, woodPriceList);
            DA.GetDataList(10, woodEmissionsList);
            DA.GetDataList(11, woodAvailabilityList);

            var dhPriceList = new List<double>();
            var dhEmissionsList = new List<double>();
            var dhAvailabilityList = new List<double>();
            var dhSupTempList = new List<double>();
            DA.GetData(12, ref potentials.DistrictHeatingName);
            DA.GetDataList(13, dhPriceList);
            DA.GetDataList(14, dhEmissionsList);
            DA.GetDataList(15, dhAvailabilityList);
            DA.GetDataList(16, dhSupTempList);

            var dcPriceList = new List<double>();
            var dcEmissionsList = new List<double>();
            var dcAvailabilityList = new List<double>();
            var dcSupTempList = new List<double>();
            DA.GetData(17, ref potentials.DistrictCoolingName);
            DA.GetDataList(18, dcPriceList);
            DA.GetDataList(19, dcEmissionsList);
            DA.GetDataList(20, dcAvailabilityList);
            DA.GetDataList(21, dcSupTempList);

            var gridPriceList = new List<double>();
            var gridEmissionsList = new List<double>();
            var gridAvailabilityList = new List<double>();
            DA.GetData(22, ref potentials.GridElectricityName);
            DA.GetDataList(23, gridPriceList);
            DA.GetDataList(24, gridEmissionsList);
            DA.GetDataList(25, gridAvailabilityList);

            
            potentials.NaturalGasPrice = naturalGasPriceList.ToArray();
            potentials.NaturalGasEmissions = naturalGasEmissionsList.ToArray();
            potentials.NaturalGasAvailability = naturalGasAvailabilityList.ToArray();
            
            potentials.BioGasPrice = bioGasPriceList.ToArray();
            potentials.BioGasEmissions = bioGasEmissionsList.ToArray();
            potentials.BioGasAvailability = bioGasAvailabilityList.ToArray();

            potentials.WoodPelletsPrice = woodPriceList.ToArray();
            potentials.WoodPelletsEmissions = woodEmissionsList.ToArray();
            potentials.WoodPelletsAvailability = woodAvailabilityList.ToArray();

            potentials.DistrictHeatingPrice = dhPriceList.ToArray();
            potentials.DistrictHeatingEmissions = dhEmissionsList.ToArray();
            potentials.DistrictHeatingAvailability = dhAvailabilityList.ToArray();
            potentials.DistrictHeatingSupplyTemp = dhSupTempList.ToArray();

            potentials.DistrictCoolingPrice = dcPriceList.ToArray();
            potentials.DistrictCoolingEmissions = dcEmissionsList.ToArray();
            potentials.DistrictCoolingAvailability = dcAvailabilityList.ToArray();
            potentials.DistrictCoolingSupplyTemp = dcSupTempList.ToArray();

            potentials.GridElectricityPrice = gridPriceList.ToArray();
            potentials.GridElectricityEmissions = gridEmissionsList.ToArray();
            potentials.GridElectricityAvailability = gridAvailabilityList.ToArray();

            DA.SetData(0, potentials);
        }


        internal class EnergyCarrierTimeseries
        {
            public string NaturalGasName;
            public double[] NaturalGasPrice;
            public double[] NaturalGasEmissions;
            public double[] NaturalGasAvailability;

            public string BioGasName;
            public double[] BioGasPrice;
            public double[] BioGasEmissions;
            public double[] BioGasAvailability;

            public string WoodPelletsName;
            public double[] WoodPelletsPrice;
            public double[] WoodPelletsEmissions;
            public double[] WoodPelletsAvailability;

            public string DistrictHeatingName;
            public double[] DistrictHeatingPrice;
            public double[] DistrictHeatingEmissions;
            public double[] DistrictHeatingAvailability;
            public double[] DistrictHeatingSupplyTemp;

            public string DistrictCoolingName;
            public double[] DistrictCoolingPrice;
            public double[] DistrictCoolingEmissions;
            public double[] DistrictCoolingAvailability;
            public double[] DistrictCoolingSupplyTemp;

            public string GridElectricityName;
            public double[] GridElectricityPrice;
            public double[] GridElectricityEmissions;
            public double[] GridElectricityAvailability;
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ed607bc4-14df-47c3-8c56-6053f9227764"); }
        }
    }
}