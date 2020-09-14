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

            var resultsTotalHeatingMonthly = results.Results.TotalFinalHeatingMonthly ?? new double[months];
            var strokeThickness = 4.0;

            var demandHeating = new ColumnSeries
            {
                ItemsSource = resultsTotalHeatingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Heating",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = SpaceHeatingColor
            };
            model.Series.Add(demandHeating);

            var resultsTotalCoolingMonthly = results.Results.TotalFinalCoolingMonthly ?? new double[months];
            var demandCooling = new ColumnSeries
            {
                ItemsSource = resultsTotalCoolingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Cooling",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = SpaceCoolingColor
            };
            model.Series.Add(demandCooling);

            var resultsTotalElectricityMonthly = results.Results.TotalFinalElectricityMonthly ?? new double[months];
            var demandElectricity = new ColumnSeries
            {
                ItemsSource = resultsTotalElectricityMonthly.Select(
                    demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Electricity",
                FillColor = BackgroundColor,
                StrokeThickness = strokeThickness,
                StrokeColor = ElectricityColor
            };
            model.Series.Add(demandElectricity);

            var resultsTotalDwhMonthly = results.Results.TotalFinalDomesticHotWaterMonthly ?? new double[months];
            var demandDhw = new ColumnSeries
            {
                ItemsSource = resultsTotalDwhMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
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
