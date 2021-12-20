
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhDemand
{
    public class Sia380 : GH_Component
    {
        public Sia380()
          : base("Building demand SIA 380", "Sia380",
              "Heating, cooling and electricity demand calculation based on SIA 380",
              "[hive]", "Demand")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddStringParameter("Room properties", "RoomProperties", "Room properties as json. Could be from SIA 2024, or custom. It needs to follow the hive sia2024-json syntax", GH_ParamAccess.item);
            pManager.AddStringParameter("Room schedules", "RoomSchedules", "Room schedules for occupancy, devices and lighting as json. Could be from SIA 2024, or custom. It needs to follow the hive sia2024-schedules-json syntax", GH_ParamAccess.item);
            pManager.AddNumberParameter("Floor area", "FloorArea", "Floor area of the zone in m^2.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Air temperature", "T_e", "Hourly ambient air temperature in Â°C.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Setpoints upper bounds", "SetpointsUB", "Hourly room temperature setpoints in Â°C (upper bound).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Setpoints lower bounds", "SetpointsLB", "Hourly room temperature setpoints in Â°C (lower bound).", GH_ParamAccess.list);
            pManager.AddNumberParameter("Envelope surface areas", "SurfaceAreas", "Envelope surface areas of all external room surfaces in m^2.", GH_ParamAccess.list);
            pManager.AddTextParameter("Envelope surface types", "SurfaceTypes", "Envelope surface types of all external room surfaces. Either 'opaque' or 'transp'.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Surface irradiation", "Q_s_tree", "Surface irradiation per transparent surface. Grasshopper tree, with each branch representing a transparent surface and containing 8760 timeseries of solar irradiation in Wh.", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Surface irradiation unobstructed", "Q_s_tree_unobstructed", "Unobstructed surface irradiation per surface. This tree is used, if the obstructed data (parameter above) is null. Grasshopper tree, same as parameter above.", GH_ParamAccess.tree);
            pManager.AddNumberParameter("G value", "gValue", "G value of windows", GH_ParamAccess.item);
            pManager.AddNumberParameter("G value with sunscreen", "gValueTotal", "G value total including sunscreen ('Sonnenschutz') of windows", GH_ParamAccess.item);
            pManager.AddNumberParameter("Setpoint sunscreen", "SetpointSunscreen", "Shading setpoint for activating sunscreen of windows, in W/m^2", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run obstructed solar simulation?", "RunObstructed?", "Boolean to indicate if an obstructed solar simulation is conducted. True if yes.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Run hourly?", "RunHourly?", "Boolean to indicate if to run monthly or hourly simulations. True if hourly.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Use adaptive comfort?", "UseAdaptiveComfort?", "Boolean to indicate if adaptive comfort should be used instead of fixed setpoints. True if yes. Defaults to yes if setpoints_ub and setpoints_lb are null.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Use natural ventilation?", "UseNaturalVentilation?", "Boolean to indicate if natural ventilation should be considered (True) or not (False).", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Use fixed time constant?", "UseFixedTimeConstant?", "Boolean to indicate if the fixed time constant from SIA 2024 should be used (True) or dynamically calculated based on capacities (False).", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Heating demand", "Q_Heat", "Heating loads of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Domestic hot water demand", "Q_dhw", "Domestic hot water demands of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Cooling demand", "Q_Cool", "Cooling loads of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Electricity demand", "Q_Elec", "Electricity loads of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Transmission heat losses", "Q_T", "Transmission heat losses of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Ventilation heat losses", "Q_V", "Ventilation heat losses of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Internal gains", "Q_i", "Internal gains of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Solar gains", "Q_s", "Solar gains of a zone in kWh.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Transmission heat losses of opaque surfaces", "Q_T_opaque", "Transmission heat losses through opaque envelope surfaces in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Transmission heat losses of transparent surfaces", "Q_T_transp", "Transmission heat losses through transparent envelope surfaces in kWh", GH_ParamAccess.list);
            pManager.AddNumberParameter("Solar gains per transparent surface", "Q_s_per_window", "Solar gains per transparent surface (window). Grasshopper tree with each branch representing one window and containing a timeseries of irradiation values in kWh.", GH_ParamAccess.tree);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Dictionary<string, object> Room_properties = new Dictionary<string, object>();
			if (!DA.GetData(0, ref Room_properties)) return;
            Dictionary<string, object> Room_schedules = new Dictionary<string, object>();
			if (!DA.GetData(1, ref Room_schedules)) return;
            double Floor_area = new double();
			if (!DA.GetData(2, ref Floor_area)) return;
            List<double> Air_temperature = new List<double>();
			if (!DA.GetDataList(3, ref Air_temperature)) return;
            List<double> Setpoints_upper_bounds = new List<double>();
			if (!DA.GetDataList(4, ref Setpoints_upper_bounds)) return;
            List<double> Setpoints_lower_bounds = new List<double>();
			if (!DA.GetDataList(5, ref Setpoints_lower_bounds)) return;
            List<double> Envelope_surface_areas = new List<double>();
			if (!DA.GetDataList(6, ref Envelope_surface_areas)) return;
            List<string> Envelope_surface_types = new List<string>();
			if (!DA.GetDataList(7, ref Envelope_surface_types)) return;
            object Surface_irradiation = new object();
			if (!DA.GetData(8, ref Surface_irradiation)) return;
            object Surface_irradiation_unobstructed = new object();
			if (!DA.GetData(9, ref Surface_irradiation_unobstructed)) return;
            double G_value = new double();
			if (!DA.GetData(10, ref G_value)) return;
            double G_value_with_sunscreen = new double();
			if (!DA.GetData(11, ref G_value_with_sunscreen)) return;
            double Setpoint_sunscreen = new double();
			if (!DA.GetData(12, ref Setpoint_sunscreen)) return;
            bool Run_obstructed_solar_simulation? = new bool();
			if (!DA.GetData(13, ref Run_obstructed_solar_simulation?)) return;
            bool Run_hourly? = new bool();
			if (!DA.GetData(14, ref Run_hourly?)) return;
            bool Use_adaptive_comfort? = new bool();
			if (!DA.GetData(15, ref Use_adaptive_comfort?)) return;
            bool Use_natural_ventilation? = new bool();
			if (!DA.GetData(16, ref Use_natural_ventilation?)) return;
            bool Use_fixed_time_constant? = new bool();
			if (!DA.GetData(17, ref Use_fixed_time_constant?)) return;
            

            var Heating_demand = new List<bool>();
            var Domestic_hot_water_demand = new List<bool>();
            var Cooling_demand = new List<bool>();
            var Electricity_demand = new List<bool>();
            var Transmission_heat_losses = new List<bool>();
            var Ventilation_heat_losses = new List<bool>();
            var Internal_gains = new List<bool>();
            var Solar_gains = new List<bool>();
            var Transmission_heat_losses_of_opaque_surfaces = new List<bool>();
            var Transmission_heat_losses_of_transparent_surfaces = new List<bool>();
            var Solar_gains_per_transparent_surface = new double();

            // TODO

            DA.SetDataList(0, Heating_demand);
            DA.SetDataList(1, Domestic_hot_water_demand);
            DA.SetDataList(2, Cooling_demand);
            DA.SetDataList(3, Electricity_demand);
            DA.SetDataList(4, Transmission_heat_losses);
            DA.SetDataList(5, Ventilation_heat_losses);
            DA.SetDataList(6, Internal_gains);
            DA.SetDataList(7, Solar_gains);
            DA.SetDataList(8, Transmission_heat_losses_of_opaque_surfaces);
            DA.SetDataList(9, Transmission_heat_losses_of_transparent_surfaces);
            DA.SetData(10, Solar_gains_per_transparent_surface);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.demand_energydemand.png;


        public override Guid ComponentGuid => new Guid("01457f8e-80d7-4186-856d-ddaf1d896218"); 
       
    }
}
