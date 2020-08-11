using System;
using Grasshopper.Kernel;
using Newtonsoft.Json;

namespace Hive.IO.GHComponents
{
    internal class EmitterProperties
    {
        public double SupplyTemperature;
        public double ReturnTemperature;
        public double Capacity;
        public double InvestmentCost;
        public double EmbodiedEmissions;
    }

    public class GhParametricInputEmitter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhEnergySystems class.
        /// </summary>
        public GhParametricInputEmitter()
          : base("Hive.IO.EnergySystems.Emitter", "EmitterInputs",
              "Heat/Cold Emitter Design Inputs (radiator, floor heating, ...).",
              "[hive]", "IO")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Vorlauf", "Vorlauf", "Supply temperature in deg C", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rücklauf", "Rücklauf", "Return temperature in deg C", GH_ParamAccess.item);
            pManager.AddNumberParameter("Capacity", "Capacity", "Maximal Heating power in kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("InvestmentCost", "InvestmentCost", "Investment cost CHF/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("EmbodiedEmissions", "EmbodiedEmissions", "Embodied emissions in kgCO2/kW", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EmitterJson", "EmitterJson", "Emitter json", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var emitter = new EmitterProperties();

            if(!DA.GetData(0, ref emitter.SupplyTemperature)) 
                emitter.SupplyTemperature = 70;
            if (!DA.GetData(1, ref emitter.ReturnTemperature)) 
                emitter.ReturnTemperature= 55;
            if (!DA.GetData(2, ref emitter.Capacity))
                emitter.Capacity = 1.0;
            if (!DA.GetData(3, ref emitter.InvestmentCost))
                emitter.InvestmentCost = 100.0;
            if (!DA.GetData(4, ref emitter.EmbodiedEmissions))
                emitter.EmbodiedEmissions = 100.0;

            DA.SetData(0, JsonConvert.SerializeObject(emitter));
            //DA.SetData(0, new Radiator(100.0, 100.0, true, false, inTemp, returnTemp));
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
            get { return new Guid("7766d14d-fa68-42d1-86ec-51d28892c265"); }
        }
    }
}