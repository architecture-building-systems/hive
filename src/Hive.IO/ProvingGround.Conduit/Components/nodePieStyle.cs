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
    public class nodePieStyle : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodePieStyle()
            : base("Conduit Pie Style", "Pie Style", "Custom pie style settings for Conduit", "Proving Ground", "HUD")
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
            get { return new Guid("15a8b313-0f56-4e32-8732-c686e222467e"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Style Name", "Name", "Optional name to add curve style to style sheet", GH_ParamAccess.item, "");
            pManager[0].Optional = true;

            pManager.AddNumberParameter("Inner Radius", "InnerRad", "Radius as a percent of bounds for inner circle (set to 0 for pie chart, >0 for donut)", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Outer Radius", "OuterRad", "Radius as a percent of bounds for outer circle (should be larger than inner radius)", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Label Radius", "LabRad", "Radius as a percent of bounds for label positions (should be larger than inner radius)", GH_ParamAccess.item, 1.0);
            pManager.AddTextParameter("Label Font Face", "LabFont", "Font face of text object (e.g 'Arial', 'Times New Roman', etc.) NOT a Conduit font style", GH_ParamAccess.item, "Arial");
            pManager.AddNumberParameter("Relative Label Height", "LabHeight", "Relative height of the label font (between 0 and 1", GH_ParamAccess.item, 0.1);
            pManager.AddColourParameter("Label Color", "LabColor", "Color of labels for this Pie Style", GH_ParamAccess.item, System.Drawing.Color.Black);
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Pie Style", "PieStyle", "Pie Style");
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
            Menu_AppendItem(menu, "Use pie style interface", Menu_MyCustomItemClicked);
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
            double m_innerRadius = 0;
            double m_outerRadius = 1.0;
            double m_labelRadius = 0.5;
            string m_labelFontFace = "Arial";
            double m_labelHeight = 0.1;
            System.Drawing.Color m_labelColor = System.Drawing.Color.Black;

            DA.GetData(0, ref m_name);
            DA.GetData(1, ref m_innerRadius);
            DA.GetData(2, ref m_outerRadius);
            DA.GetData(3, ref m_labelRadius);
            DA.GetData(4, ref m_labelFontFace);
            DA.GetData(5, ref m_labelHeight);
            DA.GetData(6, ref m_labelColor);

            clsPieStyle m_thisPieStyle = new clsPieStyle()
            {
                styleName = m_name,
                unset = false,
                innerRadius = m_innerRadius,
                outerRadius = m_outerRadius,
                labelRadius = m_labelRadius,
                labelFontFace = m_labelFontFace,
                labelRelativeHeight = m_labelHeight,
                labelColor = m_labelColor
            };

            //// ugly but it works...find a better solution for expiring objects that may rely on a given style via string input
            //foreach (IGH_ActiveObject m_componentCheck in this.OnPingDocument().ActiveObjects())
            //{
            //    // add checks for additional affected components
            //    if (m_componentCheck.ComponentGuid == new Guid("5fae5299-e9ab-410a-b0d2-256bf1340812"))
            //    {
            //        m_componentCheck.ExpireSolution(true);
            //    }
            //}

            //Output
            DA.SetData(0, m_thisPieStyle);

        }
        #endregion
    }
}

