using System.Collections.Generic;
using System.Drawing;
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
    }


    /// <summary>
    /// Photovoltaic
    /// </summary>
    public class Photovoltaic : SurfaceBased
    {
        public double RefEfficiencyElectric { get; private set; }
        public Photovoltaic(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric)
            : base(investmentCost, embodiedGhg, false, false, true, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "Photovoltaic";
            this.RefEfficiencyElectric = refEfficiencyElectric;
        }


        /// <summary>
        /// Setting input (solar potentials from a solar model) and output carrier (from a PV electricity model)
        /// </summary>
        /// <param name="solarCarrier">input energy carrier, from weather file or solar model</param>
        /// <param name="electricityCarrier">output electricity carrier from a PV electricity model.</param>
        public void SetInputOutput(Solar solarCarrier, Electricity electricityCarrier)
        {
            base.InputCarrier = solarCarrier;
            base.OutputCarriers = new EnergyCarrier[1];
            base.OutputCarriers[0] = electricityCarrier;
        }


        public void DoSth(double number)
        {
            this.RefEfficiencyElectric = number;
        }

        public void DoSthElse(Solar solarCarrier)
        {
            this.RefEfficiencyElectric = solarCarrier.AvailableEnergy[0];
        }

    }
    

    /// <summary>
    /// Solar Thermal
    /// </summary>
    public class SolarThermal : SurfaceBased
    {
        public double RefEfficiencyHeating { get; }
        public SolarThermal(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyHeating)
            : base(investmentCost, embodiedGhg, true, false, false, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "SolarThermal";
            this.RefEfficiencyHeating = refEfficiencyHeating;
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
