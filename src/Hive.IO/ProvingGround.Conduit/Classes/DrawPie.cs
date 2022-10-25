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
    public class DrawPie : iDrawObject
    {
        // local properties
        Rectangle3d _thisBoundary;
        Point3d _center;
        string[] _theseLabels;
        double[] _theseValues;
        List<Interval> _thesePieces = new List<Interval>();
        List<System.Drawing.Color> _theseColors;

        clsPieStyle _pieStyle; // style index = 0
        clsPaletteStyle _colorPalette; // style index = 1

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
        /// Instance of the draw pie object
        /// </summary>
        /// <param name="boundary"></param>
        /// <param name="labels"></param>
        /// <param name="values"></param>
        /// <param name="colors"></param>
        /// <param name="sort"></param>
        public DrawPie(Rectangle3d boundary, List<string> labels, List<double> values, acStyle[] setStyles)
        {

            drawInForeground = false;

            _thisBoundary = boundary;
            _center = boundary.Center;

            var m_tempLabels = new List<string>();
            var m_tempValues = new List<double>();

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i]>0)
                {
                    m_tempValues.Add(values[i]);
                    if (labels.Count > i)
                    { m_tempLabels.Add(labels[i]); }
                    else
                    {m_tempLabels.Add("");}
                }
            }

            _theseLabels  = m_tempLabels.ToArray();
            _theseValues = m_tempValues.ToArray();

            double m_sum = _theseValues.Sum();
            List<double> m_percent = _theseValues.Select(v => v / m_sum).ToList();

            double m_runningSum = 0;

            for (int i = 0; i < m_percent.Count; i++)
            { 
                _thesePieces.Add(new Interval(m_runningSum * 2 * Math.PI, (m_runningSum + m_percent[i]) * 2 * Math.PI));
                m_runningSum += m_percent[i];
            }

            objectX = new Interval(boundary.X.T0, boundary.X.T1);
            objectY = new Interval(boundary.Y.T0, boundary.Y.T1);

            pixelDepth = 2.0;

            styles = setStyles;

            _pieStyle = (clsPieStyle)setStyles[0];
            _colorPalette = (clsPaletteStyle)setStyles[1];
           
        }

        public void updateStyles(acStyle[] Styles)
        {
            _pieStyle = (clsPieStyle)Styles[0];
            _colorPalette = (clsPaletteStyle)Styles[1];

            _theseColors = _colorPalette.colorPalette(_theseValues.Length);
        }

        /// <summary>
        /// Calls the draw pie function
        /// </summary>
        /// <param name="e">Rhino.Display.DrawEventArgs called by the Display Conduit</param>
        /// <param name="drawPlane">Plane from the lower left of the Viewport</param>
        /// <param name="unitPerPx">Model units per pixel</param>
        public void objDraw(Rhino.Display.DrawEventArgs e, Plane drawPlane, double unitPerPx, System.Drawing.Graphics fontCheck)
        {

            double m_maxRadius = Math.Min(lengthX * e.Viewport.Size.Width, lengthY * e.Viewport.Size.Height) / 2;

            double m_innerDraw = _pieStyle.innerRadius * m_maxRadius * unitPerPx;
            double m_outerDraw = _pieStyle.outerRadius * m_maxRadius * unitPerPx;
            double m_labelDraw = _pieStyle.labelRadius * m_maxRadius * unitPerPx;
            double m_labelHeight = m_maxRadius * 2 * _pieStyle.labelRelativeHeight * unitPerPx;

            //double m_labelHeight = _pieStyle.

            Point3d m_center = clsUtility.PointOnViewport(_center.X, _center.Y,
                -unitPerPx * pixelDepth, drawPlane, unitPerPx, this, e.Viewport.Size.Width, e.Viewport.Size.Height);

            Circle m_innerCircle = new Circle(drawPlane, m_center, m_innerDraw);
            Circle m_labelCircle = new Circle(drawPlane, m_center, m_labelDraw);
            Circle m_outerCircle = new Circle(drawPlane, m_center, m_outerDraw);

            for (int i = 0; i < _thesePieces.Count; i++)
            {

                Arc m_outerArc = new Arc(m_outerCircle, _thesePieces[i]);
                double m_degrees = (_thesePieces[i].Length / (2 * Math.PI)) * 360;
                int m_div = (int)Math.Max(m_degrees, 1.0);

                Point3d[] m_outerVerts;
                var m_outerCurveT = m_outerArc.ToNurbsCurve().DivideByCount(m_div, true, out m_outerVerts);

                Mesh m_thisPiece = new Mesh();

                if (m_innerDraw > 0)
                {

                    Arc m_innerArc = new Arc(m_innerCircle, _thesePieces[i]);
                    Point3d[] m_innerVerts;
                    var m_innerCurveT = m_innerArc.ToNurbsCurve().DivideByCount(m_div, true, out m_innerVerts);

                    m_thisPiece.Vertices.Add(m_innerVerts[0] + (drawPlane.ZAxis * -unitPerPx));
                    m_thisPiece.Vertices.Add(m_outerVerts[0] + (drawPlane.ZAxis * -unitPerPx));

                    int v = 1;
                    for (int j = 2; j < m_outerVerts.Length * 2; j += 2)
                    {
                        m_thisPiece.Vertices.Add(m_innerVerts[v] + (drawPlane.ZAxis * -unitPerPx));
                        m_thisPiece.Vertices.Add(m_outerVerts[v] + (drawPlane.ZAxis * -unitPerPx));
                        m_thisPiece.Faces.AddFace(j - 2, j - 1, j + 1, j);
                        v += 1;
                    }

                }
                else
                {
                    m_thisPiece.Vertices.Add(m_center);
                    m_thisPiece.Vertices.Add(m_outerVerts[0]);

                    for (int j = 1; j < m_outerVerts.Length; j++)
                    {
                        m_thisPiece.Vertices.Add(m_outerVerts[j]);
                        m_thisPiece.Faces.AddFace(0, j, j + 1);
                    }
                }

                if (_theseColors[i].A < 255)
                {
                    e.Display.DrawMeshShaded(m_thisPiece, new Rhino.Display.DisplayMaterial(_theseColors[i], (double)(255 - _theseColors[i].A) / 255.0));

                }
                else
                {
                    for (int j = 0; j < m_thisPiece.Vertices.Count; j++) m_thisPiece.VertexColors.SetColor(j, _theseColors[i]);
                    e.Display.DrawMeshFalseColors(m_thisPiece);
                }

            }

            for (int i = 0; i < _thesePieces.Count; i++)
            {
                if (_pieStyle.labelRadius > 0)
                {

                    Curve m_labelArc = new Arc(m_labelCircle, _thesePieces[i]).ToNurbsCurve();
                    m_labelArc.Domain = new Interval(0, 1);

                    Point3d m_textLocation = m_labelArc.PointAt(0.5);

                    double m_labelLength = clsUtility.MeasureText(_theseLabels[i], m_labelHeight, _pieStyle.labelFontFace, fontCheck);


                    double m_AngleAdjustX = (Math.Cos((_thesePieces[i].Mid)) - 1.0) * 0.5;
                    double m_AngleAdjustY = (Math.Sin((_thesePieces[i].Mid)) - 1.0) * 0.5;

                    Plane labelPlane = new Plane(m_labelArc.PointAt(0.5) +
                        drawPlane.XAxis * (m_AngleAdjustX * m_labelLength) +
                        drawPlane.YAxis * (m_AngleAdjustY * m_labelHeight),
                        drawPlane.XAxis, drawPlane.YAxis);
                    e.Display.Draw3dText(_theseLabels[i], _pieStyle.labelColor, labelPlane, m_labelHeight, _pieStyle.labelFontFace);
                }
            }
        }

    }
}
