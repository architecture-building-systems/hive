using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;

using Rhino;
using Rhino.Collections;
using Rhino.Geometry;

using ProvingGround.Conduit.Classes;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProvingGround.Conduit.Utils
{
    /// <summary>
    /// Static helper class for performing routine ops
    /// </summary>
    public class clsUtility
    {
        /// <summary>
        /// Locates a point on the current viewport for a drawing object
        /// </summary>
        /// <param name="xPos">absolute x position in the drawing space</param>
        /// <param name="yPos">absolute y position in the drawing space</param>
        /// <param name="depth">target depth to draw point (usually uniPerPx * -1)</param>
        /// <param name="drawPlane">the drawing plane, from the lower left corner of the viewport</param>
        /// <param name="unitPerPx">number of model units per pixel along the viewport rectangle</param>
        /// <param name="objectSource">the iDrawObject</param>
        /// <param name="viewWidth">viewport width in pixels</param>
        /// <param name="viewHeight">viewport height in pixels</param>
        /// <returns></returns>
        public static Point3d PointOnViewport(double xPos, double yPos, double depth, Plane drawPlane, double unitPerPx, iDrawObject objectSource, double viewWidth, double viewHeight)
        {
            xPos = (objectSource.baseX + (objectSource.objectX.NormalizedParameterAt(xPos) * objectSource.lengthX))
              * viewWidth * unitPerPx;
            yPos = (objectSource.baseY + (objectSource.objectY.NormalizedParameterAt(yPos) * objectSource.lengthY))
              * viewHeight * unitPerPx;

            return drawPlane.PointAt(xPos, yPos, depth);
        }

        public static Point3d PointOnViewportDepth(double xPos, double yPos, double zPos, double depth, Plane drawPlane, double unitPerPx, iDrawObject objectSource, double viewWidth, double viewHeight)
        {
            xPos = (objectSource.baseX + (objectSource.objectX.NormalizedParameterAt(xPos) * objectSource.lengthX))
              * viewWidth * unitPerPx;
            yPos = (objectSource.baseY + (objectSource.objectY.NormalizedParameterAt(yPos) * objectSource.lengthY))
              * viewHeight * unitPerPx;
            zPos = zPos * viewHeight * unitPerPx;

            return drawPlane.PointAt(xPos, yPos, zPos + depth);
        }

        /// <summary>
        /// Gets the width in pixels for a given string
        /// </summary>
        /// <param name="textToMeasure"></param>
        /// <param name="textHeight"></param>
        /// <returns></returns>
        public static double MeasureText(string textToMeasure, double textHeight, string fontFace, System.Drawing.Graphics FontCheck)
        {

            double thisValue = 0;

            try
            {
                System.Drawing.Font FontUse = new System.Drawing.Font(fontFace, (float)textHeight, System.Drawing.FontStyle.Regular);
                var CheckValue = FontCheck.MeasureString(textToMeasure, FontUse);
                float StringWidth = CheckValue.Width;
                thisValue = (double)StringWidth;
            }
            catch { }

            return thisValue;
        }
        
    }

    public static class ByteSerializer
    {
        public static byte[] Serializer(this object _object)
        {
            byte[] bytes;
            using (var _MemoryStream = new MemoryStream())
            {
                IFormatter _BinaryFormatter = new BinaryFormatter();
                _BinaryFormatter.Serialize(_MemoryStream, _object);
                bytes = _MemoryStream.ToArray();
            }
            return bytes;
        }

        public static T Deserializer<T>(this byte[] _byteArray)
        {
            T ReturnValue;
            using (var _MemoryStream = new MemoryStream(_byteArray))
            {
                IFormatter _BinaryFormatter = new BinaryFormatter();
                ReturnValue = (T)_BinaryFormatter.Deserialize(_MemoryStream);
            }
            return ReturnValue;
        }
    }
}
