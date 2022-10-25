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
    /// Font Style
    /// </summary>
    [System.Xml.Serialization.XmlRootAttribute("FontStyle")]
    public class clsFontStyle : acStyle
    {

        [XmlIgnore]
        public override string styleName { get; set; }

        [XmlIgnore]
        public override bool unset { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("FontStyle")]
        public clsFontBasis fontBasis { get; set; }

        public clsFontStyle()
        { }

        public clsFontStyle(clsFontBasis FontBasis)
        {
            fontBasis = FontBasis;
        }

        public static List<clsFontStyle> defaults()
        {
            List<clsFontStyle> setDefaults = new List<clsFontStyle>();

            setDefaults.Add(new clsFontStyle()
            {

                styleName = "Default Font",
                unset = false,

                fontBasis = new clsFontBasis()
                {
                    AdaptiveHeight = true,
                    Bold = false,
                    Italic = false,
                    Color = System.Drawing.Color.Black,
                    FontFace = "Arial",
                    TextAlign = "Left",
                    TextHeight = 10,
                    TextSpacing = 1.15
                }
            });

            setDefaults.Add(new clsFontStyle()
            {

                styleName = "Default Chart Font",
                unset = false,

                fontBasis = new clsFontBasis()
                {
                    AdaptiveHeight = true,
                    Bold = false,
                    Italic = false,
                    Color = System.Drawing.Color.Black,
                    FontFace = "Arial",
                    TextAlign = "Center",
                    TextHeight = 10,
                    TextSpacing = 1.15
                }
            });

            //setDefaults.Add(new clsFontStyle()
            //{
            //    styleName = "Default Header Font", 
            //    unset = false,

            //    fontBasis = new clsFontBasis()
            //    {
            //        AdaptiveHeight = true,
            //        Bold = true,
            //        Italic = false,
            //        Color = System.Drawing.Color.Black,
            //        FontFace = "Arial",
            //        TextAlign = "Center",
            //        TextHeight = 12,
            //        TextSpacing = 1.15
            //    }
            //});

            //setDefaults.Add(new clsFontStyle()
            //{
            //    styleName = "Default Number Font",
            //    unset = false,

            //    fontBasis = new clsFontBasis()
            //    {
            //        AdaptiveHeight = true,
            //        Bold = false,
            //        Italic = false,
            //        Color = System.Drawing.Color.Black,
            //        FontFace = "Arial",
            //        TextAlign = "Right",
            //        TextHeight = 10,
            //        TextSpacing = 1.15
            //    }
            //});

            //setDefaults.Add(new clsFontStyle()
            //{
            //    styleName = "Default Axis Font",
            //    unset = false,

            //    fontBasis = new clsFontBasis()
            //    {
            //        AdaptiveHeight = true,
            //        Bold = false,
            //        Italic = false,
            //        Color = System.Drawing.Color.Black,
            //        FontFace = "Arial",
            //        TextAlign = "Center",
            //        TextHeight = 10,
            //        TextSpacing = 1.15
            //    }
            //});

            return setDefaults;
        }

    }
}
