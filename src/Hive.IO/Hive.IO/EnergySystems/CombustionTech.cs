

namespace Hive.IO.EnergySystems
{
    public abstract class CombustionTech : ConversionTech
    {
        protected CombustionTech(double investmentCost, double embodiedGhg, double capacity, bool isHeating, bool isElectric)
            : base(investmentCost, embodiedGhg, capacity, "kW", isHeating, false, isElectric)
        {
        }
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


        public void SetInputOutput(Gas gasInput, double[] heatingGenerated, double[] supplyTemperature)
        {
            int horizon = heatingGenerated.Length;

            var gasConsumed = new double[horizon];
            var gasPrice = new double[horizon];
            var gasEmissionsFactor = new double[horizon];

            for (int t = 0; t < horizon; t++)
            {
                gasConsumed[t] = heatingGenerated[t] / this.Efficiency;
            }
            if (horizon == Misc.MonthsPerYear)
            {
                gasPrice = Misc.GetAverageMonthlyValue(gasInput.EnergyPrice);
                gasEmissionsFactor = Misc.GetAverageMonthlyValue(gasInput.GhgEmissionsFactor);
            }
            else
            {
                gasPrice = gasInput.EnergyPrice;
                gasEmissionsFactor = gasInput.GhgEmissionsFactor;
            }

            Gas gasConsumedCarrier = new Gas(horizon, gasConsumed, gasPrice, gasEmissionsFactor);
            base.InputCarrier = gasConsumedCarrier; // infused with how much gas is consumed. input Gas carrier has no Energy information

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, heatingGenerated, null, null, supplyTemperature);
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

        public void SetInputOutput(Gas gasInput, double [] energyGenerated, double [] supplyTemperature, bool energyIsHeat = false)
        {
            int horizon = energyGenerated.Length;

            var gasConsumed = new double[horizon];
            var gasPrice = new double[horizon];
            var gasEmissionsFactor = new double[horizon];

            var heatingGenerated = new double[horizon];
            var elecGenerated = new double[horizon];

            for (int t = 0; t < horizon; t++)
            {
                if (energyIsHeat)
                {
                    heatingGenerated[t] = energyGenerated[t];
                    elecGenerated[t] = heatingGenerated[t] / this.HeatToPowerRatio;
                }
                else
                {
                    elecGenerated[t] = energyGenerated[t];
                    heatingGenerated[t] = elecGenerated[t] * this.HeatToPowerRatio;
                }
                gasConsumed[t] = elecGenerated[t] / this.ElectricEfficiency;
            }

            if (horizon == Misc.MonthsPerYear)
            {
                gasPrice = Misc.GetAverageMonthlyValue(gasInput.EnergyPrice);
                gasEmissionsFactor = Misc.GetAverageMonthlyValue(gasInput.GhgEmissionsFactor);
            }
            else
            {
                gasPrice = gasInput.EnergyPrice;
                gasEmissionsFactor = gasInput.GhgEmissionsFactor;
            }

            Gas gasConsumedCarrier = new Gas(horizon, gasConsumed, gasPrice, gasEmissionsFactor);
            base.InputCarrier = gasConsumedCarrier; // infused with how much gas is consumed. input Gas carrier has no Energy information

            base.OutputCarriers = new Carrier[2];
            base.OutputCarriers[0] = new Water(horizon, heatingGenerated, null, null, supplyTemperature);
            base.OutputCarriers[1] = new Electricity(horizon, elecGenerated, null, null);
        }




        public void SetInput(Gas gasInput)
        {
            base.InputCarrier = gasInput;
        }

        public void SetOutput(int horizon, double[] heatingGenerated, double[] electricityGenerated, double[] energyCost, double[] ghgEmissions, double[] supplyTemperature)
        {
            base.OutputCarriers = new Carrier[2];
            base.OutputCarriers[0] = new Water(horizon, heatingGenerated, energyCost, ghgEmissions, supplyTemperature);
            base.OutputCarriers[1] = new Electricity(horizon, electricityGenerated, energyCost, ghgEmissions);

        }

    }
}
