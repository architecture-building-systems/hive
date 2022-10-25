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
    public class nodeCurveStyle : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeCurveStyle()
            : base("Conduit Curve Style", "Curve Style", "Custom curve style settings for Conduit", "Proving Ground", "HUD")
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
            get { return new Guid("aa333d76-a3b2-4a2c-9ce1-9ea064023815"); }
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

            pManager.AddIntegerParameter("Curve Weight", "Weight", "Thickness of curve in pixels", GH_ParamAccess.item, 1);
            pManager.AddColourParameter("Color", "Color", "Curve Color", GH_ParamAccess.item, System.Drawing.Color.Black);

        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Curve Style", "CrvStyle", "Curve Style");
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
            Menu_AppendItem(menu, "Use curve style interface", Menu_MyCustomItemClicked);
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
            int m_weight = 0;
            System.Drawing.Color m_color = System.Drawing.Color.Black;

            DA.GetData(0, ref m_name);
            DA.GetData(1, ref m_weight);
            DA.GetData(2, ref m_color);

            clsCurveStyle m_thisCurveStyle = new clsCurveStyle()
            {
                styleName = m_name,

                unset = false,

                curveBasis = new clsCurveBasis()
                {
                    Thickness = m_weight,
                    Color = m_color
                }
            };

            // ugly but it works...find a better solution for expiring objects that may rely on a given style via string input
            foreach (IGH_ActiveObject m_componentCheck in this.OnPingDocument().ActiveObjects())
            {
                // add checks for
                if (m_componentCheck.ComponentGuid == new Guid("5fae5299-e9ab-410a-b0d2-256bf1340812"))
                {
                    m_componentCheck.ExpireSolution(true);
                }
            }

            //Output
            DA.SetData(0, m_thisCurveStyle);

            

        }
        #endregion
    }
}

