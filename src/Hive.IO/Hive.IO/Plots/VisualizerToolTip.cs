using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Hive.IO.Results;


namespace Hive.IO.Plots
{
    public class VisualizerToolTip
    {
        private string title;
        private string description;
        private Brush backgroundBrush = new SolidBrush(Color.Red);
        private Pen borderPen = new Pen(Color.FromArgb(217, 217, 217));
        private Font _standardFont = GH_FontServer.Standard;
        private Font _boldFont = GH_FontServer.StandardBold;

        public IVisualizerControl associatedElement;
        public bool display;
        public PointF cursorLocation;

        public VisualizerToolTip(string title, string description, IVisualizerControl element, Brush backgroundBrush)
        {
            this.title = title;
            this.description = description;
            this.associatedElement = element;
            this.backgroundBrush = backgroundBrush;
        }

        public VisualizerToolTip(string title, string description, IVisualizerControl element) : this(title, description, element, new SolidBrush(Color.Red))
        {
        }

        private int widthPadding = 5;
        private int heightPadding = 10;
        private int standardHeight = 100;
        private int standardWidth = 200;
        public void Render(Graphics graphics) 
        {
            var measureString = GH_FontServer.MeasureString(description, _standardFont);
            var stringHeight = measureString.Height;
            var stringWidth = measureString.Width;


            var width = Math.Max(stringWidth + 2*widthPadding, 200);
            var height = stringHeight;

            var brush = new SolidBrush(Color.Black);

            // draw box
            var size = new SizeF(width, standardHeight);
            var box = new RectangleF(cursorLocation, size);
            box.Offset(0, -standardHeight);
            graphics.DrawRectangle(borderPen, box.Left, box.Top, box.Width, box.Height);
            graphics.FillRectangle(backgroundBrush, box);

            //draw Title
            var dataX = box.Left + widthPadding;
            var dataY = box.Top + heightPadding;
            graphics.DrawString(title, _boldFont, brush, dataX, dataY);

            //draw description
            var dataSize = GH_FontServer.MeasureString(description, _standardFont);
            var unitX = box.Left + widthPadding;
            var unitY = dataY + dataSize.Height;
            graphics.DrawString(description, _standardFont, brush, unitX, unitY);

        }
    }
}
