using System;
using System.IO;
using System.Reflection;
using Hive.IO.Building;
using Hive.IO.EnergySystems;
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

            //ShowBuildingInputDialog();
            ShowEnergySystemsDialog();
        }

        private static void ShowEnergySystemsDialog()
        {
            var state = new EnergySystemsInputViewModel();
            var boiler = new ConversionTechPropertiesViewModel()
            {
                Name = "Boiler (Gas)",
            };
            boiler.SetProperties(new GasBoiler(20.0, 20.0, 20.0, 20.0, 20.0));
            state.ConversionTechnologies.Add(boiler);

            var radiator = new EmitterPropertiesViewModel()
            {
                Name = "Radiator",
            };
            radiator.SetProperties(new Radiator(12, 13, 20.0, true, false, 45, 50));
            state.Emitters.Add(radiator);


            var form = new EnergySystemsInputForm();
            form.ShowDialog(state);
        }

        private static void ShowBuildingInputDialog()
        {
            var state = new BuildingInputState(Sia2024Record.First(), null, true);
            var form = new BuildingInputForm();
            form.ShowDialog(state);
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