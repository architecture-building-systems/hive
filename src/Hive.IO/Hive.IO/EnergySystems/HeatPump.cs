using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO.EnergySystems
{
    public class HeatPump : ConversionTech
    {
        public double[] COP
        {
            get;
            protected set;
        }

        public HeatPump(double investmentCost, double embodiedGhg, double lifetime, double capacity, bool isHeating, bool isCooling) 
            : base(investmentCost, embodiedGhg, lifetime, capacity, "kW", isHeating, isCooling, false){}
    }



    public class Chiller : HeatPump
    {
        /// <summary>
        /// Ambient air carrier. This will influence COP of the Chiller
        /// </summary>
        public Air AmbientAir { get; private set; }
        public double EtaRef { get; private set; }
        public Chiller(double investmentCost, double embodiedGhg, double lifetime, double capacity, double etaRef)
            : base(investmentCost, embodiedGhg, lifetime, capacity, false, true)
        {
            this.EtaRef = etaRef;
            base.Name = "Chiller";
        }


        public void SetInputOutputSimple(Electricity electricityIn, Air ambientAir, double[] coolingGenerated, double[] tempCold)
        {
            int horizon = coolingGenerated.Length;
            base.COP = new double[horizon];

            var tempWarmHorizon = new double[horizon];
            var tempColdHorizon = new double[horizon];

            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.SpecificCost);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.SpecificEmissions);
                tempWarmHorizon = Misc.GetAverageMonthlyValue(ambientAir.Temperature);
                tempColdHorizon = Misc.GetAverageMonthlyValue(tempCold);
            }
            else
            {
                elecPrice = electricityIn.SpecificCost;
                elecEmissionsFactor = electricityIn.SpecificEmissions;
                tempWarmHorizon = ambientAir.Temperature;
                tempColdHorizon = tempCold;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = this.EtaRef * ((tempColdHorizon[t] + Misc.Kelvin) / ((tempWarmHorizon[t] + Misc.Kelvin) - (tempColdHorizon[t] + Misc.Kelvin)));
                base.COP[t] = COP;
                elecConsumed[t] = coolingGenerated[t] / COP;
                if (elecConsumed[t] < 0.0) elecConsumed[t] = 0.0;
            }

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor, electricityIn.PrimaryEnergyFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, coolingGenerated, null, null, tempCold, 1.0);
        }


        /// <summary>
        /// Source: Choi et al. 2005. Effects of stacked condensers in a high-rise apartment building.
        /// DOI: 10.1016/j.energy.2004.08.004
        /// Eq. (7)
        /// indoor temperature was assumed to be 27°C, and Tc was the on-coil temperature of the condenser
        /// Also, this equation is actually for air con, but we are creating a Water carrier as output here
        /// </summary>
        /// <param name="electricityIn">Empty electricity carrier, i.e. no information yet how much is consumed. Will be computed here and be infused into this.InputCarrier</param>
        /// <param name="airIn">also uses as on-coil temperature (air entering evaporator)</param>
        /// <param name="coolingGenerated"></param>
        /// <param name="supplyTemp"></param>
        public void SetInputOutput(Electricity electricityIn, Air airIn, double[] coolingGenerated, double[] supplyTemp)
        {
            int horizon = coolingGenerated.Length;
            base.COP = new double[horizon];

            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];
            var airTemp = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.SpecificCost);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.SpecificEmissions);
                airTemp = airIn.TemperatureMonthly;
            }
            else
            {
                elecPrice = electricityIn.SpecificCost;
                elecEmissionsFactor = electricityIn.SpecificEmissions;
                airTemp = airIn.Temperature;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = (638.95 - 4.238 * airTemp[t]) / (100.0 + 3.534 * airTemp[t]);  // this equation must be in degree C
                base.COP[t] = COP;
                elecConsumed[t] = coolingGenerated[t] / COP;
                if (elecConsumed[t] < 0.0) elecConsumed[t] = 0.0;
            }

            this.AmbientAir = new Air(horizon, null, null, null, airTemp); // how would I know air energy? i'd need that for exergy calculation?

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor, electricityIn.PrimaryEnergyFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, coolingGenerated, null, null, supplyTemp, 1.0);
        }
    }


    public class AirSourceHeatPump : HeatPump
    {
        /// <summary>
        /// Ambient air carrier. This will influence COP of the ASHP
        /// </summary>
        public Air AmbientAir { get; private set; }
        public double EtaRef { get; private set; }
        public AirSourceHeatPump(double investmentCost, double embodiedGhg, double lifetime, double capacity, double etaRef)
            : base(investmentCost, embodiedGhg, lifetime, capacity, true, false)
        {
            this.EtaRef = etaRef;
            base.Name = "AirSourceHeatPump";
        }


        // this is a copy of Chiller.setinpuitoutputsimple... can we avoid redundancy? Make abstract class HeatPump?
        public void SetInputOutputSimple(Electricity electricityIn, Air ambientAir, double[] heatingGenerated, double[] tempWarm)
        {
            int horizon = heatingGenerated.Length;
            base.COP = new double[horizon];

            var tempWarmHorizon = new double[horizon];
            var tempColdHorizon = new double[horizon];
            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.SpecificCost);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.SpecificEmissions);
                tempWarmHorizon = Misc.GetAverageMonthlyValue(tempWarm);
                tempColdHorizon = Misc.GetAverageMonthlyValue(ambientAir.Temperature);
            }
            else
            {
                elecPrice = electricityIn.SpecificCost;
                elecEmissionsFactor = electricityIn.SpecificEmissions;
                tempWarmHorizon = tempWarm;
                tempColdHorizon = ambientAir.Temperature;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = this.EtaRef * ((tempWarmHorizon[t] + Misc.Kelvin) / ((tempWarmHorizon[t] + Misc.Kelvin) - (tempColdHorizon[t] + Misc.Kelvin)));
                base.COP[t] = COP;
                elecConsumed[t] = heatingGenerated[t] / COP;
                if (elecConsumed[t] < 0.0) elecConsumed[t] = 0.0;
            }

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor, electricityIn.PrimaryEnergyFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, heatingGenerated, null, null, tempWarm, 1.0);

        }


        /// <summary>
        /// Ashouri et al 2013. Optimal design and operation of building services using mixed-integer linear programming techniques
        /// 10.1016/j.energy.2013.06.053
        /// Eq. (8c)
        /// </summary>
        /// <param name="electricityIn"></param>
        /// <param name="airIn"></param>
        /// <param name="heatingGenerated"></param>
        /// <param name="supplyTemp"></param>
        public void SetInputOutput(Electricity electricityIn, Air airIn, double[] heatingGenerated, double[] supplyTemp,
            double pi1 = 13.39, double pi2 = -0.047, double pi3 = 1.109, double pi4 = 0.012)
        {
            int horizon = heatingGenerated.Length;
            base.COP = new double[horizon];

            var elecConsumed = new double[horizon];
            var elecPrice = new double[horizon];
            var elecEmissionsFactor = new double[horizon];
            var airTemp = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.SpecificCost);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.SpecificEmissions);
                airTemp = airIn.TemperatureMonthly;
            }
            else
            {
                elecPrice = electricityIn.SpecificCost;
                elecEmissionsFactor = electricityIn.SpecificEmissions;
                airTemp = airIn.Temperature;
            }

            for (int t = 0; t < horizon; t++)
            {
                double COP = pi1 * Math.Exp(pi2 * ((supplyTemp[t] + Misc.Kelvin) - (airTemp[t] + Misc.Kelvin))) + pi3 * Math.Exp(pi4 * ((supplyTemp[t] + Misc.Kelvin) - (airTemp[t] + Misc.Kelvin)));
                base.COP[t] = COP;
                elecConsumed[t] = heatingGenerated[t] / COP;
                if (elecConsumed[t] < 0.0) elecConsumed[t] = 0.0;
            }

            Electricity electricityConsumedCarrier = new Electricity(horizon, elecConsumed, elecPrice, elecEmissionsFactor, electricityIn.PrimaryEnergyFactor);
            base.InputCarrier = electricityConsumedCarrier;

            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Water(horizon, heatingGenerated, null, null, supplyTemp, 1.0);
        }
    }

}
