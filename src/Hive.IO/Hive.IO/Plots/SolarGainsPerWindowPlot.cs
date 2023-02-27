using System;
using System.Collections.Generic;
using System.Linq;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Hive.IO.Plots
{
    public abstract class SolarGainsPerWindowPlotBase : OxyPlotBase
    {
        protected abstract bool Normalize { get; }
        protected abstract string Units { get; }
        protected abstract string PlotTitle { get; }

        private const int MaxColors = 8;
        private static readonly List<OxyColor> Colors = Enumerable.Range(0, MaxColors).Select(i => OxyColor.FromRgb(
            (byte) (235 - i * 5),
            (byte) (255 - i * 25),
            (byte) (140 - i * 20)
        )).ToList();

        protected override PlotModel CreatePlotModel(ResultsPlotting results, Dictionary<string, string> plotParameters)
        {
            var model = new PlotModel {Title = PlotTitle};
            var irradiationList = new List<double[]>();

            var windowArea = results.AreasPerWindow.Sum();
            // add the data
            for (int i = 0; i < results.IrradiationOnWindows?.Count; i++)
            {
                model.Series.Add(new ColumnSeries
                {
                    ItemsSource = results.IrradiationOnWindows[i].Select(x => new ColumnItem {Value = Normalize ? x / windowArea : x}),
                    IsStacked = true,
                    FillColor = Colors[i % MaxColors],
                    Title = $" Win{i:00}",
                    LabelFormatString = "{0:0}",
                    LabelPlacement = LabelPlacement.Middle
                });
                irradiationList.Add(results.IrradiationOnWindows[i].Select(x => Normalize ? x / windowArea : x ).ToArray());
            }

            double? axisMinimum = new double();
            double? axisMaximum = new double();
            if (Normalize)
            {
                axisMinimum = plotParameters.ReadDouble("SolarGainsNormalized-Axis-Minimum");
                axisMaximum = plotParameters.ReadDouble("SolarGainsNormalized-Axis-Maximum");
            } else
            {
                axisMinimum = plotParameters.ReadDouble("SolarGains-Axis-Minimum");
                axisMaximum = plotParameters.ReadDouble("SolarGains-Axis-Maximum");
            }
            
            
            // add the axes
            var axis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Irradiation",
                Title = Units
            };
            
            if (axisMinimum.HasValue)
            {
                axis.Minimum = axisMinimum.Value;
            }
            
            if (axisMaximum.HasValue)
            {
                axis.Maximum = axisMaximum.Value;
            }
            model.Axes.Add(axis);

            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Months",
                ItemsSource = Misc.MonthNames
            });

            var irradiation = getMonthlyIrradiation(irradiationList);

            var totalLabel = new OxyPlot.Annotations.TextAnnotation
            {
                Text = Normalize ? $" Total: { irradiation.Sum():00} kWh/m²/Year" : $" Total: { irradiation.Sum():00} kWh/Year",
                TextPosition = new DataPoint(-0.3, (axisMaximum.HasValue ? axisMaximum.Value : irradiation.Max()) *0.93),
                TextHorizontalAlignment = HorizontalAlignment.Left
            };
            model.Annotations.Add(totalLabel);

            return model;
        }

        public List<double> getMonthlyIrradiation(List<double[]> irradiationOnWindows)
        {
            if (irradiationOnWindows.Count() > 0)
            {
                var sums = Enumerable.Range(0, irradiationOnWindows[0].Length)
               .Select(i => irradiationOnWindows.Select(
                         nums => nums[i]
                      ).Sum()
               );
               return sums.ToList();
            }
            return new List<double> { 0};
        }
    }


    public class SolarGainsPerWindowPlot : SolarGainsPerWindowPlotBase
    {
        protected override bool Normalize => false;
        protected override string Units => "kWh";
        protected override string PlotTitle => $"Solar Gains per Window (kWh)";
    }

    public class SolarGainsPerWindowNormalizedPlot : SolarGainsPerWindowPlotBase
    {
        protected override bool Normalize => true;
        protected override string Units => "kWh / m²";
        protected override string PlotTitle => $"Solar Gains per Window, Normalized per Window Area (kWh / m²)";
    }

}