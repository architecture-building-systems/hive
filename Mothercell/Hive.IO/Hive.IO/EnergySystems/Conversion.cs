using System.Collections.Generic;
using System.Drawing;
using Rhino.Geometry;

namespace Hive.IO.EnergySystems
{
    #region MiscSupply

    public class ElectricityGrid : Conversion
    {
        public ElectricityGrid(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
        }
    }

    public class DistrictHeating : Conversion
    {
        public DistrictHeating(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
        }
    }


    public class DistrictCooling : Conversion
    {
        public DistrictCooling(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) 
            : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
        }

    }


    public class Chiller : Conversion
    {
        public Chiller(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric) 
            : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
        }

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
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


        protected SurfaceBased(double investmentCost, double embodiedGhg, bool isHeating, bool isCooling, bool isElectric, Mesh surfaceGeometry) 
            : base(investmentCost, embodiedGhg, isHeating, isCooling, isElectric)
        {
            this.SurfaceGeometry = surfaceGeometry;
        }
    }


    /// <summary>
    /// Photovoltaic
    /// </summary>
    public class Photovoltaic : SurfaceBased
    {
        public Photovoltaic(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric)
            : base(investmentCost, embodiedGhg, false, false, true, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "Photovoltaic";
            base.RefEfficiencyElectric = refEfficiencyElectric;
        }

        public double[] SetConversionEfficiencyElectricity()
        {
            return new double[] { };
        }

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
        }
    }


    /// <summary>
    /// Solar Thermal
    /// </summary>
    public class SolarThermal : SurfaceBased
    {
        public SolarThermal(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyHeating)
            : base(investmentCost, embodiedGhg, true, false, false, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "SolarThermal";
            base.RefEfficiencyHeating = refEfficiencyHeating;
        }


        public double[] SetConversionEfficiencyHeating()
        {
            return new double[] { };
        }

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
        }
    }


    /// <summary>
    /// Hybrid Solar Photovolatic Thermal
    /// </summary>
    public class PVT : SurfaceBased
    {
        public PVT(double investmentCost, double embodiedGhg, Mesh surfaceGeometry, string detailedName,
            double refEfficiencyElectric, double refEfficiencyHeating)
            : base(investmentCost, embodiedGhg, true, false, true, surfaceGeometry)
        {
            base.DetailedName = detailedName;
            base.Name = "PVT";
            base.RefEfficiencyElectric = refEfficiencyElectric;
            base.RefEfficiencyHeating = refEfficiencyHeating;
        }

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
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

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
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

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
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

        public override EnergyCarrier SetInputs()
        {
            throw new System.NotImplementedException();
        }

        public override EnergyCarrier[] SetOutputs()
        {
            throw new System.NotImplementedException();
        }
    }
    #endregion



    #region Base Conversion Class
    /// <summary>
    /// Heating, Cooling, Electricity generation systems (no solar tech though)
    /// E.g. CHP, boiler, heat pump, chiller, ...
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


        /// <summary>
        /// Time series of thermal conversion efficiency
        /// </summary>
        public double[] ConversionEfficiencyHeating { get; protected set; }
        /// <summary>
        /// Time series of thermal conversion efficiency
        /// </summary>
        public double[] ConversionEfficiencyCooling { get; protected set; }
        /// <summary>
        /// Time series of thermal conversion efficiency
        /// </summary>
        public double[] ConversionEfficiencyElectric { get; protected set; }


        public double RefEfficiencyElectric { get; protected set; }
        public double RefEfficiencyHeating { get; protected set; }
        public double RefEfficiencyCooling { get; protected set; }


        // Operational emissions will be part of the outputCarriers
        // so this class, Conversion, produces outputCarriers, that will carry along all emissions from the inputs as well


        protected Conversion(double investmentCost, double embodiedGhg,
            bool isHeating, bool isCooling, bool isElectric)
        {
            this.SpecificInvestmentCost = investmentCost;
            this.SpecificEmbodiedGhg = embodiedGhg;
            this.IsHeating = isHeating;
            this.IsCooling = isCooling;
            this.IsElectric = isElectric;
        }

        public abstract EnergyCarrier SetInputs();
        public abstract EnergyCarrier[] SetOutputs();
    }
    #endregion
}
