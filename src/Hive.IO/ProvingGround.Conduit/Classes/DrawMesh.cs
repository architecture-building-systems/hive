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
    public class DrawMesh : iDrawObject
    {
        // local properties
        Mesh _thisMesh;
        Point3d[] _theseVerts;

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
        /// <param name="thisMesh">Mesh to draw (should have color assigned to it)</param>
        public DrawMesh(Mesh thisMesh, double thisPixelDepth)
        {
            drawInForeground = false;

            _thisMesh = thisMesh;

            _theseVerts = _thisMesh.Vertices.ToPoint3dArray();
            Interval setX = new Interval(_theseVerts[0].X, _theseVerts[0].X);
            Interval setY = new Interval(_theseVerts[0].Y, _theseVerts[0].Y);

            bool m_colorless = (thisMesh.VertexColors.Count == 0);

            for (int i = 1; i < _theseVerts.Length; i++)
            {

                if (_theseVerts[i].X < setX.Min) setX.T0 = _theseVerts[i].X;
                if (_theseVerts[i].X > setX.Max) setX.T1 = _theseVerts[i].X;
                if (_theseVerts[i].Y < setY.Min) setY.T0 = _theseVerts[i].Y;
                if (_theseVerts[i].Y > setY.Max) setY.T1 = _theseVerts[i].Y;

                if (m_colorless) _thisMesh.VertexColors.Add(System.Drawing.Color.FromArgb(125, 125, 125));

            }

            objectX = setX;
            objectY = setY;

            pixelDepth = thisPixelDepth;

            styles = new acStyle[] { };

        }

        public void updateStyles(acStyle[] Styles)
        { }

        /// <summary>
        /// Calls the draw mesh function
        /// </summary>
        /// <param name="e">Rhino.Display.DrawEventArgs called by the Display Conduit</param>
        /// <param name="drawPlane">Plane from the lower left of the Viewport</param>
        /// <param name="unitPerPx">Model units per pixel</param>
        public void objDraw(Rhino.Display.DrawEventArgs e, Plane drawPlane, double unitPerPx, System.Drawing.Graphics fontCheck)
        {
            Mesh m_drawMesh = new Mesh();
            m_drawMesh.Append(_thisMesh);

            for (int i = 0; i < _theseVerts.Length; i++)
            {
                m_drawMesh.Vertices.SetVertex(i, clsUtility.PointOnViewportDepth(_theseVerts[i].X, _theseVerts[i].Y, -_theseVerts[i].Z,
                    -unitPerPx * pixelDepth, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height));
            }
            e.Display.DrawMeshFalseColors(m_drawMesh);
        }

    }
}
