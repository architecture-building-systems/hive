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
    public class nodeStyleSheet : GH_Component
    {
        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeStyleSheet()
            : base("Conduit Style Sheet", "Style Sheet", "Custom style sheet for Conduit", "Proving Ground", "HUD")
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
            get { return new Guid("e6b6f103-25ee-4493-a4e1-0df4c9528aad"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Styles", "Styles", "Styles to include in style sheet", GH_ParamAccess.list);
            pManager.AddTextParameter("File Path", "Path", "File path to save style sheet", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddBooleanParameter("Save", "Save", "Save style sheet to path", GH_ParamAccess.item, false);

        }

        /// <summary>
        /// Node outputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.Register_GenericParam("StyleSheet", "StyleSheet", "Font Style");
        }
        #endregion


        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<acStyle> m_theseStyles = new List<acStyle>();
            DA.GetDataList(0, m_theseStyles);

            var m_styleDict = new Dictionary<string, acStyle>();

            foreach (acStyle m_thisStyle in m_theseStyles)
            {
                if (m_thisStyle.styleName != "" && !m_thisStyle.unset)
                {
                    m_styleDict.Add(m_thisStyle.styleName, m_thisStyle);
                }
            }


            clsStyleSheet _styleSheet = new clsStyleSheet() { Styles = m_styleDict };
            _styleSheet.EnsureDefaults();

            bool m_save = false;
            DA.GetData(2, ref m_save);

            if(m_save)
            {
                try 
                { 
                    string m_filePath = "";
                    DA.GetData(1, ref m_filePath);
                    _styleSheet.WriteToXmlFile(m_filePath);
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to save style sheet: " + ex.Message);
                }
            }

            //Output
            // DA.SetData(0, _styleSheet);

        }
        #endregion
    }
}

