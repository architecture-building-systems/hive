using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Hive.IO.EnergySystems;

namespace Hive.IO.GHComponents
{
    public class GhEnvironment : GH_Component
    {
        public GhEnvironment()
          : base("Input Environment Hive", "HiveInputEnvironment",
              "Hive.IO.Environment input, describing the environment of a building.",
              "[hive]", "IO")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("EPW Path", "epwPath", "epwPath", GH_ParamAccess.item);
            pManager.AddMeshParameter("Obstacles Geometry", "ObstMesh", "Mesh geometries of any adjacent obstacles, such as buildings, trees, etc.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddGenericParameter("Potentials", "Potentials", "Energy potentials of the site, of type <EnergyPotentialsProperties>.", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Hive.IO.Environment", "HiveIOEnv", "Hive.IO.Environment, containing geometric, geographic and climate information of the buildings's environment.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            if (!DA.GetData(0, ref path)) return;

            List<Mesh> geometry = new List<Mesh>();
            DA.GetDataList(1, geometry);

            Mesh[] geometryArray = geometry.Count > 0 ? geometry.ToArray() : null;
            Environment.Environment environment = new Environment.Environment(path, geometryArray);

            var maxAvailability = new double[Misc.HoursPerYear];
            for (int i=0; i<maxAvailability.Length; i++)
                maxAvailability[i] = double.MaxValue;

            var gasCost = new double[Misc.HoursPerYear];
            var gasEmissions = new double[Misc.HoursPerYear];

            var gas = new Gas(Misc.HoursPerDay, maxAvailability, gasCost, gasEmissions);
            var inputCarriers = new EnergyCarrier[1];

            environment.SetEnergyPotentials(null);

            DA.SetData(0, environment);
        }


        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Hive.IO.Properties.Resources.IO_Environment;
            }
        }


        public override Guid ComponentGuid
        {
            get { return new Guid("c11567a1-b864-4e4e-a192-24bb487a4bac"); }
        }
    }
}