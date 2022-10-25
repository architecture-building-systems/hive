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

namespace ProvingGround.Conduit.Classes
{
    /// <summary>
    /// Primary interface for drawing objects on the HUD
    /// </summary>
    public interface iDrawObject
    {

        // original XY bounds of content
        Interval objectX { get; set; }
        Interval objectY { get; set; }

        // set pixel depth of object
        double pixelDepth { get; set; }

        // remapped bounds of content
        double baseX { get; set; } // lower X bound ranging from 0.0 to 1.0 relative to drawing boundary
        double lengthX { get; set; } // length of X as % of drawing boundary width
        double baseY { get; set; } // lower Y bound ranging from 0.0 to 1.0 relative to drawing boundary
        double lengthY { get; set; } // length of Y as % of drawing boundary height

        // object style
        acStyle[] styles { get; set; }

        bool drawInForeground { get; set; } // called if this type of draw object is to be drawn in Foreground (e.g. text is on top of meshes)

        // function to update Style
        void updateStyles(acStyle[] Styles);

        // function for drawing the object to the screen
        void objDraw(Rhino.Display.DrawEventArgs e, Plane drawPlane, double unitPerPx, System.Drawing.Graphics fontCheck);

    }
}
