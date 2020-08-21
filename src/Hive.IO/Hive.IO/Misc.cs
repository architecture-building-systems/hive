using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive.IO
{
    public static class Misc
    {
        /// <summary>
        /// Typical calendar year
        /// </summary>
        public static readonly int[] DaysPerMonth = new int[12] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public const int HoursPerDay = 24;
        public const int HoursPerYear = 8760;
        public const int MonthsPerYear = 12;

        public static double[] GetAverageMonthlyValue(double[] annualTimeSeries)
        {
            double[] monthlyTimeSeries = new double[Misc.MonthsPerYear];
            if (annualTimeSeries.Length == Misc.HoursPerYear)
            {
                int sumOfDays = 0;
                for (int t = 0; t < Misc.MonthsPerYear; t++)
                {
                    int startIndex = sumOfDays * Misc.HoursPerDay;
                    int daysThisMonth = Misc.DaysPerMonth[t];
                    sumOfDays += daysThisMonth;
                    int endIndex = sumOfDays * Misc.HoursPerDay;
                    double average = 0.0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        double temp = annualTimeSeries[i];
                        if (double.IsNaN(temp))
                            temp = 0.0;
                        average += temp;
                    }
                    average /= (daysThisMonth * Misc.HoursPerDay);
                    //double average = Enumerable.Range(startIndex, endIndex).Select(i => annualTimeSeries[i]).Average();
                    monthlyTimeSeries[t] = average;
                }
                return monthlyTimeSeries;
            }
            else if(annualTimeSeries.Length == Misc.MonthsPerYear)
            {
                return annualTimeSeries;
            }
            else
            {
                return null;
            }
        }


        public static double[] GetCumulativeMonthlyValue(double[] annualTimeSeries)
        {
            double[] monthlyTimeSeries = new double[Misc.MonthsPerYear];
            if (annualTimeSeries.Length == Misc.HoursPerYear)
            {
                int sumOfDays = 0;
                for (int t = 0; t < Misc.MonthsPerYear; t++)
                {
                    int startIndex = sumOfDays * Misc.HoursPerDay;
                    int daysThisMonth = Misc.DaysPerMonth[t];
                    sumOfDays += daysThisMonth;
                    int endIndex = sumOfDays * Misc.HoursPerDay;
                    double sum = 0.0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        double temp = annualTimeSeries[i];
                        if (double.IsNaN(temp))
                            temp = 0.0;
                        sum += temp;
                    }
                    monthlyTimeSeries[t] = sum;
                }
                return monthlyTimeSeries;
            }
            else if (annualTimeSeries.Length == Misc.MonthsPerYear)
            {
                return annualTimeSeries;
            }
            else
            {
                return null;
            }
        }
    }
}
