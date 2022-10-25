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
    public class nodeColorPalette : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeColorPalette()
            : base("Conduit ColorPalette", "Palette", "Custom color generator for Conduit", "Proving Ground", "HUD")
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
            get { return new Guid("b40b613e-60e3-4fed-8e9f-747d4e26b3a1"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Palette Name", "Name", "Optional name for adding palette to style sheet", GH_ParamAccess.item, "");
            pManager[0].Optional = true;

            pManager.AddIntegerParameter("Seed", "Seed", "An integer for the random color generator seed", GH_ParamAccess.item, 3);
            pManager.AddIntervalParameter("Hue Range", "Hue", "A domain which defines the range of Hue difference for new colors generated (default is -0.05 to 0.05)",
                GH_ParamAccess.item, new Interval(-0.05, 0.05));
            pManager.AddIntervalParameter("Saturation Range", "Sat", "A domain which defines the range of Saturation difference for new colors generated (default is -0.15 to 0.15)",
                GH_ParamAccess.item, new Interval(-0.15, 0.15));
            pManager.AddIntervalParameter("Luminance Range", "Lum", "A domain which defines the range of Luminance difference for new colors generated (default is -0.10 to 0.10)",
                GH_ParamAccess.item, new Interval(-0.10, 0.10));
            pManager.AddColourParameter("Base Colors", "Colors", "The first colors that seed the palette", GH_ParamAccess.list,
                new List<System.Drawing.Color>()
                {
                    System.Drawing.Color.FromArgb(217, 6, 167),
                    System.Drawing.Color.FromArgb(67, 193, 222),
                    System.Drawing.Color.FromArgb(255, 241, 255)
                });

            
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Palette", "Palette", "Color Palette Generator");
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
            int m_seed = 0;
            Interval m_hueRange = new Interval(-0.05, 0.05);
            Interval m_saturationRange = new Interval(-0.15, 0.15);
            Interval m_luminanceRange = new Interval(-0.10, 0.10);
            List<System.Drawing.Color> m_colors = new List<System.Drawing.Color>();

            System.Drawing.Color m_color = System.Drawing.Color.Black;

            DA.GetData(0, ref m_name);
            DA.GetData(1, ref m_seed);
            DA.GetData(2, ref m_hueRange);
            DA.GetData(3, ref m_saturationRange);
            DA.GetData(4, ref m_luminanceRange);
            DA.GetDataList(5, m_colors);

            clsPaletteStyle m_thisPalette = new clsPaletteStyle()
            {
                styleName = m_name,
                unset = false,
                seed = m_seed,
                hueRange = m_hueRange,
                saturationRange = m_saturationRange,
                luminanceRange = m_luminanceRange,
                colors = m_colors
            };

            //// ugly but it works...find a better solution for expiring objects that may rely on a given style via string input
            //foreach (IGH_ActiveObject m_componentCheck in this.OnPingDocument().ActiveObjects())
            //{
            //    // add checks for
            //    if (m_componentCheck.ComponentGuid == new Guid("5fae5299-e9ab-410a-b0d2-256bf1340812"))
            //    {
            //        m_componentCheck.ExpireSolution(true);
            //    }
            //}

            DA.SetData(0, m_thisPalette);

        }
        #endregion
    }
}


