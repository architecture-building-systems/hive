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

        public VisualizerToolTip(string title, string description, IVisualizerControl element, Brush backgroundBrush, int descriptionLineLength)
        {
            this.title = title;
            this.description = FormatDescriptionText(description, descriptionLineLength);
            this.associatedElement = element;
            this.backgroundBrush = backgroundBrush;
        }

        private int widthPadding = 5;
        private int heightPadding = 10;
        private int standardHeight = 100;
        private int standardWidth = 200;
        public void Render(Graphics graphics) 
        {
            var titleSize = GH_FontServer.MeasureString(title, _boldFont);
            var titleHeight = titleSize.Height;
            var titleWidth = titleSize.Width;

            var descSize = GH_FontServer.MeasureString(description, _standardFont);
            var descHeight = descSize.Height;
            var descWidth = descSize.Width;

            var width = Math.Max(descWidth + 2*widthPadding, standardWidth);
            var height = Math.Max(2*heightPadding + titleHeight + descHeight, standardHeight);

            var brush = new SolidBrush(Color.Black);

            // draw box
            var size = new SizeF(width, height);
            var box = new RectangleF(cursorLocation, size);
            box.Offset(0, -height);
            graphics.DrawRectangle(borderPen, box.Left, box.Top, box.Width, box.Height);
            graphics.FillRectangle(backgroundBrush, box);

            //draw Title
            var dataX = box.Left + widthPadding;
            var dataY = box.Top + heightPadding;
            graphics.DrawString(title, _boldFont, brush, dataX, dataY);

            //draw description
            var unitX = box.Left + widthPadding;
            var unitY = dataY + titleHeight;
            graphics.DrawString(description, _standardFont, brush, unitX, unitY);

        }

        private string FormatDescriptionText(string str, int maxLength)
        {
            List<string> subStrings = new List<string>();
            for (int i = 0; i < str.Length; i += maxLength)
            {
                if ((i + maxLength) < str.Length)
                    subStrings.Add(str.Substring(i, maxLength).Trim());
                else
                    subStrings.Add(str.Substring(i).Trim());
            }

            var descriptionFormatted = string.Join("\n", subStrings);
            return descriptionFormatted;
        }
    }
}
