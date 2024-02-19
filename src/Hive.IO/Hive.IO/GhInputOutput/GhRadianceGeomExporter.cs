using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.GUI.Gradient;
using Rhino.Geometry;
using Rhino.Display;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Shapes;
using Eto.Forms;

namespace Hive.IO.GhInputOutput
{
    public class GhRadianceGeomExporter : GH_Component
    {
        public GhRadianceGeomExporter()
          : base("Radiance Geometry Exporter", "RadianceGeometryExporter",
              "Export geometry of Breps to Radiance rad file",
              "[hive]", "IO")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Building Brep collection", "Buildings", "Collection of all building Breps", GH_ParamAccess.list);
            pManager.AddBrepParameter("Windows collection", "Windows", "Collection of all windows", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Vertex collection", "vertexes", "Collection of all vertexes", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string folder = @"C:\Radiance\bin\examples\";
            string fileName = "test.rad";
            string fullPath = folder + fileName;

            var breps = new List<Brep>();
            if (!DA.GetDataList(0, breps)) return;

            var windows = new List<Brep>();
            if (!DA.GetDataList(1, windows)) return;

            var modifierBldg = new List<string> { "void glow bldgOpaq", "0", "0", "4", "1\t1\t1", "0" };
            var modifierWndw = new List<string> { "void glow wndwOpaq", "0", "0", "4", "2\t2\t2", "0" };

            File.WriteAllLines(fullPath, modifierBldg);
            File.AppendAllLines(fullPath, modifierWndw);

            foreach (Brep brep in breps)
            {
                var counter = 0;

                foreach (BrepFace face in brep.Faces)
                {
                    var vertex_count = 0;
                    var vertexes = new List<string>();

                    foreach (Rhino.Geometry.Point vertice in face.ToBrep().Vertices)
                    {
                        var v_string = vertice.Location.ToString();
                        var sep_string = v_string.Replace(",", "    ");
                        vertexes.Add(sep_string);

                        vertex_count += 3;

                    }
                    var header = new List<string> { "bldgOpaq polygon bldgOpaq." + counter, "0", "0", vertex_count.ToString() };

                    counter++;

                    File.AppendAllLines(fullPath, header);
                    File.AppendAllLines(fullPath, vertexes);
                }
            }

            foreach (Brep surf in windows)
            {
                var counter = 0;
                var vertex_count = 0;
                var vertexes = new List<string>();

                foreach (Rhino.Geometry.Point vertice in surf.Vertices)
                {
                    var v_string = vertice.Location.ToString();
                    var sep_string = v_string.Replace(",", "    ");
                    vertexes.Add(sep_string);

                    vertex_count += 3;

                }
                var header = new List<string> { "wndwOpaq polygon wndwOpaq." + counter, "0", "0", vertex_count.ToString() };

                counter++;

                File.AppendAllLines(fullPath, header);
                File.AppendAllLines(fullPath, vertexes);
            }
        }

        //run eplus
        string exe = @"objview";
        string arguments = "examples/test.rad";
        //RunRadiance(exe, arguments);

        //DA.SetDataList(0, vertexes);



        internal static void RunRadiance(string FileName, string command)
        {
            string exe = FileName;
            Process P = new Process();
            //P.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            P.StartInfo.FileName = "cmd.exe";
            P.StartInfo.Arguments = @"/C dir";
            P.Start();
            P.WaitForExit();
        }



        //protected override Bitmap Icon => Properties.Resources.IOCore_VisualizerLossesGains;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bd10a68f-1dc2-4b42-b4bf-7d7435617ac2"); }
        }
    }
}
