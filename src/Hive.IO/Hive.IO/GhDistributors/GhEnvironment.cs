using System;
using Grasshopper.Kernel;

namespace Hive.IO.GhDistributors
{
    public class GhEnvironment : GH_Component
    {
        public GhEnvironment()
          : base("Distributor Environment Hive", "HiveDistEnvironment",
              "Environment distributor. Reads in an Hive.IO.Environment object and outputs the filepath of the .epw, as well as energy potentials as list of EnergyCarriers",
              "[hive]", "IO")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Environment", "HiveIOEnv", "Reads in an Hive.IO.Environment object.", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Filepath", "Filepath", "Outputs the filepath of the .epw weather file belonging to Hive.IO.Environment. Can be used to e.g. open the weather file in another component.", GH_ParamAccess.item);
            pManager.AddGenericParameter("NaturalGas", "NaturalGas", "NaturalGas", GH_ParamAccess.item);
            pManager.AddGenericParameter("BioGas", "BioGas", "BioGas", GH_ParamAccess.item);
            pManager.AddGenericParameter("WoodPellets", "WoodPellets", "WoodPellets", GH_ParamAccess.item);
            pManager.AddGenericParameter("DistrictHeating", "DistrictHeating", "DistrictHeating", GH_ParamAccess.item);
            pManager.AddGenericParameter("DistrictCooling", "DistrictCooling", "DistrictCooling", GH_ParamAccess.item);
            pManager.AddGenericParameter("GridElectricity", "GridElectricity", "GridElectricity", GH_ParamAccess.item);
            pManager.AddGenericParameter("AmbientAirTemp", "AmbientAirTemp", "AmbientAirTemp", GH_ParamAccess.item);
            pManager.AddGenericParameter("GHI", "GHI", "GHI", GH_ParamAccess.item);
            pManager.AddGenericParameter("DNI", "DNI", "DNI", GH_ParamAccess.item);
            pManager.AddGenericParameter("DHI", "DHI", "DHI", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Environment.Environment environment = null;
            if (!DA.GetData(0, ref environment)) return;

            if (environment != null)
            {
                // bad programming here... I know the order of my inputs, so I can loop...
                // I do have EnergyCarrier.Name... but that's not robust. Implement an enum for a discrete set? like BuildingType?
                DA.SetData(0, environment.EpwPath);
                for(int i=0; i<environment.EnergyPotentials.Length; i++)
                    DA.SetData(i+1, environment.EnergyPotentials[i]);
            }

        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("bf00c55e-e0bc-437d-b036-56adf7b149b5"); }
        }
    }
}