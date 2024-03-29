﻿using System;
using Rhino.Geometry;


namespace Hive.IO.EnergySystems
{
    /// <summary>
    /// Surface based energy technologies, such as PV, solar thermal, PVT, ground collectors, etc.
    /// </summary>
    public abstract class SurfaceBasedTech : ConversionTech
    {
        private static object meshFaceArea;

        /// <summary>
        /// Rhino mesh geometry object representing the energy system. Can be quad or triangles.
        /// </summary>
        public Mesh SurfaceGeometry { get; private set; }

        /// <summary>
        /// Surface Area, computed using Rhino.Geometry
        /// </summary>
        public double SurfaceArea { get; private set; }

        /// <summary>
        /// Ambient Air temperature, required for efficiency calculations
        /// </summary>
        public Air AmbientAir { get; protected set; }

        public double CostPerSquareMeter { get; protected set; }
        public double EmissionsPerSquareMeter { get; protected set; }

        /// <summary>
        /// Performance Ratio [0.0, 1.0]
        /// </summary>
        public double PR { get; set; }

        /// <summary>
        /// Surface Transmittance [0.0, 1.0]
        /// </summary>
        public double f_cover { get; set; }

        public new double InvestmentCost => CostPerSquareMeter * SurfaceArea;
        public new double EmbodiedGhg => EmissionsPerSquareMeter * SurfaceArea;


        protected SurfaceBasedTech(double investmentCost, double embodiedGhg, double lifetime, bool isHeating, bool isCooling, bool isElectric, Mesh surfaceGeometry, double PR, double f_cover)
            : base(investmentCost, embodiedGhg, lifetime, 0.0, "undefined", isHeating, isCooling, isElectric)
        {
            this.SurfaceGeometry = surfaceGeometry;
            this.SurfaceArea = Rhino.Geometry.AreaMassProperties.Compute(this.SurfaceGeometry).Area;
        }


        /// <summary>
        /// Computes the mean hourly irradiance per mesh face, taking values from the mesh face vertices
        /// </summary>
        /// <param name="hourlyIrradiance"></param>
        /// <param name="surfaceTech"></param>
        /// <returns></returns>
        protected double[] ComputeMeanHourlyEnergy(Matrix hourlyIrradiance)
        {
            int meshFacesCount = SurfaceGeometry.Faces.Count;
            int horizon = hourlyIrradiance.ColumnCount;
            double[] meanIrradiance = new double[horizon];


            double[] meshFaceAreas = new double[meshFacesCount];
            for (int t = 0; t < horizon; t++)
            {
                double totalIrradiance = 0.0;
                for (int i = 0; i < meshFacesCount; i++)
                {
                    meshFaceAreas[i] = Misc.GetMeshFaceArea(i, SurfaceGeometry);
                    double faceIrradiance = 0.0;
                    double irradianceVertex1 = hourlyIrradiance[SurfaceGeometry.Faces[i].A, t];
                    double irradianceVertex2 = hourlyIrradiance[SurfaceGeometry.Faces[i].B, t];
                    double irradianceVertex3 = hourlyIrradiance[SurfaceGeometry.Faces[i].C, t];
                    if (SurfaceGeometry.Faces[i].IsQuad)
                    {
                        double irradianceVertex4 = hourlyIrradiance[SurfaceGeometry.Faces[i].D, t];
                        faceIrradiance = ((irradianceVertex1 + irradianceVertex2 + irradianceVertex3 + irradianceVertex4) / 4) * meshFaceAreas[i];
                    }
                    else
                    {
                        faceIrradiance = ((irradianceVertex1 + irradianceVertex2 + irradianceVertex3) / 3) * meshFaceAreas[i];
                    }

                    totalIrradiance += faceIrradiance;
                }

                double temp = totalIrradiance / SurfaceArea;
                if (double.IsNaN(temp))
                    temp = 0.0;
                meanIrradiance[t] = temp;
            }


            return meanIrradiance;
        }


        // use this to more precisely compute solar energy generation of non-homogeneous solar surfaces (needs to be updated in SurfaceBasedTech.SetInputComputOutput)
        protected static double[] ComputeMeanHourlyEnergyPerFace(Matrix hourlyIrradiance, Mesh mesh, int faceIndex)
        {
            int horizon = hourlyIrradiance.ColumnCount;
            double[] meanIrradiance = new double[horizon];

            for (int t = 0; t < horizon; t++)
            {
                double meshFaceArea = Misc.GetMeshFaceArea(faceIndex, mesh);
                double faceIrradiance = 0.0;
                double irradianceVertex1 = hourlyIrradiance[mesh.Faces[faceIndex].A, t];
                double irradianceVertex2 = hourlyIrradiance[mesh.Faces[faceIndex].B, t];
                double irradianceVertex3 = hourlyIrradiance[mesh.Faces[faceIndex].C, t];
                if (mesh.Faces[faceIndex].IsQuad)
                {
                    double irradianceVertex4 = hourlyIrradiance[mesh.Faces[faceIndex].D, t];
                    faceIrradiance = ((irradianceVertex1 + irradianceVertex2 + irradianceVertex3 + irradianceVertex4) / 4) * meshFaceArea;
                }
                else
                {
                    faceIrradiance = ((irradianceVertex1 + irradianceVertex2 + irradianceVertex3) / 3) * meshFaceArea;
                }

                faceIrradiance /= meshFaceArea;
                if (double.IsNaN(faceIrradiance))
                    faceIrradiance = 0.0;
                meanIrradiance[t] = faceIrradiance;
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
        /// PV module efficiency [0.0, 1.0]
        /// </summary>
        public double RefEfficiencyElectric { get; private set; }
        

        public Photovoltaic(double investmentCost, double embodiedGhg, double lifetime, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric, double PR, double f_cover)
            : base(investmentCost, embodiedGhg, lifetime, false, false, true, surfaceGeometry, PR, f_cover)
        {
            base.DetailedName = detailedName;
            base.Name = "Photovoltaic";
            this.RefEfficiencyElectric = refEfficiencyElectric;

            this.Beta = 0.004;
            this.NOCT = 45.0;
            this.NOCT_ref = 20.0;
            this.NOCT_sol = 800.0;

            base.PR = PR;
            base.f_cover = f_cover;

            base.CapacityUnit = "kW_peak";
            base.Capacity = refEfficiencyElectric * base.SurfaceArea; // kW_peak, assuming 1kW per m^2 solar irradiance
            base.CostPerSquareMeter = SpecificInvestmentCost * refEfficiencyElectric;
            base.EmissionsPerSquareMeter = refEfficiencyElectric * embodiedGhg;
        }


        /// <summary>
        /// Set technology parameters (see: 10.1016/j.apenergy.2019.03.177)
        /// <remarks>To be set by a grasshopper component (e.g. via windows form)</remarks>
        /// </summary>
        /// <param name="beta">Temperature coefficient [-]</param>
        /// <param name="noct">Nominal operating cell temperature [deg C]</param>
        /// <param name="noct_ref">Reference temperature in [deg C]</param>
        /// <param name="noct_sol">Reference irradiance in [W/m^2]</param>
        public void SetTechnologyParameters(double beta, double noct, double noct_ref, double noct_sol)
        {
            this.Beta = beta;
            this.NOCT = noct;
            this.NOCT_ref = noct_ref;
            this.NOCT_sol = noct_sol;
        }

        public void SetTechnologyParametersSimple(double performanceRatio, double referenceEfficiency, double f_cover)
        {
            this.PR = performanceRatio;
            this.RefEfficiencyElectric = referenceEfficiency;
            this.f_cover = f_cover;
        }


        /// <summary>
        /// computes pv electricity yield according to NOCT method
        /// </summary>
        /// <param name="irradiance">Irradiance matrix from e.g. GHSolar.CResults.I_hourly in W/sqm. The mean for all vertices is calculated first.</param>
        /// <param name="ambientTemperatureCarrier">Ambient air temperature energy carrier</param>
        public void SetInputComputeOutput(Matrix irradiance, Air ambientTemperatureCarrier)
        {
            double[] meanIrradiance = ComputeMeanHourlyEnergy(irradiance);
            SetInputComputeOutput(meanIrradiance, ambientTemperatureCarrier);
        }

        /// <summary>
        /// computes pv electricity yield according to NOCT method
        /// </summary>
        /// <param name="meanIrradiance">Irradiance time series for the PV</param>
        /// <param name="ambientTemperatureCarrier">Ambient air temperature energy carrier</param>
        public void SetInputComputeOutput(double[] meanIrradiance, Air ambientTemperatureCarrier)
        {
            int horizon = Misc.HoursPerYear;
            double refTemp = 25.0;

            double[] ambientTemp = ambientTemperatureCarrier.Temperature;
            double[] electricityGenerated = new double[horizon];
            double[] energyCost = new double[horizon];
            double[] ghgEmissions = new double[horizon];
            base.InputCarrier = new Radiation(horizon, meanIrradiance);
            base.AmbientAir = new Air(horizon, null, null, null, ambientTemp);

            // compute pv electricity yield
            for (int t = 0; t < horizon; t++)
            {
                double tempPV = ambientTemp[t] + ((this.NOCT - this.NOCT_ref) / NOCT_sol) * meanIrradiance[t];
                double eta = this.RefEfficiencyElectric * (1 - this.Beta * (tempPV - refTemp)) * this.PR * this.f_cover;
                electricityGenerated[t] = this.SurfaceArea * eta * meanIrradiance[t] / 1000.0; // in kWh/m^2
            }

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Electricity(horizon, electricityGenerated, energyCost, ghgEmissions, 1.0);
        }


        /// <summary>
        /// Computes PV electricity yield according to Energie und Klimasysteme lecture FS2019
        /// E_PV = G * F_F * A * eta_PV * PR
        /// </summary>
        /// <param name="irradiance">Irradiance matrix from e.g. GHSolar.CResults.I_hourly in W/sqm</param>
        public void SetInputComputeOutputSimple(Matrix irradiance)
        {
            double[] meanIrradiance = ComputeMeanHourlyEnergy(irradiance);
            SetInputComputeOutputSimple(meanIrradiance);
        }

        /// <summary>
        /// Computes PV electricity yield according to Energie und Klimasysteme lecture FS2019
        /// E_PV = G * F_F * A * eta_PV * PR
        /// </summary>
        /// <param name="irradiance">Irradiance time series in W/sqm</param>
        public void SetInputComputeOutputSimple(double [] meanIrradiance)
        {
            int horizon = Misc.HoursPerYear;
            base.InputCarrier = new Radiation(horizon, meanIrradiance);

            // compute pv electricity yield
            double[] electricityGenerated = new double[horizon];
            for (int t = 0; t < horizon; t++)
            {
                electricityGenerated[t] = meanIrradiance[t] * this.SurfaceArea * this.RefEfficiencyElectric * this.PR * this.f_cover / 1000.0 ; // in kWh/m^2
            }

            // empty, because renewable energy
            double[] energyCost = new double[horizon];
            double[] ghgEmissions = new double[horizon];

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Electricity(horizon, electricityGenerated, energyCost, ghgEmissions, 1.0);
        }
    }

    /// <summary>
    /// Buildung Integrated PV
    /// </summary>

    public class BuildingIntegratedPV : Photovoltaic
    {
       
        public BuildingIntegratedPV(double investmentCost, double embodiedGhg, double lifetime, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric, double PR, double f_cover)
            : base(investmentCost, embodiedGhg, lifetime, surfaceGeometry,detailedName,refEfficiencyElectric, PR, f_cover)
        {
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

        public SolarThermal(double investmentCost, double embodiedGhg, double lifetime, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyHeating, double PR, double f_cover)
            : base(investmentCost, embodiedGhg, lifetime, true, false, false, surfaceGeometry, PR, f_cover)
        {
            base.DetailedName = detailedName;
            base.Name = "SolarThermal";
            this.RefEfficiencyHeating = refEfficiencyHeating;

            this.FRtauAlpha = 0.68;
            this.FRUL = 4.9;

            base.PR = PR;
            base.f_cover = f_cover;

            this.R_V = 0.95;

            base.CapacityUnit = "kW_peak";
            base.Capacity = base.SurfaceArea * refEfficiencyHeating; // kW_peak, assuming 1 kW/m^2 solar irradiance
            base.CostPerSquareMeter = investmentCost * refEfficiencyHeating;
            base.EmissionsPerSquareMeter = refEfficiencyHeating * embodiedGhg;
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
            double[] meanIrradiance = ComputeMeanHourlyEnergy(irradiance);
            SetInputComputeOutputSimple(meanIrradiance);
        }

        /// <summary>
        /// Computing heating energy according to Energy und Klimasysteme Lectures FS 2019
        /// Q_th = G * F_F * A * eta_K * R_V
        /// </summary>
        /// <param name="irradiance">Time series with annual hourly time resolved irradiance values in W/m^2</param>
        public void SetInputComputeOutputSimple(double [] meanIrradiance)
        {
            int horizon = Misc.HoursPerYear;
            Radiation solarCarrier = new Radiation(horizon, meanIrradiance);
            base.InputCarrier = solarCarrier;

            base.OutputCarriers = new Carrier[1];
            double[] availableEnergy = new double[8760];
            for (int t = 0; t < horizon; t++)
            {
                double temp = solarCarrier.Energy[t] * this.SurfaceArea * this.RefEfficiencyHeating * this.R_V / 1000.0; // in kWh/m^2
                temp = temp * this.PR * this.f_cover; //consider Performance Ratio and f_cover, default values are 1.0
                availableEnergy[t] = double.IsNaN(temp) ? 0.0 : temp;
            }

            // all zero, because renewable energy
            double[] energyCost = new double[8760];
            double[] ghgEmissions = new double[8760];

            // not included in this equation...
            double[] supplyTemperature = new double[8760];

            Water supplyWaterCarrier = new Water(horizon, availableEnergy, energyCost, ghgEmissions, supplyTemperature, 1.0);
            base.OutputCarriers[0] = supplyWaterCarrier;
        }


        /// <summary>
        /// Computing heating energy as a function of ambient air temperature and inlet water temperature.
        /// </summary>
        /// <remarks>Source: https:// doi.org/10.1016/j.enpol.2013.05.009</remarks>
        /// <param name="irradiance">Rhino.Geometry.Matrix containing annual hourly time series of solar irradiance for each mesh vertex in W/m^2</param>
        /// <param name="inletTemperature">Water carrier flowing into the solar thermal collector</param>
        /// <param name="returnTemperature">outlet temperatur from the collector. For now, assumed it is just the supply temperature of the room heating emitter</param>
        /// <param name="ambientAirCarrier">Ambient Air carrier around the solar thermal collector</param>
        public void SetInputComputeOutput(Matrix irradiance, double[] inletTemperature, double[] returnTemperature, Air ambientAirCarrier)
        {
            double[] meanIrradiance = ComputeMeanHourlyEnergy(irradiance);
            SetInputComputeOutput(meanIrradiance, inletTemperature, returnTemperature, ambientAirCarrier);
        }

        /// <summary>
        /// Computing heating energy as a function of ambient air temperature and inlet water temperature.
        /// </summary>
        /// <remarks>Source: https:// doi.org/10.1016/j.enpol.2013.05.009</remarks>
        /// <param name="irradiance">Rhino.Geometry.Matrix containing annual hourly time series of solar irradiance for each mesh vertex in W/m^2</param>
        /// <param name="inletTemperature">Water carrier flowing into the solar thermal collector</param>
        /// <param name="returnTemperature">outlet temperatur from the collector. For now, assumed it is just the supply temperature of the room heating emitter</param>
        /// <param name="ambientAirCarrier">Ambient Air carrier around the solar thermal collector</param>
        public void SetInputComputeOutput(double [] meanIrradiance, double[] inletTemperature, double[] returnTemperature, Air ambientAirCarrier)
        {
            int horizon = Misc.HoursPerYear;
            this.InletWater = new Water(horizon, null, null, null, returnTemperature, 1.0);

            base.AmbientAir = ambientAirCarrier;

            Radiation solarCarrier = new Radiation(horizon, meanIrradiance);
            base.InputCarrier = solarCarrier;

            base.OutputCarriers = new Carrier[1];
            double[] heatingEnergy = new double[horizon];
            for (int t = 0; t < horizon; t++)
            {
                double etaTemp = Math.Max(0, this.FRtauAlpha - ((this.FRUL * (inletTemperature[t] - ambientAirCarrier.Temperature[t])) / meanIrradiance[t]));
                double temp = meanIrradiance[t] * etaTemp * this.SurfaceArea / 1000.0;
                temp = temp * this.PR * this.f_cover; //consider Performance Ratio and f_cover, default values are 1.0
                heatingEnergy[t] = (double.IsNaN(temp) || temp < 0.0) ? 0.0 : temp;
            }

            // zeros, because renewable energy and because equation doesnt calc output temperature. Supply temp, I'd need to calculate according to: See ShanShans mail from 21.8.2020
            double[] energyCost = new double[horizon];
            double[] ghgEmissions = new double[horizon];
            base.OutputCarriers[0] = new Water(horizon, heatingEnergy, energyCost, ghgEmissions, returnTemperature, 1.0);
        }
    }


    /// <summary>
    /// Hybrid Solar Photovolatic Thermal
    /// </summary>
    public class PVT : SurfaceBasedTech
    {
        public double RefEfficiencyElectric { get; }
        public double RefEfficiencyHeating { get; }

        public PVT(double investmentCost, double embodiedGhg, double lifetime, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric, double refEfficiencyHeating, double PR, double f_cover)
            : base(investmentCost, embodiedGhg, lifetime, true, false, true, surfaceGeometry, PR, f_cover)
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
        public GroundCollector(double investmentCost, double embodiedGhg, double lifetime, Mesh surfaceGeometry, string detailedName, double PR, double f_cover)
            : base(investmentCost, embodiedGhg, lifetime, true, false, false, surfaceGeometry, PR, f_cover)
        {
            base.DetailedName = detailedName;
            base.Name = "GroundCollector";
        }


        public double[] SetConversionEfficiencyHeating()
        {
            return new double[] { };
        }


    }
}
