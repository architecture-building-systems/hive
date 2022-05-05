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
              "Hive Merger for an Air Source Heat Pump (ASHP) (<Hive.IO.EnergySystems.AirSourceHeatPump>). It merges all kinds of data together to update the ASHP with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Air Carrier", "Air", "Air energy carrier from weather file", GH_ParamAccess.item);
            pManager.AddGenericParameter("Electricity Carrier", "Electricity", "Electricity energy carrier, e.g. from an electricity grid.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heating Generated", "heatGenerated", "Generated heating energy time series (kWh). Either annual hourly( 8760), or monthly (12) time series.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Supply Temperature", "suppTemp", "Supply temperature time series of the heated water at the ASHP outlet. Used to calculate COP (Coefficient of Performance) of the ASHP.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("SimpleMode?", "simpleMode?", "Compute simple efficiency? If 'False', Eqt. (8c) from Ashouri et al (2013) is used (10.1016/j.energy.2013.06.053).", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("PessimisticCOP?", "PessimisticCOP?",
                "If simplemode=false, with this boolean you can decide to take the min COP between the simple and the Ashouri COP calculation. \n " +
                "Ashouri's COP seems sometimes very high, with COP in Zurich not going below 3, while the simple equation leads to extremely high COP at high ambient temperatires.",
                GH_ParamAccess.item, false);

            pManager.AddGenericParameter("Hive Air Source Heat Pump", "ASHP", "Hive Air Source Heat Pump (ASHP) (<Hive.IO.EnergySystems.AirSourceHeatPump>) that will be infused with information from above inputs.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Air Source Heat Pump", "ASHP", "Hive Air Source Heat Pump with infused data about consumed energy, operational schedule, etc.", GH_ParamAccess.item);
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

            bool simple = true;
            DA.GetData(4, ref simple);

            bool pessimisticCop = false;
            DA.GetData(5, ref pessimisticCop);

            AirSourceHeatPump ashp = null;
            DA.GetData(6, ref ashp);


            if (simple)
                ashp.SetInputOutputSimple(electricity, air, energyGenerated.ToArray(), supplyTemp.ToArray());
            else
                ashp.SetInputOutput(electricity, air, energyGenerated.ToArray(), supplyTemp.ToArray(), pessimisticCop);

            DA.SetData(0, ashp);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Mergerheatpump;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("c6279e15-1b9e-4403-aa42-f1e8c1b54d23");
    }
}