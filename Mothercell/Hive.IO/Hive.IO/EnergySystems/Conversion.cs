using System.Collections.Generic;
using System.Drawing;
using Hive.IO.EnergySystems;
using Rhino.Geometry;

namespace Hive.IO
{
    /// <summary>
    /// Namespace for Energy Systems geometry and properties
    /// </summary>
    namespace EnergySystem
    {
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


            protected SurfaceBased(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg) 
                : base(refEfficiencyThermal, refEfficiencyElectric, cost, ghg)
            {
                SurfaceGeometry = surfaceGeometry;
            }
        }


        /// <summary>
        /// Photovoltaic
        /// </summary>
        public class PV : SurfaceBased
        {
            public PV(Mesh surfaceGeometry, double refEfficiencyElectric, double cost, double ghg, string detailedName) 
                : base(surfaceGeometry, 0.0, refEfficiencyElectric, cost, ghg)
            {
                base.DetailedName = detailedName;
                base.Name = TechnologyNames.Photovoltaic;
            }
        }


        /// <summary>
        /// Solar Thermal
        /// </summary>
        public class ST : SurfaceBased
        {
            public ST(Mesh surfaceGeometry, double refEfficiencyThermal, double cost, double ghg, string detailedName)
                : base(surfaceGeometry, refEfficiencyThermal, 0.0, cost, ghg) 
            {
                base.DetailedName = detailedName;
                base.Name = TechnologyNames.SolarCollector;
            }
        }


        /// <summary>
        /// Hybrid Solar Photovolatic Thermal
        /// </summary>
        public class PVT : SurfaceBased
        {
            public PVT(Mesh surfaceGeometry, double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg, string detailedName)
                : base(surfaceGeometry, refEfficiencyThermal, refEfficiencyElectric, cost, ghg)
            {
                base.Name = TechnologyNames.HybridPhotovoltaicThermal;
                base.DetailedName = detailedName;
            }
        }


        /// <summary>
        /// Horizontal Ground Solar Collector
        /// </summary>
        public class GroundCollector : SurfaceBased
        {
            public GroundCollector(Mesh surfaceGeometry, double refEfficiencyThermal, double cost, double ghg, string detailedName)
                : base(surfaceGeometry, refEfficiencyThermal, 0.0, cost, ghg) 
            {
                base.Name = TechnologyNames.GroundCollector;
                base.DetailedName = detailedName;
            }
        }
        #endregion


        #region Combustion technology
        public abstract class Combustion : Conversion
        {
            public Combustion(double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg)
                : base(refEfficiencyThermal, refEfficiencyElectric, cost, ghg){ }
        }


        public class Boiler : Combustion
        {
            public Boiler (double refEfficiencyThermal, double cost, double ghg)
                : base(refEfficiencyThermal, 0.0, cost, ghg) 
            {
                base.Name = TechnologyNames.Boiler;
            }
        }


        public class CombinedHeatPower : Combustion
        {
            public CombinedHeatPower(double refEfficiencyThermal, double refEfficiencyElectric, double cost, double ghg)
                :base(refEfficiencyThermal, refEfficiencyElectric, cost, ghg)
            { 
                base.Name = TechnologyNames.CombinedHeatPower; 
            }
        }
        #endregion


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
            /// Specification of the technology, e.g. "Mono-cristalline PV"
            /// </summary>
            public string DetailedName { get; protected set; }

            /// <summary>
            /// Investment cost per this.CapacityUnit
            /// </summary>
            public double SpecificInvestmentCost { get; protected set; }
            /// <summary>
            /// Life cycle GHG emissions, in kgCO2eq. per this.CapacityUnit
            /// </summary>
            public double SpecificEmbodiedGhg { get; protected set; }


            /// <summary>
            /// Input streams. e.g. for a CHP this could be 'NaturalGas'
            /// </summary>
            public List<EnergyCarrier> InputCarriers { get; protected set; }
            /// <summary>
            /// Output streams. e.g. for a CHP this could be 'Water' and 'Electricity'
            /// </summary>
            public List<EnergyCarrier> OutputCarriers { get; protected set; }



            /// <summary>
            /// Time series of thermal conversion efficiency
            /// </summary>
            public double [] ConversionEfficiencyHeating { get; protected set; }
            /// <summary>
            /// Time series of thermal conversion efficiency
            /// </summary>
            public double[] ConversionEfficiencyCooling { get; protected set; }
            /// <summary>
            /// Time series of thermal conversion efficiency
            /// </summary>
            public double [] ConversionEfficiencyElectric { get; protected set; }




            // Operation and Maintenance cost? How to deal with conversionMatrix. Is the production always resulting in the same output ratio? I guess, because it is only one conversionMatrix
            // also, how to arrange conversionMatrix, which eff term corresponds to what
            // for now, 






            protected Conversion(double investmentCost, double embodiedGhg, List<EnergyCarrier> inputCarriers, List<EnergyCarrier> outputCarriers, double?[] conversionEfficiencyHeating)
            {
                this.SpecificInvestmentCost = investmentCost;
                this.SpecificEmbodiedGhg = embodiedGhg;
                this.ConversionEfficiencyHeating = conversionEfficiencyHeating;
                this.InputCarriers = new List<EnergyCarrier>(inputCarriers);
                this.OutputCarriers = new List<EnergyCarrier>(outputCarriers);

            }
        }
    }
}
