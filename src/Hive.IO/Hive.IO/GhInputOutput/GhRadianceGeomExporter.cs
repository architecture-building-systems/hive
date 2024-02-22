using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Media3D;


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
            var matPlastic = new List<string> { "void plastic matte_green", "0", "0", "5 .2 .6 .25 0 0" };

            File.WriteAllLines(fullPath, modifierBldg);
            File.AppendAllLines(fullPath, matPlastic);

            var polygonCounter = 0;
            //foreach (Brep brep in breps)
            //{

            //    foreach (BrepFace face in brep.Faces)
            //    {
            //        var vertex_count = 0;
            //        var vertexes = new List<string>();

            //        foreach (Rhino.Geometry.Point vertice in face.ToBrep().Vertices)
            //        {
            //            var v_string = vertice.Location.ToString();
            //            var sep_string = v_string.Replace(",", "    ");
            //            vertexes.Add(sep_string);

            //            vertex_count += 3;

            //        }
            //        var header = new List<string> { "bldgOpaq polygon bldgOpaq." + polygonCounter, "0", "0", vertex_count.ToString() };

            //        polygonCounter++;

            //        File.AppendAllLines(fullPath, header);
            //        File.AppendAllLines(fullPath, vertexes);
            //    }
            //}

            //foreach (Brep brep in breps)
            //{
            //    int vertex_count = 0;
            //    List<string> vertexes = new List<string>();

            //    BrepEdgeList BrepEdges = brep.Edges;

            //    for (int i = 0; i < BrepEdges.Count; i++)
            //    {
            //        var points = "";
            //        if (i == 0)
            //        {
            //            points = findConnectionPoints(BrepEdges, i, BrepEdges.Count - 1);
            //        }
            //        else
            //        {
            //            points = findConnectionPoints(BrepEdges, i - 1, i);
            //        }

            //        vertexes.Add(points);
            //        vertex_count += 3;
            //    }

            //    var header = new List<string> { "bldgOpaq polygon bldgOpaq." + polygonCounter, "0", "0", (vertex_count).ToString() };

            //    polygonCounter++;

            //    File.AppendAllLines(fullPath, header);
            //    File.AppendAllLines(fullPath, vertexes);

            //    var header2 = new List<string> { "bldgOpaq polygon bldgOpaq." + polygonCounter, "0", "0", (vertex_count).ToString() };

            //    polygonCounter++;

            //    vertexes.Reverse();

            //    File.AppendAllLines(fullPath, header2);
            //    File.AppendAllLines(fullPath, vertexes);

            //}

            foreach (Brep brep in breps)
            {
                int vertex_count = 0;
                var vertexes = new List<Point3d>();

                for (int i = 0; i < brep.Edges.Count; i++)
                {
                    var s1 = brep.Edges[i].StartVertex.Location;
                    var e1 = brep.Edges[i].EndVertex.Location;

                    vertexes.Add(e1);
                    vertexes.Add(s1);
                }

                List<Point3d> distinct_v = vertexes.Distinct().ToList();
                var len = distinct_v.Count;

                var centerX = distinct_v.Sum(x => x.X) / len;
                var centerY = distinct_v.Sum(x => x.Y) / len;
                var centerZ = distinct_v.Sum(x => x.Z) / len;

                Point3d centerP = new Point3d(centerX, centerY, centerZ);

                Point3d firstP = distinct_v[0];

                var up = new Vector3D(1, 1, 1);

                var angles = new List<(string vertex, double angle)>();

                foreach (var v in distinct_v)
                {
                    var angle = AngleBetweenThreePoints(up, centerP, firstP, v);
                    angles.Add((v.ToString().Replace(",", " "), angle));
                    Debug.Print(angle.ToString());
                    vertex_count += 3;
                }

                angles.Sort((a, b) => a.angle.CompareTo(b.angle));
                var vertex_list = angles.Select(x => x.vertex).ToList();

                var header = new List<string> { "bldgOpaq polygon bldgOpaq." + polygonCounter, "0", "0", (vertex_count).ToString() };

                polygonCounter++;

                File.AppendAllLines(fullPath, header);
                File.AppendAllLines(fullPath, vertex_list);

                var header2 = new List<string> { "bldgOpaq polygon bldgOpaq." + polygonCounter, "0", "0", (vertex_count).ToString() };

                polygonCounter++;

                vertex_list.Reverse();

                File.AppendAllLines(fullPath, header);
                File.AppendAllLines(fullPath, vertex_list);
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
                var header = new List<string> { "matte_green polygon matte_green." + windowCounter, "0", "0", vertex_count.ToString() };

                windowCounter++;

                File.AppendAllLines(fullPath, header);
                File.AppendAllLines(fullPath, vertexes);

                var header2 = new List<string> { "matte_green polygon matte_green." + windowCounter, "0", "0", vertex_count.ToString() };

                windowCounter++;

                vertexes.Reverse();

                File.AppendAllLines(fullPath, header);
                File.AppendAllLines(fullPath, vertexes);
            }

            //rpict -vta -vh 180 -vv 180 -vp 0 0 2.5 -vd 0 1 0 -vu 0 0 1 -ab 0 -av 1 1 1 octree.oct | ra_tiff -b - output.tif
            //+ exampleFolder + "/sky_overcast.mat " + exampleFolder + "/sky.rad "

            string octreeCommand = @"oconv -f " + exampleFolder + "/test.rad > " + exampleFolder + "/octree.oct";
            string renderCommand = @"rpict -x 512 -y 512 -vta -vh 180 -vv 180 -vp " + camPosition.X + " " + camPosition.Y + " " + camPosition.Z + " -vd " + camDirection.X + " " + camDirection.Y + " " + camDirection.Z + " -vu 0 0 1 -ab 0 -av 1 1 1 examples/octree.oct | ra_tiff -b - examples/output.tif";

            RunRadiance(folder, octreeCommand, renderCommand);

            //DA.SetDataList(0, vertexes);
            DA.SetData(1, folder + exampleFolder + "\\output.tif");

        }

        //internal static string findConnectionPoints(BrepEdgeList brepEdges, int i, int j)
        //{
        //    var s1 = brepEdges[i].StartVertex.Location;
        //    var e1 = brepEdges[i].EndVertex.Location;

        //    var s2 = brepEdges[j].StartVertex.Location;
        //    var e2 = brepEdges[j].EndVertex.Location;

        //    var v = new List<Point3d>() { s1, e1, s2, e2 };

        //    var connectionPoint = v.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();

        //    var result = "";

        //    if (connectionPoint.Count > 0) {
        //        result = connectionPoint[0].ToString().Replace(",", "   ");
        //    } else
        //    {
        //        result = "0 0 0";
        //    }

        //    return result;
        //}

        public static float AngleBetweenThreePoints(Vector3D up, Point3d centerP, Point3d firstP, Point3d currentP)
        {
            var centerp = new Point3D(centerP.X, centerP.Y, centerP.Z);
            var firstp = new Point3D(firstP.X, firstP.Y, firstP.Z);
            var currentp = new Point3D(currentP.X, currentP.Y, currentP.Z);

            var v1 = firstp - centerp;
            var v2 = currentp - centerp;

            var cross = Vector3D.CrossProduct(v1, v2);
            var dot = Vector3D.DotProduct(v1, v2);

            var angle = Math.Atan2(cross.Length, dot);

            var test = Vector3D.DotProduct(up, cross);
            if (test < 0.0) angle = -angle;
            return (float)angle;
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
