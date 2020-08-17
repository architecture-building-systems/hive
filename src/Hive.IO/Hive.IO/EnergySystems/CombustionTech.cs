

namespace Hive.IO.EnergySystems
{
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
        public void SetInput(Gas gasInput)
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
        public void SetOutput(int horizon, double[] availableEnergy, double[] energyCost, double[] ghgEmissions, double[] supplyTemperature)
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
            return new double[] { };
        }


    }
}
