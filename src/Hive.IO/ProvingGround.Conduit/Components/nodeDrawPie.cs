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
    public class nodeDrawPie : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeDrawPie()
            : base("Draw Pie Chart", "HUD Pie", "Draw pie chart in the HUD", "Proving Ground", "HUD")
        {

        }

        /// <summary>
        /// Component Exposure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// GUID generator http://www.guidgenerator.com/online-guid-generator.aspx
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b619adb1-1434-4076-961a-ff674d091bd2"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddRectangleParameter("Boundary Rectangle", "Bounds", "Bounds for pie", GH_ParamAccess.item);
            pManager.AddTextParameter("Labels", "Labels", "Labels for pie", GH_ParamAccess.list);
            pManager[1].Optional = true;

            pManager.AddNumberParameter("Data values", "Data", "Data values", GH_ParamAccess.list);
            
            pManager.AddGenericParameter("Pie Style", "PieStyle", "Pie style settings", GH_ParamAccess.item);
            pManager[3].Optional = true;

            pManager.AddGenericParameter("Color Palette", "Palette", "A palette style or list of colors for the chart", GH_ParamAccess.item);
            pManager[4].Optional = true;

        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("HUD Pie Chart Object", "DrawObj", "Pie Drawing Object");
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Rectangle3d R = new Rectangle3d();
            DA.GetData(0, ref R);
            
            List<string> L = new List<string>();
            DA.GetDataList<string>(1, L);
            if (!L.Any())
            {
                L = new List<string>();
            }

            List<double> D = new List<double>();
            DA.GetDataList<double>(2, D);

            clsPieStyle PS = new clsPieStyle() { styleName = "Default Pie", unset = true };
            
            if (PS.unset)
            {

                object pieS = new object();
                DA.GetData(3, ref pieS);
                if (pieS.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
                {
                    Grasshopper.Kernel.Types.GH_String m_pieName = (Grasshopper.Kernel.Types.GH_String)pieS;
                    if (m_pieName.IsValid)
                    {
                        PS = new clsPieStyle() { styleName = m_pieName.Value, unset = true };
                    }
                }
                else
                {
                    try
                    {
                        DA.GetData(3, ref PS);
                    }
                    catch { }

                }

            }

            clsPaletteStyle C = new clsPaletteStyle() { styleName = "Default Palette", unset = true };
            
            if(C.unset)
            {

                object paletteS = new object();
                DA.GetData(4, ref paletteS);
                if (paletteS.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
                {
                    Grasshopper.Kernel.Types.GH_String m_paletteName = (Grasshopper.Kernel.Types.GH_String)paletteS;
                    if (m_paletteName.IsValid)
                    {
                        C = new clsPaletteStyle() { styleName = m_paletteName.Value, unset = true };
                    }
                }
                else
                {
                    try
                    {
                        DA.GetData(4, ref C);
                    }
                    catch { }

                }

            }

            DrawPie thisDraw = new DrawPie(R, L, D, new acStyle[] { (acStyle)PS, (acStyle)C });

            DA.SetData(0, thisDraw as iDrawObject);

        }
        #endregion
    }
}

