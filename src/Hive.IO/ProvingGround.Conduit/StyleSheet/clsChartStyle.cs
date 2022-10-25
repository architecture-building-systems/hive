using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;

using Rhino;
using Rhino.Collections;
using Rhino.Geometry;

using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace ProvingGround.Conduit.Classes
{
    /// <summary>
    /// Axis Settings
    /// </summary>
    [System.Xml.Serialization.XmlRootAttribute("ChartStyle")]
    public class clsChartStyle : acStyle
    {
        [System.Xml.Serialization.XmlElementAttribute("StyleName")]
        public override string styleName { get; set; }

        [XmlIgnore]
        public override bool unset { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CategoryChart")]
        public bool CategoryChart { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("FontFamily")]
        public string FontFamily { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("XSplit")]
        public double XSplit { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("YSplit")]
        public double YSplit { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("InteriorPadding")]
        public double InteriorPadding { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("ExteriorPadding")]
        public double ExteriorPadding { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("ChartPadding")]
        public double ChartPadding { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("DrawCatAxis")]
        public bool DrawCatAxis { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CatAxisBasis")]
        public clsCurveBasis CatAxisBasis { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("DrawValAxis")]
        public bool DrawValAxis { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("DrawValAxis")]
        public double ValAxisPadding { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("ValAxisPadding")]
        public clsCurveBasis ValAxisBasis { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("DrawCatGrid")]
        public bool DrawCatGrid { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CatGridBasis")]
        public clsCurveBasis CatGridBasis { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CatPadding")]
        public double CategoryAxisPadding { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CatLabels")]
        public double CategoryLabelSize { get; set; }

        public clsChartStyle()
        { }

        public static List<clsChartStyle> defaults()
        {
            List<clsChartStyle> setDefaults = new List<clsChartStyle>();

            setDefaults.Add(new clsChartStyle()
                {
                    styleName = "Default Chart",
                    unset = false,
                    CategoryChart = true,

                    FontFamily = "Arial",

                    InteriorPadding = 0.02,
                    ExteriorPadding = 0.02,
                    ChartPadding = 0.015,

                    XSplit = 0.25,
                    YSplit = 0.25,

                    CategoryLabelSize = 0.25,
                    CategoryAxisPadding = 0.25,
                    DrawCatAxis = true,
                    CatAxisBasis = new clsCurveBasis()
                    {
                        Color = System.Drawing.Color.Black,
                        Thickness = 2
                    },

                    DrawCatGrid = true,
                    CatGridBasis = new clsCurveBasis()
                    {
                        Color = System.Drawing.Color.DarkGray,
                        Thickness = 1
                    },

                    ValAxisPadding = 0.25,
                    DrawValAxis = true,
                    ValAxisBasis = new clsCurveBasis()
                    {
                        Color = System.Drawing.Color.Black,
                        Thickness = 1
                    },
                });

            return setDefaults;
        }

    }
}

