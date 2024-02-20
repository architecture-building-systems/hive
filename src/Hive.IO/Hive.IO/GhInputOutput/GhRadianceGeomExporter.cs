﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Diagnostics;
using System.Windows;


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
            pManager.AddTextParameter("Radiance bin folder location", "Rad bin", "Location of the Radiance bin folder on disc", GH_ParamAccess.item);
            pManager.AddBrepParameter("Building Brep collection", "Buildings", "Collection of all building Breps", GH_ParamAccess.list);
            pManager.AddBrepParameter("Windows collection", "Windows", "Collection of all windows", GH_ParamAccess.list);
            pManager.AddPointParameter("Camera position", "Cam position", "Position of the camera viewpoint", GH_ParamAccess.item);
            pManager.AddVectorParameter("Camera direction", "Cam direction", "Direction of the camera view", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Vertex collection", "vertexes", "Collection of all vertexes", GH_ParamAccess.list);
            pManager.AddTextParameter("Output path", "outpath", "Path to rendered output image", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string folder = @"C:\Radiance\bin\";
            string exampleFolder = @"examples";
            string fileName = "test.rad";

            DA.GetData(0, ref folder);

            string fullPath = folder + exampleFolder + "\\" + fileName;

            var breps = new List<Brep>();
            if (!DA.GetDataList(1, breps)) return;

            var windows = new List<Brep>();
            if (!DA.GetDataList(2, windows)) return;

            Point3d camPosition = new Point3d();
            if (!DA.GetData(3, ref camPosition)) return;

            Vector3d camDirection = new Vector3d();
            if (!DA.GetData(4, ref camDirection)) return;

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

            int windowCounter = 0;
            foreach (Brep surf in windows)
            {
                
                var vertex_count = 0;
                var vertexes = new List<string>();

                foreach (Rhino.Geometry.Point vertice in surf.Vertices)
                {
                    var v_string = vertice.Location.ToString();
                    var sep_string = v_string.Replace(",", "    ");
                    vertexes.Add(sep_string);

                    vertex_count += 3;

                }
                var header = new List<string> { "wndwOpaq polygon wndwOpaq." + windowCounter, "0", "0", vertex_count.ToString() };

                windowCounter++;

                File.AppendAllLines(fullPath, header);
                File.AppendAllLines(fullPath, vertexes);
            }

            //rpict -vta -vh 180 -vv 180 -vp 0 0 2.5 -vd 0 1 0 -vu 0 0 1 -ab 0 -av 1 1 1 octree.oct | ra_tiff -b - output.tif

            string octreeCommand = @"oconv -f " + exampleFolder + "/test.rad > " + exampleFolder + "/octree.oct";
            string renderCommand = @"rpict -x 512 -y 512 -vta -vh 180 -vv 180 -vp " + camPosition.X + " " + camPosition.Y + " " + camPosition.Z + " -vd " + camDirection.X + " " + camDirection.Y + " " + camDirection.Z + " -vu 0 0 1 -ab 0 -av 1 1 1 examples/octree.oct | ra_tiff -b - examples/output.tif";

            RunRadiance(folder, octreeCommand, renderCommand);

            //DA.SetDataList(0, vertexes);
            DA.SetData(1, folder + exampleFolder + "\\output.tif");

        }

        internal static void RunRadiance(string folder, string octree, string render)
        {
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo();
            cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            cmdStartInfo.RedirectStandardOutput = true;
            cmdStartInfo.RedirectStandardError = true;
            cmdStartInfo.RedirectStandardInput = true;
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = true;

            Process cmdProcess = new Process();
            cmdProcess.StartInfo = cmdStartInfo;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();

            cmdProcess.StandardInput.WriteLine("cd " + folder);
            cmdProcess.StandardInput.WriteLine(octree);
            cmdProcess.StandardInput.WriteLine(render);
            cmdProcess.StandardInput.WriteLine("exit");

            cmdProcess.WaitForExit();
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
