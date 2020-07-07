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

    public class ElectricityGrid : Conversion
    {
        public ElectricityGrid(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }


    }

    public class DistrictHeating : Conversion
    {
        public DistrictHeating(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }


    }


    public class DistrictCooling : Conversion
    {
        public DistrictCooling(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) 
            : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }



    }


    public class Chiller : Conversion
    {
        public Chiller(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) 
            : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }




        public double[] SetConversionEfficiencyCooling()
        {
            return new double[]{};
        }
    }

    #endregion



    #region Surface based technology
    /// <summary>
    /// Surface based energy technologies, such as PV, solar thermal, PVT, ground collectors, etc.
    /// </summary>
    public abstract class SurfaceBased : Conversion
    {
        /// <summary>
        /// Rhino mesh geometry object representing the energy system. Can be quad or triangles.
        /// </summary>
        public Mesh SurfaceGeometry { get; private set; }

        /// <summary>
        /// Surface Area, computed using Rhino.Geometry
        /// </summary>
        public double SurfaceArea { get; private set; }

        protected SurfaceBased(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric, Mesh surfaceGeometry) 
            : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
            this.SurfaceGeometry = surfaceGeometry;
            this.SurfaceArea = Rhino.Geometry.AreaMassProperties.Compute(this.SurfaceGeometry).Area;
        }


        protected static double[] ComputeMeanHourlyEnergy(Matrix hourlyIrradiance, SurfaceBased surfaceTech)
        {
            Mesh mesh = surfaceTech.SurfaceGeometry;
            double surfaceArea = surfaceTech.SurfaceArea;

            int meshFacesCount = mesh.Faces.Count;
            int vertexCount = hourlyIrradiance.RowCount;
            int horizon = hourlyIrradiance.ColumnCount;
            List<double []> allIrradiances = new List<double[]>();
            double[] meanIrradiance = new double[horizon];

            for (int i = 0; i < vertexCount; i++)
            {
                double [] irradiancePerVertex = new double[horizon];
                for (int t = 0; t < horizon; t++)
                {
                    irradiancePerVertex[t] = hourlyIrradiance[i, t];
                }
                allIrradiances.Add(irradiancePerVertex);
            }

            double [] meshFaceAreas = new double[meshFacesCount];
            for (int t = 0; t < horizon; t++)
            {
                double totalIrradiance = 0.0;
                for (int i = 0; i < meshFacesCount; i++)
                {
                    meshFaceAreas[i] = GetMeshFaceArea(i, mesh);
                    double faceIrradiance = 0.0;
                    double irradianceVertex1 = allIrradiances[mesh.Faces[i].A][t];
                    double irradianceVertex2 = allIrradiances[mesh.Faces[i].B][t];
                    double irradianceVertex3 = allIrradiances[mesh.Faces[i].C][t];
                    if (mesh.Faces[i].IsQuad)
                    {
                        double irradianceVertex4 = allIrradiances[mesh.Faces[i].D][t];
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
                List<Point3d> pts = new List<Point3d>();
                pts.Add(_mesh.Vertices[_mesh.Faces[_meshFaceIndex].A]);
                pts.Add(_mesh.Vertices[_mesh.Faces[_meshFaceIndex].B]);
                pts.Add(_mesh.Vertices[_mesh.Faces[_meshFaceIndex].C]);
                if(_mesh.Faces[_meshFaceIndex].IsQuad)
                    pts.Add(_mesh.Vertices[_mesh.Faces[_meshFaceIndex].D]);

                // calculate areas of triangles
                double a = pts[0].DistanceTo(pts[1]);
                double b = pts[1].DistanceTo(pts[2]);
                double c = pts[2].DistanceTo(pts[0]);
                double p = 0.5 * (a + b + c);
                double area1 = Math.Sqrt(p * (p - a) * (p - b) * (p - c));

                // if quad, calc area of second triangle
                double area2 = 0.0;
                if(_mesh.Faces[_meshFaceIndex].IsQuad)
                {
                    a = pts[0].DistanceTo(pts[2]);
                    b = pts[2].DistanceTo(pts[3]);
                    c = pts[3].DistanceTo(pts[0]);
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
    public class Photovoltaic : SurfaceBased
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
        }


        /// <summary>
        /// Set technology parameters (see: 10.1016/j.apenergy.2019.03.177)
        /// <remarks>To be set by a grasshopper component (e.g. via windows form)</remarks>
        /// </summary>
        /// <param name="beta"></param>
        /// <param name="noct"></param>
        /// <param name="noct_ref"></param>
        /// <param name="noct_sol"></param>
        public void SetNOCTParameters(double beta, double noct, double noct_ref, double noct_sol)
        {
            this.Beta = beta;
            this.NOCT = noct;
            this.NOCT_ref = noct_ref;
            this.NOCT_sol = noct_sol;
        }


        /// <summary>
        /// Setting input (solar potentials from a solar model) and output carrier (from a PV electricity model)
        /// </summary>
        /// <param name="solarCarrier">input energy carrier, from weather file or solar model</param>
        /// <param name="electricityCarrier">output electricity carrier from a PV electricity model.</param>
        public void SetInputOutput(Radiation solarCarrier, Electricity electricityCarrier)
        {
            base.InputCarrier = solarCarrier;
            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = electricityCarrier;
        }


        /// <summary>
        /// computes pv electricity yield according to NOCT method
        /// </summary>
        /// <param name="solarCarrier"></param>
        public void SetInputOutput(Matrix irradiance, Air ambientTemperatureCarrier)
        {

            int horizon = 8760;
            double refTemp = 25.0;

            double [] ambientTemp = ambientTemperatureCarrier.AvailableEnergy;
            double [] electricityGenerated = new double[horizon];
            double [] energyCost = new double[horizon];
            double [] ghgEmissions = new double[horizon];
            double [] meanIrradiance = SurfaceBased.ComputeMeanHourlyEnergy(irradiance, this);
            base.InputCarrier = new Radiation(horizon, meanIrradiance);

            // compute pv electricity yield
            for (int t=0; t<horizon; t++)
            {
                double tempPV = ambientTemp[t] + ((this.NOCT - this.NOCT_ref) / NOCT_sol) * meanIrradiance[t];
                double eta = this.RefEfficiencyElectric * (1 - this.Beta * (tempPV - refTemp));
                electricityGenerated[t] = this.SurfaceArea * eta * meanIrradiance[t];
            }


            base.OutputCarriers = new EnergyCarrier[1];
            Electricity electricityCarrier = new Electricity(horizon, electricityGenerated, energyCost, ghgEmissions);
            base.OutputCarriers[0] = electricityCarrier;
        }
    }
    

    /// <summary>
    /// Solar Thermal
    /// </summary>
    public class SolarThermal : SurfaceBased
    {
        /// <summary>
        /// Inlet Water into the collector
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

        public double RefEfficiencyHeating { get; }
        public SolarThermal(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyHeating)
            : base(investmentCost, embodiedGhg, true, false, false, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "SolarThermal";
            this.RefEfficiencyHeating = refEfficiencyHeating;

            this.FRtauAlpha = 0.68;
            this.FRUL = 4.9;
        }


        /// <summary>
        /// Setting technology parameters (see: https://doi.org/10.1016/j.apenergy.2016.07.055)
        /// <remarks>To be set by a grasshopper component (e.g. via windows form)</remarks>
        /// </summary>
        /// <param name="frul">F_R U_L. Heat loss coefficient [W/(m^2 K)]</param>
        /// <param name="frTauAlpha">F_R(tau alpha). Optical efficiency [-]</param>
        public void SetTechnologyParameters(double frul, double frTauAlpha)
        {
            this.FRUL = frul;
            this.FRtauAlpha = frTauAlpha;
        }


        public void SetInputOutput(Radiation solarCarrier, Water inletWaterCarrier, Water supplyWaterCarrier)
        {
            this.InletWater = inletWaterCarrier;
            base.InputCarrier = solarCarrier;
            base.OutputCarriers = new EnergyCarrier[0];
            base.OutputCarriers[0] = supplyWaterCarrier;
        }

    }


    /// <summary>
    /// Hybrid Solar Photovolatic Thermal
    /// </summary>
    public class PVT : SurfaceBased
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
    public class GroundCollector : SurfaceBased
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
    public abstract class Combustion : Conversion
    {
        protected Combustion(double investmentCost, double embodiedGhg, bool isHeating, bool isElectric) 
            : base(investmentCost, embodiedGhg, isHeating, false, isElectric)
        {
        }

        protected abstract double[] SetConversionEfficiencyHeating();
    }


    public class GasBoiler : Combustion
    {
        public GasBoiler(double investmentCost, double embodiedGhg, bool isHeating, bool isElectric) 
            : base(investmentCost, embodiedGhg, isHeating, isElectric)
        {
        }


        protected override double[] SetConversionEfficiencyHeating()
        {
            throw new System.NotImplementedException();
        }


    }


    public class CombinedHeatPower : Combustion
    {
        public CombinedHeatPower(double investmentCost, double embodiedGhg, bool isHeating, bool isElectric) 
            : base(investmentCost, embodiedGhg, isHeating, isElectric)
        {
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
    public abstract class Conversion
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
        /// Unit of technology capacity (e.g. "kW", or "sqm", etc.)
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



        protected Conversion(double investmentCost, double embodiedGhg,
            bool isHeating, bool isCooling, bool isElectric)
        {
            this.SpecificInvestmentCost = investmentCost;
            this.SpecificEmbodiedGhg = embodiedGhg;
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
