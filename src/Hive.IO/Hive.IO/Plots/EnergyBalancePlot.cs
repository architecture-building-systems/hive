using System;
using System.Drawing;
using System.Linq;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Hive.IO.DataHandling;

namespace Hive.IO.Plots
{
    public class EnergyBalancePlot: IVisualizerPlot
    {
        private RectangleF _bounds;
        private const float ArrowPadding = 5f;

        private static readonly Color[] GainsColors = new []
        {
            Color.FromArgb(255, 216, 0), // solar gains
            Color.FromArgb(181, 43, 40), // internal gains
            Color.FromArgb(204, 128, 28), // primary energy
            Color.FromArgb(242, 153, 35), // renewable energy
        };

        public void Render(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            _bounds = bounds;
            var houseBounds = bounds.CloneInflate(-bounds.Width / 3, -bounds.Height / 4);
            var innerHouseBounds = RenderHouse(graphics, houseBounds);

            // inner axis, centered inside the house, left is end point of gains, right is starting point of losses
            var middleAxis = innerHouseBounds.CloneInflate(-innerHouseBounds.Width / 4f, -10);
            graphics.DrawRectangleF(new Pen(Color.Aqua), middleAxis);

            var gainsArrowStart = (bounds.Left + houseBounds.Left) / 2f;

            RenderGainsArrows(results, graphics, gainsArrowStart, middleAxis);
        }

        private void RenderGainsArrows(ResultsPlotting results, Graphics graphics, float left, RectangleF middleAxis)
        {
            var gains = new[]
            {
                results.SolarGains, 
                results.InternalGains, 
                results.PrimaryEnergy, 
                results.RenewableEnergy
            };
            var totalGains = gains.Sum();
            var newMax = middleAxis.Height - ArrowPadding * gains.GetUpperBound(0);
            var y = middleAxis.Y;
            var width = middleAxis.Left - left;
            var color = GainsColors.Select(c => c).GetEnumerator();
            color.MoveNext();
            foreach (var gain in gains)
            {
                var arrowBounds = new RectangleF(left, y, width, gain.Scale(totalGains, newMax));
                graphics.FillRectangle(new SolidBrush(color.Current), arrowBounds);

                y += arrowBounds.Height + ArrowPadding;
                color.MoveNext();
            }
        }

        private RectangleF RenderHouse(Graphics graphics, RectangleF bounds)
        {
            var house = HousePolygon(bounds);
            graphics.FillPolygon(new SolidBrush(Color.LightSlateGray), house);
            graphics.DrawPolygon(new Pen(Color.Black), house);

            var innerHouse = HousePolygon(bounds.CloneInflate(-50, -50));
            graphics.FillPolygon(new SolidBrush(Color.White), innerHouse);
            graphics.DrawPolygon(new Pen(Color.Black), innerHouse);

            return new RectangleF(innerHouse[0].X, innerHouse[1].Y, innerHouse[3].X - innerHouse[0].X,
                innerHouse[0].Y - innerHouse[1].Y);
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
