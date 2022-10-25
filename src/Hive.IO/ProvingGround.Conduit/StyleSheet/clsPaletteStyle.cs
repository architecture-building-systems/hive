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
    [System.Xml.Serialization.XmlRootAttribute("PaletteStyle")]
    public class clsPaletteStyle : acStyle
    {

        [System.Xml.Serialization.XmlElementAttribute("StyleName")]
        public override string styleName { get; set; }

        [XmlIgnore]
        public override bool unset { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Seed")]
        public int seed { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("HueRange")]
        public Interval hueRange { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("SaturationRange")]
        public Interval saturationRange { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("lumincanceRange")]
        public Interval luminanceRange { get; set; }

        [System.Xml.Serialization.XmlArrayAttribute("Colors")]
        [System.Xml.Serialization.XmlArrayItemAttribute("Color")]
        public List<System.Drawing.Color> colors { get; set; }

        public clsPaletteStyle()
        { }

        public clsPaletteStyle(int Seed, Interval HueRange, Interval SaturationRange, Interval LuminanceRange, List<System.Drawing.Color> Colors)
        {
            seed = Seed;
            hueRange = HueRange;
            saturationRange = SaturationRange;
            luminanceRange = LuminanceRange;
            colors = Colors;
        }

        /// <summary>
        /// Creates a new color palette from base color list and HSL range settings
        /// </summary>
        /// <param name="numColors"></param>
        /// <returns></returns>
        public List<System.Drawing.Color> colorPalette(int numColors)
        {

            List<System.Drawing.Color> m_colorPalette = new List<System.Drawing.Color>(colors);


            Random m_random = new Random(seed);
            int m_colorCount = m_colorPalette.Count;

            // add a complementary color to the color list if there is only one color
            if (m_colorCount == 1)
            {
                Rhino.Display.ColorHSL m_complementary = new Rhino.Display.ColorHSL(colors[0]);
                m_complementary.H = (m_complementary.H + 0.5) % 1.0;

                m_colorPalette.Add(m_complementary.ToArgbColor());
                m_colorCount = 2;
            }

            for (int i = m_colorCount; i < numColors; i++)
            {

                System.Drawing.Color m_baseColor = m_colorPalette[i % m_colorCount];
                Rhino.Display.ColorHSL setHSL = new Rhino.Display.ColorHSL(m_baseColor);

                double m_hueAdjust = (setHSL.H + hueRange.ParameterAt(m_random.NextDouble())) % 1.0;

                setHSL.H = m_hueAdjust < 0 ? 1 + m_hueAdjust : m_hueAdjust;
                setHSL.S = RhinoMath.Clamp(setHSL.S + saturationRange.ParameterAt(m_random.NextDouble()), 0.0, 1.0);
                setHSL.L = RhinoMath.Clamp(setHSL.L + luminanceRange.ParameterAt(m_random.NextDouble()), 0.0, 1.0);

                m_colorPalette.Add(setHSL.ToArgbColor());

            }

            return m_colorPalette;

        }

        public static List<clsPaletteStyle> defaults()
        {
            List<clsPaletteStyle> setDefaults = new List<clsPaletteStyle>();

            setDefaults.Add(new clsPaletteStyle()
            {
                styleName = "Default Palette",
                unset = false,

                seed = 5,
                hueRange = new Interval(-0.1, 0.1),
                saturationRange = new Interval(-0.15, 0.15),
                luminanceRange = new Interval(-0.15, 0.15),
                colors = new List<System.Drawing.Color>() 
                {
                    System.Drawing.Color.FromArgb(217, 6, 167),
                    System.Drawing.Color.FromArgb(67, 193, 222),
                    System.Drawing.Color.FromArgb(255, 241, 255)
                }
            });

            return setDefaults;
        }

    }
}
