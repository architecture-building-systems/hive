using System;
using System.IO;
using System.Reflection;
using Hive.IO.Building;
using Hive.IO.Forms;

namespace TestVisualizer
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += LoadRhinoLibraries;

            // ShowBuildingInputDialog();
            ShowEnergySystemsDialog();
        }

        private static void ShowEnergySystemsDialog()
        {
            var form = new EnergySystemsInput();
            form.ShowDialog();
        }

        private static void ShowBuildingInputDialog()
        {
            var state = new BuildingInputState(Sia2024Record.First(), null, true);
            new BuildingInput(state).ShowDialog();
        }


        private static Assembly LoadRhinoLibraries(object sender, ResolveEventArgs args)
        {
            var folderPath = @"C:\Program Files\Rhino 6\System";
            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            var assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}