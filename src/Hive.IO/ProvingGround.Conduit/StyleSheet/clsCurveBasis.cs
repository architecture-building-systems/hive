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
    /// Basis for styling Curves or Lines
    /// </summary>
    [System.Xml.Serialization.XmlRootAttribute("CurveBasis")]
    public class clsCurveBasis
    {

        [System.Xml.Serialization.XmlElementAttribute("Thickness")]
        public int Thickness { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Color")]
        public System.Drawing.Color Color { get; set; }

        public clsCurveBasis()
        { }

    }
}
