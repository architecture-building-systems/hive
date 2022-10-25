using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;

using Rhino;
using Rhino.Collections;
using Rhino.Geometry;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

using ProvingGround.Conduit.Classes;
using ProvingGround.Conduit.UI;

namespace ProvingGround.Conduit.Components
{
    public class nodeSample : GH_Component
    {
        #region Register Node

        private Rectangle _bounds;

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeSample()
            : base("Get viewport boundary dimensions", "Bounds", "A rectangle that reflects the current viewport boundary. Refresh to update", "Proving Ground", "HUD")
        {
            
            Rhino.Display.RhinoView m_activeView = RhinoDoc.ActiveDoc.Views.ActiveView;
            _bounds = m_activeView.MainViewport.Bounds;

        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("xDim", _bounds.Width);
            writer.SetInt32("yDim", _bounds.Height);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int xDim = 0;
            int yDim = 0;
            reader.TryGetInt32("xDim", ref xDim);
            reader.TryGetInt32("yDim", ref yDim);
            _bounds = new Rectangle(0,0,xDim, yDim);
            return base.Read(reader);
        }

        /// <summary>
        /// Component Exposure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// GUID generator http://www.guidgenerator.com/online-guid-generator.aspx
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c070f04b-15ad-4aa9-953a-f6d511381038"); }
        }

        /// <summary>
        /// Icon 24x24
        /// </summary>
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Refresh", "Refresh", "Refresh the Boundary Dimension", GH_ParamAccess.item, true);
            pManager.AddPointParameter("Origin", "Origin", "Origin point for showing rectangle", GH_ParamAccess.item, new Point3d(0, 0, 0));
            pManager.AddNumberParameter("Scale", "Scale", "Scale of rectangle", GH_ParamAccess.item, 0.25);
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_RectangleParam("Bounds", "Bounds", "The boundary dimensions of the active viewport");
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool m_refresh = false;
            Point3d m_origin = new Point3d(0, 0, 0);
            double m_scale = 0.25;

            DA.GetData(0, ref m_refresh);
            DA.GetData(1, ref m_origin);
            DA.GetData(2, ref m_scale);

            if (m_refresh)
            {
                Rhino.Display.RhinoView m_activeView = RhinoDoc.ActiveDoc.Views.ActiveView;
                _bounds = m_activeView.MainViewport.Bounds;
            }
            

            Plane m_setPlane = Plane.WorldXY;
            m_setPlane.Origin = m_origin;

            Rectangle3d m_outputBounds = new Rectangle3d(m_setPlane,
                (double)_bounds.Width * m_scale,
                (double)_bounds.Height * m_scale);

            //Output
            DA.SetData(0, m_outputBounds);

        }
        #endregion
    }
}

