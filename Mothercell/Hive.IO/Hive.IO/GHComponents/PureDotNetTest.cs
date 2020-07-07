using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;
using GHSolar;

namespace Hive.IO.GHComponents
{
    public class PureDotNetTest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PureDotNetTest class.
        /// </summary>
        public PureDotNetTest()
          : base("PureDotNetTest", "Nickname",
              "Description",
              "[hive]", "whaevva")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GHSolar.CResults", "GHSolar.CResults", "GHSolar.CResults", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.SurfaceBased", "Hive.IO.EnergySystems.SurfaceBased", "Hive.IO.EnergySystems.SurfaceBased", GH_ParamAccess.list);
            pManager.AddGenericParameter("Hive.IO.EnergySystems.Air", "Hive.IO.EnergySystems.Air", "Hive.IO.EnergySystems.Air", GH_ParamAccess.item);
            pManager.AddTextParameter("resolution", "resolution", "resolution", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.EnergySystems.SurfaceBased", "Hive.IO.EnergySystems.SurfaceBased", "Hive.IO.EnergySystems.SurfaceBased", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var solarResults = new List<CResults>();
            DA.GetDataList(0, solarResults);

            var srfTechList = new List<SurfaceBased>();
            DA.GetDataList(1, srfTechList);

            Air airCarrier = null;
            if(!DA.GetData(2, ref airCarrier)) return;

            string resolution = null;
            if (!DA.GetData(3, ref resolution)) return;


            List<SurfaceBased> srfTechInfused = new List<SurfaceBased>();
            int horizon = 8760;
            for(int i=0; i<srfTechList.Count; i++)
            {
                if (srfTechList[i].ToString() == "Hive.IO.EnergySystems.Photovoltaic") 
                {
                    Photovoltaic pv = (Photovoltaic)srfTechList[i];
                    pv.SetInputComputeOutput(solarResults[i].I_hourly, airCarrier);
                    srfTechInfused.Add(pv);
                }
            }

            DA.SetDataList(0, srfTechInfused);
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
            get { return new Guid("7e9117fb-f181-46a6-839f-7ad0043b814a"); }
        }
    }
}