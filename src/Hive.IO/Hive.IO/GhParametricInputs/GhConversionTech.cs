using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;


namespace Hive.IO.GhParametricInputs
{
    public class ConversionTechProperties
    {
        public double ASHPCapacity;
        public double ASHPCost;
        public double ASHPEmissions;
        public double ASHPEtaRef;

        public double GasBoilerCapacity;
        public double GasBoilerCost;
        public double GasBoilerEmissions;
        public double GasBoilerEfficiency;

        public double CHPCapacity;
        public double CHPCost;
        public double CHPEmissions;
        public double CHPHTP;  // heat to power ratio
        public double CHPEffElec;

        public double ChillerCapacity;
        public double ChillerCost;
        public double ChillerEmissions;
        public double ChillerEtaRef;

        public double HeatExchangerCapacity;
        public double HeatExchangerCost;
        public double HeatExchangerEmissions;
        public double HeatExchangerLosses;

        public double CoolExchangerCapacity;
        public double CoolExchangerCost;
        public double CoolExchangerEmissions;
        public double CoolExchangerLosses;
    }


    public class GhConversionTech : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhParametricInputEnergySystems class.
        /// </summary>
        public GhConversionTech()
          : base("Parametric Input EnergySystems Hive", "HiveParaInEnSys",
              "Parametric inputs for Hive Energy Systems for, e.g., optimization work flows or sensitivity analysis. This component will overwrite the settings in the Hive Energy Systems Input Component when connected.",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("ASHPCapacity", "ASHPCapacity", "Air source heat pump capacity, in kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("ASHPCost", "ASHPCost", "Air source heat pump cost, in CHF/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("ASHPEmissions", "ASHPEmissions", "Air source heat pump embodied green house gas emissions in kgCO2/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("ASHPEtaRef", "ASHPEtaRef", "Air source heat pump reference efficiency used for COP calculation", GH_ParamAccess.item);

            pManager.AddNumberParameter("NGBoilerCapacity", "NGBoilerCapacity", "Natural gas boiler capacity, in kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("NGBoilerCost", "NGBoilerCost", "Natural gas boiler cost, in CHF/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("NGBoilerEmissions", "NGBoilerEmissions", "Natural gas boiler embodied green house gas emissions, in kgCO2/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("NGBoilerEfficiency", "NGBoilerEfficiency", "Natural gas boiler reference efficiency [0.0, 1.0]", GH_ParamAccess.item);

            pManager.AddNumberParameter("CHPCapacity", "CHPCapacity", "Combined heat and power capacity, in kW (electric). Assumes gas as input carrier.", GH_ParamAccess.item);
            pManager.AddNumberParameter("CHPCost", "CHPCost", "Combined heat and power cost, in CHF/kW (electric)", GH_ParamAccess.item);
            pManager.AddNumberParameter("CHPEmissions", "CHPEmissions", "Combined heat and power embodied green house gas emissions, in kgCO2/kW (electric)", GH_ParamAccess.item);
            pManager.AddNumberParameter("CHPHTP", "CHPHTP", "Combined heat and power heat-to-power ratio, i.e. if htp=1.5, then 1 kW of electric power will generate 1.5 kW of heat", GH_ParamAccess.item);
            pManager.AddNumberParameter("CHPEffElec", "CHPEffElec", "Combined heat and power electric efficiency [0.0, 1.0]", GH_ParamAccess.item);

            pManager.AddNumberParameter("ChillerCapacity", "ChillerCapacity", "Chiller capacity, in kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("ChillerCost", "ChillerCost", "Chiller cost, in CHF/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("ChillerEmissions", "ChillerEmissions", "Chiller embodied green house gas emissions, in kgCO2/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("ChillerEtaRef", "ChillerEtaRef", "Chiller reference efficiency used for COP calculation", GH_ParamAccess.item);

            pManager.AddNumberParameter("HeatExchangerCapacity", "HeatExchangerCapacity", "HeatExchangerCapacity, kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("HeatExchangerCost", "HeatExchangerCost", "HeatExchangerCost, in CHF/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("HeatExchangerEmissions", "HeatExchangerEmissions", "HeatExchangerEmissions, in kgCO2/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("HeatExchangerLosses", "HeatExchangerLosses", "HeatExchangerLosses, [0.0, <1.0]", GH_ParamAccess.item);

            pManager.AddNumberParameter("CoolExchangerCapacity", "CoolExchangerCapacity", "CoolExchangerCapacity, kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("CoolExchangerCost", "CoolExchangerCost", "CoolExchangerCost, in CHF/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("CoolExchangerEmissions", "CoolExchangerEmissions", "CoolExchangerEmissions, in kgCO2/kW", GH_ParamAccess.item);
            pManager.AddNumberParameter("CoolExchangerLosses", "CoolExchangerLosses", "CoolExchangerLosses, [0.0, <1.0]", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ConversionTechJson", "ConversionTechJson", "Conversion technologies json", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConversionTechProperties energySystemProperties = new ConversionTechProperties();
            energySystemProperties.ASHPCapacity = 0.0;
            energySystemProperties.GasBoilerCapacity = 0.0;
            energySystemProperties.CHPCapacity = 0.0;
            energySystemProperties.ChillerCapacity = 0.0;
            energySystemProperties.HeatExchangerCapacity = 0.0;
            energySystemProperties.CoolExchangerCapacity = 0.0;

            DA.GetData(0, ref energySystemProperties.ASHPCapacity);
            DA.GetData(1, ref energySystemProperties.ASHPCost);
            DA.GetData(2, ref energySystemProperties.ASHPEmissions);
            DA.GetData(3, ref energySystemProperties.ASHPEtaRef);

            DA.GetData(4, ref energySystemProperties.GasBoilerCapacity);
            DA.GetData(5, ref energySystemProperties.GasBoilerCost);
            DA.GetData(6, ref energySystemProperties.GasBoilerEmissions);
            DA.GetData(7, ref energySystemProperties.GasBoilerEfficiency);

            DA.GetData(8, ref energySystemProperties.CHPCapacity);
            DA.GetData(9, ref energySystemProperties.CHPCost);
            DA.GetData(10, ref energySystemProperties.CHPEmissions);
            DA.GetData(11, ref energySystemProperties.CHPHTP);
            DA.GetData(12, ref energySystemProperties.CHPEffElec);

            DA.GetData(13, ref energySystemProperties.ChillerCapacity);
            DA.GetData(14, ref energySystemProperties.ChillerCost);
            DA.GetData(15, ref energySystemProperties.ChillerEmissions);
            DA.GetData(16, ref energySystemProperties.ChillerEtaRef);

            DA.GetData(17, ref energySystemProperties.HeatExchangerCapacity);
            DA.GetData(18, ref energySystemProperties.HeatExchangerCost);
            DA.GetData(19, ref energySystemProperties.HeatExchangerEmissions);
            DA.GetData(20, ref energySystemProperties.HeatExchangerLosses);

            DA.GetData(21, ref energySystemProperties.CoolExchangerCapacity);
            DA.GetData(22, ref energySystemProperties.CoolExchangerCost);
            DA.GetData(23, ref energySystemProperties.CoolExchangerEmissions);
            DA.GetData(24, ref energySystemProperties.CoolExchangerLosses);

            DA.SetData(0, energySystemProperties);
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
            get { return new Guid("d8bdede2-0bda-46f9-84fe-6de04c0fb870"); }
        }
    }
}