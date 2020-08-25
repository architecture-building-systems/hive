using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hive.IO;
using Hive.IO.Building;
using Hive.IO.Forms;
using Hive.IO.GHComponents;

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

            var state = new BuildingInputState(Sia2024Record.All().Take(20).Last() as Sia2024RecordEx, true);
            //if (state.SiaRoom != null)
            //{
            //    state.SiaRoom.Quality = "<Custom>";
            //    state.SiaRoom.BuildingUseType = "<Custom>";
            //}

            new BuildingInput(state).ShowDialog();

            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BuildingInput());*/
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
