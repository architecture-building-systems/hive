using System;
using System.Collections.Generic;

using Grasshopper.Kernel;


namespace Hive.IO.GHComponents
{
    public class GhParametricInputEnergyPotentials : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhParametricInputEnergyPotentials class.
        /// </summary>
        public GhParametricInputEnergyPotentials()
          : base("Parametric Input Energy Potantials Hive", "HiveParaInPotentials",
              "Description",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("NaturalGasPrice", "NaturalGasPrice", "Natural Gas Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("NaturalGasEmissions", "NaturalGasEmissions", "Natural Gas Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("NaturalGasAvailability", "NaturalGasAvailability", "Natural Gas availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddNumberParameter("BioGasPrice", "BioGasPrice", "Bio Gas Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("BioGasEmissions", "BioGasEmissions", "Bio Gas Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("BioGasAvailability", "BioGasAvailability", "Bio Gas Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddNumberParameter("WoodPelletsPrice", "WoodPelletsPrice", "Wood Pellets Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("WoodPelletsEmissions", "WoodPelletsEmissions", "Wood Pellets Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("WoodPelletsAvailability", "WoodPelletsAvailability", "Wood Pellets Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddNumberParameter("DistrictHeatingPrice", "DistrictHeatingPrice", "District Heating Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictHeatingEmissions", "DistrictHeatingEmissions", "District Heating Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictHeatingAvailability", "DistrictHeatingAvailability", "District Heating Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddNumberParameter("DistrictCoolingPrice", "DistrictCoolingPrice", "District Cooling Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictCoolingEmissions", "DistrictCoolingEmissions", "District Cooling Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("DistrictCoolingAvailability", "DistrictCoolingAvailability", "District Cooling Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddNumberParameter("GridElectricityPrice", "GridElectricityPrice", "Grid Electricity Price, in CHF/kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("GridElectricityEmissions", "GridElectricityEmissions", "Grid Electricity Emissions, in kgCO2eq./kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);
            pManager.AddNumberParameter("GridElectricityAvailability", "GridElectricityAvailability", "Grid Electricity Availability, in kWh. 8760-timeseries (annual hourly).", GH_ParamAccess.list);

            pManager.AddTextParameter("EpwWeatherFile", "EpwWeatherFile", "Path to an .EPW weather file, for solar irradiance and ambient air temperature", GH_ParamAccess.item);

            //delete this later, when epw-reader is implemented in Hive.IO
            pManager.AddNumberParameter("GHI", "GHI", "GHI", GH_ParamAccess.list);
            pManager.AddNumberParameter("DHI", "DHI", "DHI", GH_ParamAccess.list);
            pManager.AddNumberParameter("DNI", "DNI", "DNI", GH_ParamAccess.list);
            pManager.AddNumberParameter("AmbT", "AmbT", "AmbT", GH_ParamAccess.list);

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
            var potentials = new EnergyPotentialsProperties();
            potentials.Horizon = Misc.HoursPerYear;

            var naturalGasPriceList = new List<double>();
            var naturalGasEmissionsList = new List<double>();
            var naturalGasAvailabilityList = new List<double>();
            DA.GetDataList(0, naturalGasPriceList);
            DA.GetDataList(1, naturalGasEmissionsList);
            DA.GetDataList(2, naturalGasAvailabilityList);

            var bioGasPriceList = new List<double>();
            var bioGasEmissionsList = new List<double>();
            var bioGasAvailabilityList = new List<double>();
            DA.GetDataList(3, bioGasPriceList);
            DA.GetDataList(4, bioGasEmissionsList);
            DA.GetDataList(5, bioGasAvailabilityList);

            var woodPriceList = new List<double>();
            var woodEmissionsList = new List<double>();
            var woodAvailabilityList = new List<double>();
            DA.GetDataList(6, woodPriceList);
            DA.GetDataList(7, woodEmissionsList);
            DA.GetDataList(8, woodAvailabilityList);

            var dhPriceList = new List<double>();
            var dhEmissionsList = new List<double>();
            var dhAvailabilityList = new List<double>();
            DA.GetDataList(9, dhPriceList);
            DA.GetDataList(10, dhEmissionsList);
            DA.GetDataList(11, dhAvailabilityList);

            var dcPriceList = new List<double>();
            var dcEmissionsList = new List<double>();
            var dcAvailabilityList = new List<double>();
            DA.GetDataList(12, dcPriceList);
            DA.GetDataList(13, dcEmissionsList);
            DA.GetDataList(14, dcAvailabilityList);

            var gridPriceList = new List<double>();
            var gridEmissionsList = new List<double>();
            var gridAvailabilityList = new List<double>();
            DA.GetDataList(15, gridPriceList);
            DA.GetDataList(16, gridEmissionsList);
            DA.GetDataList(17, gridAvailabilityList);

            DA.GetData(18, ref potentials.EpwPath); //decoding epw here????

            var ghiList = new List<double>();
            var dhiList = new List<double>();
            var dniList = new List<double>();
            var ambTList = new List<double>();
            DA.GetDataList(19, ghiList);
            DA.GetDataList(20, dhiList);
            DA.GetDataList(21, dniList);
            DA.GetDataList(22, ambTList);
            
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

            potentials.DistrictCoolingPrice = dcPriceList.ToArray();
            potentials.DistrictCoolingEmissions = dcEmissionsList.ToArray();
            potentials.DistrictCoolingAvailability = dcAvailabilityList.ToArray();

            potentials.GridElectricityPrice = gridPriceList.ToArray();
            potentials.GridElectricityEmissions = gridEmissionsList.ToArray();
            potentials.GridElectricityAvailability = gridAvailabilityList.ToArray();

            potentials.GlobalHorizontalIrradiance = ghiList.ToArray();
            potentials.DiffuseHorizontalIrradiance = dhiList.ToArray();
            potentials.DirectNormalIrradiance = dniList.ToArray();
            potentials.AmbientAirTemperature = ambTList.ToArray();

            DA.SetData(0, potentials);
        }

        internal class EnergyPotentialsProperties
        {
            public int Horizon;

            public double[] NaturalGasPrice;
            public double[] NaturalGasEmissions;
            public double[] NaturalGasAvailability;

            public double[] BioGasPrice;
            public double[] BioGasEmissions;
            public double[] BioGasAvailability;

            public double[] WoodPelletsPrice;
            public double[] WoodPelletsEmissions;
            public double[] WoodPelletsAvailability;

            public double[] DistrictHeatingPrice;
            public double[] DistrictHeatingEmissions;
            public double[] DistrictHeatingAvailability;

            public double[] DistrictCoolingPrice;
            public double[] DistrictCoolingEmissions;
            public double[] DistrictCoolingAvailability;

            public double[] GridElectricityPrice;
            public double[] GridElectricityEmissions;
            public double[] GridElectricityAvailability;

            public string EpwPath;

            public double[] GlobalHorizontalIrradiance;
            public double[] DirectNormalIrradiance;
            public double[] DiffuseHorizontalIrradiance;
            public double[] AmbientAirTemperature;
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