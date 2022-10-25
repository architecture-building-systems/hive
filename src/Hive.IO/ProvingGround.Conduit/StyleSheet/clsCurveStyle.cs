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
    /// Curve Style
    /// </summary>
    [System.Xml.Serialization.XmlRootAttribute("CurveStyle")]
    public class clsCurveStyle : acStyle
    {

        [XmlIgnore]
        public override string styleName { get; set; }

        [XmlIgnore]
        public override bool unset { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CurveStyle")]
        public clsCurveBasis curveBasis { get; set; }

        public clsCurveStyle()
        { }

        public clsCurveStyle(clsCurveBasis CurveBasis)
        {
            curveBasis = CurveBasis;
        }

        public static List<clsCurveStyle> defaults()
        {
            List<clsCurveStyle> setDefaults = new List<clsCurveStyle>();

            setDefaults.Add(new clsCurveStyle()
            {

                styleName = "Default Curve",
                unset = false,

                curveBasis = new clsCurveBasis()
                {
                    Color = System.Drawing.Color.Black,
                    Thickness = 1
                }

            });

            setDefaults.Add(new clsCurveStyle()
            {

                styleName = "Default Axis",
                unset = false,

                curveBasis = new clsCurveBasis()
                {
                    Color = System.Drawing.Color.Black,
                    Thickness = 2
                }

            });

            return setDefaults;
        }

    }
}