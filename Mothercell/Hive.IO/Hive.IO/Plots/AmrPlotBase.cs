using System.Drawing;
using Grasshopper.Kernel;

namespace Hive.IO.Plots
{
    /// <summary>
    /// Base plot for AMR-plots - draw the grid and fill in dummy text. Define the sub-areas for subclasses
    /// to draw in.
    ///
    /// To be derived.
    /// </summary>
    public class AmrPlotBase: IVisualizerPlot
    {
        protected RectangleF Bounds { get; private set; }
        protected string Title => "AmrPlot";

        protected Font TitleFont => GH_FontServer.Large;
        protected Font BoldFont => GH_FontServer.StandardBold;
        protected Font NormalFont => GH_FontServer.Standard;

        // calculate row heights and column widths of the grid
        private float RightAxisWidth => GH_FontServer.MeasureString("1000", BoldFont).Width;
        private float LeftAxisWidth => RightAxisWidth;
        private float EmbodiedColumnWidth => (Bounds.Width - RightAxisWidth - LeftAxisWidth) / 2;
        private float OperationColumnWidth => EmbodiedColumnWidth;

        private float TitleHeight => GH_FontServer.MeasureString("Title", TitleFont).Height;
        private float ColumnTitleHeight => GH_FontServer.MeasureString("Embodied / Operation", BoldFont).Height;
        private float ColumnLegendHeight => GH_FontServer.MeasureString("legend", NormalFont).Height;

        private float BuildingsRowHeight =>
            (Bounds.Height - TitleHeight - ColumnTitleHeight - ColumnLegendHeight * 2) / 2;

        private float SystemsRowHeight => BuildingsRowHeight;

        // title rows
        protected RectangleF TitleBounds => new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, TitleHeight);

        protected RectangleF LeftTopBounds => new RectangleF(TitleBounds.X, TitleBounds.Bottom, LeftAxisWidth,
            ColumnTitleHeight + ColumnLegendHeight);


        protected RectangleF EmbodiedTitleBounds => new RectangleF(LeftTopBounds.Right, LeftTopBounds.Top,
            EmbodiedColumnWidth, ColumnTitleHeight);

        protected RectangleF OperationTitleBounds => EmbodiedTitleBounds.CloneRight(OperationColumnWidth);

        protected RectangleF EmbodiedBuildingsLegendBounds => new RectangleF(EmbodiedTitleBounds.X, EmbodiedTitleBounds.Bottom,
            EmbodiedColumnWidth, ColumnLegendHeight);

        protected RectangleF OperationBuildingsLegendBounds =>
            EmbodiedBuildingsLegendBounds.CloneWithOffset(EmbodiedColumnWidth, 0);

        protected RectangleF RightTopBounds => new RectangleF(OperationTitleBounds.Right, OperationTitleBounds.Top,
            RightAxisWidth, LeftTopBounds.Height);


        // buildings row
        protected RectangleF BuildingsLeftAxisBounds => LeftTopBounds.CloneDown(BuildingsRowHeight);

        protected RectangleF EmbodiedBuildingsPlotBounds =>
            BuildingsLeftAxisBounds.CloneRight(EmbodiedColumnWidth);

        protected RectangleF OperationBuildingsPlotBounds =>
            EmbodiedBuildingsPlotBounds.CloneRight(OperationColumnWidth);
        protected RectangleF BuildingsRightAxisBounds => OperationBuildingsPlotBounds.CloneRight(RightAxisWidth);


        // systems row
        protected RectangleF SystemsLeftAxisBounds => BuildingsLeftAxisBounds.CloneDown(SystemsRowHeight);

        protected RectangleF EmbodiedSystemsPlotBounds => SystemsLeftAxisBounds.CloneRight(EmbodiedColumnWidth);

        protected RectangleF OperationSystemsPlotBounds => EmbodiedSystemsPlotBounds.CloneRight(OperationColumnWidth);
        protected RectangleF SystemsRightAxisBounds => OperationSystemsPlotBounds.CloneRight(RightAxisWidth);

        // bottom legend row
        protected RectangleF LeftBottomBounds => SystemsLeftAxisBounds.CloneDown(ColumnLegendHeight);
        protected RectangleF EmbodiedSystemsLegendBounds => LeftBottomBounds.CloneRight(EmbodiedColumnWidth);

        protected RectangleF OperationSystemsLegendBounds =>
            EmbodiedSystemsLegendBounds.CloneRight(OperationColumnWidth);

        protected RectangleF RightBottomBounds => OperationSystemsLegendBounds.CloneRight(RightAxisWidth);

        public void Render(Results results, Graphics graphics, RectangleF bounds)
        {
            Bounds = bounds;

            // draw the grid
            var borderPen = new Pen(Color.Black);
            graphics.DrawRectangleF(borderPen, LeftTopBounds);
            graphics.DrawRectangleF(borderPen, EmbodiedTitleBounds);
            graphics.DrawRectangleF(borderPen, OperationTitleBounds);
            graphics.DrawRectangleF(borderPen, EmbodiedBuildingsLegendBounds);
            graphics.DrawRectangleF(borderPen, OperationBuildingsLegendBounds);
            graphics.DrawRectangleF(borderPen, RightTopBounds);
            
            graphics.DrawRectangleF(borderPen, BuildingsLeftAxisBounds);
            graphics.DrawRectangleF(borderPen, EmbodiedBuildingsPlotBounds);
            graphics.DrawRectangleF(borderPen, OperationBuildingsPlotBounds);
            graphics.DrawRectangleF(borderPen, BuildingsRightAxisBounds);

            graphics.DrawRectangleF(borderPen, SystemsLeftAxisBounds);
            graphics.DrawRectangleF(borderPen, EmbodiedSystemsPlotBounds);
            graphics.DrawRectangleF(borderPen, OperationSystemsPlotBounds);
            graphics.DrawRectangleF(borderPen, SystemsRightAxisBounds);

            graphics.DrawRectangleF(borderPen, LeftBottomBounds);
            graphics.DrawRectangleF(borderPen, EmbodiedSystemsLegendBounds);
            graphics.DrawRectangleF(borderPen, OperationSystemsLegendBounds);
            graphics.DrawRectangleF(borderPen, RightBottomBounds);
        }

        public void NewData(Results results)
        {
            // for the moment, we'll not do any caching...
        }
    }

    public static class RectangleFExtensions
    {
        private static RectangleF Clone(this RectangleF self)
        {
            return new RectangleF(self.Location, self.Size);
        }

        /// <summary>
        /// Create a copy of self and move it by deltaX and deltaY.
        /// </summary>
        /// <param name="self">The rectangle to copy</param>
        /// <param name="deltaX">Move the result by this amount in x direction</param>
        /// <param name="deltaY">Move the result by this amount in y direction</param>
        /// <returns></returns>
        public static RectangleF CloneWithOffset(this RectangleF self, float deltaX, float deltaY)
        {
            var result = self.Clone();
            result.Offset(deltaX, deltaY);
            return result;
        }

        public static RectangleF CloneRight(this RectangleF self, float? newWidth = null)
        {
            var result = self.CloneWithOffset(self.Width, 0);
            if (newWidth.HasValue)
            {
                result.Width = newWidth.Value;
            }
            return result;
        }

        public static RectangleF CloneDown(this RectangleF self, float? newHeight = null)
        {
            var result = self.CloneWithOffset(0, self.Height);
            if (newHeight.HasValue)
            {
                result.Height = newHeight.Value;
            }

            return result;
        }

        public static void DrawRectangleF(this Graphics self, Pen pen, RectangleF rectangle)
        {
            self.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}
