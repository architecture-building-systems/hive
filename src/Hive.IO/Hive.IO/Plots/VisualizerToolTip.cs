using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
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
        private Brush textBrush;

        public IVisualizerControl associatedElement;
        public bool display;

        public VisualizerToolTip(string title, string description, Brush textBrush, IVisualizerControl element)
        {
            this.title = title;
            this.description = description;
            this.textBrush = textBrush;
            this.associatedElement = element;
        }

        public VisualizerToolTip(string title, string description, IVisualizerControl element) : this(title, description, new SolidBrush(Color.Black), element)
        {
        }

        public void Render(Graphics graphics, RectangleF bounds) 
        {
            var brush = new SolidBrush(Color.Black);

            // draw box
            var box = new RectangleF(bounds.Location, bounds.Size);
            graphics.DrawRectangle(borderPen, box.Left, box.Top, box.Width, box.Height);
            graphics.FillRectangle(backgroundBrush, box);

            var dataSize = GH_FontServer.MeasureString(description, font);
            var dataX = bounds.Left + (bounds.Width - dataSize.Width) / 2;
            var dataY = bounds.Top + bounds.Height / 2 - (float)dataSize.Height / 2;
            graphics.DrawString(description, font, brush, dataX, dataY);
        }
    }
}
