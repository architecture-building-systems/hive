using System;
using System.Drawing;
using System.Linq;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Hive.IO.DataHandling;

namespace Hive.IO.Plots
{
    public class EnergyBalancePlot : IVisualizerPlot
    {
        private RectangleF _bounds;
        private const float ArrowPadding = 5f;
        private const float ArrowIndent = 10f;

        private static readonly Color[] GainsColors = {
            Color.FromArgb(255, 216, 0), // solar gains
            Color.FromArgb(181, 43, 40), // internal gains
            Color.FromArgb(204, 128, 28), // primary energy
            Color.FromArgb(242, 153, 35), // renewable energy
        };

        private static readonly Color[] LossesColors = {
            Color.FromArgb(62, 124, 47), // Electricity (Light + Equipment)
            Color.FromArgb(59, 88, 166), // Ventilation losses
            Color.FromArgb(120,138, 163), // Envelope losses
            Color.FromArgb(153, 179, 214), // Windows losses
            Color.FromArgb(133, 101, 99), // System losses
            Color.FromArgb(255, 255, 255), // Primary transfer losses
        };

        public void Render(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            _bounds = bounds;
            var houseBounds = bounds.CloneInflate(-bounds.Width / 3, -bounds.Height / 4);
            var innerHousePolygon = RenderHouse(graphics, houseBounds);

            RenderGainsArrows(results, graphics, innerHousePolygon, houseBounds, bounds);
            RenderLossesArrows(results, graphics, innerHousePolygon, houseBounds, bounds);
        }

        private void RenderGainsArrows(ResultsPlotting results, Graphics graphics,
            PointF[] innerHousePolygon, RectangleF houseBounds, RectangleF bounds)
        {
            // inner axis, centered inside the house, left is end point of gains, right is starting point of losses
            var innerHouseBounds = HousePolygonToInnerRectangleF(innerHousePolygon);
            var houseCenterBounds = innerHouseBounds.CloneInflate(-innerHouseBounds.Width / 4f, -10);

            var inflectionPointRight = innerHouseBounds.X + 10f;
            var rightBounds = new RectangleF(
                inflectionPointRight, houseCenterBounds.Y,
                houseCenterBounds.Left - inflectionPointRight, houseCenterBounds.Height);

            var gainsArrowLeft = (bounds.Left + houseBounds.Left) / 2f;
            var innerHouseTop = innerHousePolygon[2].Y;
            var leftBounds = new RectangleF(gainsArrowLeft, innerHouseTop, rightBounds.Width, houseBounds.Bottom - innerHouseTop);

            var gains = new[]
            {
                results.SolarGains,
                results.InternalGains,
                results.PrimaryEnergy,
                results.RenewableEnergy
            };
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
                    var arrowHeight = gain.Scale(totalGains, newMax);
                    var rightArrowBounds = new RectangleF(rightBounds.Left, rightY, rightBounds.Width, arrowHeight);
                    var leftArrowBounds = new RectangleF(leftBounds.Left, leftY, leftBounds.Width, arrowHeight);

                    var arrowPolygon = CreateGainsArrowPolygon(leftArrowBounds, rightArrowBounds);
                    graphics.FillPolygon(new SolidBrush(color.Current), arrowPolygon);
                    graphics.DrawPolygon(blackPen, arrowPolygon);

                    // write the percentages
                    var gainPercent = gain / totalGains * 100;
                    graphics.DrawString($"{gainPercent:F0}%", GH_FontServer.Standard, blackBrush, rightArrowBounds, format);

                    rightY += rightArrowBounds.Height + ArrowPadding;
                    leftY += rightArrowBounds.Height + leftArrowPadding;
                    color.MoveNext();
                }
            }
        }

        private void RenderLossesArrows(ResultsPlotting results, Graphics graphics,
            PointF[] innerHousePolygon, RectangleF houseBounds, RectangleF bounds)
        {
            // inner axis, centered inside the house, left is end point of gains, right is starting point of losses
            var innerHouseBounds = HousePolygonToInnerRectangleF(innerHousePolygon);

            var houseCenterBounds = innerHouseBounds.CloneInflate(-innerHouseBounds.Width / 4f, -10);

            var inflectionPointLeft = innerHouseBounds.Right - 10f;
            var leftBounds = new RectangleF(
                houseCenterBounds.Right, houseCenterBounds.Y,
                inflectionPointLeft - houseCenterBounds.Right, houseCenterBounds.Height);

            var lossesArrowRight = (bounds.Right + houseBounds.Right) / 2f;
            var innerHouseTop = innerHousePolygon[2].Y;
            var rightBounds = new RectangleF(lossesArrowRight - leftBounds.Width, innerHouseTop, leftBounds.Width, houseBounds.Bottom - innerHouseTop);

            var losses = new[]
            {
                results.Electricity,
                results.VentilationLosses,
                results.EnvelopeLosses,
                results.WindowsLosses,
                results.SystemLosses,
                results.PrimaryTransferLosses,
            };
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
                    var arrowHeight = loss.Scale(totalLosses, newMax);
                    var leftArrowBounds = new RectangleF(leftBounds.Left, leftY, leftBounds.Width, arrowHeight);
                    var rightArrowBounds = new RectangleF(rightBounds.Left, rightY, rightBounds.Width, arrowHeight);

                    var arrowPolygon = CreateLossesArrowPolygon(leftArrowBounds, rightArrowBounds);
                    graphics.FillPolygon(new SolidBrush(color.Current), arrowPolygon);
                    graphics.DrawPolygon(blackPen, arrowPolygon);

                    // write the percentages
                    var lossPercent = loss / totalLosses * 100;
                    graphics.DrawString($"{lossPercent:F0}%", GH_FontServer.Standard, blackBrush, leftArrowBounds, format);

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
                new PointF(leftBounds.Left + ArrowIndent, leftMiddleY), // 9 
            };
        }
        /// <summary>
        ///
        ///            2-------------3
        ///  0        /               \
        ///  ---------1                > 4
        ///  \                        /
        ///   > 9       6-------------5
        ///  /          /
        /// 8 ----------7
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
                new PointF(leftBounds.Left + ArrowIndent, leftMiddleY), // 9 
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
                new PointF(xLeft, yBottom),  // 0
                new PointF(xLeft, yRoof),  // 1
                new PointF(xMiddle, yTop),  // 2
                new PointF(xRight, yRoof),  // 3
                new PointF(xRight, yBottom),  // 4
            };
            return house;
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
    }
}
