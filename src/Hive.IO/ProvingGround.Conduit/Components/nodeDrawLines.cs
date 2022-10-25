using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;

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

using ProvingGround.Conduit.Classes;

namespace ProvingGround.Conduit
{
    public class nodeDrawLines : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeDrawLines()
            : base("Draw Lines", "HUD Line", "Draw lines in the HUD", "Proving Ground", "HUD")
        {

        }

        /// <summary>
        /// Component Exposure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// GUID generator http://www.guidgenerator.com/online-guid-generator.aspx
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5fae5299-e9ab-410a-b0d2-256bf1340812"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "Lines", "Lines in the drawing space", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Thickness Override", "Thick", "Optional Line thickness list to override curve style", GH_ParamAccess.list);
            pManager.AddColourParameter("Color Override", "Color", "Optional color list to override curve style", GH_ParamAccess.list);
            pManager.AddGenericParameter("Custom Curve Style", "Style", "Optional custom curve style", GH_ParamAccess.item);
            pManager.AddNumberParameter("Pixel Depth", "PxDepth", "Pixel depth for line stacking", GH_ParamAccess.item, 1.0);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("HUD Drawing Object", "DrawObj", "Line drawing object");
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //Variables
            List<Line> L = new List<Line>();
            DA.GetDataList<Line>(0, L);

            List<int> T = new List<int>();
            DA.GetDataList<int>(1, T);

            List<Color> C = new List<Color>();
            DA.GetDataList<Color>(2, C);

             //Matches lists of varying lengths for the thickness and color
             //of lines to the length of the list of lines

            for (int i = 0; i < L.Count; i++)
            {
                if (i + 1 > T.Count && T.Count > 0) T.Add(T.Last());
                if (i + 1 > C.Count && C.Count > 0) C.Add(C.Last());
            }

            if (T.Count > L.Count) T = T.GetRange(0, L.Count);
            if (C.Count > L.Count) C = C.GetRange(0, L.Count);

            clsCurveStyle m_setStyle = new clsCurveStyle() { styleName = "Default Curve", unset = true};

            if (m_setStyle.unset)
            {
                object S = new object();
                DA.GetData(3, ref S);

                if (S.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
                {
                    Grasshopper.Kernel.Types.GH_String m_styleName = (Grasshopper.Kernel.Types.GH_String)S;
                    if (m_styleName.IsValid)
                    {
                        m_setStyle = new clsCurveStyle() { styleName = m_styleName.Value, unset = true };
                    }
                }
                else
                {
                    try
                    {
                        DA.GetData(3, ref m_setStyle);
                    }
                    catch { }
                }

            }

            double PD = 1.0;
            DA.GetData(4, ref PD);

            DrawLines thisDraw = new DrawLines(L, T, C, new clsCurveStyle[] { m_setStyle }, PD);
            DA.SetData(0, thisDraw as iDrawObject);

        }

        

        #endregion
    }
}

