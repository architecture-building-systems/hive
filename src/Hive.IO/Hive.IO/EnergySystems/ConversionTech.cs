using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Rhino.Geometry;
using Rhino.Input.Custom;

namespace Hive.IO.EnergySystems
{
    #region MiscSupply

    // these are all networks, containing input energy carriers. but not conversion tech
    /*
    public class ElectricityGrid : ConversionTech
    {
        public ElectricityGrid(double investmentCost, double embodiedGhg) 
            : base(investmentCost, embodiedGhg, double.MaxValue, "kW", false, false, true)
        {
        }


    }

    public class DistrictHeating : ConversionTech
    {
        public DistrictHeating(double investmentCost, double embodiedGhg) 
            : base(investmentCost, embodiedGhg, Double.MaxValue, "kW", true, false, false)
        {
        }


    }


    public class DistrictCooling : ConversionTech
    {
        public DistrictCooling(double investmentCost, double embodiedGhg) 
            : base(investmentCost, embodiedGhg, double.MaxValue, "kW", false, true, false)
        {
        }



    }
    */


    public class Chiller : ConversionTech
    {
        public Chiller(double investmentCost, double embodiedGhg, double capacity) 
            : base(investmentCost, embodiedGhg, capacity, "kW", false, true, false)
        {
        }


        public double[] SetConversionEfficiencyCooling()
        {
            return new double[]{};
        }
    }


    public class AirSourceHeatPump : ConversionTech
    {
        /// <summary>
        /// Ambient air carrier. This will influence COP of the ASHP
        /// </summary>
        public Air AmbientAir { get; private set; }
		public double COP { get; private set; }
        public AirSourceHeatPump(double investmentCost, double embodiedGhg, double capacity, double COP) 
            : base(investmentCost, embodiedGhg, capacity, "kW", true, false, false)
        {
			this.COP = COP;
            base.Name = "AirSourceHeatPump";
        }


        /// <summary>
        /// inputs from Hive.IO.Environment. But electricity also needs information on quantity... form Core simulator? 
        /// </summary>
        /// <param name="ambientAir"></param>
        /// <param name="electricity"></param>
        public void SetInput(Air ambientAir, Electricity electricity)
        {
            this.AmbientAir = ambientAir;
            base.InputCarrier = electricity;
        }


        /// <summary>
        /// parameters from a simulator in Hive.IO.Core
        /// </summary>
        /// <param name="horizon"></param>
        /// <param name="availableEnergy"></param>
        /// <param name="energyCost"></param>
        /// <param name="ghgEmissions"></param>
        /// <param name="supplyTemperature"></param>
        public void SetOutput(int horizon, double[] availableEnergy, double[] energyCost, double[] ghgEmissions, double[] supplyTemperature)
        {
            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = new Water(horizon, availableEnergy, energyCost, ghgEmissions, supplyTemperature);
        }
    }

    #endregion



    #region Surface based technology
    /// <summary>
    /// Surface based energy technologies, such as PV, solar thermal, PVT, ground collectors, etc.
    /// </summary>
    public abstract class SurfaceBasedTech : ConversionTech
    {
        /// <summary>
        /// Rhino mesh geometry object representing the energy system. Can be quad or triangles.
        /// </summary>
        public Mesh SurfaceGeometry { get; private set; }

        /// <summary>
        /// Surface Area, computed using Rhino.Geometry
        /// </summary>
        public double SurfaceArea { get; private set; }

        protected SurfaceBasedTech(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric, Mesh surfaceGeometry) 
            : base(investmentCost, embodiedGhg, 0.0, "undefined", isHeating, isCooling, isElectric)
        {
            this.SurfaceGeometry = surfaceGeometry;
            this.SurfaceArea = Rhino.Geometry.AreaMassProperties.Compute(this.SurfaceGeometry).Area;
        }


        protected static double[] ComputeMeanHourlyEnergy(Matrix hourlyIrradiance, SurfaceBasedTech surfaceTech)
        {
            Mesh mesh = surfaceTech.SurfaceGeometry;
            double surfaceArea = surfaceTech.SurfaceArea;

            int meshFacesCount = mesh.Faces.Count;
            int horizon = hourlyIrradiance.ColumnCount;
            double[] meanIrradiance = new double[horizon];


            double [] meshFaceAreas = new double[meshFacesCount];
            for (int t = 0; t < horizon; t++)
            {
                double totalIrradiance = 0.0;
                for (int i = 0; i < meshFacesCount; i++)
                {
                    meshFaceAreas[i] = GetMeshFaceArea(i, mesh);
                    double faceIrradiance = 0.0;
                    double irradianceVertex1 = hourlyIrradiance[mesh.Faces[i].A, t];
                    double irradianceVertex2 = hourlyIrradiance[mesh.Faces[i].B, t];
                    double irradianceVertex3 = hourlyIrradiance[mesh.Faces[i].C, t];
                    if (mesh.Faces[i].IsQuad)
                    {
                        double irradianceVertex4 = hourlyIrradiance[mesh.Faces[i].D, t];
                        faceIrradiance = ((irradianceVertex1 + irradianceVertex2 + irradianceVertex3 + irradianceVertex4) / 4) * meshFaceAreas[i];
                    }
                    else
                    {
                        faceIrradiance = ((irradianceVertex1 + irradianceVertex2 + irradianceVertex3) / 3) * meshFaceAreas[i];
                    }

                    totalIrradiance += faceIrradiance;
                }
                meanIrradiance[t] = totalIrradiance / surfaceArea;
            }

            // source: http://james-ramsden.com/area-of-a-mesh-face-in-c-in-grasshopper/
            double GetMeshFaceArea(int _meshFaceIndex, Mesh _mesh)
            {
                // get points into a nice, concise format
                Point3d pt0 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].A];
                Point3d pt1 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].B];
                Point3d pt2 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].C];

                // calculate areas of triangles
                double a = pt0.DistanceTo(pt1);
                double b = pt1.DistanceTo(pt2);
                double c = pt2.DistanceTo(pt0);
                double p = 0.5 * (a + b + c);
                double area1 = Math.Sqrt(p * (p - a) * (p - b) * (p - c));

                // if quad, calc area of second triangle
                double area2 = 0.0;
                if(_mesh.Faces[_meshFaceIndex].IsQuad)
                {
                    Point3d pt3 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].D];
                    a = pt0.DistanceTo(pt2);
                    b = pt2.DistanceTo(pt3);
                    c = pt3.DistanceTo(pt0);
                    p = 0.5 * (a + b + c);
                    area2 = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
                }

                return area1 + area2;
            }

            return meanIrradiance;
        }
    }


    /// <summary>
    /// Photovoltaic
    /// </summary>
    public class Photovoltaic : SurfaceBasedTech
    {
        /// <summary>
        /// Temperature coefficient (unitless). Default is 0.004
        /// </summary>
        public double Beta { get; private set; }
        /// <summary>
        /// Nominal Operating Cell Temperature (deg C). Defalt is 45.0
        /// </summary>
        public double NOCT { get; private set; }
        /// <summary>
        /// Reference temperature (deg C). Default is 20.0
        /// </summary>
        public double NOCT_ref { get; private set; }
        /// <summary>
        /// Reference irradiance (W/sqm). Default is 800.0
        /// </summary>
        public double NOCT_sol { get; private set; }

        /// <summary>
        /// Performance Ratio [0.0, 1.0]
        /// </summary>
        public double PR { get; private set; }

        /// <summary>
        /// PV module efficiency [0.0, 1.0]
        /// </summary>
        public double RefEfficiencyElectric { get; private set; }

        public Photovoltaic(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric)
            : base(investmentCost, embodiedGhg, false, false, true, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "Photovoltaic";
            this.RefEfficiencyElectric = refEfficiencyElectric;

            this.Beta = 0.004;
            this.NOCT = 45.0;
            this.NOCT_ref = 20.0;
            this.NOCT_sol = 800.0;

            this.PR = 1.0;

            base.CapacityUnit = "kW_peak";
            base.Capacity = refEfficiencyElectric * base.SurfaceArea; // kW_peak, assuming 1kW per m^2 solar irradiance
        }


        /// <summary>
        /// Set technology parameters (see: 10.1016/j.apenergy.2019.03.177)
        /// <remarks>To be set by a grasshopper component (e.g. via windows form)</remarks>
        /// </summary>
        /// <param name="beta">Temperature coefficient [-]</param>
        /// <param name="noct">Nominal operating cell temperature [deg C]</param>
        /// <param name="noct_ref">Reference temperature in [deg C]</param>
        /// <param name="noct_sol">Reference irradiance in [W/m^2]</param>
        public void SetNOCTParameters(double beta, double noct, double noct_ref, double noct_sol)
        {
            this.Beta = beta;
            this.NOCT = noct;
            this.NOCT_ref = noct_ref;
            this.NOCT_sol = noct_sol;
        }


        /// <summary>
        /// computes pv electricity yield according to NOCT method
        /// </summary>
        /// <param name="irradiance">Irradiance matrix from e.g. GHSolar.CResults.I_hourly in W/sqm</param>
        /// <param name="ambientTemperatureCarrier">Ambient air temperature energy carrier</param>
        public void SetInputComputeOutput(Matrix irradiance, Air ambientTemperatureCarrier)
        {
            int horizon = 8760;
            double refTemp = 25.0;

            double [] ambientTemp = ambientTemperatureCarrier.AvailableEnergy;
            double [] electricityGenerated = new double[horizon];
            double [] energyCost = new double[horizon];
            double [] ghgEmissions = new double[horizon];
            double [] meanIrradiance = SurfaceBasedTech.ComputeMeanHourlyEnergy(irradiance, this);
            base.InputCarrier = new Radiation(horizon, meanIrradiance);

            // compute pv electricity yield
            for (int t=0; t<horizon; t++)
            {
                double tempPV = ambientTemp[t] + ((this.NOCT - this.NOCT_ref) / NOCT_sol) * meanIrradiance[t];
                double eta = this.RefEfficiencyElectric * (1 - this.Beta * (tempPV - refTemp));
                electricityGenerated[t] = this.SurfaceArea * eta * meanIrradiance[t] / 1000.0; // in kWh/m^2
            }

            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = new Electricity(horizon, electricityGenerated, energyCost, ghgEmissions);
        }


        /// <summary>
        /// Computes PV electricity yield according to Energie und Klimasysteme lecture FS2019
        /// E_PV = G * F_F * A * eta_PV * PR
        /// </summary>
        /// <param name="irradiance">Irradiance matrix from e.g. GHSolar.CResults.I_hourly in W/sqm</param>
        public void SetInputComputeOutputSimple(Matrix irradiance)
        {
            int horizon = 8760;
            double[] meanIrradiance = SurfaceBasedTech.ComputeMeanHourlyEnergy(irradiance, this);
            base.InputCarrier = new Radiation(horizon, meanIrradiance);

            // compute pv electricity yield
            double[] electricityGenerated = new double[horizon];
            for (int t=0; t<horizon; t++)
            {
                electricityGenerated[t] = meanIrradiance[t] * this.SurfaceArea * this.RefEfficiencyElectric * this.PR;
            }

            // empty, because renewable energy
            double[] energyCost = new double[horizon];
            double[] ghgEmissions = new double[horizon];

            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = new Electricity(horizon, electricityGenerated, energyCost, ghgEmissions);
        }
    }
    

    /// <summary>
    /// Solar Thermal
    /// </summary>
    public class SolarThermal : SurfaceBasedTech
    {
        /// <summary>
        /// Inlet Water into the collector. Assume it is the return temperature from the heat emitter (e.g. radiator or floor heating)
        /// </summary>
        public Water InletWater { get; private set; }
        /// <summary>
        /// Optical efficiency [-]
        /// </summary>
        public double FRtauAlpha { get; private set; }
        /// <summary>
        /// Heat lost coefficient [W/(m^2 K)]
        /// </summary>
        public double FRUL { get; private set; }

        /// <summary>
        /// Distribution loss coefficient [0.0, 1.0]
        /// </summary>
        public double R_V { get; private set; }
        /// <summary>
        /// Thermal efficiency of collector [0.0, 1.0]
        /// </summary>
        public double RefEfficiencyHeating { get; private set; }

        public SolarThermal(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyHeating)
            : base(investmentCost, embodiedGhg, true, false, false, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "SolarThermal";
            this.RefEfficiencyHeating = refEfficiencyHeating;

            this.FRtauAlpha = 0.68;
            this.FRUL = 4.9;

            this.R_V = 0.95;

            base.CapacityUnit = "kW_peak";
            base.Capacity = base.SurfaceArea * refEfficiencyHeating; // kW_peak, assuming 1 kW/m^2 solar irradiance
        }


        /// <summary>
        /// Setting technology parameters for inflow water temperature dependant method (see: https://doi.org/10.1016/j.apenergy.2016.07.055)
        /// <remarks>To be set by a grasshopper component (e.g. via windows form)</remarks>
        /// </summary>
        /// <param name="_FRUL">F_R U_L. Heat loss coefficient [W/(m^2 K)]</param>
        /// <param name="_FRtauAlpha">F_R(tau alpha). Optical efficiency [-]</param>
        public void SetTechnologyParameters(double _FRUL, double _FRtauAlpha)
        {
            this.FRUL = _FRUL;
            this.FRtauAlpha = _FRtauAlpha;
        }

        /// <summary>
        /// Setting technology parameters for simple heating energy calculation
        /// </summary>
        /// <param name="_eta_K"></param>
        /// <param name="_R_V"></param>
        public void SetTechnologyParametersSimple(double _eta_K, double _R_V)
        {
            this.RefEfficiencyHeating = _eta_K;
            this.R_V = _R_V;
        }


        /// <summary>
        /// Computing heating energy according to Energy und Klimasysteme Lectures FS 2019
        /// Q_th = G * F_F * A * eta_K * R_V
        /// </summary>
        /// <param name="irradiance">Matrix with annual hourly time resolved irradiance values for each mesh vertex in W/m^2</param>
        public void SetInputComputeOutputSimple(Matrix irradiance)
        {
            int horizon = 8760;
            double[] meanIrradiance = SurfaceBasedTech.ComputeMeanHourlyEnergy(irradiance, this);
            Radiation solarCarrier = new Radiation(horizon, meanIrradiance);
            base.InputCarrier = solarCarrier;

            base.OutputCarriers = new EnergyCarrier[1];
            double[] availableEnergy = new double[8760];
            for (int t=0; t<horizon; t++)
                availableEnergy[t] = solarCarrier.AvailableEnergy[t] * this.SurfaceArea * this.RefEfficiencyHeating * this.R_V / 1000.0; // in kWh/m^2

            // all zero, because renewable energy
            double[] energyCost = new double[8760];
            double[] ghgEmissions = new double[8760];

            // not included in this equation...
            double[] supplyTemperature = new double[8760];

            Water supplyWaterCarrier = new Water(horizon, availableEnergy, energyCost, ghgEmissions, supplyTemperature);
            base.OutputCarriers[0] = supplyWaterCarrier;
        }


        /// <summary>
        /// Computing heating energy as a function of ambient air temperature and inlet water temperature.
        /// </summary>
        /// <remarks>Source: https:// doi.org/10.1016/j.enpol.2013.05.009</remarks>
        /// <param name="irradiance">Rhino.Geometry.Matrix containing annual hourly time series of solar irradiance for each mesh vertex in W/m^2</param>
        /// <param name="inletWaterCarrier">Water carrier flowing into the solar thermal collector</param>
        /// <param name="ambientAirCarrier">Ambient Air carrier around the solar thermal collector</param>
        public void SetInputComputeOutput(Matrix irradiance, Water inletWaterCarrier, Air ambientAirCarrier)
        {
            int horizon = 8760;
            this.InletWater = inletWaterCarrier;

            double[] meanIrradiance = SurfaceBasedTech.ComputeMeanHourlyEnergy(irradiance, this);
            Radiation solarCarrier = new Radiation(horizon, meanIrradiance);
            base.InputCarrier = solarCarrier;

            base.OutputCarriers = new EnergyCarrier[1];
            double[] availableEnergy = new double[horizon];
            for(int t=0; t<horizon; t++)
            {
                double etaTemp = Math.Max(0, this.FRtauAlpha - ((this.FRUL * (inletWaterCarrier.SupplyTemperature[t] - ambientAirCarrier.AvailableEnergy[t])) / meanIrradiance[t]));
                availableEnergy[t] = meanIrradiance[t] * etaTemp * this.SurfaceArea / 1000.0;
            }

            // zeros, because renewable energy and because equation doesnt calc output temperature
            double[] energyCost = new double[horizon];
            double[] ghgEmissions = new double[horizon];
            double[] supplyTemperature = new double[horizon];
            base.OutputCarriers[0] = new Water(horizon, availableEnergy, energyCost, ghgEmissions, supplyTemperature);
        }
    }


    /// <summary>
    /// Hybrid Solar Photovolatic Thermal
    /// </summary>
    public class PVT : SurfaceBasedTech
    {
        public double RefEfficiencyElectric { get; }
        public double RefEfficiencyHeating { get; }

        public PVT(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric, double refEfficiencyHeating)
            : base(investmentCost, embodiedGhg, true, false, true, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "PVT";

            this.RefEfficiencyElectric = refEfficiencyElectric;
            this.RefEfficiencyHeating = refEfficiencyHeating;
        }


        public double[] SetConversionEfficiencyHeating()
        {
            return new double[] { };
        }

        public double[] SetConversionEfficiencyElectricity()
        {
            return new double[] { };
        }
    }


    /// <summary>
    /// Horizontal Ground Solar Collector
    /// </summary>
    public class GroundCollector : SurfaceBasedTech
    {
        public GroundCollector(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName)
            : base(investmentCost, embodiedGhg, true, false, false, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "GroundCollector";
        }


        public double[] SetConversionEfficiencyHeating()
        {
            return new double[] { };
        }


    }
    #endregion



    #region Combustion technology
    public abstract class CombustionTech : ConversionTech
    {
        protected CombustionTech(double investmentCost, double embodiedGhg, double capacity, bool isHeating, bool isElectric) 
            : base(investmentCost, embodiedGhg, capacity, "kW", isHeating, false, isElectric)
        {
        }

        protected abstract double[] SetConversionEfficiencyHeating();
    }


    public class GasBoiler : CombustionTech
    {
        public double Efficiency { get; private set; }
        public GasBoiler(double investmentCost, double embodiedGhg, double capacity, double efficiency) 
            : base(investmentCost, embodiedGhg, capacity, true, false)
        {
            this.Efficiency = efficiency;
            base.Name = "GasBoiler";
        }


        protected override double[] SetConversionEfficiencyHeating()
        {
            throw new System.NotImplementedException();
        }


        //where is gas defined?? for solar, its clear, its from a weather file...
        // so all Input Carriers should be part of Hive.IO.Environment!
        // indicate, whether we have biogas, wood pellets, district heating, electricity grid, natural gas, oil, etc
        public void SetInput(NaturalGas gasInput)
        {
            base.InputCarrier = gasInput;
        }


        /// <summary>
        /// all these parameters are computed externally, with some simulator within the Core
        /// </summary>
        /// <param name="horizon"></param>
        /// <param name="availableEnergy"></param>
        /// <param name="energyCost"></param>
        /// <param name="ghgEmissions"></param>
        /// <param name="supplyTemperature"></param>
        public void SetOutput(int horizon, double [] availableEnergy, double [] energyCost, double [] ghgEmissions, double [] supplyTemperature)
        {
            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = new Water(horizon, availableEnergy, energyCost, ghgEmissions, supplyTemperature);
        }
    }


    public class CombinedHeatPower : CombustionTech
    {
        public double HeatToPowerRatio { get; private set; }
        public double ElectricEfficiency { get; private set; }
        public CombinedHeatPower(double investmentCost, double embodiedGhg, double capacityElectric, double heatToPowerRatio, double electricEfficiency) 
            : base(investmentCost, embodiedGhg, capacityElectric, true, true)
        {
            this.HeatToPowerRatio = heatToPowerRatio;
            this.ElectricEfficiency = electricEfficiency;
            base.Name = "CombinedHeatPower";
        }

        protected override double[] SetConversionEfficiencyHeating()
        {
            return new double[] { };
        }

        public double[] SetConversionEfficiencyElectricity()
        {
            return new double[]{};
        }


    }
    #endregion



    #region Base Conversion Class
    /// <summary>
    /// Heating, Cooling, Electricity generation systems
    /// E.g. CHP, boiler, heat pump, chiller, PV ...
    /// </summary>
    public abstract class ConversionTech
    {
        /// <summary>
        /// Technology name
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Specification of the technology, e.g. "Mono-cristalline PV"
        /// </summary>
        public string DetailedName { get; protected set; }


        /// <summary>
        /// Indicating whether this technology produces electricity
        /// </summary>
        public bool IsElectric { get; protected set; }
        /// <summary>
        /// Indicating whether this technology produces heat
        /// </summary>
        public bool IsHeating { get; protected set; }
        /// <summary>
        /// Indicating whether this technology produces cooling
        /// </summary>
        public bool IsCooling { get; protected set; }


        /// <summary>
        /// Capacity of technology. Unit is defined in 'CapacityUnit'
        /// </summary>
        public double Capacity { get; protected set; }

        /// <summary>
        /// Capacity unit of technology capacity (e.g. "kW", "kWh", "kW_peak", etc.)
        /// </summary>
        public string CapacityUnit { get; protected set; }

        /// <summary>
        /// Investment cost per this.CapacityUnit
        /// </summary>
        public double SpecificInvestmentCost { get; protected set; }
        /// <summary>
        /// Life cycle GHG emissions, in kgCO2eq. per this.CapacityUnit
        /// </summary>
        public double SpecificEmbodiedGhg { get; protected set; }


        /// <summary>
        /// Input stream. e.g. for a CHP this could be 'NaturalGas'
        /// </summary>
        public EnergyCarrier InputCarrier { get; protected set; }
        /// <summary>
        /// Output streams. e.g. for a CHP this could be 'Heating' and 'Electricity'
        /// </summary>
        public EnergyCarrier[] OutputCarriers { get; protected set; }



        protected ConversionTech(double investmentCost, double embodiedGhg,
            double capacity, string capacityUnity,
            bool isHeating, bool isCooling, bool isElectric)
        {
            this.SpecificInvestmentCost = investmentCost;
            this.SpecificEmbodiedGhg = embodiedGhg;
            this.Capacity = capacity;
            this.CapacityUnit = capacityUnity;
            this.IsHeating = isHeating;
            this.IsCooling = isCooling;
            this.IsElectric = isElectric;
        }


        // how can I change parameters of methods in derived classes? the argument should still be an EnergyCarrier, but I wanna specifiy, e.g. restricting to Solar

        ///// <summary>
        ///// Not part of the constructor, because it can be set at a later stage of the program
        ///// For example, for PV, solar potentials need to be calculated first, which might happen after a PV object has been instantiated
        ///// </summary>
        ///// <param name="inputCarrier"></param>
        //public virtual void SetInput(EnergyCarrier inputCarrier) { }
        ///// <summary>
        ///// Same as with inputs, the outputs might be calculated later after a Conversion class has been instantiated
        ///// </summary>
        ///// <param name="outputCarriers"></param>
        //public virtual void SetOutputs(EnergyCarrier[] outputCarriers) { }
    }


    #endregion
}
