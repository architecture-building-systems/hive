using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using System.Linq;


namespace Hive.IO.GhMergers
{
    public class GhPurchasedElectricity : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhGridSubstation class.
        /// </summary>
        public GhPurchasedElectricity()
          : base("Merger Direct Electricity Hive", "HiveMergerDirectElectricity",
              "Hive Merger for Direct Electricity, e.g. an electrical substation, or the connection from the grid to the building (<Hive.IO.EnergySystems.DirectElectricity>). It merges all kinds of data together to update the electricity grid connection with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Electricity Carrier", "Electricity", "Electricity energy carrier, e.g. from an electricity grid.", GH_ParamAccess.item);

            pManager.AddGenericParameter("Direct Electricity", "DirectElectricity", "Direct Electricity of type <Hive.IO.EnergySystems.DirectElectricity>, e.g. an electrical substation, or the connection between the electricity grid and the building.", GH_ParamAccess.item);

            pManager.AddBooleanParameter("hourly", "hourly", "toggle switch for true/false", GH_ParamAccess.item);

            pManager.AddGenericParameter("Hive Chiller", "Chiller", "Hive Chiller (<Hive.IO.EnergySystems.Chiller>) that will be infused with information from above inputs.", GH_ParamAccess.item);

            pManager.AddGenericParameter("Hive Air Source Heat Pump", "ASHP", "Hive Air Source Heat Pump (ASHP) (<Hive.IO.EnergySystems.AirSourceHeatPump>) that will be infused with information from above inputs.", GH_ParamAccess.item);

            pManager.AddNumberParameter("Electricity demand", "siaElec", "Electricity loads of a zone in kWh.", GH_ParamAccess.list);

            pManager.AddGenericParameter("Solar Technologies", "pvOrSTlist", "Hive.IO.EnergySystems.SurfaceBasedTech, such as PV, PVT, Solar Thermal, Ground Collector", GH_ParamAccess.list);

            pManager.AddNumberParameter("Heating demand", "heatDemandIn", "Heating loads of a zone in kWh.", GH_ParamAccess.list);

        }


    

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Direct Electricity", "DirectElectricity", "DirectElectricity, 'fake' conversion tech (the connection from the building to the electricity grid) 'infused' with elecConsumed from the grid (input carrier) and provided into the building (output carrier), and operational cost and emissions (input carrier)", GH_ParamAccess.item);

            //pManager.AddGenericParameter("Additional Electricity", "Additional Electricity", "Additional Electricity", GH_ParamAccess.item);

            //pManager.AddGenericParameter("Electricity Demand Out", "Electricity Demand Out", "Electricity Demand Out",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Electricity elec = null;
            DA.GetData(0, ref elec);

            DirectElectricity substation = null;
            DA.GetData(1, ref substation);

            bool hourly = true;
            DA.GetData(2, ref hourly);

            Chiller chiller = null;
            DA.GetData(3, ref chiller);

            AirSourceHeatPump ashp = null;
            DA.GetData(4, ref ashp);

            var siaElec = new List<double>();
            DA.GetDataList(5, siaElec);

            var solarTech = new List<SurfaceBasedTech>();
            DA.GetDataList(6, solarTech);

            var heatDemandIn = new List<double>();
            DA.GetDataList(7, heatDemandIn);

            IEnumerable<double> additionalelec = null;
            if (hourly == false)
            {
                if (chiller != null && ashp != null)
                    additionalelec = chiller.InputCarrier.EnergyMonthlyCumulative.Zip(ashp.InputCarrier.EnergyMonthlyCumulative, (x, y) => x + y).Zip(siaElec, (x, y) => x + y);
                else if (ashp != null && chiller == null)
                    additionalelec = ashp.InputCarrier.EnergyMonthlyCumulative.Zip(siaElec, (x, y) => x + y);
                else if (ashp == null && chiller != null)
                    additionalelec = chiller.InputCarrier.EnergyMonthlyCumulative.Zip(siaElec, (x, y) => x + y);
                else
                    additionalelec = siaElec;
            }
            else
            {
                if (chiller != null && ashp != null)
                    additionalelec = chiller.InputCarrier.Energy.Zip(ashp.InputCarrier.Energy, (x, y) => x + y).Zip(siaElec, (x, y) => x + y);
                else if (ashp != null && chiller == null)
                    additionalelec = ashp.InputCarrier.Energy.Zip(siaElec, (x, y) => x + y);
                else if (ashp == null && chiller != null)
                    additionalelec = chiller.InputCarrier.Energy.Zip(siaElec, (x, y) => x + y);
                else
                    additionalelec = siaElec;
            }

            var horizon = 8760;
            if (hourly == false)
                horizon = 12;


            IEnumerable<double> totalRenewableHeat = new double[horizon];
            IEnumerable<double> totalRenewableElec = new double[horizon];
            IEnumerable<double> energyRenewable = null;
            IEnumerable<double> elecPurchased = null;


            if (hourly == false)
            {
                foreach (var pvOrSt in solarTech)
                {
                    if (pvOrSt != null)
                        energyRenewable = pvOrSt.OutputCarriers[0].EnergyMonthlyCumulative;
                    if (pvOrSt.GetType() == typeof(Photovoltaic) || pvOrSt.GetType() == typeof(BuildingIntegratedPV))
                        totalRenewableElec = totalRenewableElec.Zip(energyRenewable, (x, y) => x + y);
                    else if (pvOrSt.GetType() == typeof(SolarThermal))
                        totalRenewableHeat = totalRenewableHeat.Zip(energyRenewable, (x, y) => x + y);
                }
            }
            else
            {
                foreach (var pvOrSt in solarTech)
                {
                    if (pvOrSt != null)
                        energyRenewable = pvOrSt.OutputCarriers[0].Energy;
                    if (pvOrSt.GetType() == typeof(Photovoltaic) || pvOrSt.GetType() == typeof(BuildingIntegratedPV))
                        totalRenewableElec = totalRenewableElec.Zip(energyRenewable, (x, y) => x + y);
                    else if (pvOrSt.GetType() == typeof(SolarThermal))
                        totalRenewableHeat = totalRenewableHeat.Zip(energyRenewable, (x, y) => x + y);
                }
            }

            elecPurchased = additionalelec.Zip(totalRenewableElec, (x, y) => x - y);

            substation.SetInputOutput(elec, elecPurchased.ToArray());

            DA.SetData(0, substation);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.IOCore_Mergerelectricalstation;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7644b488-19ed-4819-8195-bc1e24428cca");
    }
}