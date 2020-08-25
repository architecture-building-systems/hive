using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Hive.IO.Building;
using Hive.IO.Forms;

namespace TestVisualizer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadRhinoDlls);

            var state = new BuildingInputState(Sia2024Record.First(), null, true);
            new BuildingInput(state).ShowDialog();
        }
        

        static Assembly LoadRhinoDlls(object sender, ResolveEventArgs args)
        {
            string folderPath = @"C:\Program Files\Rhino 6\System";
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}
