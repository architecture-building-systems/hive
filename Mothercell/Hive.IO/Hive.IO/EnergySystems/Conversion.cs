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
            /// Name of the technology
            /// </summary>
            public TechnologyNames Name;
            public enum TechnologyNames
            {
                Boiler,
                CombinedHeatPower,
                HeatPump,
                Photovoltaic,
                HybridPhotovoltaicThermal,
                SolarCollector,
                GroundCollector,
            }
            /// <summary>
            /// Specification of the technology, e.g. "Mono-cristalline PV"
            /// </summary>
            public string DetailedName { get; protected set; }

            /// <summary>
            /// Investment cost per m2
            /// </summary>
            public double InvestmentCost { get; protected set; }
            /// <summary>
            /// Life cycle GHG emissions, in kgCO2eq./m2
            /// </summary>
            public double EmbodiedGhg { get; protected set; }


            /// <summary>
            /// Input streams. e.g. for a CHP this could be 'NaturalGas'
            /// </summary>
            public List<Carrier> InputCarriers { get; protected set; }
            /// <summary>
            /// Output streams. e.g. for a CHP this could be 'Water' and 'Electricity'
            /// </summary>
            public List<Carrier> OutputCarriers { get; protected set; }
            /// <summary>
            /// Conversion matrix of size this.InputCarriers x this.OutputCarriers.
            /// e.g. [input1 input2 input2] x [eta1 eta2; eta3 eta4; eta5 eta6] = [output1 output2]
            /// </summary>
            public double [,] ConversionMatrix { get; protected set; }

            /// <summary>
            /// Capacity of technology. Unit is defined in 'CapacityUnit'
            /// </summary>
            public double Capacity { get; private set; }
            /// <summary>
            /// Unit of technology capacity (e.g. "kW", or "sqm", etc.)
            /// </summary>
            public string CapacityUnit { get; private set; }
        

            // Operation and Maintenance cost? How to deal with conversionMatrix. Is the production always resulting in the same output ratio? I guess, because it is only one conversionMatrix
            // also, how to arrange conversionMatrix, which eff term corresponds to what
            // for now, 


            protected Conversion(double investmentCost, double embodiedGhg, List<Carrier> inputCarriers, List<Carrier> outputCarriers, double [,] conversionMatrix)
            {
                this.InvestmentCost = investmentCost;
                this.EmbodiedGhg = embodiedGhg;
                this.InputCarriers = new List<Carrier>(inputCarriers);
                this.OutputCarriers = new List<Carrier>(outputCarriers);

            }
        }
    }
}
