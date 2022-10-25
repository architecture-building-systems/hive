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

namespace ProvingGround.Conduit
{
    public class nodeHUD : GH_Component
    {

        #region Node Variables

        DrawMain _drawConduit;
        private clsStyleSheet _styleSheet;

        bool _s = false;

        System.Drawing.Graphics _fontSizer;

        [System.Runtime.CompilerServices.CompilerGenerated]
        private string _ViewportFilter;

        public string ViewportFilter
        {
            get
            {
                return this._ViewportFilter;
            }
            set
            {
                this._ViewportFilter = value;
            }
        }

        #endregion

        #region Register Node

        /// <summary>
        /// Load Node Template
        /// </summary>
        public nodeHUD()
            : base("Heads Up Display (HUD)", "HUD", "The core node for drawing a heads up display.", "Proving Ground", "HUD")
        {
            _styleSheet = new clsStyleSheet() { Styles = clsStyleSheet.Defaults() };
            this.ViewportFilter = string.Empty;
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

            writer.SetString("ViewportFilter", this.ViewportFilter);
            
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

            this.ViewportFilter = string.Empty;
            if (reader.ItemExists("ViewportFilter"))
            {
                this.ViewportFilter = reader.GetString("ViewportFilter");
            }
            this.Message = this.ViewportFilter;

            return base.Read(reader);
        }

        /// <summary>
        /// Component Exposure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }

        /// <summary>
        /// GUID generator http://www.guidgenerator.com/online-guid-generator.aspx
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1820008b-2c67-4171-91df-3e07a5062a8f"); }
            // get { return new Guid("003759ac-efbd-42d5-9920-0fa39b384315"); }
        }
        #endregion

        #region Inputs/Outputs
        /// <summary>
        /// Node inputs
        /// </summary>
        /// <param name="pManager"></param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Show HUD", "Show", "Toggle for showing HUD", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("Drawing Objects", "DrawObjs", "Objects to draw", GH_ParamAccess.list);
            pManager.AddRectangleParameter("Drawing Boundary", "Bounds", "Rectangular sketch container for objects (in the XY Plane)", GH_ParamAccess.item);
        }

        // Grasshopper.Kernel.Special.GH_ColourSwatch.GH_ColourSwatchPublishProxy
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Load style sheet from file", LoadStyleSheet);
            Menu_AppendItem(menu, "Save style sheet to file", SaveStyleSheet);
            System.Windows.Forms.ToolStripMenuItem item = GH_DocumentObject.Menu_AppendItem(menu, "Viewport Filter"); // via the Display Preview component
            System.Windows.Forms.ToolStripTextBox vpItem = GH_DocumentObject.Menu_AppendTextItem(menu, _ViewportFilter, new GH_MenuTextBox.KeyDownEventHandler(this.VPFilterKeyDown), null, false); // same as above
            //return true;
        }

        //yoink!
        private void VPFilterKeyDown(GH_MenuTextBox sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Return)
            {
                if (String.Compare(this.ViewportFilter, sender.Text, false) == 0)
                {
                    return;
                }
                
                this.RecordUndoEvent("Filter: " + sender.Text);
                this.ViewportFilter = sender.Text;
                this.Message = sender.Text;
                this.Attributes.ExpireLayout();
                this.ExpireSolution(true);
                Instances.RedrawAll();
               
            }
        }

        private void LoadStyleSheet(object sender, EventArgs e)
        {

            string filePath = OpenFile("", "", "Conduit style sheet (*.xml)|*.xml");
            if (System.IO.File.Exists(filePath))
            {

                try
                {
                    _styleSheet = clsStyleSheet.ReadFromXmlFile(filePath);
                    this.ExpireSolution(true);
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

        }

        private void SaveStyleSheet(object sender, EventArgs e)
        {

            string filePath = SaveFile("", ".xml", "");
            try
            {
                _styleSheet.WriteToXmlFile(filePath);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Unable to save file: " + ex.Message);
                throw;
            }

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //throw new NotImplementedException();
        }
        #endregion

        #region Solution
        /// <summary>
        /// Code by the component
        /// </summary>
        /// <param name="DA"></param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //bool S = false;
            DA.GetData(0, ref _s);
            List<iDrawObject> m_DrawObjects = new List<iDrawObject>();
            Rectangle3d B = new Rectangle3d();

            if (_s) 
            {
                try
                {
                    if (_drawConduit != null) _drawConduit.Enabled = false;
                    DA.GetDataList(1, m_DrawObjects);
                    DA.GetData(2, ref B);

                    System.Drawing.Bitmap FontCheckBM = new System.Drawing.Bitmap(2000, 1000);
                    _fontSizer = System.Drawing.Graphics.FromImage(FontCheckBM);

                    //// checks if named styles exist in the dictionary,adds them if not and updates
                    //// withnew settings
                    foreach (iDrawObject thisDrawObject in m_DrawObjects)
                    {
                        try
                        { 
                        foreach (acStyle styleCheck in thisDrawObject.styles)
                        {
                            if (styleCheck.styleName != "")
                            {
                                if (!_styleSheet.Styles.ContainsKey(styleCheck.styleName))
                                {
                                    _styleSheet.Styles.Add(styleCheck.styleName, styleCheck);
                                }
                                else if (!styleCheck.unset)
                                {
                                    _styleSheet.Styles[styleCheck.styleName] = styleCheck;
                                }
                            }
                        }
                            }
                        catch (Exception ex) { MessageBox.Show("Not cycling through styles: " + ex.Message); }
                    }

                    // update each Draw Object from the style sheet
                    foreach (iDrawObject thisDrawObject in m_DrawObjects)
                    {
                        var setStyles = new List<acStyle>();
                        foreach (acStyle styleCheck in thisDrawObject.styles)
                        {
                            if (styleCheck.styleName != "")
                            {
                                setStyles.Add(_styleSheet.Styles[styleCheck.styleName]);
                            }
                            else
                            {
                                setStyles.Add(styleCheck); // will update using itself; works as placeholder for draw objects that rely on multiple styles
                            }
                        }
                        thisDrawObject.updateStyles(setStyles.ToArray());
                    }
                    // create the main drawing system
                    _drawConduit = new DrawMain(
                        new Interval(B.Corner(0).X, B.Corner(1).X),
                        new Interval(B.Corner(0).Y, B.Corner(3).Y),
                        m_DrawObjects, _fontSizer, _styleSheet, 
                        this.ViewportFilter);

                    _drawConduit.Enabled = true;

                    // feature to add to prevent style sheets
                    //_styleSheet.Compact(_DrawObjects);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                
            }
            else if (_drawConduit != null)
            {
                _drawConduit.Enabled = false;
            }

        }

        /// <summary>
        /// Manage enabling of the conduit based on component removal from the canvas
        /// </summary>
        /// <param name="document"></param>
        public override void RemovedFromDocument(GH_Document document)
        {
            try
            {
                _drawConduit.Enabled = false;
                base.RemovedFromDocument(document);
            }
            catch { }
        }

        /// <summary>
        /// Manage enabling of the conduit based on document events
        /// </summary>
        /// <param name="document"></param>
        /// <param name="context"></param>
        public override void DocumentContextChanged(GH_Document document, GH_DocumentContext context)
        {
            try
            {
                base.DocumentContextChanged(document, context);
                if (context == GH_DocumentContext.Close || context == GH_DocumentContext.Unloaded)
                {
                    _drawConduit.Enabled = false;
                }
                else if ((context == GH_DocumentContext.Open || context == GH_DocumentContext.Loaded) &&
                    _s && _drawConduit.Enabled == false && _drawConduit != null)
                {
                    _drawConduit.Enabled = true;
                }
            }
            catch { }
        }
        #endregion

        #region Local Utility

        public string OpenFile(string fname, string ext, string filt)
        {
            try
            {
                //configure open file dialog
                System.Windows.Forms.OpenFileDialog m_OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
                m_OpenFileDialog.FileName = fname;
                m_OpenFileDialog.DefaultExt = ext;
                m_OpenFileDialog.Filter = filt;

                //show save file dialog box
                System.Windows.Forms.DialogResult m_result = m_OpenFileDialog.ShowDialog();
                if (m_OpenFileDialog.FileName.ToString() != null)
                {
                    return m_OpenFileDialog.FileName.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }

        public string SaveFile(string fname, string ext, string filt)
        {
            try
            {
                //configure save file dialog
                System.Windows.Forms.SaveFileDialog m_SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
                m_SaveFileDialog.FileName = fname;
                m_SaveFileDialog.DefaultExt = ext;
                m_SaveFileDialog.Filter = filt;

                //show save file dialog box
                System.Windows.Forms.DialogResult m_result = m_SaveFileDialog.ShowDialog();
                if (m_SaveFileDialog.FileName.ToString() != null)
                {
                    return m_SaveFileDialog.FileName.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }

        }

#endregion

    }

    
}