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

using ProvingGround.Conduit.Classes;

namespace ProvingGround.Conduit.Utils
{
    class clsTiler
    {
        /// <summary>
        /// Return a list of rectangular tiles from a boundary condition (in the XY Plane)
        /// </summary>
        /// <param name="B">Bounds to tile</param>
        /// <param name="R">List of relative values that drive the tiling</param>
        /// <param name="W">Width of tiles relative to bounds (0-1)</param>
        /// <param name="WP">Width padding as pct of bounds</param>
        /// <param name="HP">Height padding as pct of bounds</param>
        /// <param name="PA">Padding Axis</param>
        /// <returns></returns>
        public static List<Rectangle3d> VerticalTiles(Rectangle3d B, List<double> R, double W, double WP, double HP)
        {

            var m_tileSet = new List<Rectangle3d>();
            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            var m_available = B.Height - (HP * B.Height);

            double m_total = R.Sum();
            double m_hPadding = (HP * B.Height) / (R.Count + 1);
            double m_wPadding = (WP * B.Width) / 2; ;
            double m_width = B.Width * W - (WP * B.Width)/2;

            double m_yValue = 0;

            for (int i = 0; i < R.Count; i++)
            {
                m_yValue += m_hPadding;
                double m_nextY = m_yValue + (R[i] / m_total) * m_available;
                m_tileSet.Add(new Rectangle3d(m_basePlane,
                    m_basePlane.PointAt(m_wPadding, m_yValue, 0),
                    m_basePlane.PointAt(m_width, m_nextY, 0)));
                m_yValue = m_nextY;
            }

            return m_tileSet;
        }

        /// <summary>
        /// Return a list of rectangular tiles from a boundary condition (in the XY Plane)
        /// </summary>
        /// <param name="B">Bounds to tile</param>
        /// <param name="R">List of relative values that drive the tiling</param>
        /// <param name="H">Height of tiles relative to bounds (0-1)</param>
        /// <param name="WP">Width padding as pct of bounds</param>
        /// <param name="HP">Height padding as pct of bounds</param>
        /// <param name="PA">Padding Axis</param>
        /// <returns></returns>
        public static List<Rectangle3d> HorizontalTiles(Rectangle3d B, List<double> R, double H, double WP, double HP)
        {
            var m_tileSet = new List<Rectangle3d>();
            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            var m_available = B.Width - (WP * B.Width);

            double m_total = R.Sum();
            double m_hPadding = (HP * B.Height) / 2;
            double m_wPadding = (WP * B.Width) / (R.Count + 1);
            double m_height = B.Height * H - (HP * B.Height)/2;

            double m_xValue = 0;

            for (int i = 0; i < R.Count; i++)
            {
                m_xValue += m_wPadding;
                double m_nextX = m_xValue + (R[i] / m_total) * m_available;
                m_tileSet.Add(new Rectangle3d(m_basePlane,
                    m_basePlane.PointAt(m_xValue, m_hPadding, 0),
                    m_basePlane.PointAt(m_nextX, m_height, 0)));
                m_xValue = m_nextX;
            }

            return m_tileSet;
        }

        /// <summary>
        /// Create an even grid of tiles in a rectangular boundary
        /// </summary>
        /// <param name="B">Bounds to tile</param>
        /// <param name="IP">Interior padding as percent of padding axis</param>
        /// <param name="EP">Exterior padding as percent of padding axis</param>
        /// <param name="C">Number of columns</param>
        /// <param name="R">Number of rows</param>
        /// <param name="PA">Padding Axis</param>
        /// <returns></returns>
        public static List<Rectangle3d> GridTiles(Rectangle3d B, double IP, double EP, int C, int R, int PA)
        {


            var m_tileSet = new List<Rectangle3d>();
            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            double m_padSource = (PA == 0 ? B.Width : B.Height);
            var m_iPadding = IP * m_padSource;
            var m_ePadding = EP * m_padSource;

            var m_cellWidth = (B.Width - (m_iPadding * (C - 1)) - (m_ePadding * 2)) / C;
            var m_cellHeight = (B.Height - (m_iPadding * (R - 1)) - (m_ePadding * 2)) / R;

            for (int i = 0; i < R; i++)
            {
                double m_llY = m_ePadding + m_cellHeight * i + m_iPadding * i;
                double m_urY = m_ePadding + (m_cellHeight * (i + 1)) + m_iPadding * i;
                for (int j = 0; j < C; j++)
                {
                    double m_llX = m_ePadding + m_cellWidth * j + m_iPadding * j;
                    double m_urX = m_ePadding + (m_cellWidth * (j + 1)) + m_iPadding * j;
                    m_tileSet.Add(new Rectangle3d(m_basePlane, m_basePlane.PointAt(m_llX, m_llY, 0), m_basePlane.PointAt(m_urX, m_urY, 0)));
                }

            }
            return m_tileSet;
        }

        /// <summary>
        /// Create an irregular grid according to variable relative column and row sizes
        /// </summary>
        /// <param name="B">Bounds to tile</param>
        /// <param name="XR">List of relative values that drive the column tiling</param>
        /// <param name="YR">List of relative values that drive the row tiling</param>
        /// <param name="EP">Exterior padding as a percent of padding axis</param>
        /// <param name="IP">Interior padding as a percent of padding axis</param>
        /// <param name="PA">Padding Axis</param>
        /// <returns></returns>
        public static List<Rectangle3d> IrregularTiles(Rectangle3d B, List<double> XR, List<double> YR, double EP, double IP, int PA)
        {

            var m_tileSet = new List<Rectangle3d>();

            Plane m_basePlane = new Plane(B.Corner(0), B.Plane.XAxis, B.Plane.YAxis);

            double m_xSum = XR.Sum();
            double m_ySum = YR.Sum();

            double m_padSource = (PA == 0 ? B.Width : B.Height);
            var m_iPadding = IP * m_padSource;
            var m_ePadding = EP * m_padSource;
            
            double m_availableWidth = B.Width - (2 * m_ePadding) - ((XR.Count - 1) * m_iPadding);
            double m_availableHeight = B.Height - (2 * m_ePadding) - ((YR.Count - 1) * m_iPadding);

            double m_rowVal = 0;

            for (int i = 0; i < YR.Count; i++)
            {
                m_rowVal += (i == 0 ? m_ePadding : m_iPadding);
                double m_nextY = m_rowVal + (YR[i] / m_ySum) * m_availableHeight;

                double m_colVal = 0;
                for (int j = 0; j < XR.Count; j++)
                {
                    m_colVal += (j == 0 ? m_ePadding : m_iPadding);
                    double m_nextX = m_colVal + (XR[j] / m_xSum) * m_availableWidth;

                    m_tileSet.Add(new Rectangle3d(m_basePlane,
                      m_basePlane.PointAt(m_colVal, m_rowVal, 0),
                      m_basePlane.PointAt(m_nextX, m_nextY, 0)));

                    m_colVal = m_nextX;
                }

                m_rowVal = m_nextY;
            }

            return m_tileSet;

        }

    }
}
