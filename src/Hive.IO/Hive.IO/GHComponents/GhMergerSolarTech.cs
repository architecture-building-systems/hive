using System;
using System.Collections.Generic;
using System.Reflection;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Hive.IO.GHComponents
{
    public class GhMergerSolarTech : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerPV class.
        /// </summary>
        public GhMergerSolarTech()
          : base("Merger SolarTech Hive", "HiveMergerSolarTech",
              "Description",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.quinary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GHSolarResults", "GHSolarResults", "Solar potential results from GHSolar (https://github.com/christophwaibel/GH_Solar_V2) yearly hourly simulations", GH_ParamAccess.list);
            //pManager.AddGenericParameter("SolarTech", "SolarTech", "Hive.IO.EnergySystems.SurfaceBasedTech, such as PV, PVT, Solar Thermal, Ground Collector", GH_ParamAccess.list);
            //pManager.AddGenericParameter("Air", "Air", "Hive.IO.EnergySystems.Air carrier, containing ambient air temperature", GH_ParamAccess.item);
            //pManager.AddGenericParameter("Emitter", "Emitter", "Hive.IO.EnergySystems.Emitter, containing information about return water temperature (for thermal technologies ST, PVT and GC)", GH_ParamAccess.item);
            //pManager.AddTextParameter("TimeResolution", "TimeResolution", "Time resolution, either 'hourly', or 'monthly'. Default is 'monthly'", GH_ParamAccess.item, "monthly");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SolarTech", "SolarTech", "Hive.IO.EnergySystems.SurfaceBasedTech (PV, PVT, ST, GC), infused with output energy", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string assemblyPath = @"C:\Users\chwaibel\AppData\Roaming\Grasshopper\Libraries\GHSolar.gha";
            var asm = Assembly.LoadFrom(assemblyPath);
            //Type [] types = asm.GetTypes();
            Type type = asm.GetType("GHSolar.CResults");
            //var instance = Activator.CreateInstance(type);

            List<object> cResults = new List<object>();
            DA.GetDataList(0, cResults);


            //GHSolar.CResults result1 = (GHSolar.CResults)cResults[0];

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bffaaabf-85f2-4374-b593-ca11f25a6a11"); }
        }
    }
}