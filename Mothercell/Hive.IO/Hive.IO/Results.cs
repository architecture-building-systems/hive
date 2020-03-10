using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO
{
    /// <summary>
    /// Results class that contains all kinds of building simulation results
    /// - demand
    /// - solar potentials
    /// - OPEX CAPEX
    /// - carbon emissions
    /// - etc
    /// Also includes rhino geometry that represent e.g. solar potentials on buildings
    /// </summary>
    public class Results
    {
        #region General
        public double InterestRate { get; private set; }
        #endregion


        #region Total Loads
        public double[] TotalElecMonthly { get; private set; }
        public double[] TotalElecHourly { get; private set; }

        public double[] TotalCoolingMonthly { get; private set; }
        public double[] TotalCoolingHourly { get; private set; }

        public double[] TotalHeatingMonthly { get; private set; }
        public double[] TotalHeatingHourly { get; private set; }

        public double[] TotalDHWMonthly { get; private set; }
        public double[] TotalDHWHourly { get; private set; }
        #endregion


        #region Zone-wise loads
        public double[][] ZoneElecMonthly { get; private set; }
        public double[][] ZoneElecHourly { get; private set; }
        public double[][] ZoneCoolingMonthly { get; private set; }
        public double[][] ZoneCoolingHourly { get; private set; }
        public double[][] ZoneHeatingMonthly { get; private set; }
        public double[][] ZoneHeatingHourly { get; private set; }
        public double[][] ZoneDHWMonthly { get; private set; }
        public double[][] ZoneDHWHourly { get; private set; }
        #endregion


        #region Energy Supply Systems
        /// <summary>
        /// Names for each supply technology, e.g. "Boiler", "CHP", ...
        /// </summary>
        public string[] SupplyNames { get; private set; }
        /// <summary>
        /// Matrix defining the suitability of each technology per carrier type.
        /// Rows: Technologies
        /// Columns: [0]: Electricity, [1]: Heating, [2]: Cooling
        /// </summary>
        public bool[,] SupplyTypes { get; private set; }
        /// <summary>
        /// Capacity per technology. Unit is defined in 'SupplyCapUnits
        /// </summary>
        public double[] SupplyCapacities { get; private set; }
        /// <summary>
        /// Defining Capacity unit per technology. E.g. m2, kW, kWh, ...
        /// </summary>
        public string[] SupplyCapUnits { get; private set; }
        /// <summary>
        /// Total investment cost per technology
        /// </summary>
        public double[] SupplyCapex { get; private set; }
        /// <summary>
        /// Levelized investment cost per technology (considering their lifetime and interest rate)
        /// </summary>
        public double[] SupplyCapexLev { get; private set; }
        /// <summary>
        /// Total levelized operation expenditures per supply technology
        /// </summary>
        public double[] SupplyOpexLev { get; private set; }
        /// <summary>
        /// Time-resolved (hourly for a year) OPEX per technology
        /// </summary>
        public double[][] SupplyOpexHourly { get; private set; }
        /// <summary>
        /// Operation schedule per technology and hour. Unit defined in 'SupplyOperationUnit'
        /// </summary>
        public double[][] SupplyOperationHourly { get; private set; }
        /// <summary>
        /// Operation schedule per technology and month. Unit defined in 'SupplyOperationUnit'
        /// </summary>
        public double [][] SupplyOperationMonthly { get; private set; }
        /// <summary>
        /// Defining unit of operation per technology, e.g. "kWh", "Wh", ...
        /// </summary>
        public string[] SupplyOperationUnit { get; private set; }
        #endregion


        #region constants
        private const int months = 12;
        private const int hours = 8760;
        #endregion



        public Results()
        {
            this.TotalCoolingMonthly = new double[Results.months];
            this.TotalElecMonthly = new double[Results.months];
            this.TotalHeatingMonthly = new double[Results.months];
            this.TotalDHWMonthly = new double[Results.months];

            this.TotalCoolingHourly = new double[Results.hours];
            this.TotalHeatingHourly = new double[Results.hours];
            this.TotalElecHourly = new double[Results.hours];
            this.TotalDHWHourly = new double[Results.hours];

            this.SupplyNames = null;
            this.SupplyTypes = null;
            this.SupplyCapacities = null;
            this.SupplyCapUnits = null;
        }


        public void SetSupplySystemsCapacity(string [] supplyTechNames, bool [,] supplyTypes, double[] supplyCapacities, string[] supplyCapUnits)
        {
            // should contain checks, like lengths of arrays match etc


            this.SupplyNames = new string[supplyTechNames.Length];
            supplyTechNames.CopyTo(this.SupplyNames, 0);

            this.SupplyTypes = supplyTypes.Clone() as bool[,];
            /// Rows: Technologies
            /// Columns: [0]: Electricity, [1]: Heating, [2]: Cooling

            this.SupplyCapacities = new double[supplyCapacities.Length];
            supplyCapacities.CopyTo(this.SupplyCapacities, 0);

            this.SupplyCapUnits = new string[supplyCapUnits.Length];
            supplyCapUnits.CopyTo(this.SupplyCapUnits, 0);
        }


        public void SetSupplySystemsGenerationMonthly(double [][] supplyOperationMonthly)
        {
            if (supplyOperationMonthly.Length <= 0) return;

            this.SupplyOperationMonthly = new double[supplyOperationMonthly.Length][];
            for (int i = 0; i < this.SupplyOperationMonthly.Length; i++)
            {
                if(supplyOperationMonthly[i].Length == Results.months) 
                {
                    this.SupplyOperationMonthly[i] = new double[Results.months];
                    supplyOperationMonthly.CopyTo(this.SupplyOperationMonthly, 0);
                }
            }

        }


        public void SetSupplySystemsGenerationHourly(double[][] supplyOperationHourly)
        {
            if (supplyOperationHourly.Length <= 0) return;

            this.SupplyOperationMonthly = new double[supplyOperationHourly.Length][];
            for (int i = 0; i < this.SupplyOperationMonthly.Length; i++)
            {
                if (supplyOperationHourly[i].Length == Results.hours)
                {
                    this.SupplyOperationMonthly[i] = new double[Results.hours];
                    supplyOperationHourly.CopyTo(this.SupplyOperationMonthly, 0);
                }
            }
        }


        /// <summary>
        /// Setting monthly energy demand of the entire building for heating, cooling, electricity, and domestic hot water
        /// </summary>
        /// <param name="coolingDemand"></param>
        /// <param name="heatingDemand"></param>
        /// <param name="electricityDemand"></param>
        /// <param name="dhwDemand"></param>
        public void SetTotalDemandMonthly(double[] coolingDemand, double[] heatingDemand, 
            double[] electricityDemand, double [] dhwDemand)
        {
            if (coolingDemand != null && coolingDemand.Length == Results.months)
                coolingDemand.CopyTo(this.TotalCoolingMonthly, 0);
            else
                this.TotalCoolingMonthly = null;

            if (heatingDemand != null && heatingDemand.Length == Results.months)
                heatingDemand.CopyTo(this.TotalHeatingMonthly, 0);
            else
                this.TotalHeatingMonthly = null;

            if (electricityDemand != null && electricityDemand.Length == Results.months)
                electricityDemand.CopyTo(this.TotalElecMonthly, 0);
            else
                this.TotalElecMonthly = null;

            if (dhwDemand != null && dhwDemand.Length == Results.months)
                dhwDemand.CopyTo(this.TotalDHWMonthly, 0);
        }


        /// <summary>
        /// Setting hourly energy demand of the entire building for heating, cooling, electricity, and domestic hot water
        /// </summary>
        /// <param name="coolingDemand"></param>
        /// <param name="heatingDemand"></param>
        /// <param name="electricityDemand"></param>
        /// <param name="dhwDemand"></param>
        public void SetTotalDemandHourly(double[] coolingDemand, double[] heatingDemand, 
            double[] electricityDemand, double [] dhwDemand)
        {
            if (coolingDemand != null && coolingDemand.Length == Results.hours)
                coolingDemand.CopyTo(this.TotalCoolingHourly, 0);
            else
                this.TotalCoolingHourly = null;

            if (heatingDemand != null && heatingDemand.Length == Results.hours)
                heatingDemand.CopyTo(this.TotalHeatingHourly, 0);
            else
                this.TotalHeatingHourly = null;

            if (electricityDemand != null && electricityDemand.Length == Results.hours)
                electricityDemand.CopyTo(this.TotalElecHourly, 0);
            else
                this.TotalElecHourly = null;

            if (dhwDemand != null && dhwDemand.Length == Results.hours)
                dhwDemand.CopyTo(this.TotalDHWHourly, 0);
        }

    }
}
