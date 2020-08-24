using System;
using System.Collections.Generic;
using System.Reflection;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhMergers
{
    public class GhSolarTech : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerPV class.
        /// </summary>
        public GhSolarTech()
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
            pManager.AddMatrixParameter("I_hourly", "I_hourly", "I_hourly from GHSolar_CResults as Rhino.Geometry.Matrix. Solar potential results from GHSolar (https://github.com/christophwaibel/GH_Solar_V2) yearly hourly simulations", GH_ParamAccess.list);
            pManager.AddGenericParameter("SolarTech", "SolarTech", "Hive.IO.EnergySystems.SurfaceBasedTech, such as PV, PVT, Solar Thermal, Ground Collector", GH_ParamAccess.list);
            pManager.AddGenericParameter("Air", "Air", "Hive.IO.EnergySystems.Air carrier, containing ambient air temperature", GH_ParamAccess.item);
            pManager.AddNumberParameter("SupplyTemp", "SupplyTemp", "SupplyTemp from Hive.IO.EnergySystems.Emitter", GH_ParamAccess.list);
            pManager.AddNumberParameter("ReturnTemp", "ReturnTemp", "ReturnTemp from Hive.IO.EnergySystems.Emitter", GH_ParamAccess.list);

            pManager.AddBooleanParameter("SimpleMode", "SimpleMode", "SimpleMode for efficiency calculations, i.e. not depending on ambient air temperature", GH_ParamAccess.item, false);
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
            //string assemblyPath = @"C:\Users\chwaibel\AppData\Roaming\Grasshopper\Libraries\GHSolar.gha";
            //var asm = Assembly.LoadFrom(assemblyPath);
            ////Type [] types = asm.GetTypes();
            //Type type = asm.GetType("GHSolar.CResults");
            ////var instance = Activator.CreateInstance(type);

            //List<object> cResults = new List<object>();
            //DA.GetDataList(0, cResults);

            var irradHourly = new List<Matrix>();
            DA.GetDataList(0, irradHourly);

            var solarTech = new List<SurfaceBasedTech>();
            DA.GetDataList(1, solarTech);

            Air air = null;
            DA.GetData(2, ref air);

            var supTemp = new List<double>();
            DA.GetDataList(3, supTemp);

            var retTemp = new List<double>();
            DA.GetDataList(4, retTemp);

            bool simple = false;
            DA.GetData(5, ref simple);

            var solarTechInfused = new List<SurfaceBasedTech>();
            for(int i=0; i<solarTech.Count; i++)
            {
                if (solarTech[i] is Photovoltaic)
                {
                    var tech = solarTech[i] as Photovoltaic;
                    if (simple)
                    {
                        tech.SetInputComputeOutputSimple(irradHourly[i]);
                    }
                    else
                    {
                        tech.SetInputComputeOutput(irradHourly[i], air);
                    }
                    solarTechInfused.Add(tech);
                }
                else if (solarTech[i] is SolarThermal)
                {
                    var tech = solarTech[i] as SolarThermal;
                    if (simple)
                    {
                        tech.SetInputComputeOutputSimple(irradHourly[i]);
                    }
                    else
                    {
                        tech.SetInputComputeOutput(irradHourly[i], retTemp.ToArray(), supTemp.ToArray(), air);
                    }
                    solarTechInfused.Add(tech);
                }
            }


            DA.SetDataList(0, solarTechInfused);
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