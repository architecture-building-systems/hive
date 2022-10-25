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
using Grasshopper.Kernel.Parameters;

namespace ProvingGround.Conduit
{
    public class nodeLoadStyleSheet : GH_Component, IGH_VariableParameterComponent
    {

        #region Node Variables

        private clsStyleSheet _styleSheet;

        #endregion

        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeLoadStyleSheet()
            : base("Load Style Sheet", "Style Sheet", "Explicit loader for saved style sheets.", "Proving Ground", "HUD")
        {
            _styleSheet = new clsStyleSheet() { Styles = clsStyleSheet.Defaults() };
        }

        public override bool Write(GH_IWriter writer)
        {
            try
            {
                writer.SetString("ConduitStyleSheet", _styleSheet.XmlString());
            }
            catch
            {

            }

            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            string m_styleSheetAsString = "";
            try
            {
                reader.TryGetString("ConduitStyleSheet", ref m_styleSheetAsString);
                _styleSheet = clsStyleSheet.ReadFromXmlString(m_styleSheetAsString);
            }
            catch
            {
                _styleSheet = new clsStyleSheet() { Styles = clsStyleSheet.Defaults() };
            }

            return base.Read(reader);
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
            //get { return new Guid("1820008b-2c67-4171-91df-3e07a5062a8f"); }
             get { return new Guid("003759ac-efbd-42d5-9920-0fa39b384315"); }
        }
        #endregion

        #region Custom menu items

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem m_matchOutputs = Menu_AppendItem(menu, "Match outputs", new System.EventHandler(this.Menu_MatchOutputs_Clicked));
            m_matchOutputs.ToolTipText = "Update the outputs to reflect the Document-targeted Semantic parameters in the Rhino Document";
            base.AppendAdditionalComponentMenuItems(menu);
        }

        /// <summary>
        /// Tries to match outputs with parameter list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_MatchOutputs_Clicked(object sender, System.EventArgs e)
        {
            TryMatchOutputs();
        }

        /// <summary>
        /// Derived from D. Rutten's "Explode Tree" component
        /// </summary>
        private void TryMatchOutputs()
        {
            if (this.Params.Output.Count == _styleSheet.Styles.Count)
            {
                return;
            }

            if (this.Params.Output.Count < _styleSheet.Styles.Count)
            {
                while (this.Params.Output.Count < _styleSheet.Styles.Count)
                {
                    IGH_Param param = this.CreateParameter(GH_ParameterSide.Output, this.Params.Output.Count);
                    this.Params.RegisterOutputParam(param);
                }
            }
            else if (this.Params.Output.Count > _styleSheet.Styles.Count)
            {
                while (this.Params.Output.Count > _styleSheet.Styles.Count)
                {
                    this.Params.UnregisterOutputParameter(this.Params.Output[this.Params.Output.Count - 1]);
                }
            }
            this.Params.OnParametersChanged();
            this.VariableParameterMaintenance();
            this.ExpireSolution(true);
        }

        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("StyleSheet Path", "Path", "File path for StyleSheet", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            
            //return;
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string m_filePath = "";

            DA.GetData(0, ref m_filePath);

            if (System.IO.File.Exists(m_filePath))
            {
                try
                {
                    _styleSheet = clsStyleSheet.ReadFromXmlFile(m_filePath);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to parse file: " + ex.Message);
                    throw;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Unable to find file");
            }

            List<string> keys = _styleSheet.Styles.Keys.ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                if (i < Params.Output.Count)
                {
                    DA.SetData(i, _styleSheet.Styles[keys[i]]);
                }
            }

        }

        #endregion

        #region Variable parameter implementation

        /// <summary>
        /// This function will get called before an attempt is made to insert a parameter.
        /// Since this method is potentially called on Canvas redraws, it must be <i>fast</i>.
        /// </summary>
        /// <param name="side">Parameter side.</param>
        /// <param name="index">Insertion index of parameter. Index=0 means the parameter will be in the topmost spot.</param>
        /// <returns>
        /// Return True if your component supports a variable parameter at the given location.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will get called before an attempt is made to remove a parameter.
        /// Since this method is potentially called on Canvas redraws, it must be <i>fast</i>.
        /// </summary>
        /// <param name="side">Parameter side.</param>
        /// <param name="index">Removal index of parameter.</param>
        /// <returns>
        /// Return True if your component supports removing a parameter at the given location.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        /// <summary>
        /// This function will be called when a new parameter is about to be inserted.
        /// You <i>must</i> provide a valid parameter or insertion will be skipped.
        /// You do not, repeat <i>not</i>, need to insert the parameter yourself.
        /// </summary>
        /// <param name="side">Parameter side.</param>
        /// <param name="index">Insertion index of parameter. Index=0 means the parameter will be in the topmost spot.</param>
        /// <returns>
        /// A valid IGH_Param instance to be inserted.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return new Param_GenericObject();
        }

        /// <summary>
        /// This function will be called when a parameter is about to be removed.
        /// You do not need to do anything, but this would be a good time to remove any event handlers
        /// that might be attached to the parameter in question.
        /// </summary>
        /// <param name="side">Parameter side.</param>
        /// <param name="index">Removal index of parameter.</param>
        /// <returns>
        /// Return True if the parameter in question can indeed be removed.
        /// Note, this is only in emergencies, typically the CanRemoveParameter function should return false
        /// if the parameter in question is not removable.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        /// <summary>
        /// This method will be called when a closely related set of variable parameter
        /// operations completes. This would be a good time to ensure all Nicknames and parameter
        /// properties are correct. This method will also be called upon IO operations such as
        /// Open, Paste, Undo and Redo.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void VariableParameterMaintenance()
        {

            List<string> keys = _styleSheet.Styles.Keys.ToList();

            for (int i = 0; i < Params.Output.Count; i++)
            {
                IGH_Param ThisOutput = Params.Output[i];
                if (i < keys.Count)
                {
                    ThisOutput.NickName = keys[i];
                }
                else
                {
                    ThisOutput.NickName = "Empty";
                }
                ThisOutput.Access = GH_ParamAccess.item;
                ThisOutput.Optional = true;
            }

        }

        #endregion

    }


}
