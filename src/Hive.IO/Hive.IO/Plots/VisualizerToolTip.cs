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
        private Font font = GH_FontServer.Standard;

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

        private int width = 200;
        private int height = 100;

        public void Render(Graphics graphics) 
        {
            var brush = new SolidBrush(Color.Black);

            // draw box
            var size = new SizeF(width, height);
            var box = new RectangleF(cursorLocation, size);
            box.Offset(0, -height);
            graphics.DrawRectangle(borderPen, box.Left, box.Top, box.Width, box.Height);
            graphics.FillRectangle(backgroundBrush, box);

            var dataSize = GH_FontServer.MeasureString(description, font);
            var dataX = box.Left + (box.Width - dataSize.Width) / 2;
            var dataY = box.Top + box.Height / 2 - (float)dataSize.Height / 2;
            graphics.DrawString(description, font, brush, dataX, dataY);

        }
    }
}
