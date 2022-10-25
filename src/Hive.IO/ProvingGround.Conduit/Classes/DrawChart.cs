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
    public class DrawChart : iDrawObject
    {
        // local properties
        Rectangle3d _thisBoundary;

        Rectangle3d _chartSpace;
        Rectangle3d _categorySpace;
        Rectangle3d _valueSpace;

        List<List<double>> _values;
        Interval _valueRange;

        List<Line> _lines;

        //string[] _categoryLabels;
        //double _categoryRotation;
        //double _maxCategoryString;

        //string _categoryAxis;

        //string _valueAxis;

        //List<System.Drawing.Color> _theseColors;

        List<double> _xSplit;
        List<double> _ySplit;

        clsChartStyle _chartStyle; // style index = 0
        clsPaletteStyle _colorPalette; // style index = 1

        Mesh _chartMesh;
        Point3d[] _chartVerts;

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


        //public DrawChart(Rectangle3d boundary, List<string> categoryLabels, double categoryRotation, string categoryAxis, string valueAxis, List<string> seriesLabels, List<List<double>> values, Interval valueRange,  acStyle[] setStyles)
        public DrawChart(Rectangle3d boundary, List<List<double>> values, acStyle[] setStyles)
    {

            drawInForeground = false;

            _thisBoundary = boundary;
            _values = values;

            //_categoryLabels = categoryLabels.ToArray();

            styles = setStyles;

            _chartStyle = (clsChartStyle)setStyles[0];
            _colorPalette = (clsPaletteStyle)setStyles[1];

            var m_absBounds = new Box(_thisBoundary.BoundingBox);

            objectX = m_absBounds.X; // new Interval(_thisBoundary.Corner(0).X, _thisBoundary.Corner(1).X);
            objectY = m_absBounds.Y; // new Interval(_thisBoundary.Corner(0).Y, _thisBoundary.Corner(3).Y);

        }

        public void updateStyles(acStyle[] Styles)
        {
            
            _chartStyle = (clsChartStyle)Styles[0];
            _colorPalette = (clsPaletteStyle)Styles[1];

            _xSplit = new List<double>() { _chartStyle.XSplit, 1 - _chartStyle.XSplit };
            _ySplit = new List<double>() { _chartStyle.YSplit, 1 - _chartStyle.YSplit };

            List<Rectangle3d> m_quads = clsTiler.IrregularTiles(_thisBoundary, _xSplit, _ySplit, _chartStyle.ExteriorPadding, _chartStyle.InteriorPadding, 0);

            _categorySpace = m_quads[1];
            _valueSpace = m_quads[2];
            _chartSpace = m_quads[3];

            _valueRange = new Interval(
                _values.Min(v => v.Min()),
                _values.Max(v => v.Max()));

            List<Rectangle3d> m_valueGroups = clsTiler.GridTiles(_chartSpace, _chartStyle.ChartPadding, 0, _values.Count, 1, 0);

            _chartMesh = new Mesh();

            _lines = new List<Line>();

            // standard side-by side column
            for (int cg = 0; cg < _values.Count; cg++) // cg = CategoryGroup
            {
                var m_colors = _colorPalette.colorPalette(_values[cg].Count);
                List<Rectangle3d> m_valueContainers = clsTiler.GridTiles(m_valueGroups[cg], 0, 0, _values[cg].Count, 1, 0);
                for (int v = 0; v < _values[cg].Count; v++) //  v = value
                {

                    //for (int k = 0; k < 4; k++)
                    //{
                    //    _lines.Add(new Line(m_valueContainers[j].Corner(k), m_valueContainers[j].Corner((k + 1) % 4)));
                    //}

                    double m_normalValue = _valueRange.NormalizedParameterAt(_values[cg][v]);

                    List<Rectangle3d> m_valueContainer = clsTiler.VerticalTiles(
                        m_valueContainers[v],
                        new List<double>() { m_normalValue, 1.0 - m_normalValue },
                        1.0, 0, 0);

                    //_lines.Add(new Line(m_valueContainer[0].Corner(0), m_valueContainer[0].Corner(1)));

                    int vs = _chartMesh.Vertices.Count;

                    for (int c = 0; c < 4; c++) // c = corner index
                    {
                        _chartMesh.Vertices.Add(m_valueContainer[0].Corner(c));
                        _chartMesh.VertexColors.Add(m_colors[v]);
                    }

                    _chartMesh.Faces.AddFace(vs, vs + 1, vs + 2, vs + 3);

                }
            }

            

            _chartVerts = _chartMesh.Vertices.ToPoint3dArray();
           
        }

        /// <summary>
        /// Calls the draw pie function
        /// </summary>
        /// <param name="e">Rhino.Display.DrawEventArgs called by the Display Conduit</param>
        /// <param name="drawPlane">Plane from the lower left of the Viewport</param>
        /// <param name="unitPerPx">Model units per pixel</param>
        public void objDraw(Rhino.Display.DrawEventArgs e, Plane drawPlane, double unitPerPx, System.Drawing.Graphics fontCheck)
        {

            Point3d m_from = clsUtility.PointOnViewport(_chartSpace.Corner(0).X, _chartSpace.Corner(0).Y,
                    -unitPerPx * 1.0, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);
            Point3d m_to = clsUtility.PointOnViewport(_chartSpace.Corner(1).X, _chartSpace.Corner(1).Y,
                -unitPerPx * 1.0, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

            e.Display.DrawLine(m_from, m_to, _chartStyle.CatAxisBasis.Color, _chartStyle.CatAxisBasis.Thickness);

            //foreach (Line line in _lines)
            //{
            //    Point3d m_fromL = clsUtility.PointOnViewport(line.From.X, line.From.Y,
            //        -unitPerPx * 1.0, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);
            //    Point3d m_toL = clsUtility.PointOnViewport(line.To.X, line.To.Y,
            //        -unitPerPx * 1.0, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

            //    e.Display.DrawLine(m_from, m_to, _chartStyle.CatAxisBasis.Color, _chartStyle.CatAxisBasis.Thickness);
            //}

            Mesh m_drawMesh = new Mesh();
            m_drawMesh.Append(_chartMesh);

            for (int i = 0; i < _chartVerts.Length; i++)
            {
                m_drawMesh.Vertices.SetVertex(i, clsUtility.PointOnViewportDepth(_chartVerts[i].X, _chartVerts[i].Y, -_chartVerts[i].Z,
                    -unitPerPx * 2.0, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height));
            }

            e.Display.DrawMeshFalseColors(m_drawMesh);

        }

    }
}
