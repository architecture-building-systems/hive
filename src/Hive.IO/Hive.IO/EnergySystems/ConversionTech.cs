using System;

namespace Hive.IO.EnergySystems
{
    #region MiscSupply
    public class DirectElectricity : ConversionTech
    {
        public double Efficiency { get; private set; }
        public DirectElectricity(double investmentCost, double embodiedGhg, double capacity, double efficiency)
            : base(investmentCost, embodiedGhg, capacity, "kW", false, false, true)
        {
            this.Efficiency = efficiency;
            base.Name = "DirectEletricity";
        }


        public void SetInputOutput(Electricity electricityIn, double[] finalElectricityDemand)
        {
            int horizon = finalElectricityDemand.Length;
            var purchasedElectricity = new double[horizon];
            finalElectricityDemand.CopyTo(purchasedElectricity, 0);

            double[] elecPrice;
            double[] elecEmissionsFactor;
            
            if (horizon == Misc.MonthsPerYear)
            {
                elecPrice = Misc.GetAverageMonthlyValue(electricityIn.EnergyPrice);
                elecEmissionsFactor = Misc.GetAverageMonthlyValue(electricityIn.GhgEmissionsFactor);
            }
            else
            {
                elecPrice = electricityIn.EnergyPrice;
                elecEmissionsFactor = electricityIn.GhgEmissionsFactor;
            }
            base.InputCarrier = new Electricity(horizon, purchasedElectricity, elecPrice, elecEmissionsFactor, electricityIn.PrimaryEnergyFactor);
            base.OutputCarriers = new Carrier[1];
            base.OutputCarriers[0] = new Electricity(horizon, finalElectricityDemand, null, null, 1.0); // costs and emissions are already accounted for by the grid in the InputCarrier
        }
    }




    public class HeatCoolingExchanger : ConversionTech
    {
        // assume perfect heat exchange?, according to Dr. Somil District Heating Miglani, only distribution losses. but since we don't have network implemented yet, lets use a coefficient
        public double DistributionLosses { get; private set; }


        public HeatCoolingExchanger(double investmentCost, double embodiedGhg, double capacity, double losses, bool heatingExchanger = true, bool coolingExchanger = false)
            : base(investmentCost, embodiedGhg, capacity, "kW", heatingExchanger, coolingExchanger, false)
        {
            this.DistributionLosses = losses;
            if (heatingExchanger && !coolingExchanger)
                base.Name = "HeatExchanger";
            else if (coolingExchanger && !heatingExchanger)
                base.Name = "CoolingExchanger";
            else if (heatingExchanger && coolingExchanger)
                base.Name = "HeatCoolingExchanger";
        }


        public void SetInputOutput(Water districtFluid, double[] generatedEnergy, double [] supplyTemp, double [] returnTemp)
        {
            int horizon = generatedEnergy.Length;
            var consumedEnergy = new double[horizon];
            var energyPrice = new double[horizon];
            var energyGhg = new double[horizon];

            if (horizon == Misc.MonthsPerYear)
            {
                energyPrice = Misc.GetAverageMonthlyValue(districtFluid.EnergyPrice);
                energyGhg = Misc.GetAverageMonthlyValue(districtFluid.GhgEmissionsFactor);
            }
            else
            {
                energyPrice = districtFluid.EnergyPrice;
                energyGhg = districtFluid.GhgEmissionsFactor;
            }

            for(int t=0; t<horizon; t++)
            {
                consumedEnergy[t] = generatedEnergy[t] / (1.0 - this.DistributionLosses);
            }

            base.InputCarrier = new Water(horizon, consumedEnergy, energyPrice, energyGhg, supplyTemp, districtFluid.PrimaryEnergyFactor);  // simplified assumption, that DH supply temp is already at the right level

            base.OutputCarriers = new Carrier[1] { new Water(horizon, generatedEnergy, null, null, supplyTemp, 1.0) };

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
        public Carrier InputCarrier { get; protected set; }
        /// <summary>
        /// Output streams. e.g. for a CHP this could be 'Heating' and 'Electricity'
        /// </summary>
        public Carrier[] OutputCarriers { get; protected set; }


        protected ConversionTech(double investmentCost, double embodiedGhg, double capacity, string capacityUnity, bool isHeating, bool isCooling, bool isElectric)
        {
            this.SpecificInvestmentCost = investmentCost;
            this.SpecificEmbodiedGhg = embodiedGhg;
            this.Capacity = capacity;
            this.CapacityUnit = capacityUnity;
            this.IsHeating = isHeating;
            this.IsCooling = isCooling;
            this.IsElectric = isElectric;
        }

    }


    #endregion
}
