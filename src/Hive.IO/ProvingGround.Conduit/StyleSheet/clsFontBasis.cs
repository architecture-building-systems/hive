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
    /// Basis for styling Text
    /// </summary>
    [System.Xml.Serialization.XmlRootAttribute("FontBasis")]
    public class clsFontBasis
    {

        [System.Xml.Serialization.XmlElementAttribute("Bold")]
        public bool Bold { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Italic")]
        public bool Italic { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("TextHeight")]
        public double TextHeight { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("AdaptiveHeight")]
        public bool AdaptiveHeight { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Spacing")]
        public double TextSpacing { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("FontFace")]
        public string FontFace { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Color")]
        public System.Drawing.Color Color { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("TextAlign")]
        public string TextAlign { get; set; }

        public clsFontBasis()
        { }

    }
}
