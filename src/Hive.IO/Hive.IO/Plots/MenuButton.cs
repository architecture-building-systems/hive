﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Hive.IO.Results;

namespace Hive.IO.Plots
{
    /// <summary>
    ///     MenuButtons are clickable regions that can render themselves to to a canvas.
    ///     They are managed by the PlotSelector class.
    /// </summary>
    public class MenuButton : IVisualizerControl
    {
        private readonly Brush _backgroundBrush;

        private readonly Pen _borderPen = new Pen(Color.FromArgb(217, 217, 217));
        private readonly Font _font;
        private readonly Brush _textBrush;
        private RectangleF _bounds = RectangleF.Empty;

        public MenuButton(string text) : this(text, GH_FontServer.Standard, new SolidBrush(Color.Black),
            new SolidBrush(Color.Transparent))
        {
        }

        public MenuButton(string text, Font font, Brush textBrush, Brush backgroundBrush)
        {
            Text = text;
            _font = font;
            _textBrush = textBrush;
            _backgroundBrush = backgroundBrush;
        }

        public string Text { get; }
          
        public void Render(ResultsPlotting results, Dictionary<string, string> plotParameters, Graphics graphics, RectangleF bounds)
        {
            _bounds = bounds;

            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.FillRectangle(_backgroundBrush, bounds);
            graphics.DrawString(Text, _font, _textBrush, bounds, format);
            graphics.DrawRectangleF(_borderPen, bounds);
        }

        public bool Contains(PointF location)
        {
            return _bounds.Contains(location);
        }

        public void Clicked(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            OnClicked?.Invoke(this, e);
        }

        public event EventHandler OnClicked;
    }

    public class CategoryMenuButton : MenuButton
    {
        public CategoryMenuButton(string text) : base(text, GH_FontServer.StandardBold, new SolidBrush(Color.Black),
            new SolidBrush(Color.FromArgb(217, 217, 217)))
        {
        }
    }

    public class BlackMenuButton : MenuButton
    {
        public BlackMenuButton(string text) : base(text, GH_FontServer.Standard, new SolidBrush(Color.White),
            new SolidBrush(Color.Black))
        {
        }
    }
}