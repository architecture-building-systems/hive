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
    [System.Xml.Serialization.XmlRootAttribute("PieStyle")]
    public class clsPieStyle : acStyle
    {
        [System.Xml.Serialization.XmlElementAttribute("StyleName")]
        public override string styleName { get; set; }

        [XmlIgnore]
        public override bool unset { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("InnerRad")]
        public double innerRadius { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("OuterRad")]
        public double outerRadius { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("LabelRad")]
        public double labelRadius { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("LabelFontFace")]
        public string labelFontFace { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("LabelRelativeHeight")]
        public double labelRelativeHeight { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("LabelColor")]
        public System.Drawing.Color labelColor { get; set; }

        public clsPieStyle()
        { }

        public static List<clsPieStyle> defaults()
        {
            List<clsPieStyle> setDefaults = new List<clsPieStyle>();

            setDefaults.Add(new clsPieStyle()
            {
                styleName = "Default Pie",
                unset = false,
                innerRadius = 0.3,
                outerRadius = 0.9,
                labelRadius = 0.5,
                labelColor = System.Drawing.Color.Black,
                labelFontFace = "Arial",
                labelRelativeHeight = 0.1
            });

            return setDefaults;
        }

    }
}
