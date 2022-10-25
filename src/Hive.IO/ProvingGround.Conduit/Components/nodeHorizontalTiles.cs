﻿using GH_IO;
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
    public class nodeHorizontalTiles : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeHorizontalTiles()
            : base("Horizontal Tiles", "Horz Tiles", "Subdivide a rectangle with horizontally stacked tiles", "Proving Ground", "HUD")
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
            get { return new Guid("b1b121ed-a802-46f5-b70a-101d14282e5a"); }
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
            pManager.AddRectangleParameter("Boundary", "Bounds", "The boundary to tile", GH_ParamAccess.item);
            pManager.AddNumberParameter("Relative Sizes", "RelSize", "List of numbers that define tiles relative sizes", GH_ParamAccess.list, new List<double>() { 1.0, 1.0, 1.0 });
            pManager.AddNumberParameter("Height", "Height", "Height of tiles as pct of boundary height", GH_ParamAccess.item, 0.25);
            pManager.AddNumberParameter("Width Padding", "WidthPad", "Width padding as pct of padding axis", GH_ParamAccess.item, 0.02);
            pManager.AddNumberParameter("Height Padding", "HeightPad", "Height padding as pct of padding axis", GH_ParamAccess.item, 0.02);
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

            List<double> R = new List<double>();
            DA.GetDataList(1, R);

            double H = 0.25;
            DA.GetData(2, ref H);

            double WP = 0.02;
            DA.GetData(3, ref WP);

            double HP = 0.02;
            DA.GetData(4, ref HP);

            //Output
            DA.SetDataList(0, clsTiler.HorizontalTiles(B, R, H, WP, HP));

        }
        #endregion
    }
}


