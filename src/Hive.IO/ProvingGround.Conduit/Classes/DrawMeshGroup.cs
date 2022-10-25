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
    public class DrawMeshGroup : iDrawObject
    {
        // local properties
        List<Mesh> _theseMeshes;
        Dictionary<int, Point3d[]> _theseVerts;
        clsPaletteStyle _thisPalette;
        bool _stack;
       
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
        public DrawMeshGroup(List<Mesh> theseMeshes, double thisPixelDepth, clsPaletteStyle thisPalette, bool stack)
        {

            drawInForeground = false;

            _theseMeshes = theseMeshes;
            _theseVerts = new Dictionary<int, Point3d[]>();

            Point3d m_startVert = _theseMeshes[0].Vertices.ToPoint3dArray()[0];
            Interval setX = new Interval(m_startVert.X, m_startVert.X);
            Interval setY = new Interval(m_startVert.Y, m_startVert.Y);

            for (int i = 0; i < _theseMeshes.Count; i++)
            {

                _theseVerts.Add(i, _theseMeshes[i].Vertices.ToPoint3dArray());

                for (int j = 0; j < _theseVerts[i].Length; j++)
                {

                    if (_theseVerts[i][j].X < setX.Min) setX.T0 = _theseVerts[i][j].X;
                    if (_theseVerts[i][j].X > setX.Max) setX.T1 = _theseVerts[i][j].X;
                    if (_theseVerts[i][j].Y < setY.Min) setY.T0 = _theseVerts[i][j].Y;
                    if (_theseVerts[i][j].Y > setY.Max) setY.T1 = _theseVerts[i][j].Y;

                    _theseMeshes[i].VertexColors.Add(System.Drawing.Color.FromArgb(125, 125, 125));

                }
            }

            objectX = setX;
            objectY = setY;

            pixelDepth = thisPixelDepth;

            styles = new acStyle[] { (acStyle)thisPalette };
            _thisPalette = thisPalette;

            _stack = stack;
            
        }

        public void updateStyles(acStyle[] Styles)
        {

            _thisPalette = (clsPaletteStyle)Styles[0];

            List<System.Drawing.Color> m_theseColors = _thisPalette.colorPalette(_theseMeshes.Count);

            for (int i = 0; i < _theseMeshes.Count; i++)
            {

                for (int j = 0; j < _theseMeshes[i].Vertices.Count; j++)
                {
                    _theseMeshes[i].VertexColors.SetColor(j, m_theseColors[i]);
                }

            }
        
        }

        /// <summary>
        /// Calls the draw mesh function
        /// </summary>
        /// <param name="e">Rhino.Display.DrawEventArgs called by the Display Conduit</param>
        /// <param name="drawPlane">Plane from the lower left of the Viewport</param>
        /// <param name="unitPerPx">Model units per pixel</param>
        public void objDraw(Rhino.Display.DrawEventArgs e, Plane drawPlane, double unitPerPx, System.Drawing.Graphics fontCheck)
        {
            Mesh m_drawMesh = new Mesh();

            for (int i = 0; i < _theseMeshes.Count; i++)
            {
                
                for (int j = 0; j < _theseVerts[i].Length; j++)
                {
                    _theseMeshes[i].Vertices.SetVertex(j, clsUtility.PointOnViewportDepth(_theseVerts[i][j].X, _theseVerts[i][j].Y, _theseVerts[i][j].Z,
                        -unitPerPx * (_stack ? (1 + (i*0.1)) * pixelDepth : pixelDepth), drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height));
                }

                m_drawMesh.Append(_theseMeshes[i]);
            }

            e.Display.DrawMeshFalseColors(m_drawMesh);

        }

    }
}

