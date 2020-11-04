using System.Linq;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Hive.IO.Plots
{
    public class DemandMonthlyPlot : OxyPlotBase
    {
        protected override PlotModel CreatePlotModel(ResultsPlotting results)
        {
            const int months = 12;
            var model = new PlotModel { Title = "Energy demand (Total Monthly)" };

            var demandHeating = new ColumnSeries
            {
                ItemsSource = results.TotalHeatingMonthly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Space Heating",
                FillColor = SpaceHeatingColor
            };
            model.Series.Add(demandHeating);

            var demandCooling = new ColumnSeries
            {
                ItemsSource = results.TotalCoolingMonthly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Space Cooling",
                FillColor = SpaceCoolingColor
            };
            model.Series.Add(demandCooling);

            var demandElectricity = new ColumnSeries
            {
                ItemsSource = results.TotalElectricityMonthly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Grid Electricity",
                FillColor = ElectricityColor
            };
            model.Series.Add(demandElectricity);

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Demand",
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
