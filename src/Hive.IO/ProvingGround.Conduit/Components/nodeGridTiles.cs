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

using ProvingGround.Conduit.Utils;

namespace ProvingGround.GrasshopperAddon
{
    public class nodeGridTiles : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeGridTiles()
            : base("Grid Tiles", "Grid Tiles", "Subdivide a rectangle with a grid of evenly sized tiles", "Proving Ground", "HUD")
        {

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
            get { return new Guid("ec0382a7-6fc8-4cce-81c4-7779cc716742"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddRectangleParameter("Boundary", "Bounds", "The boundary to tile", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Columns", "Col", "Number of columns for boundary subdivision", GH_ParamAccess.item, 5);
            pManager.AddIntegerParameter("Rows", "Row", "Number of rows for boundary subdivision", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Interior Padding", "IntPad", "Interior padding between cells as pct of padding axis", GH_ParamAccess.item, 0.02);
            pManager.AddNumberParameter("Exterior Padding", "ExtPad", "Padding around exterior as pct of padding axis", GH_ParamAccess.item, 0.02);
            pManager.AddIntegerParameter("Padding Axis", "PadAxis", "Axis to calculate padding from (0=X axis, 1=Y axis)", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_RectangleParam("Tiles", "Tiles", "Tile subdivisions", GH_ParamAccess.list);
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

            Rectangle3d B = Rectangle3d.Unset;
            DA.GetData(0, ref B);

            int C = 5;
            DA.GetData(1, ref C);

            int R = 5;
            DA.GetData(2, ref R);

            double IP = 0.02;
            DA.GetData(3, ref IP);

            double EP = 0.02;
            DA.GetData(4, ref EP);

            int PA = 0;
            DA.GetData(5, ref PA);

            //Output
            DA.SetDataList(0, clsTiler.GridTiles(B, IP, EP, C, R, PA));

        }
        #endregion
    }
}



