using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Security.Policy;


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

            var matBldg = new List<string> { "void plastic bldgOpaq", "0", "0", "5", "1 0 0 0 0" };
            var matWndw = new List<string> { "void plastic wndw", "0", "0", "5 0 0.69 0.15  0 0" };
            var matGround = new List<string> { "void plastic ground", "0", "0", "5 1 1 0 0 0" };

            File.WriteAllLines(fullPath, matBldg);
            File.AppendAllLines(fullPath, matWndw);
            File.AppendAllLines(fullPath, matGround);

            var polygonCounter = 0;

            Vector3D up = new Vector3D(1, 1, 1);

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

                Point3d centerP = CalculateCenter(distinct_v);

                Point3d firstP = distinct_v[0];

                var angles_strings = new List<(string vertex, double angle)>();
                var angles = new List<(Point3d vertex, double angle)>();

                foreach (var v in distinct_v)
                {
                    var angle = AngleBetweenThreePoints(up, centerP, firstP, v);
                    angles_strings.Add((v.ToString().Replace(",", " "), angle));
                    angles.Add((v, angle));
                    Debug.Print(angle.ToString());
                    vertex_count += 3;
                }

                angles_strings.Sort((a, b) => a.angle.CompareTo(b.angle));
                angles.Sort((a, b) => a.angle.CompareTo(b.angle));

                var vertex_strings_list = angles_strings.Select(x => x.vertex).ToList();
                var vertex_list = angles.Select(x => x.vertex).ToList();

                //decide which ordering (counter/clockwise) of polygons results in a normal vector that points towards the camera,
                //so only that vertex list is written to the .rad file
                var result = PointOnWhichSide(centerP, camPosition, vertex_list);

                if (result < 0)
                {
                    WriteGeometryToRadFile("bldgOpaq", polygonCounter, vertex_count, vertex_strings_list, fullPath);

                    polygonCounter++;
                } else
                {
                    vertex_strings_list.Reverse();

                    WriteGeometryToRadFile("bldgOpaq", polygonCounter, vertex_count, vertex_strings_list, fullPath);

                    polygonCounter++;
                }
            }

            int windowCounter = 0;
            foreach (Brep surf in windows)
            {

                var vertex_count = 0;
                var vertexes = new List<Point3d>();
                var vertex_strings = new List<string>();

                foreach (Rhino.Geometry.Point vertice in surf.Vertices)
                {
                    var v_string = vertice.Location.ToString();
                    var sep_string = v_string.Replace(",", "    ");
                    vertex_strings.Add(sep_string);
                    vertexes.Add(vertice.Location);

                    vertex_count += 3;
                }

                WriteGeometryToRadFile("wndw", windowCounter, vertex_count, vertex_strings, fullPath);

                windowCounter++;

                vertex_strings.Reverse();

                WriteGeometryToRadFile("wndw", windowCounter, vertex_count, vertex_strings, fullPath);

                windowCounter++;
            }

            //rpict -vta -vh 180 -vv 180 -vp 0 0 2.5 -vd 0 1 0 -vu 0 0 1 -ab 0 -av 1 1 1 octree.oct | ra_tiff -b - output.tif
            //+ exampleFolder + "/sky_overcast.mat " + exampleFolder + "/sky.rad "

            string octreeCommand = @"oconv -f " + exampleFolder + "/sky_overcast.mat " + exampleFolder + "/sky.rad " + exampleFolder + "/test.rad > " + exampleFolder + "/octree.oct";
            string renderCommand = @"rpict -x 512 -y 512 -vta -vh 180 -vv 180 -vp " + camPosition.X + " " + camPosition.Y + " " + camPosition.Z + " -vd " + camDirection.X + " " + camDirection.Y + " " + camDirection.Z + " -vu 0 0 1 -ab 0 -av 1 1 1 examples/octree.oct | ra_tiff - examples/output.tif";

            RunRadiance(folder, octreeCommand, renderCommand);

            //DA.SetDataList(0, vertexes);
            DA.SetData(1, folder + exampleFolder + "\\output.tif");

        }

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

        public static double PointOnWhichSide(Point3d center, Point3d cam, List<Point3d> v)
        {
            Vector3d camVector = center - cam; //maybe the other way around?

            Vector3d refVector1 = v[1] - v[0];
            Vector3d refVector2 = v[2] - v[0];

            var normal = Vector3D.CrossProduct(new Vector3D(refVector1.X, refVector1.Y, refVector1.Z), new Vector3D(refVector2.X, refVector2.Y, refVector2.Z));
            var dot = Vector3D.DotProduct(normal, new Vector3D(camVector.X, camVector.Y, camVector.Z));

            return dot;
        }

        public static Point3d CalculateCenter(List<Point3d> v)
        {
            var len = v.Count;

            var centerX = v.Sum(x => x.X) / len;
            var centerY = v.Sum(x => x.Y) / len;
            var centerZ = v.Sum(x => x.Z) / len;

            Point3d centerP = new Point3d(centerX, centerY, centerZ);

            return centerP;
        }

        public static void WriteGeometryToRadFile(string name, int itemCount, int vertexCount, List<string> stringList, string path)
        {
            var header = new List<string> { name + " polygon " + name +"." + itemCount, "0", "0", (vertexCount).ToString() };

            File.AppendAllLines(path, header);
            File.AppendAllLines(path, stringList);
        }

        internal static void RunRadiance(string folder, string octree, string render)
        {
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo();
            cmdStartInfo.WorkingDirectory = folder;
            cmdStartInfo.EnvironmentVariables["RAYPATH"] = "C:\\Radiance\\lib";
            cmdStartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            //cmdStartInfo.RedirectStandardOutput = true;
            //cmdStartInfo.RedirectStandardError = true;
            cmdStartInfo.RedirectStandardInput = true;
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = true;

            Process cmdProcess = new Process();
            cmdProcess.StartInfo = cmdStartInfo;
            cmdProcess.EnableRaisingEvents = true;
            cmdProcess.Start();

            //cmdProcess.StandardInput.WriteLine("cd " + folder);
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
