using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    public abstract class EnergyBalancePlotBase : IVisualizerPlot
    {
        public abstract bool Normalized { get; }

        private const float ArrowPadding = 5f;
        private const float ArrowIndent = 10f;
        private const float IconPadding = 5f;
        private const float IconWidth = 20f;
        private const float LegendPadding = 20f;

        private static readonly Color[] GainsColors =
        {
            Color.FromArgb(255, 216, 0), // solar gains
            Color.FromArgb(181, 43, 40), // internal gains
            Color.FromArgb(204, 128, 28), // primary energy
            Color.FromArgb(242, 153, 35), // renewable energy
            Color.FromArgb(59, 88, 166), // Ventilation gains
            Color.FromArgb(120, 138, 163), // Envelope gains
            Color.FromArgb(153, 179, 214) // Windows gains
        };

        private static readonly Color[] LossesColors =
        {
            Color.FromArgb(62, 124, 47), // Electricity (Light + Equipment)
            Color.FromArgb(59, 88, 166), // Ventilation losses
            Color.FromArgb(120, 138, 163), // Envelope losses
            Color.FromArgb(153, 179, 214), // Windows losses
            Color.FromArgb(133, 101, 99), // System losses
            // Color.FromArgb(255, 255, 255) // Primary transfer losses
            Color.FromArgb(204, 255, 255), // Active cooling
            Color.FromArgb(204, 255, 153), // Surplus electricity
            Color.FromArgb(255, 153, 51) // Surplus heating energy
        };

        private static readonly string[] EnergyInStrings =
        {
            "Solar gains",
            "Internal gains",
            "Primary energy",
            "Renewable energy",
            "Ventilation gains",
            "Envelope gains",
            "Windows gains"
        };

        private static readonly string[] EnergyOutStrings =
        {
            "Electricity (Light + Equipment)",
            "Ventilation losses",
            "Envelope losses",
            "Windows losses",
            "System losses",
            "Active cooling",
            "Surplus electricity",
            "Surplus heating energy"
        };

        private RectangleF _bounds;

        /// <summary>
        ///     Figure out the height of the largest legend (hint: it's always the one on the right)
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        private static float LegendHeight
        {
            get
            {
                var height = (float) GH_FontServer.MeasureString("ENERGY IN/OUT", GH_FontServer.StandardBold).Height;
                height += EnergyOutStrings.Select(s => GH_FontServer.MeasureString(s, GH_FontServer.Standard).Height)
                    .Sum();
                height += IconPadding * EnergyOutStrings.Length;
                return height + 2 * LegendPadding;
            }
        }

        public void Render(ResultsPlotting results, Dictionary<string, string> plotParameters, Graphics graphics,
            RectangleF bounds)
        {
            _bounds = bounds;
            var houseBounds =
                bounds.CloneInflate(-bounds.Width / 3, 0); // place it in the middle, one third of the width
            houseBounds.Height = bounds.Height - LegendHeight; // leave room for the legend
            var innerHousePolygon = RenderHouse(graphics, houseBounds);

            var gains = new[]
            {
                results.SolarGains(Normalized),
                results.InternalGains(Normalized),
                results.PrimaryEnergy(Normalized),
                results.RenewableEnergy(Normalized),
                results.VentilationGains(Normalized),
                results.EnvelopeGains(Normalized),
                results.WindowsGains(Normalized)
            };
            var losses = new[]
            {
                results.Electricity(Normalized),
                results.VentilationLosses(Normalized),
                results.EnvelopeLosses(Normalized),
                results.WindowsLosses(Normalized),
                results.SystemLosses(Normalized),
                results.ActiveCooling(Normalized),
                results.SurplusElectricity(Normalized),
                results.SurplusHeating(Normalized)
            };

            if (losses.Any(loss => loss < 0.0f) || gains.Any(gain => gain < 0.0f))
            {
                // throw new Exception("Losses and Gains need to be positive values.");
                // FIXME: remove this when chris has fixed the bug
                losses = losses.Select(loss => Math.Max(0.0f, loss)).ToArray();
                gains = gains.Select(gain => Math.Max(0.0f, gain)).ToArray();
            }

            var maxTotal = Math.Max(gains.Sum(), losses.Sum());

            RenderGainsArrows(graphics, innerHousePolygon, houseBounds, bounds, gains, maxTotal);
            RenderLossesArrows(graphics, innerHousePolygon, houseBounds, bounds, losses, maxTotal);

            var legendBounds =
                new RectangleF(bounds.X, houseBounds.Bottom, bounds.Width, bounds.Height - houseBounds.Height)
                    .CloneInflate(-LegendPadding, -LegendPadding);
            RenderLegend(graphics, gains, losses, legendBounds);
        }

        public bool Contains(PointF location)
        {
            return _bounds.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            throw new NotImplementedException();
        }

        public void NewData(ResultsPlotting results)
        {
            // ignore for now
        }

        private void RenderLegend(Graphics graphics, float[] gains, float[] losses, RectangleF legendBounds)
        {
            var units = Normalized ? "kWh/m²" : "kWh";

            var leftTitle = "ENERGY IN";
            var totalGains = gains.Sum();
            var energyInStrings = gains.Zip(EnergyInStrings,
                    (gain, s) => $"{s}: {gain:0} {units} ({gain / totalGains * 100:0}%)"
                ).ToList();
            var leftLegendWidth =
                energyInStrings.Concat(new[] {leftTitle}).Select(
                    // NOTE: the "xxx" in the string template is used for a bit of padding
                    s => GH_FontServer.MeasureString($"{s}xxx", GH_FontServer.Standard).Width).Max() + IconWidth +
                IconPadding;

            var leftLegendBounds = new RectangleF(legendBounds.X, legendBounds.Y, leftLegendWidth, legendBounds.Height);
            RenderLegendColumn(graphics, leftTitle, leftLegendBounds, energyInStrings, GainsColors);

            var rightTitle = "ENERGY OUT";
            var totalLosses = losses.Sum();
            var energyOutStrings = losses.Zip(EnergyOutStrings,
                (loss, s) => $"{s}: {loss:0} {units} ({loss / totalLosses * 100:0}%)").ToList();
            var rightLegendWidth
                = energyOutStrings.Concat(new[] {rightTitle}).Select(
                      s => GH_FontServer.MeasureString($"{s}xxx", GH_FontServer.Standard).Width).Max() + IconWidth +
                  IconPadding;
            var rightLegendBounds = new RectangleF(legendBounds.Right - rightLegendWidth, legendBounds.Y,
                rightLegendWidth, legendBounds.Height);
            RenderLegendColumn(graphics, rightTitle, rightLegendBounds, energyOutStrings, LossesColors);
        }

        private static void RenderLegendColumn(Graphics graphics, string title, RectangleF legendBounds,
            IEnumerable<string> names,
            Color[] colors)
        {
            graphics.DrawString(title, GH_FontServer.StandardBold, Brushes.Black, legendBounds);
            var titleHeight = GH_FontServer.MeasureString(title, GH_FontServer.StandardBold).Height;
            var y = legendBounds.Y + titleHeight;
            using (var color = colors.Select(c => c).GetEnumerator())
            {
                color.MoveNext();
                foreach (var s in names)
                {
                    var size = GH_FontServer.MeasureString(s, GH_FontServer.Standard);
                    var iconBounds = new RectangleF(legendBounds.X, y, IconWidth, size.Height);
                    graphics.FillRectangle(new SolidBrush(color.Current), iconBounds);
                    graphics.DrawRectangleF(Pens.Black, iconBounds);
                    var textBounds = iconBounds.CloneRight(legendBounds.Width - IconWidth - IconPadding)
                        .CloneWithOffset(IconPadding, 0);
                    graphics.DrawString(s, GH_FontServer.Standard, Brushes.Black, textBounds);
                    y += size.Height + IconPadding;
                    color.MoveNext();
                }
            }
        }


        private void RenderGainsArrows(Graphics graphics, PointF[] innerHousePolygon, RectangleF houseBounds,
            RectangleF bounds, float[] gains, float max)
        {
            if (max.IsClose(0.0f))
                // no data computed yet
                return;

            // inner axis, centered inside the house, left is end point of gains, right is starting point of losses
            var innerHouseBounds = HousePolygonToInnerRectangleF(innerHousePolygon);
            var houseCenterBounds = innerHouseBounds.CloneInflate(-innerHouseBounds.Width / 4f, -10);

            var inflectionPointRight = innerHouseBounds.X + 10f;
            var rightBounds = new RectangleF(
                inflectionPointRight, houseCenterBounds.Y,
                houseCenterBounds.Left - inflectionPointRight, houseCenterBounds.Height);

            var gainsArrowLeft = (bounds.Left + houseBounds.Left) / 2f;
            var innerHouseTop = innerHousePolygon[2].Y;
            var leftBounds = new RectangleF(gainsArrowLeft, innerHouseTop, rightBounds.Width,
                houseBounds.Bottom - innerHouseTop);

            var totalGains = gains.Sum();
            var newMax = rightBounds.Height - ArrowPadding * gains.GetUpperBound(0);
            var leftArrowPadding = (leftBounds.Height - newMax) / gains.GetUpperBound(0);
            var rightY = rightBounds.Y;
            var leftY = leftBounds.Y;
            var blackPen = new Pen(Color.Black);
            var blackBrush = new SolidBrush(Color.Black);
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            using (var color = GainsColors.Select(c => c).GetEnumerator())
            {
                color.MoveNext();
                foreach (var gain in gains)
                {
                    var arrowHeight = gain.Scale(max, newMax);
                    var rightArrowBounds = new RectangleF(rightBounds.Left, rightY, rightBounds.Width, arrowHeight);
                    var leftArrowBounds = new RectangleF(leftBounds.Left, leftY, leftBounds.Width, arrowHeight);

                    var arrowPolygon = CreateGainsArrowPolygon(leftArrowBounds, rightArrowBounds);
                    graphics.FillPolygon(new SolidBrush(color.Current), arrowPolygon);
                    graphics.DrawPolygon(blackPen, arrowPolygon);

                    // write the percentages
                    var gainPercent = gain / totalGains * 100;
                    graphics.DrawString($"{gainPercent:F0}%", GH_FontServer.Standard, blackBrush, rightArrowBounds,
                        format);

                    rightY += rightArrowBounds.Height + ArrowPadding;
                    leftY += rightArrowBounds.Height + leftArrowPadding;
                    color.MoveNext();
                }
            }
        }

        private void RenderLossesArrows(Graphics graphics, PointF[] innerHousePolygon, RectangleF houseBounds,
            RectangleF bounds, float[] losses, float max)
        {
            if (max.IsClose(0.0f))
                // no data computed yet
                return;

            // inner axis, centered inside the house, left is end point of gains, right is starting point of losses
            var innerHouseBounds = HousePolygonToInnerRectangleF(innerHousePolygon);

            var houseCenterBounds = innerHouseBounds.CloneInflate(-innerHouseBounds.Width / 4f, -10);

            var inflectionPointLeft = innerHouseBounds.Right - 10f;
            var leftBounds = new RectangleF(
                houseCenterBounds.Right, houseCenterBounds.Y,
                inflectionPointLeft - houseCenterBounds.Right, houseCenterBounds.Height);

            var lossesArrowRight = (bounds.Right + houseBounds.Right) / 2f;
            var innerHouseTop = innerHousePolygon[2].Y;
            var rightBounds = new RectangleF(lossesArrowRight - leftBounds.Width, innerHouseTop, leftBounds.Width,
                houseBounds.Bottom - innerHouseTop);

            var totalLosses = losses.Sum();
            var newMax = leftBounds.Height - ArrowPadding * losses.GetUpperBound(0);
            var rightArrowPadding = (rightBounds.Height - newMax) / losses.GetUpperBound(0);
            var leftY = leftBounds.Y;
            var rightY = rightBounds.Y;
            var blackPen = new Pen(Color.Black);
            var blackBrush = new SolidBrush(Color.Black);
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            using (var color = LossesColors.Select(c => c).GetEnumerator())
            {
                color.MoveNext();
                foreach (var loss in losses)
                {
                    var arrowHeight = loss.Scale(max, newMax);
                    var leftArrowBounds = new RectangleF(leftBounds.Left, leftY, leftBounds.Width, arrowHeight);
                    var rightArrowBounds = new RectangleF(rightBounds.Left, rightY, rightBounds.Width, arrowHeight);

                    var arrowPolygon = CreateLossesArrowPolygon(leftArrowBounds, rightArrowBounds);
                    graphics.FillPolygon(new SolidBrush(color.Current), arrowPolygon);
                    graphics.DrawPolygon(blackPen, arrowPolygon);

                    // write the percentages
                    var lossPercent = loss / totalLosses * 100;
                    graphics.DrawString($"{lossPercent:F0}%", GH_FontServer.Standard, blackBrush, leftArrowBounds,
                        format);

                    leftY += leftArrowBounds.Height + ArrowPadding;
                    rightY += rightArrowBounds.Height + rightArrowPadding;
                    color.MoveNext();
                }
            }
        }

        private PointF[] CreateGainsArrowPolygon(RectangleF leftBounds, RectangleF rightBounds)
        {
            var rightMiddleY = rightBounds.Y + 0.5f * rightBounds.Height;
            var leftMiddleY = leftBounds.Y + 0.5f * leftBounds.Height;
            return new[]
            {
                new PointF(leftBounds.Left, leftBounds.Top), // 0
                new PointF(leftBounds.Right, leftBounds.Top), // 1
                new PointF(rightBounds.Left, rightBounds.Top), // 2
                new PointF(rightBounds.Right - ArrowIndent, rightBounds.Top), // 3
                new PointF(rightBounds.Right, rightMiddleY), // 4
                new PointF(rightBounds.Right - ArrowIndent, rightBounds.Bottom), // 5
                new PointF(rightBounds.Left, rightBounds.Bottom), // 6
                new PointF(leftBounds.Right, leftBounds.Bottom), // 7
                new PointF(leftBounds.Left, leftBounds.Bottom), // 8
                new PointF(leftBounds.Left + ArrowIndent, leftMiddleY) // 9 
            };
        }

        /// <summary>
        ///     2-------------3
        ///     0        /               \
        ///     ---------1                > 4
        ///     \                        /
        ///     > 9       6-------------5
        ///     /          /
        ///     8 ----------7
        /// </summary>
        /// <param name="leftBounds"></param>
        /// <param name="rightBounds"></param>
        /// <returns></returns>
        private PointF[] CreateLossesArrowPolygon(RectangleF leftBounds, RectangleF rightBounds)
        {
            var leftMiddleY = leftBounds.Y + 0.5f * leftBounds.Height;
            var rightMiddleY = rightBounds.Y + 0.5f * rightBounds.Height;
            return new[]
            {
                new PointF(leftBounds.Left, leftBounds.Top), // 0
                new PointF(leftBounds.Right, leftBounds.Top), // 1
                new PointF(rightBounds.Left, rightBounds.Top), // 2
                new PointF(rightBounds.Right - ArrowIndent, rightBounds.Top), // 3
                new PointF(rightBounds.Right, rightMiddleY), // 4
                new PointF(rightBounds.Right - ArrowIndent, rightBounds.Bottom), // 5
                new PointF(rightBounds.Left, rightBounds.Bottom), // 6
                new PointF(leftBounds.Right, leftBounds.Bottom), // 7
                new PointF(leftBounds.Left, leftBounds.Bottom), // 8
                new PointF(leftBounds.Left + ArrowIndent, leftMiddleY) // 9 
            };
        }

        private PointF[] RenderHouse(Graphics graphics, RectangleF bounds)
        {
            var house = HousePolygon(bounds);
            graphics.FillPolygon(new SolidBrush(Color.LightSlateGray), house);
            graphics.DrawPolygon(new Pen(Color.Black), house);

            var innerHouse = HousePolygon(bounds.CloneInflate(-50, -50));
            graphics.FillPolygon(new SolidBrush(Color.White), innerHouse);
            graphics.DrawPolygon(new Pen(Color.Black), innerHouse);

            return innerHouse;
        }

        private static RectangleF HousePolygonToInnerRectangleF(PointF[] housePolygon)
        {
            return new RectangleF(housePolygon[0].X, housePolygon[1].Y, housePolygon[3].X - housePolygon[0].X,
                housePolygon[0].Y - housePolygon[1].Y);
        }

        private static PointF[] HousePolygon(RectangleF bounds)
        {
            var xLeft = bounds.Left;
            var xRight = bounds.Right;
            var xMiddle = xLeft + bounds.Width / 2;
            var yBottom = bounds.Y + bounds.Height;
            var yTop = bounds.Top;
            var yRoof = bounds.Y + bounds.Height * 0.33f;
            var house = new[]
            {
                // start at bottom left, clockwise
                new PointF(xLeft, yBottom), // 0
                new PointF(xLeft, yRoof), // 1
                new PointF(xMiddle, yTop), // 2
                new PointF(xRight, yRoof), // 3
                new PointF(xRight, yBottom) // 4
            };
            return house;
        }
    }

    public class EnergyBalancePlot : EnergyBalancePlotBase
    {
        public override bool Normalized => false;
    }

    public class EnergyBalanceNormalizedPlot : EnergyBalancePlotBase
    {
        public override bool Normalized => true;
    }
}