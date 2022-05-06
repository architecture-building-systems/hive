using System.Collections.Generic;
using System.Linq;
using Hive.IO.Results;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Hive.IO.Plots
{
    public class DemandMonthlyNormalizedPlot: OxyPlotBase
    {
        protected override PlotModel CreatePlotModel(ResultsPlotting results, Dictionary<string, string> plotParameters)
        {
            var model = new PlotModel { Title = "Energy demand (Normalized Monthly)" };
            var totalFloorArea = results.TotalFloorArea;

            //var strokeThickness = 4.0;

            var demandHeating = new ColumnSeries
            {
                ItemsSource = results.TotalHeatingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Heating",
                FillColor = SpaceHeatingColor,
                //StrokeThickness = strokeThickness,
                //StrokeColor = SpaceHeatingColor
            };
            model.Series.Add(demandHeating);

            var demandCooling = new ColumnSeries
            {
                ItemsSource = results.TotalCoolingMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Space Cooling",
                FillColor = SpaceCoolingColor,
                //StrokeThickness = strokeThickness,
                //StrokeColor = SpaceCoolingColor
            };
            model.Series.Add(demandCooling);

            var demandElectricity = new ColumnSeries
            {
                ItemsSource = results.TotalElectricityMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " Grid Electricity",
                FillColor = ElectricityColor,
                //StrokeThickness = strokeThickness,
                //StrokeColor = ElectricityColor
            };
            model.Series.Add(demandElectricity);

            var demandDhw = new ColumnSeries
            {
                ItemsSource = results.TotalDomesticHotWaterMonthly.Select(demand => new ColumnItem { Value = demand / totalFloorArea }),
                Title = " DHW",
                FillColor = DhwColor,
                //StrokeThickness = strokeThickness,
                //StrokeColor = DhwColor,
            };
            model.Series.Add(demandDhw);

            var axis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Demand",
                Title = "kWh/m²",
            };
            var axisMinimum = plotParameters.ReadDouble("EnergyDemandNormalized-Axis-Minimum");
            if (axisMinimum.HasValue)
            {
                axis.Minimum = axisMinimum.Value;
            }

            var axisMaximum = plotParameters.ReadDouble("EnergyDemandNormalized-Axis-Maximum");
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
