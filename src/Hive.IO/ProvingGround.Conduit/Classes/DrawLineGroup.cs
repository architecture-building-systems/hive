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
    public class DrawLineGroup : iDrawObject
    {
        // local properties
        List<List<Line>> _theseLines;
        List<int> _weights;
        List<System.Drawing.Color> _colors;

        clsPaletteStyle paletteStyle;

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
        /// Instance of each draw mesh
        /// </summary>
        /// <param name="_thisMesh">Mesh to draw (should have color assigned to it)</param>
        public DrawLineGroup(List<List<Line>> lines, List<int> weights, acStyle[] setStyles)
        {

            drawInForeground = true;
            pixelDepth = 1;

            _theseLines = lines;
            _weights = weights;

            Interval setX = new Interval(_theseLines[0][0].PointAt(0).X, _theseLines[0][0].PointAt(0).X);
            Interval setY = new Interval(_theseLines[0][0].PointAt(0).Y, _theseLines[0][0].PointAt(0).Y);

            for (int i = 0; i < _theseLines.Count; i++)
            {
                for (int j = 0; j < _theseLines[i].Count; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Point3d m_testPoint = _theseLines[i][j].PointAt((double)k);

                        if (m_testPoint.X < setX.Min) setX.T0 = m_testPoint.X;
                        if (m_testPoint.X > setX.Max) setX.T1 = m_testPoint.X;
                        if (m_testPoint.Y < setY.Min) setY.T0 = m_testPoint.Y;
                        if (m_testPoint.Y > setY.Max) setY.T1 = m_testPoint.Y;
                    }
                }
            }
            objectX = setX;
            objectY = setY;



            // set the default style if none is supplied
            styles = setStyles;
            paletteStyle = (clsPaletteStyle)setStyles[0];

        }

        public void updateStyles(acStyle[] Styles)
        {
            paletteStyle = (clsPaletteStyle)Styles[0];
            _colors = paletteStyle.colorPalette(_theseLines.Count);
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
                for (int j = 0; j < _theseLines[i].Count; j++)
                {
                    Point3d m_from = clsUtility.PointOnViewport(_theseLines[i][j].From.X, _theseLines[i][j].From.Y,
                    -unitPerPx * pixelDepth, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

                    Point3d m_to = clsUtility.PointOnViewport(_theseLines[i][j].To.X, _theseLines[i][j].To.Y,
                        -unitPerPx * pixelDepth, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

                    e.Display.DrawLine(m_from, m_to, _colors[i], _weights[i]);
                }
                
            }
        }

    }
}

