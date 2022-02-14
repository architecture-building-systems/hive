using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhDistributors
{
    public class GhDistEnvironment : GH_Component
    {
        public GhDistEnvironment()
          : base("Distributor Environment Hive", "HiveDistEnvironment",
              "Environment distributor. Reads in an Hive.IO.Environment object and outputs the filepath of the .epw, as well as energy potentials as list of EnergyCarriers",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive Environment", "Environment", "Reads in an Environment object of type <Hive.Environment.Environment> that contains geometric, geographic, climate, and energy potentials information of the building's environment.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("EPW", "EPW", "Outputs the parsed EPW associated with the Hive Environment as  Hive.IO.EPW object. Can be distributed with another component", GH_ParamAccess.item);
            pManager.AddGenericParameter("NaturalGas", "NaturalGas", "NaturalGas energy carrier", GH_ParamAccess.item);
            pManager.AddGenericParameter("BioGas", "BioGas", "BioGas energy carrier", GH_ParamAccess.item);
            pManager.AddGenericParameter("WoodPellets", "WoodPellets", "WoodPellets energy carrier", GH_ParamAccess.item);
            pManager.AddGenericParameter("DistrictHeating", "DistrictHeating", "DistrictHeating energy carrier", GH_ParamAccess.item);
            pManager.AddGenericParameter("DistrictCooling", "DistrictCooling", "DistrictCooling energy carrier", GH_ParamAccess.item);
            pManager.AddGenericParameter("GridElectricity", "GridElectricity", "GridElectricity energy carrier", GH_ParamAccess.item);
            pManager.AddGenericParameter("AmbientAirTemp", "AmbientAirTemp", "Ambient air temperature annual hourly time series.", GH_ParamAccess.item);
            pManager.AddGenericParameter("GHI", "GHI", "Global Horizontal Irradiation annual hourly time series.", GH_ParamAccess.item);
            pManager.AddGenericParameter("DNI", "DNI", "Direct Normal Irradiation annual hourly time series.", GH_ParamAccess.item);
            pManager.AddGenericParameter("DHI", "DHI", "Diffuse Horizontal Irradiation annual hourly time series.", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Environment.Environment environment = null;
            if (!DA.GetData(0, ref environment)) return;

            if (environment != null)
            {
                // bad programming here... I know the order of my inputs, so I can loop...
                // I do have EnergyCarrier.Name... but that is not robust to use... implement enum?
                DA.SetData(0, environment.EpwData);
                for(int i=0; i<environment.EnergyPotentials.Length; i++)
                    DA.SetData(i+1, environment.EnergyPotentials[i]);
            }

        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Distenv;


        public override Guid ComponentGuid => new Guid("bf00c55e-e0bc-437d-b036-56adf7b149b5");
    }
}