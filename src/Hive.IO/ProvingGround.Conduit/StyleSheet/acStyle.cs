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
    /// Abstract class for managing style types
    /// </summary>
    [DataContract]
    [KnownType(typeof(clsPaletteStyle))]
    [KnownType(typeof(clsFontStyle))]
    [KnownType(typeof(clsCurveStyle))]
    [KnownType(typeof(clsAxisStyle))]
    [KnownType(typeof(clsPieStyle))]
    [KnownType(typeof(clsChartStyle))]
    [System.Xml.Serialization.XmlRootAttribute("Style")]
    public abstract class acStyle
    {
        [System.Xml.Serialization.XmlElementAttribute("StyleName")]
        public abstract string styleName { get; set; }

        [XmlIgnore]
        public abstract bool unset { get; set; }

    }

    public class clsStyleGroup
    {
        public List<acStyle> styles { get; set; }

        public Dictionary<int, acStyle> dictStyle { get; set; }

        // Registers when a style has been referred to by 
        public Dictionary<int, string> dictName { get; set; }

        public clsStyleGroup()
        { }
    }



    /// <summary>
    /// Empty style
    /// </summary>
    //public class clsEmptyStyle : acStyle
    //{

    //    [System.Xml.Serialization.XmlElementAttribute("StyleName")]
    //    public override string styleName { get; set; }

    //    [XmlIgnore]
    //    public override bool unset { get; set; }

    //    // TO DO create an override for default

    //}


}
