using System;
using System.Linq;
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

        public static readonly string[] MonthNames =
        {
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec"
        };

        public const double PEFNaturalGas = 1.13; // https://www.bregroup.com/wp-content/uploads/2019/10/Briefing-note-on-derivation-of-PE-factors-V1.3-01-10-2019.pdf
        public const double PEFBioGas = 1.286; // "
        public const double PEFWoodPellets = 1.325; // "
        public const double PEFElectricitySwiss = 2.02; // 40% Nuclear à 2.8 and 60% hydro à 1.5 http://go.leonardo-energy.org/rs/europeancopper/images/PEF-finalreport.pdf

        public const double DefaultBuildingLifetime = 80;
        public const double DefaultInterestRate = 0.05;

        public const double Kelvin = 273.15;

        public const string DefaultConstructionType = "default"; // For using fixed tau values instead of variable for SIA 380 demand calc

        public static double[] GetAverageMonthlyValue(double[] annualTimeSeries)
        {
            if (annualTimeSeries == null)
            {
                return new double[Misc.MonthsPerYear];
            }

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



        public static double[] ComputeTiltAzimuth(Mesh mesh)
        {
            double azimuth, tilt;

            mesh.FaceNormals.ComputeFaceNormals();
            Vector3d normal = mesh.Normals[0];
            Point3d cen = mesh.Faces.GetFaceCenter(0);
            Vector3d vnormal = new Vector3d(0, 0, 1);
            Vector3d tiltZ0 = new Vector3d(0, 0, 1);
            double tiltValue = Vector3d.VectorAngle(normal, tiltZ0, vnormal);
            tilt = tiltValue * (180.0 / Math.PI);

            if (tiltValue == 0)
            {
                azimuth = 0;
            }
            else
            {
                double azimuthValue = Vector3d.VectorAngle(new Vector3d(normal.X, normal.Y, 0), new Vector3d(0, 1, 0), vnormal);
                azimuth = azimuthValue * (180 / Math.PI);
            }

            //Line normalLine = new Line(cen, cen + normal);
            //Line azimuthLine = new Line(cen, cen + new Vector3d(0, 1, 0));
            //Line tiltLine = new Line(cen, cen + tiltZ0);

            return new double[2] { tilt, azimuth };
        }


        public static double[] ComputeTiltAzimuthArea(Brep x)
        {
            Surface srf = x.Surfaces[0];

            AreaMassProperties massProp = AreaMassProperties.Compute(srf);
            Point3d cen = massProp.Centroid;
            double u, v;
            srf.ClosestPoint(cen, out u, out v);
            Vector3d normal = srf.NormalAt(u, v);

            if (x.Faces[0].OrientationIsReversed)
                normal.Reverse();

            Vector3d vnormal = new Vector3d(0, 0, 1);
            Vector3d tiltZ0 = new Vector3d(0, 0, 1);

            double tiltValue = Vector3d.VectorAngle(normal, tiltZ0, vnormal);
            double tilt = tiltValue * (180.0 / Math.PI);

            double azimuthValue = Vector3d.VectorAngle(new Vector3d(normal.X, normal.Y, 0), new Vector3d(0, 1, 0), vnormal);
            double azimuth = azimuthValue * (180 / Math.PI);

            double area = AreaMassProperties.Compute(x.Faces[0]).Area;

            if (azimuth is Double.NaN) azimuth = 0;
            if (tilt is Double.NaN) tilt = 0;
            if (area is Double.NaN) area = 0;

            return new double[3] { tilt, azimuth, area };
        }

        /// <summary>
        /// From https://www.homerenergy.com/products/pro/docs/latest/annualized_cost.html
        /// Interest rate, liftime must be in the same time resolution / units !
        /// </summary>
        /// <param name="interestRate">The real discount rate (includes inflation)</param>
        /// <param name="projectLifetime">The duration of the entire projects lifetime</param>
        /// <returns>Levelised values in units provided for duration of projectLifetime.</returns>
        public static double ComputeLevelisedValues(double[] valuesNonLevelised, double interestRate, double projectLifetime)
        {
            // From (with real discount rate) https://www.homerenergy.com/products/pro/docs/latest/capital_recovery_factor.html
            double CapitalRecoveryFactor(double i, double N) => i != 0 ? i * Math.Pow(1 + i, N) / (Math.Pow(1 + i, N) - 1) : 1;

            double NetPresentCost(double[] values, double i)
            {
                double[] netPresentValues = new double[values.Length];
                for (int t = 0; t < values.Length; t++)
                {
                    netPresentValues[t] = DiscountFactor(i, t) * values[t];
                }
                return netPresentValues.Sum();
            }

            double DiscountFactor(double i, double N) => 1 / Math.Pow(1 + i, N);

            double costLevelised = CapitalRecoveryFactor(interestRate, projectLifetime) * NetPresentCost(valuesNonLevelised, interestRate);

            return costLevelised;
        }
    }
}
