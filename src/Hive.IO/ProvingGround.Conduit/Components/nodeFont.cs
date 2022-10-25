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
    public class nodeFont : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeFont()
            : base("Conduit Font", "Font", "Custom font settings for Conduit", "Proving Ground", "HUD")
        {

        }

        /// <summary>
        /// Component Exposure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// GUID generator http://www.guidgenerator.com/online-guid-generator.aspx
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c3396783-f85e-4086-aa67-f6899da0bdcf"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Style Name", "Name", "Optional name to add font to style sheet", GH_ParamAccess.item, "");
            pManager.AddTextParameter("Font", "Font", "Target font as text", GH_ParamAccess.item, "Arial");
            pManager.AddNumberParameter("Text Height", "Height", "Height of text", GH_ParamAccess.item, 12.0);
            pManager.AddBooleanParameter("Adaptive Height", "Adapt", "Adapt height of text to boundary", GH_ParamAccess.item, true);
            pManager.AddNumberParameter("Text Spacing", "Spacing", "Spacing between multiple text lines", GH_ParamAccess.item, 1.15);
            pManager.AddIntegerParameter("Alignment", "Align", "Horizontal alignment of text: 0=Left, 1=Center, 2=Right", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Bold", "Bold", "Text is bold face", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Italic", "Italic", "Text is italicized", GH_ParamAccess.item, false);
            pManager.AddColourParameter("Color", "Color", "Text Color", GH_ParamAccess.item, System.Drawing.Color.Black);
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Font Style", "Font", "Font Style");
        }
        #endregion

        #region Menus
        /// <summary>
        /// Sample Menu item
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Use font interface", Menu_MyCustomItemClicked);
            return true;
        }

        /// <summary>
        /// Menu item clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_MyCustomItemClicked(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Not yet implemented");
            //Custom Menu Code
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

            string m_name = "";
            string m_font = "";
            double m_height = 0;
            bool m_adapt = true;
            double m_spacing = 1.15;
            int m_alignment = 0;
            bool m_bold = false;
            bool m_italics = false;
            System.Drawing.Color m_color = System.Drawing.Color.Black;

            DA.GetData(0, ref m_name);
            DA.GetData(1, ref m_font);
            DA.GetData(2, ref m_height);
            DA.GetData(3, ref m_adapt);
            DA.GetData(4, ref m_spacing);
            DA.GetData(5, ref m_alignment);
            DA.GetData(6, ref m_bold);
            DA.GetData(7, ref m_italics);
            DA.GetData(8, ref m_color);

            clsFontStyle m_thisFont = new clsFontStyle()
            {
                styleName = m_name,

                unset = false,

                fontBasis = new clsFontBasis()
                {
                    FontFace = m_font,
                    TextHeight = m_height,
                    AdaptiveHeight = m_adapt,
                    TextSpacing = m_spacing,
                    TextAlign = m_alignment == 0
                    ? "Left"
                    : m_alignment == 1
                    ? "Center"
                    : "Right",
                    Bold = m_bold,
                    Italic = m_italics,
                    Color = m_color
                }
            };

            //Output
            DA.SetData(0, m_thisFont);

        }
        #endregion
    }
}
