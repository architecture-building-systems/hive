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
        #endregion


        #region Zone-wise loads
        public double[][] ZoneElecMonthly { get; private set; }
        public double[][] ZoneElecHourly { get; private set; }
        public double[][] ZoneCoolingMonthly { get; private set; }
        public double[][] ZoneCoolingHourly { get; private set; }
        public double[][] ZoneHeatingMonthly { get; private set; }
        public double[][] ZoneHeatingHourly { get; private set; }
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
        /// Defining unit of operation per technology, e.g. "kWh", "Wh", ...
        /// </summary>
        public string[] SupplyOperationUnit { get; private set; }
        #endregion


        #region constants
        private const int months = 12;
        private const int days = 365;
        private const int hours = 8760;
        #endregion



        public Results()
        {
            this.TotalCoolingMonthly = new double[Results.months];
            this.TotalElecMonthly = new double[Results.months];
            this.TotalHeatingMonthly = new double[Results.months];

            this.TotalCoolingHourly = new double[Results.hours];
            this.TotalHeatingHourly = new double[Results.hours];
            this.TotalElecHourly = new double[Results.hours];

        }


        public void SetTotalDemandMonthly(double[] coolingDemand, double[] heatingDemand, double[] electricityDemand)
        {
            if (coolingDemand != null && coolingDemand.Length == Results.months)
                coolingDemand.CopyTo(this.TotalCoolingMonthly, 0);
            else
                this.TotalCoolingMonthly = null;
            if (heatingDemand is null)
            {
                throw new ArgumentNullException(nameof(heatingDemand));
            }

            if (heatingDemand != null && heatingDemand.Length == Results.months)
                heatingDemand.CopyTo(this.TotalHeatingMonthly, 0);
            else
                this.TotalHeatingMonthly = null;

            if (electricityDemand != null && electricityDemand.Length == Results.months)
                electricityDemand.CopyTo(this.TotalElecMonthly, 0);
            else
                this.TotalElecMonthly = null;
        }


        public void SetTotalDemandHourly(double[] coolingDemand, double[] heatingDemand, double[] electricityDemand)
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
        }

    }
}
