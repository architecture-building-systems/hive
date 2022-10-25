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
    /// Draw a line on the HUD
    /// </summary>
    public class DrawLines : iDrawObject
    {
        // local properties
        List<Line> _theseLines;
        List<int> _weights;
        List<System.Drawing.Color> _colors;

        clsCurveStyle curveStyle;

        // interface required properties
        public Interval objectX { get; set; }
        public Interval objectY { get; set; }

        public double pixelDepth { get; set; }

        public double baseX { get; set; }
        public double lengthX { get; set; }
        public double baseY { get; set; }
        public double lengthY { get; set; }

        public acStyle[] styles { get; set; }

        public bool drawInForeground { get; set; }

        /// <summary>
        /// Instance of each draw lines
        /// </summary>
        /// <param name="_thisMesh">Mesh to draw (should have color assigned to it)</param>
        public DrawLines(List<Line> lines, List<int> weights, List<System.Drawing.Color> colors, acStyle[] setStyles, double PixelDepth)
        {

            drawInForeground = false;
            pixelDepth = PixelDepth;

            _theseLines = lines;
            _weights = weights;
            _colors = colors;

            Interval setX = new Interval(_theseLines[0].PointAt(0).X, _theseLines[0].PointAt(0).X);
            Interval setY = new Interval(_theseLines[0].PointAt(0).Y, _theseLines[0].PointAt(0).Y);

            for (int i = 0; i < _theseLines.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Point3d m_testPoint = _theseLines[i].PointAt((double)j);

                    if (m_testPoint.X < setX.Min) setX.T0 = m_testPoint.X;
                    if (m_testPoint.X > setX.Max) setX.T1 = m_testPoint.X;
                    if (m_testPoint.Y < setY.Min) setY.T0 = m_testPoint.Y;
                    if (m_testPoint.Y > setY.Max) setY.T1 = m_testPoint.Y;
                }
            }
            objectX = setX;
            objectY = setY;

             // set the default style if none is supplied
            styles = setStyles;
            curveStyle = (clsCurveStyle)setStyles[0];

        }

        public void updateStyles(acStyle[] Styles)
        {

            curveStyle = (clsCurveStyle)Styles[0];

            // update the color list and weights with overrides or font
            var m_setWeights = new List<int>();
            var m_setColors = new List<System.Drawing.Color>();

            for (int i = 0; i < _theseLines.Count; i++)
            {
                int m_lineWeight =
                    _weights.Count == 0
                    ? curveStyle.curveBasis.Thickness
                    : _weights.Count > i
                    ? _weights[i]
                    : _weights.Last();

                System.Drawing.Color m_lineColor =
                   _colors.Count == 0
                   ? curveStyle.curveBasis.Color
                   :  _colors.Count > i
                   ? _colors[i]
                   : _colors.Last();

                m_setWeights.Add(m_lineWeight);
                m_setColors.Add(m_lineColor);

            }

            _weights.Clear();
            _weights= m_setWeights;
            _colors.Clear();
            _colors = m_setColors;
        
        }

        /// <summary>
        /// Calls the draw line function
        /// </summary>
        /// <param name="e">Rhino.Display.DrawEventArgs called by the Display Conduit</param>
        /// <param name="drawPlane">Plane from the lower left of the Viewport</param>
        /// <param name="unitPerPx">Model units per pixel</param>
        public void objDraw(Rhino.Display.DrawEventArgs e, Plane drawPlane, double unitPerPx, System.Drawing.Graphics fontCheck)
        {
            for (int i = 0; i < _theseLines.Count; i++)
            {
                Point3d m_from = clsUtility.PointOnViewport(_theseLines[i].From.X, _theseLines[i].From.Y,
                    -unitPerPx * pixelDepth, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

                Point3d m_to = clsUtility.PointOnViewport(_theseLines[i].To.X, _theseLines[i].To.Y,
                    -unitPerPx * pixelDepth, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

                e.Display.DrawLine(m_from, m_to, _colors[i], _weights[i]);
            }
        }

    }
}
