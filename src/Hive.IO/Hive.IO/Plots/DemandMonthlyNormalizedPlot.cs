using System.Linq;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Hive.IO.Plots
{
    public class DemandMonthlyNormalizedPlot: OxyPlotBase
    {
        protected override PlotModel CreatePlotModel(ResultsPlotting results)
        {
            const int months = 12;
            var model = new PlotModel { Title = "Energy demand (Normalized Monthly)" };
            var totalFloorArea = results.TotalFloorArea;

            var strokeThickness = 4.0;

            var demandHeating = new ColumnSeries
            {
                ItemsSource = results.TotalHeatingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Heating",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = SpaceHeatingColor
            };
            model.Series.Add(demandHeating);

            var demandCooling = new ColumnSeries
            {
                ItemsSource = results.TotalCoolingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Cooling",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = SpaceCoolingColor
            };
            model.Series.Add(demandCooling);

            var demandElectricity = new ColumnSeries
            {
                ItemsSource = results.TotalElectricityMonthly.Select(
                    demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Electricity",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = ElectricityColor
            };
            model.Series.Add(demandElectricity);

            var demandDhw = new ColumnSeries
            {
                ItemsSource = results.TotalDomesticHotWaterMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " DWH",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = DhwColor,
            };
            model.Series.Add(demandDhw);

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Demand",
                Title = "kWh/m²"
            });

            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Months",
                ItemsSource = new[]
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
                }
            });
            return model;
        }
    }
}
