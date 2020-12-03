using System.Collections.Generic;
using System.Linq;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Hive.IO.Plots
{
    public class SolarGainsPerWindowPlot : OxyPlotBase
    {
        private const int MaxColors = 8;
        private static readonly List<OxyColor> Colors = Enumerable.Range(0, MaxColors).Select(i => OxyColor.FromRgb(
            (byte) (235 - i * 5),
            (byte) (255 - i * 25),
            (byte) (140 - i * 20)
        )).ToList();

        protected override PlotModel CreatePlotModel(ResultsPlotting results, Dictionary<string, string> plotParameters)
        {
            var model = new PlotModel {Title = "Solar Gains per Window (kWh)"};

            // add the data
            var i = 0;
            foreach (var windowIrradiation in results.IrradiationOnWindows)
            {
                model.Series.Add(new ColumnSeries
                {
                    ItemsSource = windowIrradiation.Select(x => new ColumnItem {Value = x}),
                    IsStacked = true,
                    FillColor = Colors[i % MaxColors],
                    Title = $" Win{i:00}",
                    LabelFormatString = "{0:0}",
                    LabelPlacement = LabelPlacement.Middle
                });
                i++;
            }

            // add the axes
            var axis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Irradiation",
                Title = "kWh"
            };
            var axisMinimum = plotParameters.ReadDouble("SolarGains-Axis-Minimum");
            if (axisMinimum.HasValue)
            {
                axis.Minimum = axisMinimum.Value;
            }

            var axisMaximum = plotParameters.ReadDouble("SolarGains-Axis-Maximum");
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

            return model;
        }
    }
}