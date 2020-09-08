using System.Linq;
using Hive.IO.DataHandling;
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

            var resultsTotalHeatingMonthly = results.Results.TotalFinalHeatingMonthly ?? new double[months];
            var demandHeating = new ColumnSeries
            {
                ItemsSource = resultsTotalHeatingMonthly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Space Heating",
                FillColor = SpaceHeatingColor
            };
            model.Series.Add(demandHeating);

            var resultsTotalCoolingMonthly = results.Results.TotalFinalCoolingMonthly ?? new double[months];
            var demandCooling = new ColumnSeries
            {
                ItemsSource = resultsTotalCoolingMonthly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Space Cooling",
                FillColor = SpaceCoolingColor
            };
            model.Series.Add(demandCooling);

            var resultsTotalElectricityMonthly = results.Results.TotalFinalElectricityMonthly ?? new double[months];
            var demandElectricity = new ColumnSeries
            {
                ItemsSource = resultsTotalElectricityMonthly.Select(demand => new ColumnItem { Value = demand }),
                Title = " Electricity",
                FillColor = ElectricityColor
            };
            model.Series.Add(demandElectricity);

            var resultsTotalDwhMonthly = results.Results.TotalFinalDomesticHotWaterMonthly ?? new double[months];
            var demandDhw = new ColumnSeries
            {
                ItemsSource = resultsTotalDwhMonthly.Select(demand => new ColumnItem { Value = demand }),
                Title = " DWH",
                FillColor = DhwColor,
            };
            model.Series.Add(demandDhw);

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
