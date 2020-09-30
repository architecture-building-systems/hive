using System.Collections.Generic;
using System.Linq;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Hive.IO.Plots
{
    public class IrradiationOnWindowsPlot : OxyPlotBase
    {
        private static readonly List<OxyColor> Colors = Enumerable.Range(0, 8).Select(i => OxyColor.FromRgb(
            (byte) (235 - i * 5),
            (byte) (255 - i * 25),
            (byte) (140 - i * 20)
        )).ToList();

        protected override PlotModel CreatePlotModel(ResultsPlotting results)
        {
            var model = new PlotModel {Title = "Total Irradiation (kWh)"};

            // add the data
            var i = 0;
            foreach (var windowIrradiation in results.IrradiationOnWindows)
            {
                model.Series.Add(new ColumnSeries
                {
                    ItemsSource = windowIrradiation.Select(x => new ColumnItem {Value = x}),
                    IsStacked = true,
                    FillColor = Colors[i],
                    Title = $" Window{i}"
                });
                i++;
            }

            // add the axes
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Irradiation",
                Title = "kWh"
            });

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