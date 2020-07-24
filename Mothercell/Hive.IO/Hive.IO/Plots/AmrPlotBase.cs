using System;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace Hive.IO.Plots
{
    /// <summary>
    /// Base plot for AMR-plots - draw the grid and fill in dummy text. Define the sub-areas for subclasses
    /// to draw in.
    ///
    /// To be derived.
    /// </summary>
    public class AmrPlotBase : IVisualizerPlot
    {
        public AmrPlotBase(bool normalized)
        {
            Normalized = normalized;
        }

        protected ResultsPlotting Results { get; private set; }

        private float textBoxPadding = 50;

        protected RectangleF Bounds { get; private set; }
        protected virtual string Title => "AmrPlot";

        protected Font TitleFont => GH_FontServer.Large;
        protected Font BoldFont => GH_FontServer.StandardBold;
        protected Font NormalFont => GH_FontServer.Standard;

        protected Brush TextBrush => new SolidBrush(Color.Black);

        // calculate row heights and column widths of the grid
        private float RightAxisWidth => GH_FontServer.MeasureString("10000", BoldFont).Width + textBoxPadding;
        private float LeftAxisWidth => RightAxisWidth;
        private float EmbodiedColumnWidth => (Bounds.Width - RightAxisWidth - LeftAxisWidth) / 2;
        private float OperationColumnWidth => EmbodiedColumnWidth;

        private float TitleHeight => GH_FontServer.MeasureString("Title", TitleFont).Height + textBoxPadding;
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

        // Column titles (subclasses should override these to provide the calculated values)
        protected virtual string Unit => "kgCO2";

        protected float AxisMax => EmbodiedBuildings + EmbodiedSystems + OperationBuildings + OperationSystems;

        public void Render(ResultsPlotting results, Graphics graphics, RectangleF bounds)
        {
            if (Results == null)
            {
                Results = results;
            }

            Bounds = bounds;
            RenderGrid(graphics);
            RenderTitle(graphics);
            RenderColumnTitles(graphics);
            RenderLeftAxis(graphics);
            RenderRightAxis(graphics);

            RenderPlot(graphics);
        }

        protected virtual void RenderPlot(Graphics graphics)
        {
        }

        public bool Contains(PointF location)
        {
            return Bounds.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
        }

        private void RenderTitle(Graphics graphics)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;

            graphics.DrawString(Title, TitleFont, TextBrush, TitleBounds, format);
        }

        protected virtual float EmbodiedBuildings => (float)Results.EmbodiedEmissionsBuildings(Normalized);
        protected virtual float EmbodiedSystems => (float)Results.EmbodiedEmissionsSystems(Normalized);
        protected virtual float OperationBuildings => (float)Results.OperationEmissionsBuildings(Normalized);
        protected virtual float OperationSystems => (float)Results.OperationEmissionsSystems(Normalized);
        protected virtual float TotalEmbodied => (float) Results.TotalEmbodiedEmissions(Normalized);
        protected virtual float TotalOperation => (float) Results.TotalOperationEmissions(Normalized);
        protected virtual float Total => (float) Results.TotalEmissions(Normalized);

        protected bool Normalized { get; }

        private void RenderColumnTitles(Graphics graphics)
        {
            // we need to do some calculations, since we're mixing bold and standard fonts and have to do alignment ourselves...
            string ColumnText(double absoluteValue, string unit, double relativeValue) => $" = {absoluteValue:0} {unit} ({relativeValue:0}%)";

            graphics.DrawStringTwoFonts("Embodied", BoldFont,
                ColumnText(TotalEmbodied, Unit, TotalEmbodied / Total * 100), NormalFont, TextBrush,
                EmbodiedTitleBounds);

            graphics.DrawStringTwoFonts("Operation", BoldFont,
                ColumnText(TotalOperation, Unit, TotalOperation / Total * 100), NormalFont,
                TextBrush, OperationTitleBounds);
        }

        private void RenderLeftAxis(Graphics graphics)
        {
            graphics.DrawStringVertical("Buildings", BoldFont, TextBrush, BuildingsLeftAxisBounds);
            graphics.DrawStringVertical("Systems", BoldFont, TextBrush, SystemsLeftAxisBounds);
        }

        private void RenderRightAxis(Graphics graphics)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            graphics.DrawString($"{AxisMax:0}", BoldFont, TextBrush, BuildingsRightAxisBounds.CloneWithOffset(0, 2), format);

            format.LineAlignment = StringAlignment.Far;
            graphics.DrawString($"{AxisMax:0}", BoldFont, TextBrush, SystemsRightAxisBounds.CloneWithOffset(0, 2), format);
        }

        private void RenderGrid(Graphics graphics)
        {
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

        public void NewData(ResultsPlotting results)
        {
            Results = results;
        }
    }
}
