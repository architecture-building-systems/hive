using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper;
using Grasshopper.Kernel;
using GH_IO;
using System.Windows.Forms;
using System.Drawing;

using Rhino;
using Rhino.Geometry;

using ProvingGround.Conduit.UI;
using ProvingGround.Conduit.Classes;
using ProvingGround.Conduit.Utils;

using System.Windows.Interop;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace ProvingGround.Conduit.Components
{
    public class nodeDrawChartHive : GH_Component
    {

        #region Private members

        private clsChartStyleDynamic _chartStyle;
        private System.Drawing.Bitmap _fontCheckBM = new System.Drawing.Bitmap(2000, 1000);
        private System.Drawing.Graphics _fontSizer;

        private double _textSizeAdjuster = 0.7;

        #endregion

        #region Register node

        /// <summary>
        /// Initializes a new instance of the <see cref="nodeDrawChart"/> class.
        /// </summary>
        /// <param name="name">The name of this component. Keep it short, single words are best.</param>
        /// <param name="nickname">The abbreviation of this component. Keep it short, 1~5 character words are best.</param>
        /// <param name="description">The description of this component. Be succinct but clear. You can supply whole sentences.</param>
        /// <param name="category">The category of this component. The category controls in which tab the component will end up.</param>
        /// <param name="subCategory">the SubCategory for this component. The SubCategory controls in which panel the component will end up.</param>
        public nodeDrawChartHive() 
            : base("Hive Conduit Chart", "HiveConduitChart", "Conduit Chart Component with customized functionality for Hive", "[hive]", "misc")
        {
            _chartStyle = new clsChartStyleDynamic(this);
            _chartStyle.ChartType = "Column";
            _chartStyle.ValueFormat = "0";
        }

        /// <summary>
        /// Returns a consistent ID for this object type. Every object must supply a unique and unchanging
        /// ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get
            {
                return new Guid("ca71862b-3564-48bd-b0b8-ac69249d78c1");
            }
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// The default is to expose everywhere.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }

        #endregion

        #region Menu

        /// <summary>
        /// Appends the additional menu items.
        /// </summary>
        /// <param name="menu">The menu.</param>
        //public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        //{
        //    Menu_AppendItem(menu, "Chart Setup", ChartSetup);
        //}

        public override bool AppendMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Chart Setup", ChartSetup);
            return true;
        }

        private void ChartSetup(object sender, EventArgs e)
        {

            formChartStyle m_chartStyle = new formChartStyle(_chartStyle.CategoryAxisColor, _chartStyle.ValueAxisColor);
            m_chartStyle.DataContext = _chartStyle;
            WindowInteropHelper ownerHelper = new WindowInteropHelper(m_chartStyle);
            
            ownerHelper.Owner = Grasshopper.Instances.DocumentEditor.Handle;

            m_chartStyle.Show();
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetByteArray("ChartStyle", ByteSerializer.Serializer(_chartStyle));
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            try
            {
                _chartStyle = ByteSerializer.Deserializer<clsChartStyleDynamic>(reader.GetByteArray("ChartStyle"));
                _chartStyle.SetOwner(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return base.Read(reader);
        }

        #endregion

        #region Inputs/Outputs

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {

            pManager.AddRectangleParameter("Bounds", "Bounds", "Bounding rectangle for chart object", GH_ParamAccess.item);
            pManager.AddNumberParameter("Values", "Values", "DataTree of chart values (each branch reflects a different period)", GH_ParamAccess.tree);

            pManager.AddTextParameter("Categories", "Categories", "Optional list of categories (should be equal to length of lists in each branch)", GH_ParamAccess.list);
            pManager.AddTextParameter("Periods", "Periods", "Optional list of group values for time/sequence-driven data (e.g. months, quarters; if used, should be equal to branch count)", GH_ParamAccess.list);
            pManager.AddTextParameter("Title", "Title", "Optional chart title", GH_ParamAccess.item);
            pManager.AddTextParameter("Category Title", "CatTitle", "Optional title for category axis", GH_ParamAccess.item);
            pManager.AddTextParameter("Value Title", "ValTitle", "Optional title for value axis", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Range", "Range", "Optional domain describing min/max values for value axis (leaving empty will have bounds auto-assigned", GH_ParamAccess.item);
            pManager.AddGenericParameter("Color Palette", "Palette", "Optional color palette to use for chart (otherwise default is used)", GH_ParamAccess.item); // opt 11
            pManager.AddGenericParameter("Font", "Font", "Optional font style to use for chart (otherwise default is used)", GH_ParamAccess.item); // opt 11
            pManager.AddGenericParameter("Chart Settings", "Settings", "Optional chart settings created in setup of alternate chart component", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Tick Amount", "Tick Amount", "Optional fixed number of y-axis tick marks, overrides all tick frequency inputs", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tick Frequency", "Tick Frequency", "Optional input to set the tick frequency, overrides tick frequency from chart setup menu.", GH_ParamAccess.item);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;
            pManager[10].Optional = true;
            pManager[11].Optional = true;
            pManager[12].Optional = true;

        }

        /// <summary>
        /// Registers the output parameters.
        /// </summary>
        /// <param name="pManager">The p manager.</param>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("HUD Drawing Objects", "DrawObjs", "The drawing objects that comprise the chart", GH_ParamAccess.list);
            pManager.AddGenericParameter("Chart Settings", "Settings", "Settings to pass to other chart objects", GH_ParamAccess.item);
        }

        #endregion

        #region Solution

        /// <summary>
        /// Solves the instance.
        /// </summary>
        /// <param name="DA">The da.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            base.DestroyIconCache();

            var testout = new List<Rectangle3d>();

            // Set the font sizer
            _fontSizer = Graphics.FromImage(_fontCheckBM);

            // Input variables
            Rectangle3d m_bounds = new Rectangle3d();
            GH_Structure<GH_Number> m_values = new GH_Structure<GH_Number>();

            List<string> m_categories = new List<string>();
            List<string> m_periods = new List<string>();

            string m_title = string.Empty;
            string m_categoryTitle = string.Empty;
            string m_valueTitle = string.Empty;

            Interval m_valueRange = new Interval();

            

            // Get input variables
            DA.GetData(0, ref m_bounds);
            DA.GetDataTree(1, out m_values);

            DA.GetDataList(2, m_categories);
            DA.GetDataList(3, m_periods);

            DA.GetData(4, ref m_title);
            DA.GetData(5, ref m_categoryTitle);
            DA.GetData(6, ref m_valueTitle);
            DA.GetData(7, ref m_valueRange);

            clsPaletteStyle m_thisPalette = new clsPaletteStyle() { styleName = "Default Palette", unset = true };

            // Get a color palette if it's been connected
            if (m_thisPalette.unset)
            {

                object m_objPalette = new object();
                DA.GetData(8, ref m_objPalette);

                if (m_objPalette.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
                {
                    Grasshopper.Kernel.Types.GH_String m_paletteName = (Grasshopper.Kernel.Types.GH_String)m_objPalette;
                    if (m_paletteName.IsValid)
                    {
                        m_thisPalette = new clsPaletteStyle() { styleName = m_paletteName.Value, unset = true };
                    }
                }
                else
                {
                    try
                    {
                        DA.GetData(8, ref m_thisPalette);
                    }
                    catch { }
                }
            }

            clsFontStyle m_defaultFontStyle = new clsFontStyle() { styleName = "Default Chart Font", unset = true };
            if (m_defaultFontStyle.unset)
            {

                object m_objFont = new object();
                DA.GetData(9, ref m_objFont);

                if (m_objFont.GetType() == typeof(Grasshopper.Kernel.Types.GH_String))
                {
                    Grasshopper.Kernel.Types.GH_String m_fontName = (Grasshopper.Kernel.Types.GH_String)m_objFont;
                    if (m_fontName.IsValid)
                    {
                        m_defaultFontStyle = new clsFontStyle() { styleName = m_fontName.Value, unset = true };
                    }
                }
                else
                {
                    try
                    {
                        DA.GetData(9, ref m_defaultFontStyle);
                    }
                    catch { }
                }
            }

            clsChartStyleDynamic m_chartSetup = null;
            DA.GetData(10, ref m_chartSetup);

            if (m_chartSetup != null)
            {
                try
                { 
                    _chartStyle = m_chartSetup;
                }
                catch { _chartStyle = new clsChartStyleDynamic(this); }
            }

            // Assign initial chart styling based on inputs
            if (!string.IsNullOrEmpty(m_title) && !_chartStyle.TitleOverridden)
            {
                _chartStyle.ChartTitle = true;
                _chartStyle.TitleHeight = 0.125;
                _chartStyle.TitlePadding = 0.2;
            }

            if (!string.IsNullOrEmpty(m_categoryTitle) && !_chartStyle.CategoryOverridden)
            {
                _chartStyle.CategoryTitle = true;
                _chartStyle.CategoryTitleHeight = 0.09;
                _chartStyle.CategoryPadding = 0.2;
            }

            if (!string.IsNullOrEmpty(m_valueTitle) && !_chartStyle.ValueOverridden)
            {
                _chartStyle.ValueTitle = true;
                _chartStyle.ValueTitleHeight = 0.05;
                _chartStyle.ValuePadding = 0.2;
            }

            if ((m_categories.Count > 0 || m_periods.Count > 0) && !_chartStyle.CategoryLabelsOverridden)
            {
                _chartStyle.CategoryLabels = true;
                _chartStyle.CategoryLabelSize = 0.09;
                _chartStyle.CategoryLabelRotation = 0;
            }


            // Module variables     

            List<iDrawObject> m_chartObjects = new List<iDrawObject>();

            
            clsCurveStyle m_defaultCurveStyle = new clsCurveStyle() { styleName = "Default Curve", unset = true };

            // A category chart reflects one with multiple categories, but only one set of data (meant to be groups)
            bool m_isCategoryChart = (_chartStyle.CategoryLabels && m_values.PathCount == 1 && _chartStyle.ChartType != "Line");

            // Chart orientation swaps locations of value and category axes
            enumOrientation m_orientation = _chartStyle.ChartType == "Bar" ? enumOrientation.Vertical : enumOrientation.Horizontal;

            // Define value groupings for chart
            var m_valueGroups = new List<double>();
            var m_valueSet = new List<List<double>>();

            // Category group counts are equal to the main branch
            int m_valueGroupCount = m_isCategoryChart ?
                m_values.Branches[0].Count :
                m_values.PathCount;

            // Padding between value groups
            double valueGroupPad = _chartStyle.CategoryGroupSpacing / m_valueGroupCount;

            // Size of each value group
            double valueGroupSize = (1 - _chartStyle.CategoryGroupSpacing) / m_valueGroupCount;

            double m_min = m_values.Branches[0][0].Value;
            double m_max = m_values.Branches[0][0].Value;

            int m_branchCount = 0;

            if (m_isCategoryChart) // This is a category-based chart
            {
                m_valueSet.Add(new List<double>());

                for (int i = 0; i < m_values.Branches[0].Count; i++)
                {
                    if (i == 0) m_valueGroups.Add(valueGroupPad * 0.5);
                    m_valueGroups.Add(valueGroupSize);
                    m_valueGroups.Add(i == m_values.Branches[0].Count - 1 ? valueGroupPad * 0.5 : valueGroupPad);

                    double thisValue = m_values.Branches[0][i].Value;

                    m_valueSet[0].Add(thisValue);
                    m_min = Math.Min(thisValue, m_min);
                    m_max = Math.Max(thisValue, m_max);
                }
            }
            else
            {
                for (int i = 0; i < m_values.PathCount; i++)
                {
                    m_valueSet.Add(new List<double>());

                    if (i == 0) m_valueGroups.Add(valueGroupPad * 0.5);
                    m_valueGroups.Add(valueGroupSize);
                    m_valueGroups.Add(i == m_values.PathCount - 1 ? valueGroupPad * 0.5 : valueGroupPad);

                    m_branchCount = Math.Max(m_branchCount, m_values.Branches[i].Count);

                    for (int j = 0; j < m_values.Branches[i].Count; j++)
                    {
                        double thisValue = m_values.Branches[i][j].Value;

                        m_valueSet[i].Add(thisValue);
                        m_min = Math.Min(thisValue, m_min);
                        m_max = Math.Max(thisValue, m_max);
                    }
                }
            }

            if (m_valueRange.Length == 0)
            {
                m_valueRange.T0 = m_min;
                m_valueRange.T1 = m_max;
            }


            // Create the list of tick marks and sizing
            List<string> m_ticks = new List<string>();
            List<double> m_tickGroups = new List<double>();

            int tickNumber = 0;
            double tickFrequency = 0;
            DA.GetData(11, ref tickNumber);
            DA.GetData(12, ref tickFrequency);

            if (_chartStyle.HasTicks && m_valueRange.Length > 0 & _chartStyle.TickFrequency > 0)
            {
                int m_tickCount = 0;

                if (tickNumber > 0)
                {
                    m_tickCount = tickNumber;
                    _chartStyle.TickFrequency = m_valueRange.Length / m_tickCount;
                }
                else if (tickFrequency > 0)
                {
                    _chartStyle.TickFrequency = tickFrequency;
                    m_tickCount = Math.Max(1, (int)Math.Floor(m_valueRange.Length / _chartStyle.TickFrequency)) + 1;
                }
                else
                {
                    m_tickCount = Math.Max(1, (int)Math.Floor(m_valueRange.Length / _chartStyle.TickFrequency)) + 1;
                }

                for (int i = 0; i < m_tickCount; i++)
                {
                    string m_thisTick = (m_valueRange.T0 + i * _chartStyle.TickFrequency).ToString(_chartStyle.ValueFormat);
                    m_ticks.Add(m_thisTick);

                    if (i < m_tickCount - 1) m_tickGroups.Add(_chartStyle.TickFrequency);
                }

                m_tickGroups.Add(m_valueRange.Length % _chartStyle.TickFrequency);

            }

            // Get sizing for Axis groups
            double m_rawCategoryTitleSize = _chartStyle.CategoryTitle ? _chartStyle.CategoryTitleHeight : 0;
            double m_rawCategoryLabelSize = _chartStyle.CategoryLabels ? _chartStyle.CategoryLabelSize : 0;

            double m_adjustedCategoryLabelSize = _chartStyle.CategoryLabelSize;

            double m_adjustedCategorySize = AxisGroupSize(_chartStyle.CategoryTitle,
                _chartStyle.CategoryLabels,
                _chartStyle.CategoryTitleHeight,
                ref m_adjustedCategoryLabelSize,
                _chartStyle.CategoryLabelRotation,
                m_isCategoryChart ? m_categories : m_periods, _textSizeAdjuster);

            double m_adjustedValueLabelSize = _chartStyle.ValueLabelSize;

            double m_adjustedValueSize = AxisGroupSize(_chartStyle.ValueTitle,
                _chartStyle.HasTicks,
                _chartStyle.ValueTitleHeight,
                ref m_adjustedValueLabelSize,
                _chartStyle.ValueLabelRotation,
                m_ticks, _textSizeAdjuster);

            // Create a nine-square grid
            double[] m_gridX = new double[3];
            double[] m_gridY = new double[3];

            m_gridX[2] = _chartStyle.HasLegend ? _chartStyle.LegendWidth : 0;
            m_gridY[2] = _chartStyle.ChartTitle ? _chartStyle.TitleHeight : 0;

            switch (m_orientation)
            {
                case enumOrientation.Horizontal:
                    m_gridX[0] = m_adjustedValueSize;
                    m_gridY[0] = m_adjustedCategorySize;
                    break;
                case enumOrientation.Vertical:
                    m_gridX[0] = m_adjustedCategorySize;
                    m_gridY[0] = m_adjustedValueSize;
                    break;
                default:
                    break;
            }

            m_gridX[1] = 1 - (m_gridX[0] + m_gridX[2]);
            m_gridY[1] = 1 - (m_gridY[0] + m_gridY[2]);

            var m_tiles = clsTiler.IrregularTiles(m_bounds, m_gridX.ToList(), m_gridY.ToList(), 0, 0, 0);

            // The chart title
            if (_chartStyle.ChartTitle)
            {
                m_chartObjects.Add(TitleText(m_tiles[7], m_title, _chartStyle.TitlePadding, m_defaultFontStyle));
            }


            // The chart body
            Rectangle3d m_body = m_tiles[4];

            if (m_orientation == enumOrientation.Vertical)
            {
                m_body = new Rectangle3d(new Plane(m_body.Corner(3), -m_body.Plane.YAxis, m_body.Plane.XAxis), m_body.Height, m_body.Width);
            }

            // Set category axis bounds
            Rectangle3d m_categoryBounds = m_orientation == enumOrientation.Horizontal ?
                    m_tiles[1] :
                    FlipRectangle(m_tiles[3]);

            // Set value axis bounds
            Rectangle3d m_valueBounds = m_orientation == enumOrientation.Horizontal ?
                    FlipRectangle(m_tiles[3]) :
                    m_tiles[1];

            double m_categorySize = m_orientation == enumOrientation.Horizontal ?
                m_bounds.Height :
                m_bounds.Width;

            double m_valueSize = m_orientation == enumOrientation.Horizontal ?
                m_bounds.Width :
                m_bounds.Height;

            // Develop axes;
            var m_axisLines = new List<Line>();
            var m_axisColors = new List<Color>();
            var m_axisThicknesses = new List<int>();

            if (_chartStyle.ShowCategoryAxis)
            {
                m_axisLines.Add(new Line(m_body.Corner(0), m_body.Corner(1)));
                m_axisColors.Add(_chartStyle.CategoryAxisColor);
                m_axisThicknesses.Add(_chartStyle.CategoryAxisWeight);
            }

            if (_chartStyle.ShowValueAxis)
            {
                m_axisLines.Add(m_orientation == enumOrientation.Horizontal ? 
                    new Line(m_body.Corner(0), m_body.Corner(3)) :
                    new Line(m_body.Corner(1), m_body.Corner(2)));
                m_axisColors.Add(_chartStyle.ValueAxisColor);
                m_axisThicknesses.Add(_chartStyle.ValueAxisWeight);
            }

            // Develop category values

            if (_chartStyle.CategoryTitle || _chartStyle.CategoryLabels)
            {

                var m_categorySplit = new List<double>();

                m_categorySplit.Add(_chartStyle.CategoryTitle ? _chartStyle.CategoryTitleHeight / m_adjustedCategorySize : 0);
                m_categorySplit.Add(_chartStyle.CategoryLabels ? m_adjustedCategoryLabelSize / m_adjustedCategorySize : 0);

                var m_categoryTiles = clsTiler.VerticalTiles(m_categoryBounds, m_categorySplit, 1.0, 0.0, 0.0);

                if (_chartStyle.CategoryTitle)
                {
                    m_chartObjects.Add(TitleText(m_categoryTiles[0], m_categoryTitle, _chartStyle.CategoryPadding, m_defaultFontStyle));
                }

                if (_chartStyle.CategoryLabels)
                {

                    var m_labels = m_isCategoryChart ? m_categories : m_periods;

                    if (m_labels.Count > 0)
                    {
                        var m_labelTiles = clsTiler.HorizontalTiles(m_categoryTiles[1], m_valueGroups, 1.0, 0, 0);

                        for (int i = 0; i < Math.Min(m_labels.Count, m_valueGroupCount); i++)
                        {

                            Rectangle3d m_labelBase = m_labelTiles[1 + i * 2];
                            double m_textSize = _chartStyle.CategoryLabelSize * (m_orientation == enumOrientation.Horizontal ? m_bounds.Height : m_bounds.Width);
                            RotatedText(m_labels[i], m_labelBase, _chartStyle.CategoryLabelRotation, m_textSize, ref m_chartObjects, m_defaultFontStyle);
                        }

                    }
                }

            }

            if (_chartStyle.ValueTitle || _chartStyle.HasTicks)
            {

                var m_valueSplit = new List<double>();

                m_valueSplit.Add(_chartStyle.ValueTitle ? _chartStyle.ValueTitleHeight / m_adjustedValueSize : 0);
                m_valueSplit.Add(_chartStyle.HasTicks ? m_adjustedValueLabelSize / m_adjustedValueSize : 0);
                var m_valueTiles = clsTiler.VerticalTiles(m_valueBounds, m_valueSplit, 1.0, 0.0, 0.0);

                if (_chartStyle.ValueTitle)
                {
                    m_chartObjects.Add(TitleText(m_valueTiles[0], m_valueTitle, _chartStyle.ValuePadding, m_defaultFontStyle));
                }

                if (_chartStyle.HasTicks)
                {

                    if (m_ticks.Count > 0)
                    {

                        if (m_orientation == enumOrientation.Horizontal) m_ticks.Reverse();
                        if (m_orientation == enumOrientation.Horizontal) m_tickGroups.Reverse();

                        double m_adjustTickLength = _chartStyle.TickLength * (m_orientation == enumOrientation.Horizontal ? m_bounds.Width : m_bounds.Height);

                        var m_labelTiles = clsTiler.HorizontalTiles(m_valueTiles[1], m_tickGroups, 1.0, 0, 0);

                        var m_templateTile = m_orientation == enumOrientation.Horizontal ? m_labelTiles.Last() : m_labelTiles[0];
                        var m_mover = m_templateTile.Plane.XAxis * m_templateTile.Width / 2; //* (m_orientation == enumOrientation.Horizontal ? 1 : -1) ;

                        for (int i = 0; i < m_ticks.Count; i++)
                        {
                            Rectangle3d m_labelBase = m_orientation == enumOrientation.Horizontal ?
                                new Rectangle3d(
                                new Plane(m_labelTiles[i].Corner(1) - m_mover, m_templateTile.Plane.XAxis, m_templateTile.Plane.YAxis),
                                m_templateTile.Width,
                                m_templateTile.Height) :
                                new Rectangle3d(
                                new Plane(m_labelTiles[i].Corner(0) - m_mover, m_templateTile.Plane.XAxis, m_templateTile.Plane.YAxis),
                                m_templateTile.Width,
                                m_templateTile.Height);

                            double m_textSize = _chartStyle.ValueLabelSize * (m_orientation == enumOrientation.Horizontal ? m_bounds.Width : m_bounds.Height);
                            RotatedText(m_ticks[i], m_labelBase, _chartStyle.ValueLabelRotation, m_textSize, ref m_chartObjects, m_defaultFontStyle);
                            testout.Add(m_labelBase);

                            m_axisLines.Add(new Line(
                                m_orientation == enumOrientation.Horizontal ? m_labelTiles[i].Corner(3) : m_labelTiles[i].Corner(2),
                                m_templateTile.Plane.YAxis,
                                m_adjustTickLength));
                            m_axisThicknesses.Add(_chartStyle.ValueAxisWeight);
                            m_axisColors.Add(_chartStyle.ValueAxisColor);
                        }
                    }
                }

            }

            if (m_axisLines.Count > 0)
            {
                m_chartObjects.Add(new DrawLines(m_axisLines, m_axisThicknesses, m_axisColors, new acStyle[] { m_defaultCurveStyle }, 0.5));
            }


            var m_bodyTiles = clsTiler.HorizontalTiles(m_body, m_valueGroups, 1.0, 0, 0);

            if (_chartStyle.ChartType != "Line")
            {

                List<Mesh> m_chartMeshes = new List<Mesh>();

                if (!m_isCategoryChart)
                {
                    for (int i = 0; i < m_branchCount; i++)
                    {
                        m_chartMeshes.Add(new Mesh());
                    }
                }

                double m_dataPointLabelHeight = _chartStyle.DataPointHeight * m_bounds.Height;

                // Create a column chart from Meshes
                for (int i = 0; i < m_valueGroupCount; i++)
                {
                    Rectangle3d m_baseTile = m_bodyTiles[1 + i * 2];

                    if (m_isCategoryChart)
                    {
                        // A category chart (one branch for values, with different categories)

                        Mesh m_valueMesh = ValueAsMesh(m_baseTile, m_valueSet[0][i], m_valueRange, m_dataPointLabelHeight, ref m_chartObjects, m_defaultFontStyle, _chartStyle.DataPointRotation);
                        m_chartMeshes.Add(m_valueMesh);
                    }
                    else
                    {
                        // A period based chart, with multiple branches (multiple periods and multiple categories)
                        var m_groupDiv = new List<double>();

                        for (int j = 0; j < m_branchCount; j++)
                        {
                            m_groupDiv.Add(1.0);
                        }

                        var m_groupTiles = clsTiler.HorizontalTiles(m_baseTile, m_groupDiv, 1.0, 0, 0);

                        for (int j = 0; j < m_branchCount; j++)
                        {
                            double m_thisValue = m_valueSet[i].Count > j ?
                                m_valueSet[i][j] : 0;

                            Rectangle3d m_groupTile = m_groupTiles[j];

                            Mesh m_valueMesh = ValueAsMesh(m_groupTile, m_valueSet[i][j], m_valueRange, m_dataPointLabelHeight, ref m_chartObjects, m_defaultFontStyle, _chartStyle.DataPointRotation);
                            m_chartMeshes[j].Append(m_valueMesh);
                        }
                    }
                }

                m_chartObjects.Add(new DrawMeshGroup(m_chartMeshes, 1.0, m_thisPalette, false));
            }
            else
            {

                List<List<Line>> m_chartLines = new List<List<Line>>();
                List<List<Point3d>> m_chartPoints = new List<List<Point3d>>();

                if (!m_isCategoryChart)
                {
                    for (int i = 0; i < m_branchCount; i++)
                    {
                        m_chartLines.Add(new List<Line>());
                        m_chartPoints.Add(new List<Point3d>());
                    }
                }

                double m_dataPointLabelHeight = _chartStyle.DataPointHeight * m_bounds.Height;

                // Create a Line chart
                for (int i = 0; i < m_valueGroupCount; i++)
                {
                    Rectangle3d m_baseTile = m_bodyTiles[1 + i * 2];

                    if (m_isCategoryChart || m_valueSet.Count == 1)
                    {
                        // A category chart (one branch for values, with different categories)

                        // A period based chart, with multiple branches (multiple periods and multiple categories)
                        var m_groupDiv = new List<double>();

                        for (int j = 0; j < m_valueSet[0].Count; j++)
                        {
                            m_groupDiv.Add(1.0);
                        }

                        var m_groupTiles = clsTiler.HorizontalTiles(m_baseTile, m_groupDiv, 1.0, 0, 0);

                        for (int j = 0; j < m_valueSet[0].Count; j++)
                        {
                            Point3d m_valuePoint = ValueAsPoint(m_groupTiles[j], m_valueSet[0][j], m_valueRange);// ValueAsMesh(m_baseTile, m_valueSet[0][i], m_valueRange, m_dataPointLabelHeight, ref m_chartObjects, m_defaultFontStyle, _chartStyle.DataPointRotation);
                            m_chartPoints[0].Add(m_valuePoint);
                        }

                    }
                    else
                    {
                        
                        for (int j = 0; j < m_branchCount; j++)
                        {
                            double m_thisValue = m_valueSet[i].Count > j ?
                                m_valueSet[i][j] : 0;

                            Point3d m_valuePoint = ValueAsPoint(m_baseTile, m_valueSet[i][j], m_valueRange);
                            m_chartPoints[j].Add(m_valuePoint);
                        }
                    }
                }


                List<int> m_weights = new List<int>();

                for (int i = 0; i < m_chartPoints.Count; i++)
                {

                    m_weights.Add(1);

                    for (int j = 1; j < m_chartPoints[i].Count; j++)
                    {
                        m_chartLines[i].Add(new Line(m_chartPoints[i][j - 1], m_chartPoints[i][j]));
                    }
                }

                m_chartObjects.Add(new DrawLineGroup(m_chartLines, m_weights, new acStyle[] { m_thisPalette }));

            }

            //if (_chartStyle.HasLegend)
            //{

            //    var m_legend = m_tiles[5];

            //    double maxHeight = 0.02;

            //    var m_legendTiles

            //}


            // Add chart axes (relies on orientation of body)
            
            DA.SetDataList(0, m_chartObjects);
            DA.SetData(1, _chartStyle);

        }

        #endregion

        #region Private methods

        private double AxisGroupSize(bool hasTitle, bool hasLabels, double titleSize, ref double labelSize, double labelRotation, List<string> labels, double textSizeAdjuster)
        {

            double m_axisGroup = 0;

            if (hasTitle || hasLabels)
            {

                if (hasTitle)
                {
                    m_axisGroup += titleSize;
                }

                if (hasLabels)
                {
                    double m_maxLabelWidth = 0;

                    for (int i = 0; i < labels.Count; i++)
                    {
                        double m_thisWidth = clsUtility.MeasureText(labels[i], labelSize, "Arial", _fontSizer) * textSizeAdjuster;
                        m_maxLabelWidth = Math.Max(m_maxLabelWidth, m_thisWidth);
                    }

                    // Measure rotational change due to category label maximum width and existing label size

                    labelRotation = Math.Sin(labelRotation * Math.PI);
                    labelSize = labelRotation * m_maxLabelWidth + ((1 - labelRotation) * labelSize);

                    m_axisGroup += labelSize;
                }

            }

            return m_axisGroup;

        }

        private Rectangle3d FlipRectangle(Rectangle3d source)
        {
            return new Rectangle3d(new Plane(source.Corner(3), -source.Plane.YAxis, source.Plane.XAxis), source.Height, source.Width);
        }


        private Point3d ValueAsPoint(Rectangle3d groupTile, double thisValue, Interval valueRange)
        {

            double m_normalValue = Rhino.RhinoMath.Clamp((thisValue - valueRange.T0) / valueRange.Length, 0, 1);
            Rectangle3d m_valueTile = clsTiler.VerticalTiles(groupTile, new List<double>() { m_normalValue, 1 - m_normalValue }, 1.0, 0, 0)[0];

            return (m_valueTile.Corner(2) + m_valueTile.Corner(3)) * 0.5;
        }


        private Mesh ValueAsMesh(Rectangle3d groupTile, double thisValue, Interval valueRange, double dataPointLabelHeight, ref List<iDrawObject> chartObjects, clsFontStyle defaultFontStyle, double textRotation)
        {
            // Converts value into mesh using the chart domain
            Mesh m_valueMesh = new Mesh();

            double m_normalValue = Rhino.RhinoMath.Clamp((thisValue - valueRange.T0) / valueRange.Length, 0, 1);
            Rectangle3d m_valueTile = clsTiler.VerticalTiles(groupTile, new List<double>() { m_normalValue, 1 - m_normalValue }, 1.0, 0, 0)[0];

            m_valueMesh.Vertices.Add(m_valueTile.Corner(0));
            m_valueMesh.Vertices.Add(m_valueTile.Corner(1));
            m_valueMesh.Vertices.Add(m_valueTile.Corner(2));
            m_valueMesh.Vertices.Add(m_valueTile.Corner(3));
            m_valueMesh.Faces.AddFace(0, 1, 2, 3);

            // When specified, adds a data label to the mesh end point 
            if (_chartStyle.LabelDataPoints && _chartStyle.DataPointHeight > 0)
            {
                Plane m_origin = new Plane(m_valueTile.Corner(3), m_valueTile.Plane.XAxis, m_valueTile.Plane.YAxis);
                var m_labelString = thisValue.ToString(_chartStyle.ValueFormat);
                Rectangle3d m_labelTile = new Rectangle3d(m_origin, m_valueTile.Width, RotatedHeight(m_labelString, dataPointLabelHeight));
                RotatedText(m_labelString, m_labelTile, textRotation, dataPointLabelHeight, ref chartObjects, defaultFontStyle);
            }

            return m_valueMesh;

        }

        private double RotatedHeight(string text, double height)
        {
            return clsUtility.MeasureText(text, height, "Arial", _fontSizer);
        }        

        private void RotatedText(string labelText, Rectangle3d bounds, double rotation, double textSize, ref List<iDrawObject> chartObjects, clsFontStyle fontStyle)
        {

            double pctRotated = rotation / 0.5;
            double slide = (bounds.Width / 2) + (textSize / 2);

            Point3d labelBase = bounds.Corner(0) + (bounds.Plane.XAxis * pctRotated * slide);

            Vector3d newX = bounds.Plane.XAxis;
            newX.Rotate(rotation * Math.PI, Vector3d.ZAxis);
            Vector3d newY = new Vector3d(newX);
            newY.Rotate(Math.PI / 2, Vector3d.ZAxis);

            Plane labelOrigin = new Plane(labelBase, newX, newY);

            Interval adjustedWidthRange = new Interval(bounds.Width, bounds.Height);
            double newWidth = adjustedWidthRange.ParameterAt(pctRotated);

            Rectangle3d newLabel = new Rectangle3d(labelOrigin, newWidth, textSize);

            chartObjects.Add(TitleText(newLabel, labelText, 0, fontStyle));

        }

        private DrawText TitleText(Rectangle3d bounds, string text, double padding, clsFontStyle fontStyle)
        {

            var m_titleBoundary = clsTiler.VerticalTiles(bounds,new List<double>(){ padding, 1 - (padding * 2) , padding },1, 0, 0)[1];
            return new DrawText(new List<string>() { text }, m_titleBoundary, 0, 0, new List<Color>(), false, new acStyle[] { fontStyle });

        }

        #endregion
    }

    public enum enumOrientation
    {
        Horizontal,
        Vertical
    }
}
