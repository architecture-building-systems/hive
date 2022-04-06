using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using Hive.IO.GhParametricInputs;
using Rhino.Geometry;

namespace Hive.IO.GhInputOutput
{
    public class GhEnvironment : GH_Component
    {
        public GhEnvironment()
          : base("Input Environment Hive", "HiveInputEnvironment",
              "Hive Environment input to define the environment of a building, specifically energy potentials, obstacles (such as adjacent buildings to topography), as well as the weather file.",
              "[hive]", "IO")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Epw Path", "Epw", "Local path to an .epw weather file.", GH_ParamAccess.item);
            pManager.AddMeshParameter("Obstacle geometries", "Obstacles", "(Optional input) Mesh geometries of any adjacent obstacles, such as buildings, trees, etc.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddGenericParameter("Potentials", "Potentials", 
                "(Optional input) An <EnergyPotentialsProperties> input from a Hive Parametric Input Energy Potentials component that defines the energy potentials of the site. If no input is given, default values are assumed.", 
                GH_ParamAccess.item);
            pManager[2].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Environment", "Environment", 
                "Creates an Environment definition of type <Hive.IO.Environment.Environment> that contains geometric, geographic, climate, and energy potentials information of the building's environment.", 
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string epwPath = null;
            if (!DA.GetData(0, ref epwPath)) return;

            List<Mesh> geometry = new List<Mesh>();
            DA.GetDataList(1, geometry);

            GhEnergyPotentials.EnergyCarrierTimeseries potentials = null; // new GhParametricInputEnergyPotentials.EnergyPotentialsProperties();
            DA.GetData(2, ref potentials);


            Mesh[] geometryArray = geometry.Count > 0 ? geometry.ToArray() : null;
            Environment.Environment environment = new Environment.Environment(epwPath, geometryArray);
            if (potentials == null)
            {
                environment.SetDefaultEnergyPotentials();    // replace this with inputs from the windows form later
            }
            else
            {
                Carrier[] inputCarriers = new Carrier[6];
                inputCarriers[0] = new Gas(Misc.HoursPerYear, potentials.NaturalGasAvailability, potentials.NaturalGasPrice, potentials.NaturalGasEmissions, Misc.PEFNaturalGas);
                inputCarriers[0].Name = potentials.NaturalGasName;
                inputCarriers[1] = new Gas(Misc.HoursPerYear, potentials.BioGasAvailability, potentials.BioGasPrice, potentials.BioGasEmissions, Misc.PEFBioGas);
                inputCarriers[1].Name = potentials.BioGasName;
                inputCarriers[2] = new Pellets(Misc.HoursPerYear, potentials.WoodPelletsAvailability, potentials.WoodPelletsPrice, potentials.WoodPelletsEmissions, Misc.PEFWoodPellets);
                inputCarriers[2].Name = potentials.WoodPelletsName;
                inputCarriers[3] = new Water(Misc.HoursPerYear, potentials.DistrictHeatingAvailability, potentials.DistrictHeatingPrice, potentials.DistrictHeatingEmissions, potentials.DistrictHeatingSupplyTemp, Misc.PEFBioGas);
                inputCarriers[3].Name = potentials.DistrictHeatingName;
                inputCarriers[4] = new Water(Misc.HoursPerYear, potentials.DistrictCoolingAvailability, potentials.DistrictCoolingPrice, potentials.DistrictCoolingEmissions, potentials.DistrictCoolingSupplyTemp, Misc.PEFBioGas);
                inputCarriers[4].Name = potentials.DistrictCoolingName;
                inputCarriers[5] = new Electricity(Misc.HoursPerYear, potentials.GridElectricityAvailability, potentials.GridElectricityPrice, potentials.GridElectricityEmissions, Misc.PEFElectricitySwiss);
                inputCarriers[5].Name = potentials.GridElectricityName;
                environment.SetEnergyPotentials(inputCarriers);
            }

            DA.SetData(0, environment);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IO_Environment;


        public override Guid ComponentGuid => new Guid("c11567a1-b864-4e4e-a192-24bb487a4bac");
    }
}