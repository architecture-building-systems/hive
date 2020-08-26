using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;


namespace Hive.IO.GhMergers
{
    public class GhHeatPump : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMergerHeatPump class.
        /// </summary>
        public GhHeatPump()
          : base("Merger AirSourceHeatPump Hive", "HiveMergerASHP",
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
            pManager.AddGenericParameter("Air", "Air", "Air energy carrier from weather file", GH_ParamAccess.item);
            pManager.AddGenericParameter("Electricity", "Electricity", "Electricity", GH_ParamAccess.item);
            pManager.AddNumberParameter("heatGenerated", "heatGenerated", "heatGenerated (kWh)", GH_ParamAccess.list);
            pManager.AddNumberParameter("suppTemp", "suppTemp", "suppTemp for water output. necessary to know for COP calculation", GH_ParamAccess.list);
            pManager.AddNumberParameter("returnTemp", "returnTemp", "returnTemp. required for simple COP", GH_ParamAccess.list);
            pManager.AddBooleanParameter("simpleMode?", "simpleMode?", "simpleMode?", GH_ParamAccess.item, true);

            pManager.AddGenericParameter("ASHP", "ASHP", "Hive.IO.EnergySystems.AirSourceHeatPump", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.AirSourceHeatPump", "AirSourceHeatPump", "AirSourceHeatPump", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Air air = null;
            DA.GetData(0, ref air);

            Electricity electricity = null;
            DA.GetData(1, ref electricity);

            var energyGenerated = new List<double>();
            DA.GetDataList(2, energyGenerated);

            var supplyTemp = new List<double>();
            DA.GetDataList(3, supplyTemp);

            var returnTemp = new List<double>();
            DA.GetDataList(4, returnTemp);

            bool simple = true;
            DA.GetData(5, ref simple);

            AirSourceHeatPump ashp = null;
            DA.GetData(6, ref ashp);


            if (simple)
                ashp.SetInputOutputSimple(electricity, energyGenerated.ToArray(), returnTemp.ToArray(), supplyTemp.ToArray());
            else
                ashp.SetInputOutput(electricity, air, energyGenerated.ToArray(), supplyTemp.ToArray());

            DA.SetData(0, ashp);
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
            get { return new Guid("c6279e15-1b9e-4403-aa42-f1e8c1b54d23"); }
        }
    }
}