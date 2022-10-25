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
    public class nodeDrawText : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeDrawText()
            : base("Draw Text", "HUD Text", "Draw text in the HUD", "Proving Ground", "HUD")
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
            get { return new Guid("413f6917-a3de-4e32-ac07-cb73619c95b1"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddRectangleParameter("Boundary", "Bounds", "Boundary for text", GH_ParamAccess.item);
            pManager.AddTextParameter("Text", "Text", "Text lines to write", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Text Ordering", "Order", "Order direction for text stacking (0=Top to bottom, 1=Bottom to top)", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Text Distribution", "Distrib", "Distibution of text (0=Distribute over full boundary, 1=Absolute by text height)", GH_ParamAccess.item, 0);
            pManager.AddColourParameter("Color Override", "Color", "Optional color list to override font", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Orient Vertically", "Vert", "Orient text vertically", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("Custom Font", "Font", "Optional custom font", GH_ParamAccess.item);

            pManager[4].Optional = true;
            pManager[6].Optional = true;
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Drawing Object", "DrawObjs", "Text drawing object");
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rectangle3d B = new Rectangle3d();
            DA.GetData(0, ref B);

            List<string> T = new List<string>();
            DA.GetDataList<string>(1, T);

            int O = 0;
            DA.GetData(2, ref O);

            int D = 0;
            DA.GetData(3, ref D);

            List<Color> C = new List<Color>();
            DA.GetDataList<Color>(4, C);

            bool OV = false;
            DA.GetData(5, ref OV);

            clsFontStyle m_setStyle = new clsFontStyle() { styleName="Default Font", unset = true };

            if (m_setStyle.unset)
            {
                object S = new object();
                DA.GetData(6, ref S);

                if (S.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
                {
                    Grasshopper.Kernel.Types.GH_String m_fontName = (Grasshopper.Kernel.Types.GH_String)S;
                    if(m_fontName.IsValid)
                    {
                        m_setStyle = new clsFontStyle() { styleName = m_fontName.Value, unset = true };
                    }
                }
                else
                {
                    try
                    {
                        DA.GetData(6, ref m_setStyle);
                    }
                    catch { }
                }
            }

            DrawText thisDraw = new DrawText(T, B, O, D, C, OV, new clsFontStyle[] { m_setStyle });
            DA.SetData(0, thisDraw as iDrawObject);
            
        }
        #endregion
    }
}

