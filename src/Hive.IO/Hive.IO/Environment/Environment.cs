using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rg = Rhino.Geometry;
using Hive.IO.EnergySystems;
using System.IO;

namespace Hive.IO.Environment
{
    /// <summary>
    /// Environment class, defining the environment the building is embedded in. 
    /// Contains climate data, location, adjacent obstacles and trees, etc.
    /// Grid access and renewable potentials as well?
    /// </summary>
    public class Environment
    {
        public readonly int totalPotentials = 10; // gas, biogas, wood, dh, dc, grid, ambientair, ghi, dni, dhi
        public readonly int Horizon = Misc.HoursPerYear;
        public Epw EpwData;

        public rg.Mesh[] Geometry { get; private set; }

        /// <summary>
        /// Irradiation, ambient air, etc.
        /// </summary>
        public Carrier[] EnergyPotentials { get; private set; }


        public Environment(string epwFilePath, rg.Mesh[] geometry = null)
        {
            this.Geometry = geometry;
            this.EpwData = new Epw() { FilePath = @epwFilePath };
            this.EpwData.Parse();
        }

        // setting all inpuit energy carriers as part of Hive.IO.Environment
        // gas, oil, biogas, wood, district heating, electricity grid, etc.
        internal void SetEnergyPotentials(Carrier[] inputCarriers)
        {
            this.EnergyPotentials = new Carrier[this.totalPotentials];
            inputCarriers.CopyTo(this.EnergyPotentials, 0);
            this.EnergyPotentials[6] = new Air(this.Horizon, null, null, null, this.EpwData.DryBulbTemperature);
            this.EnergyPotentials[6].Name = "DryBulbTemperature";
            this.EnergyPotentials[7] = new Radiation(this.Horizon, this.EpwData.GHI);
            this.EnergyPotentials[7].Name = "GHI"; // redundant, because <Radiation> has a enum for GHI
            this.EnergyPotentials[8] = new Radiation(this.Horizon, this.EpwData.DNI, 1, null, Radiation.RadiationType.DNI);
            this.EnergyPotentials[8].Name = "DNI";
            this.EnergyPotentials[9] = new Radiation(this.Horizon, this.EpwData.DHI, 1, null, Radiation.RadiationType.DHI);
            this.EnergyPotentials[9].Name = "DHI";
        }


        internal void SetDefaultEnergyPotentials()
        {
            this.EnergyPotentials = new Carrier[this.totalPotentials];
            this.EnergyPotentials[0] = new Gas(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.09, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.237, this.Horizon).ToArray(), Misc.PEFNaturalGas);
            this.EnergyPotentials[0].Name = "NaturalGas";
            this.EnergyPotentials[1] = new Gas(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.15, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.15, this.Horizon).ToArray(), Misc.PEFBioGas);
            this.EnergyPotentials[1].Name = "BioGas";
            this.EnergyPotentials[2] = new Pellets(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.2, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.1, this.Horizon).ToArray(), Misc.PEFWoodPellets);
            this.EnergyPotentials[2].Name = "ZueriWald";
            this.EnergyPotentials[3] = new Water(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.09, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.237, this.Horizon).ToArray(), Enumerable.Repeat<double>(65.0, this.Horizon).ToArray(), 1.0);
            this.EnergyPotentials[3].Name = "DistrictHeating";
            this.EnergyPotentials[4] = new Water(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.09, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.237, this.Horizon).ToArray(), Enumerable.Repeat<double>(15.0, this.Horizon).ToArray(), 1.0);
            this.EnergyPotentials[4].Name = "DistrictCooling";
            this.EnergyPotentials[5] = new Electricity(this.Horizon, Enumerable.Repeat<double>(double.MaxValue, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.12, this.Horizon).ToArray(), Enumerable.Repeat<double>(0.597, this.Horizon).ToArray(), Misc.PEFElectricitySwiss);
            this.EnergyPotentials[5].Name = "UCT-Mix";
            this.EnergyPotentials[6] = new Air(this.Horizon, null, null, null, this.EpwData.DryBulbTemperature);
            this.EnergyPotentials[6].Name = "DryBulbTemperature";
            this.EnergyPotentials[7] = new Radiation(this.Horizon, this.EpwData.GHI);
            this.EnergyPotentials[7].Name = "GHI"; // redundant, because <Radiation> has a enum for GHI
            this.EnergyPotentials[8] = new Radiation(this.Horizon, this.EpwData.DNI, 1, null, Radiation.RadiationType.DNI);
            this.EnergyPotentials[8].Name = "DNI";
            this.EnergyPotentials[9] = new Radiation(this.Horizon, this.EpwData.DHI, 1, null, Radiation.RadiationType.DHI);
            this.EnergyPotentials[9].Name = "DHI";
        }
    }
}
