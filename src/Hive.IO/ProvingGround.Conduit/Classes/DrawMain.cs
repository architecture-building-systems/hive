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
using Rhino.Display;
using Rhino.Geometry;

namespace ProvingGround.Conduit.Classes
{
    /// <summary>
    /// The display conduit that manages the HUD
    /// </summary>
    public class DrawMain : Rhino.Display.DisplayConduit
    {
        //the bounds of the source drawing area
        public Interval boundX;
        public Interval boundY;
        public List<iDrawObject> DrawObjects;
        public System.Drawing.Graphics FontSizer;
        public string ViewportFilter;

        /// <summary>
        /// Empty constructor
        /// </summary>
        public DrawMain()
        { }

        /// <summary>
        /// The primary conduit drawing class
        /// </summary>
        /// <param name="_boundX">Width boundary of the drawing space in the XY Plane</param>
        /// <param name="_boundY">Height boundary of the drawing space in XY Plane</param>
        /// <param name="drawObjects">List of iDrawObjects to draw</param>
        public DrawMain(Interval _boundX, Interval _boundY, List<iDrawObject> drawObjects, System.Drawing.Graphics fontSizer, clsStyleSheet styleSheet, string viewportFilter)
        {
            
            boundX = _boundX;
            boundY = _boundY;
            DrawObjects = drawObjects;
            FontSizer = fontSizer;
            ViewportFilter = viewportFilter;

            // set the adjusted X and Y intervals
            // for all draw objects
            foreach (iDrawObject thisDrawObject in DrawObjects)
            {
                thisDrawObject.baseX = boundX.NormalizedParameterAt(thisDrawObject.objectX.Min);
                thisDrawObject.lengthX = thisDrawObject.objectX.Length / boundX.Length;
                thisDrawObject.baseY = boundY.NormalizedParameterAt(thisDrawObject.objectY.Min);
                thisDrawObject.lengthY = thisDrawObject.objectY.Length / boundY.Length;
            }

        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            base.CalculateBoundingBox(e);
            // get the current view corners
            Point3d[] m_farCorners = e.Viewport.GetFarRect();
            Point3d[] m_nearCorners = e.Viewport.GetNearRect();

            Point3d[] m_viewCorners = new Point3d[4]
            {
                (m_farCorners[0] * 0.15 + m_nearCorners[0] * 0.85),
                (m_farCorners[1] * 0.15 + m_nearCorners[1] * 0.85),
                (m_farCorners[2] * 0.15 + m_nearCorners[2] * 0.85),
                (m_farCorners[3] * 0.15 + m_nearCorners[3] * 0.85)
                };

            BoundingBox b = new BoundingBox(m_viewCorners);

            //List<Point3d> boxCorners = new List<Point3d>();
            //boxCorners.AddRange(m_farCorners);
            //boxCorners.AddRange(m_nearCorners);

            //BoundingBox b = new BoundingBox(boxCorners);
            e.IncludeBoundingBox(b);
        }

        /// <summary>
        /// Primary conduit override for drawing objects
        /// </summary>
        /// <param name="e">Inherits the DrawEventArgs from the DisplayConduit</param>
        //protected override void PostDrawObjects(Rhino.Display.DrawEventArgs e)
        //{

        //    base.PostDrawObjects(e);
        //    if (!string.IsNullOrEmpty(ViewportFilter) && 
        //        String.Compare(this.ViewportFilter, "*", false) != 0 && 
        //        !(e.Viewport.Name.Contains(ViewportFilter)))
        //    {
        //        return;
        //    }

        //    // get the current view corners
        //    Point3d[] m_farCorners = e.Viewport.GetFarRect();
        //    Point3d[] m_nearCorners = e.Viewport.GetNearRect();

        //    Point3d[] m_viewCorners = new Point3d[3]
        //    {
        //        (m_farCorners[0] + m_nearCorners[0]) * 0.5,
        //        (m_farCorners[1] + m_nearCorners[1]) * 0.5,
        //        (m_farCorners[2] + m_nearCorners[2]) * 0.5,
        //        };

        //    Plane m_drawPlane = new Plane(m_viewCorners[0], m_viewCorners[1], m_viewCorners[2]);

        //    // get the width and height of the current view
        //    double m_viewWidth = m_viewCorners[0].DistanceTo(m_viewCorners[1]);
        //    double m_viewHeight = m_viewCorners[0].DistanceTo(m_viewCorners[2]);

        //    // get the pixel width and height of the viewport
        //    //System.Drawing.Rectangle m_theseBounds = e.Viewport.Bounds;

        //    // set multiple to transform units for the drawing plane
        //    // between viewport rectangle in the model and the viewport
        //    // pixel dimensions
        //    double m_unitPerPx = m_viewWidth / e.Viewport.Size.Width;

        //    // draw each object
        //    foreach (iDrawObject thisDrawObject in DrawObjects)
        //    {
        //        if (!thisDrawObject.drawInForeground)
        //        {
        //            thisDrawObject.objDraw(e, m_drawPlane, m_unitPerPx, FontSizer);
        //        } 
        //    }
        //}

        protected override void DrawForeground(Rhino.Display.DrawEventArgs e)
        {
            base.DrawForeground(e);
            if (!string.IsNullOrEmpty(ViewportFilter) &&
                String.Compare(this.ViewportFilter, "*", false) != 0 &&
                !(e.Viewport.Name.Contains(ViewportFilter)))
            {
                return;
            }

            // get the current view corners
            Point3d[] m_farCorners = e.Viewport.GetFarRect();
            Point3d[] m_nearCorners = e.Viewport.GetNearRect();

            Point3d[] m_viewCorners = new Point3d[3]
            {
                (m_farCorners[0] * 0.15 + m_nearCorners[0] * 0.85),
                (m_farCorners[1] * 0.15 + m_nearCorners[1] * 0.85),
                (m_farCorners[2] * 0.15 + m_nearCorners[2] * 0.85)
                };

            //e.Viewport.GetNearRect();

            Plane m_drawPlane = new Plane(m_viewCorners[0], m_viewCorners[1], m_viewCorners[2]);

            // get the width and height of the current view
            double m_viewWidth = m_viewCorners[0].DistanceTo(m_viewCorners[1]);
            double m_viewHeight = m_viewCorners[0].DistanceTo(m_viewCorners[2]);

            // get the pixel width and height of the viewport
            //System.Drawing.Rectangle m_theseBounds = e.Viewport.Bounds;

            // set multiple to transform units for the drawing plane
            // between viewport rectangle in the model and the viewport
            // pixel dimensions
            double m_unitPerPx = m_viewWidth / e.Viewport.Size.Width;

            // draw each object
            foreach (iDrawObject thisDrawObject in DrawObjects)
            {
                //if (thisDrawObject.drawInForeground)
                //{
                    thisDrawObject.objDraw(e, m_drawPlane, m_unitPerPx, FontSizer);
                //}
            }
        }
    }
}
