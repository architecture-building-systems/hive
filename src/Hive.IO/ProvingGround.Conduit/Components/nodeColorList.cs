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
    public class nodeColorList : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeColorList()
            : base("Color list from ColorPalette", "Colors", "Generate colors from a Conduit ColorPalette", "Proving Ground", "HUD")
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
            get { return new Guid("f7ca6bf0-5f48-4702-9e5e-eb85e8ba3fca"); }
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
            pManager.AddGenericParameter("Color Palette", "Palette", "The color palette for generating colors", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number of Colors", "Count", "Number of colors to generate", GH_ParamAccess.item, 10);
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_ColourParam("Colors", "Colors", "Color", GH_ParamAccess.list);
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Solution

            clsPaletteStyle C = new clsPaletteStyle();
            DA.GetData(0, ref C);

            int CC = 10;
            DA.GetData(1, ref CC);

            //Output
            DA.SetDataList(0, C.colorPalette(CC));

        }
        #endregion
    }
}
