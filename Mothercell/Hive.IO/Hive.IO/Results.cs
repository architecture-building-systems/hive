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

        public double[] TotalClgMonthly { get; private set; }
        public double[] TotalClgHourly { get; private set; }

        public double[] TotalHtgMonthly { get; private set; }
        public double[] TotalHtgHourly { get; private set; }
        #endregion


        #region Zone-wise loads
        public double [][] ZoneElecMonthly { get; private set; }
        public double[][] ZoneElecHourly { get; private set; }
        public double[][] ZoneClgMonthly { get; private set; }
        public double[][] ZoneClgHourly { get; private set; }
        public double[][] ZoneHtgMonthly { get; private set; }
        public double[][] ZoneHtgHourly { get; private set; }
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
        public bool [,] SupplyTypes { get; private set; }
        /// <summary>
        /// Capacity per technology. Unit is defined in 'SupplyCapUnits
        /// </summary>
        public double [] SupplyCapacities { get; private set; }
        /// <summary>
        /// Defining Capacity unit per technology. E.g. m2, kW, kWh, ...
        /// </summary>
        public string [] SupplyCapUnits { get; private set; }
        /// <summary>
        /// Total investment cost per technology
        /// </summary>
        public double [] SupplyCapex { get; private set; }
        /// <summary>
        /// Levelized investment cost per technology (considering their lifetime and interest rate)
        /// </summary>
        public double [] SupplyCapexLev { get; private set; }     
        /// <summary>
        /// Total levelized operation expenditures per supply technology
        /// </summary>
        public double [] SupplyOpexLev { get; private set; }
        /// <summary>
        /// Time-resolved (hourly for a year) OPEX per technology
        /// </summary>
        public double [][] SupplyOpexHourly { get; private set; }
        /// <summary>
        /// Operation schedule per technology and hour. Unit defined in 'SupplyOperationUnit'
        /// </summary>
        public double [][] SupplyOperationHourly { get; private set; }
        /// <summary>
        /// Defining unit of operation per technology, e.g. "kWh", "Wh", ...
        /// </summary>
        public string [] SupplyOperationUnit { get; private set; }
        #endregion


        #region constants
        public const int Months = 12;
        public const int Days = 365;
        public const int Hours = 8760;
        #endregion



        public Results()
        {
            this.TotalClgMonthly = new double[Results.Months];
            this.TotalElecMonthly = new double[Results.Months];
            this.TotalHtgMonthly = new double[Results.Months];

            this.TotalHtgHourly = new double[Results.Hours];
            this.TotalClgHourly = new double[Results.Hours];
            this.TotalElecHourly = new double[Results.Hours];
        }


        public void SetTotalDemandMonthly(double[] clgDemand, double[] htgDemand, double[] elecDemand)
        {
            if (clgDemand != null && clgDemand.Length == Results.Months)
                clgDemand.CopyTo(this.TotalClgMonthly, 0);
            else
                this.TotalClgMonthly = null;

            if (htgDemand != null && htgDemand.Length == Results.Months)
                htgDemand.CopyTo(this.TotalHtgMonthly, 0);
            else
                this.TotalHtgMonthly = null;

            if (elecDemand != null && elecDemand.Length == Results.Months)
                elecDemand.CopyTo(this.TotalElecMonthly, 0);
            else
                this.TotalElecMonthly = null;
        }


        public void SetTotalDemandHourly(double[] clgDemand, double[] htgDemand, double[] elecDemand)
        {

            if (clgDemand != null && clgDemand.Length == Results.Hours)
                clgDemand.CopyTo(this.TotalClgHourly, 0);
            else
                this.TotalClgHourly = null;

            if (htgDemand != null && htgDemand.Length == Results.Hours)
                htgDemand.CopyTo(this.TotalHtgHourly, 0);
            else
                this.TotalHtgHourly = null;

            if (elecDemand != null && elecDemand.Length == Results.Hours)
                elecDemand.CopyTo(this.TotalElecHourly, 0);
            else
                this.TotalElecHourly = null;
        }

    }
}
