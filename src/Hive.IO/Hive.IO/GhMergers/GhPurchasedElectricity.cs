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
          : base("Merger Additional Electricity Hive", "HiveMergerAddedElectricity",
              "Hive Merger for Additional electricity, from conversion technologies such as chillers or ASHP. Also for PV (reduces elec consumption) \n" +
                "It merges all kinds of data together to update the electricity grid connection with information on consumed energy, cost, operational schedule, etc. that have been computed in other components.",
              "[hive]", "IO-Core")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //0
            pManager.AddGenericParameter("Electricity Carrier", "Electricity", "Electricity energy carrier, e.g. from an electricity grid.", GH_ParamAccess.item);
            //1
            pManager.AddGenericParameter("Direct Electricity", "DirectElectricity", "Direct Electricity of type <Hive.IO.EnergySystems.DirectElectricity>, e.g. an electrical substation, or the connection between the electricity grid and the building.", GH_ParamAccess.item);
            //2
            pManager.AddBooleanParameter("hourly", "hourly", "toggle switch for true/false", GH_ParamAccess.item);
            //3
            pManager.AddGenericParameter("Hive Chiller", "Chiller", "Hive Chiller (<Hive.IO.EnergySystems.Chiller>) that will be infused with information from above inputs.", GH_ParamAccess.item);
            //4
            pManager.AddGenericParameter("Hive Air Source Heat Pump", "ASHP", "Hive Air Source Heat Pump (ASHP) (<Hive.IO.EnergySystems.AirSourceHeatPump>) that will be infused with information from above inputs.", GH_ParamAccess.item);
            //5
            pManager.AddNumberParameter("Ideal Loads Electricity", "IdealElectricity", "Ideal electric loads from appliances and lighting. In kWh, either monthly, or hourly", GH_ParamAccess.list);
            //6
            pManager.AddGenericParameter("Solar Technologies", "pvOrSTlist", "Hive.IO.EnergySystems.SurfaceBasedTech, such as PV, PVT, Solar Thermal, Ground Collector", GH_ParamAccess.list);
            //7
            pManager.AddNumberParameter("Ideal Loads Heating", "IdealHeating", "Ideal heating loads of a zone. In kWh, either monthly, or hourly.", GH_ParamAccess.list);
        }




        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Direct Electricity", "DirectElectricity", "DirectElectricity, 'fake' conversion tech (the connection from the building to the electricity grid) 'infused' with elecConsumed from the grid (input carrier) and provided into the building (output carrier), and operational cost and emissions (input carrier)", GH_ParamAccess.item);

            pManager.AddGenericParameter("Final Heating Demand", "FinalHeating", "Final heating demand, e.g., substracting generated solar thermal from loads", GH_ParamAccess.list);

            pManager.AddGenericParameter("Final Electricity Demand", "FinalElectricity", "Final electricity demand, i.e., including chiller/ashp/PV", GH_ParamAccess.list);
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

            var idealElec = new List<double>();
            DA.GetDataList(5, idealElec);

            var solarTech = new List<SurfaceBasedTech>();
            DA.GetDataList(6, solarTech);

            var idealHeating = new List<double>();
            DA.GetDataList(7, idealHeating);

            int horizon = hourly ? Misc.HoursPerYear : Misc.MonthsPerYear;
            double[] finalElec = idealElec.ToArray(); //this requires these loads time series to have the correct horizon
            double[] finalHeat = idealHeating.ToArray();

            if (!hourly) 
            { 
                if (chiller != null)
                    finalElec = finalElec.Zip(chiller.InputCarrier.EnergyMonthlyCumulative, (x, y) => x + y).ToArray();
                if (ashp != null)
                    finalElec = finalElec.Zip(ashp.InputCarrier.EnergyMonthlyCumulative, (x, y) => x + y).ToArray();
                if(solarTech.Count > 0)
                {
                    foreach (var solTech in solarTech)
                    {
                        if (solTech.GetType() == typeof(Photovoltaic) || solTech.GetType() == typeof(BuildingIntegratedPV))
                            finalElec = finalElec.Zip(solTech.OutputCarriers[0].EnergyMonthlyCumulative, (x, y) => x - y).ToArray();
                        else if (solTech.GetType() == typeof(SolarThermal))
                            finalHeat = finalHeat.Zip(solTech.OutputCarriers[0].EnergyMonthlyCumulative, (x, y) => x - y).ToArray();
                    }
                }
            }
            else
            {
                if (chiller != null)
                    finalElec = finalElec.Zip(chiller.InputCarrier.Energy, (x, y) => x + y).ToArray();
                if (ashp != null)
                    finalElec = finalElec.Zip(ashp.InputCarrier.Energy, (x, y) => x + y).ToArray();
                if (solarTech.Count > 0)
                {
                    foreach (var solTech in solarTech)
                    {
                        if (solTech.GetType() == typeof(Photovoltaic) || solTech.GetType() == typeof(BuildingIntegratedPV))
                            finalElec = finalElec.Zip(solTech.OutputCarriers[0].Energy, (x, y) => x - y).ToArray();
                        else if (solTech.GetType() == typeof(SolarThermal))
                            finalHeat = finalHeat.Zip(solTech.OutputCarriers[0].Energy, (x, y) => x - y).ToArray();                  
                    }
                }
            }


            
            substation.SetInputOutput(elec, finalElec);
            DA.SetData(0, substation);
            DA.SetDataList(1, finalHeat.ToList());
            DA.SetDataList(2, finalElec.ToList());

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