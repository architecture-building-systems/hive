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
    [DataContract]
    public class clsStyleSheet
    {

        #region Public Members

        public clsStyleSheet() { }

        [DataMember]
        public Dictionary<string, acStyle> Styles { get; set; }

        public string XmlString()
        {

            var m_serializer = new DataContractSerializer(typeof(clsStyleSheet));

            string m_xmlString;
            using (var m_stringWriter = new StringWriter())
            {
                using (var m_writer = new XmlTextWriter(m_stringWriter))
                {
                    m_writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                    m_serializer.WriteObject(m_writer, this);
                    m_writer.Flush();
                    m_xmlString = m_stringWriter.ToString();
                }
            }

            return m_xmlString;

        }

        public void WriteToXmlFile(string filePath)
        {

            var m_streamWriter = new StreamWriter(filePath);
            m_streamWriter.Write(this.XmlString());
            m_streamWriter.Close();

        }

        public static clsStyleSheet ReadFromXmlFile(string filePath)
        {

            var m_fileStream = new FileStream(filePath, FileMode.Open);
            var m_deserializer = new DataContractSerializer(typeof(clsStyleSheet));

            clsStyleSheet m_sheetRead = (clsStyleSheet)m_deserializer.ReadObject(m_fileStream);
            m_fileStream.Close();

            return m_sheetRead;

        }

        public static clsStyleSheet ReadFromXmlString(string Xml)
        {

            using (MemoryStream m_memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Xml)))
            {
                XmlDictionaryReader m_reader = XmlDictionaryReader.CreateTextReader(m_memoryStream, Encoding.UTF8, new XmlDictionaryReaderQuotas(), null);
                var m_deserializer = new DataContractSerializer(typeof(clsStyleSheet));
                return (clsStyleSheet)m_deserializer.ReadObject(m_reader);
            }

        }

        #endregion

        #region Default Constructor

        public static Dictionary<string, acStyle> Defaults()
        {

            Dictionary<string, acStyle> m_defaultStyles = new Dictionary<string, acStyle>();

            // foreach (clsAxisStyle thisStyle in clsAxisStyle.defaults()) m_defaultStyles.Add(thisStyle.styleName, thisStyle);
            // foreach (clsChartStyle thisStyle in clsChartStyle.defaults()) m_defaultStyles.Add(thisStyle.styleName, thisStyle);
            foreach (clsCurveStyle thisStyle in clsCurveStyle.defaults()) m_defaultStyles.Add(thisStyle.styleName, thisStyle);
            foreach (clsFontStyle thisStyle in clsFontStyle.defaults()) m_defaultStyles.Add(thisStyle.styleName, thisStyle);
            foreach (clsPaletteStyle thisStyle in clsPaletteStyle.defaults()) m_defaultStyles.Add(thisStyle.styleName, thisStyle);
            foreach (clsPieStyle thisStyle in clsPieStyle.defaults()) m_defaultStyles.Add(thisStyle.styleName, thisStyle);

            return m_defaultStyles;
        }

        public void EnsureDefaults()
        {

            Dictionary<string, acStyle> m_getDefaults = clsStyleSheet.Defaults();

            foreach (string m_defaultKey in m_getDefaults.Keys)
            {
                if (!this.Styles.ContainsKey(m_defaultKey))
                {
                    this.Styles.Add(m_defaultKey, m_getDefaults[m_defaultKey]);
                }
            }

        }

        public void Compact(List<iDrawObject> DrawObjects)
        {

            List<string> m_keep = new List<string>();

            foreach (iDrawObject m_checkObject in DrawObjects)
            {
                foreach (acStyle m_checkStyle in m_checkObject.styles)
                {
                    if(m_keep.Contains(m_checkStyle.styleName) && m_checkStyle.styleName != "")
                    {
                        m_keep.Add(m_checkStyle.styleName);
                    }
                }
            }

            List<string> m_existingKeys = this.Styles.Keys.ToList();

            foreach (string m_existingKey in m_existingKeys)
            {
                if (!m_keep.Contains(m_existingKey)) this.Styles.Remove(m_existingKey);
            }

            this.EnsureDefaults();

        }

        #endregion

    }
}
