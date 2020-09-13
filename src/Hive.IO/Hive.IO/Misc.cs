using System;
using Rhino.Geometry;

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

        public const double PEFNaturalGas = 1.13; // https://www.bregroup.com/wp-content/uploads/2019/10/Briefing-note-on-derivation-of-PE-factors-V1.3-01-10-2019.pdf
        public const double PEFBioGas = 1.286; // "
        public const double PEFWoodPellets = 1.325; // "
        public const double PEFElectricitySwiss = 2.02; // 40% Nuclear à 2.8 and 60% hydro à 1.5 http://go.leonardo-energy.org/rs/europeancopper/images/PEF-finalreport.pdf

        public const double Kelvin = 273.15;

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

        // source: http://james-ramsden.com/area-of-a-mesh-face-in-c-in-grasshopper/
        public static double GetMeshFaceArea(int _meshFaceIndex, Mesh _mesh)
        {
            // get points into a nice, concise format
            Point3d pt0 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].A];
            Point3d pt1 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].B];
            Point3d pt2 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].C];

            // calculate areas of triangles
            double a = pt0.DistanceTo(pt1);
            double b = pt1.DistanceTo(pt2);
            double c = pt2.DistanceTo(pt0);
            double p = 0.5 * (a + b + c);
            double area1 = Math.Sqrt(p * (p - a) * (p - b) * (p - c));

            // if quad, calc area of second triangle
            double area2 = 0.0;
            if (_mesh.Faces[_meshFaceIndex].IsQuad)
            {
                Point3d pt3 = _mesh.Vertices[_mesh.Faces[_meshFaceIndex].D];
                a = pt0.DistanceTo(pt2);
                b = pt2.DistanceTo(pt3);
                c = pt3.DistanceTo(pt0);
                p = 0.5 * (a + b + c);
                area2 = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            }

            return area1 + area2;
        }

    }
}
