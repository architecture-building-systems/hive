using Grasshopper.Kernel;
using System;


namespace Hive.IO.GhParametricInputs
{
    internal class EmitterProperties
    {
        public double SupplyTemperature;
        public double ReturnTemperature;
        public double Capacity;
        public double InvestmentCost;
        public double EmbodiedEmissions;
        public string Name;
        public bool IsAir;
        public bool IsRadiation;
        public bool IsCooling;
        public bool IsHeating;
    }

    public class GhEmitter : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhEnergySystems class.
        /// </summary>
        public GhEmitter()
          : base("Parametric Input Emitter Hive", "HiveParaInEmitter",
              "Parametric inputs for Hive Emitter Systems for, e.g., optimization work flows or sensitivity analysis. This component will overwrite the settings in the Hive Energy Systems Input Component when connected.",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        
        
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
            pManager.AddTextParameter("Name", "Name", "Name of the emitter", GH_ParamAccess.item);
            pManager.AddBooleanParameter("AirEmitter?", "AirEmitter?", "AirEmitter? radiator by default", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Cooling?", "Cooling?", "Cooling? Heating by default", GH_ParamAccess.item, false);
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
            if (!DA.GetData(5, ref emitter.Name))
                emitter.Name = "ConventionalRadiator";
            if (!DA.GetData(6, ref emitter.IsAir))
                emitter.IsAir = false;
            if (emitter.IsAir) emitter.IsRadiation = false;
            else emitter.IsRadiation = true;
            if (!DA.GetData(7, ref emitter.IsCooling))
                emitter.IsCooling = false;
            if (emitter.IsCooling) emitter.IsHeating = false;
            else emitter.IsHeating = true;
            
            DA.SetData(0, emitter);
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