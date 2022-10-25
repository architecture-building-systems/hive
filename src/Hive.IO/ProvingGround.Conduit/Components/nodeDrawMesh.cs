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
    public class nodeDrawMesh : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeDrawMesh()
            : base("Draw a Mesh", "HUD Mesh", "Draw a mesh in the HUD", "Proving Ground", "HUD")
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
            get { return new Guid("0468ee36-4f02-4f31-9fdc-aebb2a13f0e0"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh in the drawing space", GH_ParamAccess.item);
            pManager.AddNumberParameter("Pixel Depth", "PxDepth", "Depth in pixels for object to be drawn away from viewport (to control stacking of elements)", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.Register_GenericParam("Drawing Object","DrawObj", "Mesh drawing object");
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh M = new Mesh();
            DA.GetData(0, ref M);

            double PD = 1.0;
            DA.GetData(1, ref PD);

            DrawMesh thisDraw = new DrawMesh(M, PD);

            DA.SetData(0, thisDraw as iDrawObject);
        }
        #endregion
    }
}
