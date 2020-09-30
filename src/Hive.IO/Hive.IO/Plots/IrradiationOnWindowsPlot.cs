using System.Linq;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Hive.IO.Plots
{
    public class IrradiationOnWindowsPlot : OxyPlotBase
    {
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
                    FillColor = SpaceHeatingColor,
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