using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ProvingGround.Conduit.Classes
{
    [Serializable]
    class clsChartStyleDynamic : INotifyPropertyChanged
    {

        public clsChartStyleDynamic(GH_Component owner)
        {
            _owner = owner;
            _categoryAxisColor = Color.Black;
            _valueAxisColor = Color.Black;
        }

        #region Private members for interface

        [NonSerialized]
        private GH_Component _owner;

        private string _chartType = "Column";

        private bool _chartTitle;

        private double _titleHeight;

        private double _titlePadding;

        private bool _categoryTitle;

        private double _categoryTitleHeight;

        private double _categoryPadding;

        private bool _categoryLabels;

        private double _categoryLabelSize;

        private double _categoryLabelRotation = 0;

        private double _categoryGroupSpacing;

        private double _valueSpacing;

        private bool _valueTitle;

        private double _valueTitleHeight;

        private double _valuePadding;

        private double _valueLabelSize;

        private double _valueLabelRotation = 0;

        private string _valueFormat = "0";

        private bool _hasTicks;

        private double _tickFrequency;

        private double _tickLength;

        private bool _hasLegend;

        private double _legendWidth;

        private bool _showCategoryAxis = true;

        private int _categoryAxisWeight = 1;

        private Color _categoryAxisColor = Color.Black;

        private bool _showValueAxis = false;

        private int _valueAxisWeight = 1;

        private Color _valueAxisColor = Color.Black;

        private bool _labelDataPoints;

        private double _dataPointHeight;

        private double _dataPointRotation;

        #endregion

        #region Public members for interface

        public string ChartType
        {
            get { return _chartType; }
            set
            {
                _chartType = value;
                NotifyPropertyChanged("ChartType");
                
            }
        }

        public bool ChartTitle
        {
            get { return _chartTitle; }
            set
            {
                _chartTitle = value;
                if (!TitleOverridden)
                {
                    TitleOverridden = true;
                }
                NotifyPropertyChanged("ChartTitle");
            }
        }

        public double TitleHeight
        {
            get { return _titleHeight; }
            set
            {
                _titleHeight = value;
                NotifyPropertyChanged("TitleHeight");
            }
        }

        public double TitlePadding
        {
            get { return _titlePadding; }
            set
            {
                _titlePadding = value;
                NotifyPropertyChanged("TitlePadding");
            }
        }

        public bool CategoryTitle
        {
            get { return _categoryTitle; }
            set
            {
                _categoryTitle = value;
                if (!CategoryOverridden)
                {
                    CategoryOverridden = true;
                }
                NotifyPropertyChanged("CategoryTitle");
            }
        }

        public double CategoryTitleHeight
        {
            get { return _categoryTitleHeight; }
            set
            {
                _categoryTitleHeight = value;
                NotifyPropertyChanged("CategoryTitleHeight");
            }
        }

        public double CategoryPadding
        {
            get { return _categoryPadding; }
            set
            {
                _categoryPadding = value;
                NotifyPropertyChanged("CategoryPadding");
            }
        }

        public bool CategoryLabels
        {
            get { return _categoryLabels; }
            set
            {
                _categoryLabels = value;
                if (!CategoryLabelsOverridden)
                {
                    CategoryLabelsOverridden = true;
                }
                NotifyPropertyChanged("CategoryLabels");
            }
        }

        public double CategoryLabelSize
        {
            get { return _categoryLabelSize; }
            set
            {
                _categoryLabelSize = value;
                NotifyPropertyChanged("CategoryLabelSize");
            }
        }

        public double CategoryLabelRotation
        {
            get { return _categoryLabelRotation; }
            set
            {
                _categoryLabelRotation = value;
                NotifyPropertyChanged("CategoryLabelRotation");
            }
        }

        public double CategoryGroupSpacing
        {
            get { return _categoryGroupSpacing; }
            set
            {
                _categoryGroupSpacing = value;
                NotifyPropertyChanged("CategoryGroupSpacing");
            }
        }

        public bool ValueTitle
        {
            get { return _valueTitle; }
            set
            {
                _valueTitle = value;
                NotifyPropertyChanged("ValueTitle");
                if (!ValueOverridden)
                {
                    ValueOverridden = true;
                }
            }
        }

        public double ValueTitleHeight
        {
            get { return _valueTitleHeight; }
            set
            {
                _valueTitleHeight = value;
                NotifyPropertyChanged("ValueTitleHeight");
            }
        }

        public double ValuePadding
        {
            get { return _valuePadding; }
            set
            {
                _valuePadding = value;
                NotifyPropertyChanged("ValuePadding");
            }
        }

        public double ValueLabelSize
        {
            get { return _valueLabelSize; }
            set
            {
                _valueLabelSize = value;
                NotifyPropertyChanged("ValueLabelSize");
            }
        }

        public double ValueLabelRotation
        {
            get { return _valueLabelRotation; }
            set
            {
                _valueLabelRotation = value;
                NotifyPropertyChanged("ValueLabelRotation");
            }
        }

        public double ValueSpacing
        {
            get { return _valueSpacing; }
            set
            {
                _valueSpacing = value;
                NotifyPropertyChanged("ValueSpacing");
            }

        }

        public string ValueFormat
        {
            get { return _valueFormat; }
            set
            {
                _valueFormat = value;
                NotifyPropertyChanged("ValueFormat");
            }
        }

        public bool HasTicks
        {
            get { return _hasTicks; }
            set
            {
                _hasTicks = value;
                NotifyPropertyChanged("HasTicks");
            }
        }

        public double TickFrequency
        {
            get { return _tickFrequency; }
            set
            {
                _tickFrequency = value;
                NotifyPropertyChanged("TickFrequency");
            }
        }

        public double TickLength
        {
            get { return _tickLength; }
            set
            {
                _tickLength = value;
                NotifyPropertyChanged("TickLength");
            }
        }

        public bool HasLegend
        {
            get { return _hasLegend; }
            set
            {
                _hasLegend = value;
                NotifyPropertyChanged("HasLegend");
            }
        }

        public double LegendWidth
        {
            get { return _legendWidth; }
            set
            {
                _legendWidth = value;
                NotifyPropertyChanged("LegendWidth");
            }
        }

        public bool ShowCategoryAxis
        {
            get { return _showCategoryAxis; }
            set
            {
                _showCategoryAxis = value;
                NotifyPropertyChanged("ShowCategoryAxis");
            }
        }

        public int CategoryAxisWeight
        {
            get { return _categoryAxisWeight; }
            set
            {
                _categoryAxisWeight = value;
                NotifyPropertyChanged("CategoryAxisWeight");
            }
        }

        public Color CategoryAxisColor
        {
            get { return _categoryAxisColor; }
            set
            {
                _categoryAxisColor = value;
                NotifyPropertyChanged("CategoryAxisColor");
            }
        }

        public bool ShowValueAxis
        {
            get { return _showValueAxis; }
            set
            {
                _showValueAxis = value;
                NotifyPropertyChanged("ShowValueAxis");
            }
        }

        public int ValueAxisWeight
        {
            get { return _valueAxisWeight; }
            set
            {
                _valueAxisWeight = value;
                NotifyPropertyChanged("ValueAxisWeight");
            }
        }

        public Color ValueAxisColor
        {
            get { return _valueAxisColor; }
            set
            {
                _valueAxisColor = value;
                NotifyPropertyChanged("ValueAxisColor");
            }
        }

        public bool LabelDataPoints
        {
            get { return _labelDataPoints; }
            set
            {
                _labelDataPoints = value;
                NotifyPropertyChanged("LabelDataPoints");
            }
        }

       public double DataPointHeight
        {
            get { return _dataPointHeight; }
            set
            {
                _dataPointHeight = value;
                NotifyPropertyChanged("DataPointHeight");
            }
        }

        public double DataPointRotation
        {
            get { return _dataPointRotation; }
            set
            {
                _dataPointRotation = value;
                NotifyPropertyChanged("DataPointRotation");
            }
        }

        #endregion

        #region Public members

        public bool TitleOverridden = false;
        public bool CategoryOverridden = false;
        public bool CategoryLabelsOverridden = false;
        public bool ValueOverridden = false;

        #endregion

        #region Public resources

        public List<string> ChartTypes
        {
            get {
                return new List<string>()
                {
                    "Line",
                    "Column",
                    "Bar"
                };
            }
        }

        public List<string> ValueFormats
        {
            get
            {
                return new List<string>()
                {
                    "0",
                    "0.0",
                    "0.00",
                    "0.000",
                    "#,0",
                    "#,0.0",
                    "#,0.00",
                    "#,0.000",
                    "$0",
                    "$0.00",
                    "$#,0",
                    "$#,0.00"
                };
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                _owner.ExpireSolution(true);
            }
        }

        #endregion

        #region Public methods

        public void SetOwner(GH_Component owner)
        {
            _owner = owner;
        }


        #endregion
    }

    public enum enumChartType
    {
        LineChart,
        ColumnChart,
        BarChart
    }

}
