﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using Hive.IO.Forms;
using Rhino.Geometry;
//using Newtonsoft.Json;


namespace Hive.IO.GhParametricInputs
{
    internal class SolarTechProperties
    {
        public string Type; //{'PV', 'BIPV', 'PVT', 'ST', 'GC'}
        public string Technology; // e.g. 'Monocrystalline. Not really necessary, just for information
        public double ElectricEfficiency;
        public double ThermalEfficiency;
        public double PerformanceRatio;
        public double SurfaceTransmittance;
        public double InvestmentCost;
        public double EmbodiedEmissions;
        public double Lifetime; // FIXME implement this properly

        public Mesh MeshSurface;
    }


    public class GhSolarSystems : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhParametricInputSolarSystems class.
        /// </summary>
        public GhSolarSystems()
          : base("Parametric Input SolarTech Hive", "HiveParaInSolarTech",
              "Parametric inputs (Solar Technologies only) for Hive Energy Systems for, e.g., optimization work flows or sensitivity analysis. This component will overwrite the settings in the Hive Energy Systems Input Component, when connected.",
              "[hive]", "IO")
        {
        }


        public override GH_Exposure Exposure => GH_Exposure.tertiary;


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "Mesh surface of the solar system", GH_ParamAccess.list);
            pManager.AddTextParameter("Type", "Type", "Type of solar system {'PV', 'PVT', 'ST', 'GC'} standing for Photovoltaic (PV), Hybrid PV and Solar Thermal (PVT), Solar Thermal (ST), Ground Collector (GC)", GH_ParamAccess.item);
            pManager.AddTextParameter("Technology", "Technology", "Specific technology of solar systen, e.g. Polycrystalline, Monocrystalline, etc.", GH_ParamAccess.item);
            pManager.AddNumberParameter("EfficiencyElectric", "EfficiencyElectric", "Electric Efficiency [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("EfficiencyThermal", "EfficiencyThermal", "Thermal Efficiency [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("PerformanceRatio", "PerformanceRatio", "Performance Ratio [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("SurfaceTransmittance", "SurfaceTransmittance", "Surface Transmittance [0.0, 1.0]", GH_ParamAccess.item);
            pManager.AddNumberParameter("InvestmentCost", "InvestmentCost", "Investment cost in CHF/m^2", GH_ParamAccess.item);
            pManager.AddNumberParameter("EmbodiedEmissions", "EmbodiedEmissions", "Embodied emissions in kgCO2/m^2", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("SolarTech", "SolarTech", "SolarTech", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var meshes = new List<Mesh>();
            DA.GetDataList(0, meshes);

            string solarTechType = null;
            DA.GetData(1, ref solarTechType);
            string solarTechTechnology = null;
            DA.GetData(2, ref solarTechTechnology);
            double solarTechElectricEfficiency = 0.0; 
            DA.GetData(3, ref solarTechElectricEfficiency);
            double solarTechThermalEfficiency = 0.0;
            DA.GetData(4, ref solarTechThermalEfficiency);
            double solarTechInvestmentCost = 0.0;
            DA.GetData(5, ref solarTechInvestmentCost);
            double solarTechEmbodiedEmissions = 0.0;
            DA.GetData(6, ref solarTechEmbodiedEmissions);
            double solarTechPerformanceRatio = 0.0;
            DA.GetData(7, ref solarTechPerformanceRatio);
            double solarTechSurfaceTransmittance = 0.0;
            DA.GetData(8, ref solarTechSurfaceTransmittance);

            var solarTechList = new List<SolarTechProperties>();
            foreach (var mesh in meshes)
            {
                var solarTech = new SolarTechProperties
                {
                    MeshSurface = mesh,
                    Type = solarTechType,
                    Technology = solarTechTechnology,
                    ElectricEfficiency = solarTechElectricEfficiency,
                    ThermalEfficiency = solarTechThermalEfficiency,
                    InvestmentCost = solarTechInvestmentCost,
                    EmbodiedEmissions = solarTechEmbodiedEmissions, 
                    PerformanceRatio = solarTechPerformanceRatio,
                    SurfaceTransmittance = solarTechSurfaceTransmittance
                };

                solarTechList.Add(solarTech);
            }

            DA.SetDataList(0, solarTechList);
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_Para_InputSolarTech;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("e64d2335-dde6-4c85-9e03-1cbab1ce5266");
    }
}