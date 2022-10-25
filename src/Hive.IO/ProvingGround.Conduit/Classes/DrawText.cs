using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;

using Rhino;
using Rhino.Collections;
using Rhino.Geometry;

using ProvingGround.Conduit.Utils;

namespace ProvingGround.Conduit.Classes
{
    /// <summary>
    /// Draw a mesh on the HUD
    /// </summary>
    public class DrawText : iDrawObject
    {
        // local properties
        List<string> _theseStrings;
        Rectangle3d _thisBoundary;
        int _direction; // 0 = top down, 1 = bottom up
        int _distribution; // 0 = even, 1 = absolute
        List<System.Drawing.Color> _colors;
        bool _orientVert;

        double _rotation;
        double _rotationAdjY;
        double _rotationAdjX;

        clsFontStyle fontStyle;

        // interface required properties

        public Interval objectX { get; set; }
        public Interval objectY { get; set; }

        public double pixelDepth { get; set;}

        public double baseX { get; set; }
        public double lengthX { get; set; }
        public double baseY { get; set; }
        public double lengthY { get; set; }

        public acStyle[] styles { get; set; }

        public bool drawInForeground { get; set; }

        /// <summary>
        /// Instance of the draw text object
        /// </summary>
        /// <param name="strings">List of strings to draw</param>
        /// <param name="boundary">A rectangle 3d in the drawing space to bound the text</param>
        /// <param name="direction">Order of the text (0=top to bottom, 1=bottom to top)</param>
        /// <param name="distribution">Whether the text should be evenly distributed throughout the rectangle (0=even, overrides spacing, 1=absolute, can overflow bounds)</param>
        public DrawText(List<string> strings, Rectangle3d boundary, 
            int direction, int distribution, List<System.Drawing.Color> colors, bool orientVert, acStyle[] setStyles)
        {

            // text is always written on top
            drawInForeground = true;
            pixelDepth = 1;

            _theseStrings = strings;
            _thisBoundary = boundary;
            _direction = direction;
            _distribution = distribution;
            _colors = colors;
            _orientVert = orientVert;

            var m_absBounds = new Box(_thisBoundary.BoundingBox);

            _rotation = Vector3d.VectorAngle(Vector3d.XAxis, _thisBoundary.Plane.XAxis, Plane.WorldXY);
            _rotationAdjY = _thisBoundary.Corner(3).DistanceTo(_thisBoundary.Corner(0)) / m_absBounds.Y.Length;
            _rotationAdjX = _thisBoundary.Corner(1).DistanceTo(_thisBoundary.Corner(0)) / m_absBounds.X.Length;


            objectX = m_absBounds.X; // new Interval(_thisBoundary.Corner(0).X, _thisBoundary.Corner(1).X);
            objectY = m_absBounds.Y; // new Interval(_thisBoundary.Corner(0).Y, _thisBoundary.Corner(3).Y);

            styles = setStyles;
            fontStyle = (clsFontStyle)setStyles[0];
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Styles"></param>
        public void updateStyles(acStyle[] Styles)
        {
            if (Styles.Length > 0)
            {            
                fontStyle = (clsFontStyle)Styles[0];
            }

            // update the color list with overrides or font
            var m_setColors = new List<System.Drawing.Color>();

            for (int i = 0; i < _theseStrings.Count; i++)
            {

                System.Drawing.Color m_textColor =
                    _colors.Count == 0
                    ? fontStyle.fontBasis.Color
                    : _colors.Count > i 
                    ? _colors[i]
                    : _colors.Last();

                m_setColors.Add(m_textColor);
            }

            _colors = m_setColors;

        }

        /// <summary>
        /// Calls the draw text function
        /// </summary>
        /// <param name="e">Rhino.Display.DrawEventArgs called by the Display Conduit</param>
        /// <param name="drawPlane">Plane from the lower left of the Viewport</param>
        /// <param name="unitPerPx">Model units per pixel</param>
        public void objDraw(Rhino.Display.DrawEventArgs e, Plane drawPlane, double unitPerPx, System.Drawing.Graphics fontCheck)
        {

            // make orientation adjustments
            int m_viewWidth = e.Viewport.Size.Width;
            int m_viewHeight = e.Viewport.Size.Height;

            int m_LL = 0;
            int m_LR = 1;
            int m_UR = 2;
            int m_UL = 3;

            Vector3d m_yAxis = drawPlane.YAxis;
            Vector3d m_xAxis = drawPlane.XAxis;

            m_yAxis.Rotate(_rotation, drawPlane.ZAxis);
            m_xAxis.Rotate(_rotation, drawPlane.ZAxis);

            double m_adjX = lengthX * _rotationAdjX;
            double m_adjY = lengthY * _rotationAdjY;

            // locate the anchor point
            Point3d anchorStart = _thisBoundary.Corner(m_LL);
            if (_direction == 1 && fontStyle.fontBasis.TextAlign == "Right") anchorStart = _thisBoundary.Corner(m_LR);
            else if (_direction == 0 && fontStyle.fontBasis.TextAlign == "Right") anchorStart = _thisBoundary.Corner(m_UR);
            else if (_direction == 0 && (fontStyle.fontBasis.TextAlign == "Left" || 
                fontStyle.fontBasis.TextAlign == "Center")) anchorStart = _thisBoundary.Corner(m_UL);

            List<Point3d> m_anchors = new List<Point3d>();

            Point3d m_drawAnchor = clsUtility.PointOnViewport(anchorStart.X, anchorStart.Y,
                    -unitPerPx * pixelDepth, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

            // calculate the increment values for y orient
            double m_yIncrementPx = ((m_adjY * m_viewHeight) / _theseStrings.Count);
            double m_yIncrement = m_yIncrementPx * unitPerPx;

            // set height, considering if it is adaptive
            double m_textHeight = 
                fontStyle.fontBasis.AdaptiveHeight && _distribution == 0
                ? ((m_yIncrementPx * 0.7) / Math.Max(1.0, fontStyle.fontBasis.TextSpacing))
                : fontStyle.fontBasis.TextHeight;

            // adjust the anchor point and increment based on the distribution and direction settings
            if (_distribution == 1) m_yIncrement = (fontStyle.fontBasis.TextHeight * fontStyle.fontBasis.TextSpacing) * unitPerPx;
            if (_direction == 0) m_drawAnchor -= m_yAxis * m_yIncrement;

            // set each text element
            for (int i = 0; i < _theseStrings.Count; i++)
            {

                double m_xShift = 0;
                if (fontStyle.fontBasis.TextAlign == "Right") m_xShift = -clsUtility.MeasureText(_theseStrings[i], m_textHeight, fontStyle.fontBasis.FontFace, fontCheck) * unitPerPx;
                else if (fontStyle.fontBasis.TextAlign == "Center") m_xShift = ((m_adjX * m_viewWidth * unitPerPx) - 
                    clsUtility.MeasureText(_theseStrings[i], m_textHeight, fontStyle.fontBasis.FontFace, fontCheck) * unitPerPx) * 0.5;

                Rhino.Display.Text3d m_thisText = new Rhino.Display.Text3d(_theseStrings[i], new Plane(m_drawAnchor + (m_xAxis * m_xShift) + (m_yAxis * (m_textHeight * 0.3) * unitPerPx)  , m_xAxis, m_yAxis), m_textHeight * unitPerPx)
                {
                    Bold = fontStyle.fontBasis.Bold,
                    Italic = fontStyle.fontBasis.Italic,
                    FontFace = fontStyle.fontBasis.FontFace
                };

                

                e.Display.Draw3dText(m_thisText, _colors[i]);

                m_drawAnchor += 
                    _direction == 0 
                    ? -m_yAxis * m_yIncrement 
                    : m_yAxis * m_yIncrement;
            }

        }

    }
}
