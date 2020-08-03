using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types.Transforms;

namespace Hive.IO.Plots
{
    public static class DrawingExtensions
    {
        public static RectangleF Clone(this RectangleF self)
        {
            return new RectangleF(self.Location, self.Size);
        }

        /// <summary>
        /// Create a copy of self and move it by x and y.
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

        public static RectangleF CloneInflate(this RectangleF self, float x, float y)
        {
            var result = self.Clone();
            result.Inflate(x, y);
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


        /// <summary>
        /// DrawString, except text is drawn from bottom to top, centered vertically and horizontally.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="bounds"></param>
        public static void DrawStringVertical(this Graphics graphics, string text, Font font, Brush brush,
            RectangleF bounds)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            var rotatedBounds = new RectangleF(bounds.Left, bounds.Bottom, bounds.Height, bounds.Width);
            var translatedBounds = rotatedBounds.CloneWithOffset(-rotatedBounds.X, -rotatedBounds.Y);

            graphics.TranslateTransform(bounds.X, bounds.Y + bounds.Height);
            graphics.RotateTransform(-90);
            graphics.DrawString(text, font, brush, translatedBounds, format);
            graphics.RotateTransform(+90);
            graphics.TranslateTransform(-bounds.X, -bounds.Y - bounds.Height);
        }

        /// <summary>
        /// Draw two strings inside the same bounds, adjacent, with two separate fonts, e.g. bold and standard.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="textA"></param>
        /// <param name="fontA"></param>
        /// <param name="textB"></param>
        /// <param name="fontB"></param>
        /// <param name="brush"></param>
        /// <param name="bounds"></param>
        public static void DrawStringTwoFonts(this Graphics graphics, string textA, Font fontA, string textB,
            Font fontB, Brush brush, RectangleF bounds)
        {
            var format = StringFormat.GenericTypographic;
            format.Alignment = StringAlignment.Near;
            format.Trimming = StringTrimming.None;

            var boldTextSize = GH_FontServer.MeasureString(textA, fontA);
            var standardTextSize = GH_FontServer.MeasureString(textB, fontB);

            var leftPaddingWidth = (bounds.Width - boldTextSize.Width - standardTextSize.Width) / 2;
            var leftPaddingBounds = bounds.Clone();
            leftPaddingBounds.Width = leftPaddingWidth;
            var boldTextBounds = leftPaddingBounds.CloneRight(boldTextSize.Width);
            graphics.DrawString(textA, fontA, brush, boldTextBounds, format);

            var standardTextBounds = boldTextBounds.CloneRight(standardTextSize.Width);
            graphics.DrawString(textB, fontB, brush, standardTextBounds, format);
        }

        public static float Scale(this float value, float maxValue, float newMaxValue)
        {
            if (value >= maxValue)
            {
                return newMaxValue;
            }

            return value / maxValue * newMaxValue;
        }

        public static float[] ToFloatArray(this IEnumerable<double> self)
        {
            return self.Select(d => (float)d).ToArray();
        }
    }
}