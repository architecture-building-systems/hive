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
    [System.Xml.Serialization.XmlRootAttribute("AxisStyle")]
    public class clsAxisStyle : acStyle
    {
        [System.Xml.Serialization.XmlElementAttribute("StyleName")]
        public override string styleName { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CurveStyle")]
        public clsCurveBasis curveBasis { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("HasTickMarks")]
        public bool hasTickMarks { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("YAxis")]
        public bool yAxis { get; set; }

        [XmlIgnore]
        public override bool unset { get; set; }

        public clsAxisStyle()
        { }

        public clsAxisStyle(clsCurveBasis CurveBasis, bool HasTickMarks, bool YAxis)
        {
            curveBasis = CurveBasis;
            hasTickMarks = HasTickMarks;
            yAxis = YAxis;
        }

        public static List<clsAxisStyle> defaults()
        {
            List<clsAxisStyle> setDefaults = new List<clsAxisStyle>();

            return setDefaults;
        }

    }
}

