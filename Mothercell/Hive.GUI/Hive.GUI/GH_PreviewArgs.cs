using Grasshopper.Kernel;
using Rhino.Display;
using Rhino.Geometry;
using System.Drawing;

namespace Hive.GUI
{
    public class GH_PreviewArgs : IGH_PreviewArgs
    {
        private GH_Document m_doc;
        private DisplayPipeline m_pipeline;
        private RhinoViewport m_viewport;
        private int m_curve_thickness;
        private Color m_wire_normal;
        private Color m_wire_selected;
        private DisplayMaterial m_face_normal;
        private DisplayMaterial m_face_selected;
        private MeshingParameters m_mesh_params;

        internal GH_PreviewArgs(
          GH_Document doc,
          DisplayPipeline pl,
          RhinoViewport vp,
          int curve_thickness,
          Color wire,
          Color wire_sel,
          DisplayMaterial face,
          DisplayMaterial face_sel,
          MeshingParameters mesh_params)
        {
            this.m_doc = doc;
            this.m_pipeline = pl;
            this.m_viewport = vp;
            this.m_curve_thickness = curve_thickness;
            this.m_wire_normal = wire;
            this.m_wire_selected = wire_sel;
            this.m_face_normal = face;
            this.m_face_selected = face_sel;
            this.m_mesh_params = mesh_params;
        }

        /// <summary>Gets the Grasshopper document that is currently drawing the preview.</summary>
        public GH_Document Document
        {
            get
            {
                return this.m_doc;
            }
        }

        /// <summary>Gets the Pipeline that is being used to draw the current preview.</summary>
        public DisplayPipeline Display
        {
            get
            {
                return this.m_pipeline;
            }
        }

        /// <summary>Gets the Viewport in which the current preview is drawn.</summary>
        public RhinoViewport Viewport
        {
            get
            {
                return this.m_viewport;
            }
        }

        /// <summary>Gets the curve thickness as defined by the viewport display scheme.</summary>
        public int DefaultCurveThickness
        {
            get
            {
                return this.m_curve_thickness;
            }
        }

        /// <summary>Gets the document default material for unselected Faces.</summary>
        public DisplayMaterial ShadeMaterial
        {
            get
            {
                return this.m_face_normal;
            }
        }

        /// <summary>Gets the document default material for selected Faces.</summary>
        public DisplayMaterial ShadeMaterial_Selected
        {
            get
            {
                return this.m_face_selected;
            }
        }

        /// <summary>Gets the document default colour for unselected Wires.</summary>
        public Color WireColour
        {
            get
            {
                return this.m_wire_normal;
            }
        }

        /// <summary>Gets the document default colour for selected Wires.</summary>
        public Color WireColour_Selected
        {
            get
            {
                return this.m_wire_selected;
            }
        }

        /// <summary>Gets the meshing parameters to be used during meshing breps.</summary>
        public MeshingParameters MeshingParameters
        {
            get
            {
                return this.m_mesh_params;
            }
        }
    }
}
