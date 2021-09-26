using System.Collections.Generic;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    /// <summary>
    ///     Base plot for AMR-plots - draw the grid and fill in dummy text. Define the sub-areas for subclasses
    ///     to draw in.
    ///     To be derived.
    ///     
    ///     Looks like this:
    ///     
    ///     |________HeaderLeft________|___________HeaderCenter_____|___________HeaderRight______|
    ///     | LeftTop       |__EmbodiedTitle__      |__OperationTitle___     | RightTop          |
    ///     |_______________|___EmbodiedLegend______|__OperationLegend__     |___________________|
    ///     |               |                       |                        |                   |
    ///     | BuildingsLeft | EmbodiedBuildingsPlot | OperationBuildingsPlot | BuildingRightAxis |
    ///     |_______________|_______________________|________________________|___________________|
    ///     |               |                       |                        |                   |
    ///     | SystemsLeft   | EmbodiedSystemsPlot   | OperationSystemsPlot   | SystemsRightAxis  |
    ///     |_______________|_______________________|________________________|___________________|
    /// 
    /// </summary>
    public class AmrPlotBase : IVisualizerPlot
    {
        private readonly float textBoxPadding = 50;

        private readonly float Spacer = 13; // half of MenuButtonPanel spacer =25.

        public AmrPlotBase(string title, string description, AmrPlotDataAdaptor data, AmrPlotStyle style)
        {
            Title = title;
            Description = description;
            Data = data;
            Style = style;
        }

        protected AmrPlotDataAdaptor Data { get; }
        protected AmrPlotStyle Style { get; }


        protected RectangleF Bounds { get; private set; }
        protected string Title { get; }
        protected string Description { get; }

        protected Font TitleFont => GH_FontServer.Large;
        protected Font BoldFont => GH_FontServer.StandardBold;
        protected Font NormalFont => GH_FontServer.Standard;

        protected Brush TextBrush => new SolidBrush(Color.Black);

        // calculate row heights and column widths of the grid
        private float RightAxisWidth => GH_FontServer.MeasureString("10000", BoldFont).Width + textBoxPadding;
        private float LeftAxisWidth => RightAxisWidth;
        private float EmbodiedColumnWidth => (Bounds.Width - RightAxisWidth - LeftAxisWidth) / 2;
        private float OperationColumnWidth => EmbodiedColumnWidth;

        private float HeaderCenterWidth => Bounds.Width / 3;
        private float HeaderLeftWidth => HeaderCenterWidth;
        private float HeaderRightWidth => HeaderCenterWidth;


        private float HeaderHeight => GH_FontServer.MeasureString("Title", TitleFont).Height + textBoxPadding;
        private float ColumnTitleHeight => GH_FontServer.MeasureString("Embodied / Operation", BoldFont).Height;
        private float ColumnLegendHeight => GH_FontServer.MeasureString("legend", NormalFont).Height;

        private float BuildingsRowHeight =>
            (Bounds.Height - HeaderHeight - ColumnTitleHeight - ColumnLegendHeight * 2) / 2;

        private float SystemsRowHeight => BuildingsRowHeight;

        // title rows
        protected RectangleF HeaderLeftBounds => new RectangleF(Bounds.X, Bounds.Y, HeaderLeftWidth, HeaderHeight);
        protected RectangleF HeaderCenterBounds => HeaderLeftBounds.CloneRight(HeaderCenterWidth);
        protected RectangleF HeaderRightBounds => HeaderLeftBounds.CloneRight(HeaderRightWidth);

        protected RectangleF LeftTopBounds => new RectangleF(HeaderLeftBounds.X, HeaderLeftBounds.Bottom, LeftAxisWidth,
            ColumnTitleHeight + ColumnLegendHeight);


        protected RectangleF EmbodiedTitleBounds => new RectangleF(LeftTopBounds.Right, LeftTopBounds.Top,
            EmbodiedColumnWidth, ColumnTitleHeight);

        protected RectangleF OperationTitleBounds => EmbodiedTitleBounds.CloneRight(OperationColumnWidth);

        protected RectangleF EmbodiedBuildingsLegendBounds => new RectangleF(EmbodiedTitleBounds.X,
            EmbodiedTitleBounds.Bottom,
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

        protected virtual float AxisMax => Data.EmbodiedBuildings + Data.EmbodiedSystems + Data.OperationBuildings +
                                           Data.OperationSystems;

        protected virtual float TotalBuildings => Data.TotalBuildings;
        protected virtual float TotalSystems => Data.TotalSystems;
        protected virtual float TotalEmbodied => Data.TotalEmbodied;
        protected virtual float TotalOperation => Data.TotalOperation;


        public void Render(ResultsPlotting results, Dictionary<string, string> plotProperties, Graphics graphics,
            RectangleF bounds)
        {
            Bounds = bounds;
            RenderGrid(graphics);
            RenderTitle(graphics);
            RenderColumnTitles(graphics);
            RenderLeftAxis(graphics);
            RenderRightAxis(graphics);

            RenderPlot(graphics);
        }

        public bool Contains(PointF location)
        {
            return Bounds.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
        }

        public void NewData(ResultsPlotting results)
        {
            Data.NewData(results);
        }

        protected virtual void RenderPlot(Graphics graphics)
        {
        }

        private void RenderTitle(Graphics graphics)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            graphics.DrawString(Title, TitleFont, TextBrush, HeaderCenterBounds, format);
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
            graphics.DrawString(Description, NormalFont, TextBrush, HeaderLeftBounds.CloneInflate(-Spacer,0).CloneWithOffset(Spacer,0), format);
        }

        private void RenderColumnTitles(Graphics graphics)
        {
            // we need to do some calculations, since we're mixing bold and standard fonts and have to do alignment ourselves...
            string Caption(double absoluteValue, string unit, double relativeValue)
            {
                return $"{absoluteValue:0} {unit} ({relativeValue:0}%)";
            }

            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;

            graphics.DrawString("Embodied", BoldFont, TextBrush, EmbodiedTitleBounds, format);
            graphics.DrawString(Caption(TotalEmbodied, Data.Unit, Data.TotalEmbodied / Data.Total * 100), NormalFont, TextBrush, EmbodiedBuildingsLegendBounds, format);

            graphics.DrawString("Operation", BoldFont, TextBrush, OperationTitleBounds, format);
            graphics.DrawString(Caption(TotalOperation, Data.Unit, Data.TotalOperation / Data.Total * 100), NormalFont, TextBrush, OperationBuildingsLegendBounds, format);

            //graphics.DrawStringTwoFonts("Embodied\n", BoldFont,
            //    Caption(TotalEmbodied, Data.Unit, Data.TotalEmbodied / Data.Total * 100), NormalFont, TextBrush,
            //    EmbodiedTitleBounds);

            //graphics.DrawStringTwoFonts("Operation\n", BoldFont,
            //    Caption(TotalOperation, Data.Unit, Data.TotalOperation / Data.Total * 100), NormalFont,
            //    TextBrush, OperationTitleBounds);
        }

        private void RenderLeftAxis(Graphics graphics)
        {
            graphics.DrawStringVertical("Buildings", BoldFont, TextBrush, BuildingsLeftAxisBounds);
            graphics.DrawStringVertical("Systems", BoldFont, TextBrush, SystemsLeftAxisBounds);
        }

        private void RenderRightAxis(Graphics graphics)
        {
            string Caption(double absoluteValue, string unit, double relativeValue)
            {
                return $"{absoluteValue:0} {unit}\n({relativeValue:0}%)";
            }
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            graphics.DrawString(Caption(TotalBuildings, Data.Unit, Data.TotalBuildings / Data.Total * 100), NormalFont, TextBrush, BuildingsRightAxisBounds.CloneWithOffset(0, 2),
                format);

            format.LineAlignment = StringAlignment.Center;
            graphics.DrawString(Caption(TotalSystems, Data.Unit, Data.TotalSystems / Data.Total * 100), NormalFont, TextBrush, SystemsRightAxisBounds.CloneWithOffset(0, 2),
                format);
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

            //graphics.DrawRectangleF(borderPen, LeftBottomBounds);
            //graphics.DrawRectangleF(borderPen, EmbodiedSystemsLegendBounds);
            //graphics.DrawRectangleF(borderPen, OperationSystemsLegendBounds);
            //graphics.DrawRectangleF(borderPen, RightBottomBounds);
        }
    }
}