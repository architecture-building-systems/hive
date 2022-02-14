
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhDemand
{
    public class Sia2024Creator : GH_Component
    {
        public Sia2024Creator()
          : base("SIA 2024 Room Creator", "Sia2024Creator",
              "Creates a SIA 2024 room Json",
              "[hive]", "Demand")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Description", "Description", "Sia 2024 Room name. Must follow hive sia 2024 naming convention", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Time constant", "Ï„ [h]", "'Zeitkonstante', time constant in hours, descriping thermal latency of the room [h]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Cooling setpoint", "Î¸_i cooling [Â°C]", "Raumlufttemperatur Auslegung Kuehlung (Sommer) - cooling setpoint in summer [Â°C]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heating setpoint", "Î¸_i heating [Â°C]", "Raumlufttemperatur Auslegung Heizen (Winter) - heating setpoint in winter [Â°C]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Cooling setback", "Î¸_i_s cooling [Â°C]", "Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur - cooling setback in summer [Â°C]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heating setback", "Î¸_i_s heating [Â°C]", "Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur - heating setback in winter [Â°C]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Floor area", "FloorArea", "Nettogeschossflaeche - floor area [m^2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Envelope area", "EnvelopeArea", "Thermische Gebaeudehuellflaeche - envelope surface area [m^2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Glazing ratio", "GlazingRatio", "Glasanteil - glazing ratio [0, 1]", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value opaque", "U_op [W/(m^2K)]", "'U-Wert opake Bauteile' - U-value of opaque surfaces [W/(m^2K)]", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value transparent", "U_w [W/(m^2K)]", "'U-Wert Fenster' - U-value of transparent surfaces [W/(m^2K)]", GH_ParamAccess.item);
            pManager.AddNumberParameter("g value", "g [-]", "'Gesamtenergiedurchlassgrad Verglasung' - g-value of windows [0, 1]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Windows frame reduction factor", "windowsFrameReduction", "'Abminderungsfaktor fuer Fensterrahmen' - window frames reduction factor", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volumetric air flow rate", "V̇_e [m3/h]", "Aussenluft-Volumenstrom [m^3/h] - exterior volumetric air flow rate", GH_ParamAccess.item);
            pManager.AddNumberParameter("Infiltration", "VÌ‡_inf [m^3/h]", "Aussenluft-Volumenstrom durch Infiltration [m^3/h] - volume air flow rate through infiltration", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heat recovery", "Î·_rec [-]", "Nutzungsgrad der WÃ¤rmerÃ¼ckgewinnung [-] - heat recovery", GH_ParamAccess.item);
            pManager.AddNumberParameter("Occupation internal heat gains ", "Ï†_P [W]", "WÃ¤rmeabgabe Personen [W] - internal heat gains from occupation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lighting internal heat gains", "Ï†_L [W]", "WÃ¤rmeabgabe Beleuchtung [W] - internal heat gains from lighting", GH_ParamAccess.item);
            pManager.AddNumberParameter("Equipment internal heat gains", "Ï†_G [W]", "WÃ¤rmeabgabe GerÃ¤te [W] - internal heat gains from equipment", GH_ParamAccess.item);
            pManager.AddNumberParameter("Occupation load hours ", "t_P [h]", "Vollaststunden Personen [h] - occupation load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lighting load hours", "t_L [h]", "Vollaststunden Beleuchtung [h] - lighting load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("Equipment load hours", "t_A [h]", "Vollaststunden GerÃ¤te [h] - equipment load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("Opaque construction cost", "OpaqueCost", "'Kosten opake Bauteile' - cost of opaque construction [CHF/m^2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Transparent construction cost", "TransparentCost", "'Kosten transparente Bauteile' - cost of transparent construction [CHF/m^2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Opaque construction embodied emissions", "OpaqueEmissions", "'Emissionen opake Bauteile' - embodied emissions of opaque construction [kgCO2/m^2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Transparent construction embodied emissions", "TransparentEmissions", "'Emissionen transparente Bauteile' - embodied emissions of transparent construction [kgCO2/m^2]", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddStringParameter("Sia 2024 room", "RoomJson", "Room information according to hive sia 2024 naming convention, as a Json", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Description = new string();
			if (!DA.GetData(0, ref Description)) return;
            int Time_constant = new int();
			if (!DA.GetData(1, ref Time_constant)) return;
            double Cooling_setpoint = new double();
			if (!DA.GetData(2, ref Cooling_setpoint)) return;
            double Heating_setpoint = new double();
			if (!DA.GetData(3, ref Heating_setpoint)) return;
            double Cooling_setback = new double();
			if (!DA.GetData(4, ref Cooling_setback)) return;
            double Heating_setback = new double();
			if (!DA.GetData(5, ref Heating_setback)) return;
            double Floor_area = new double();
			if (!DA.GetData(6, ref Floor_area)) return;
            double Envelope_area = new double();
			if (!DA.GetData(7, ref Envelope_area)) return;
            double Glazing_ratio = new double();
			if (!DA.GetData(8, ref Glazing_ratio)) return;
            double U_value_opaque = new double();
			if (!DA.GetData(9, ref U_value_opaque)) return;
            double U_value_transparent = new double();
			if (!DA.GetData(10, ref U_value_transparent)) return;
            double g_value = new double();
			if (!DA.GetData(11, ref g_value)) return;
            double Windows_frame_reduction_factor = new double();
			if (!DA.GetData(12, ref Windows_frame_reduction_factor)) return;
            double Volumetric_air_flow_rate = new double();
			if (!DA.GetData(13, ref Volumetric_air_flow_rate)) return;
            double Infiltration = new double();
			if (!DA.GetData(14, ref Infiltration)) return;
            double Heat_recovery = new double();
			if (!DA.GetData(15, ref Heat_recovery)) return;
            double Occupation_internal_heat_gains_ = new double();
			if (!DA.GetData(16, ref Occupation_internal_heat_gains_)) return;
            double Lighting_internal_heat_gains = new double();
			if (!DA.GetData(17, ref Lighting_internal_heat_gains)) return;
            double Equipment_internal_heat_gains = new double();
			if (!DA.GetData(18, ref Equipment_internal_heat_gains)) return;
            double Occupation_load_hours_ = new double();
			if (!DA.GetData(19, ref Occupation_load_hours_)) return;
            double Lighting_load_hours = new double();
			if (!DA.GetData(20, ref Lighting_load_hours)) return;
            double Equipment_load_hours = new double();
			if (!DA.GetData(21, ref Equipment_load_hours)) return;
            double Opaque_construction_cost = new double();
			if (!DA.GetData(22, ref Opaque_construction_cost)) return;
            double Transparent_construction_cost = new double();
			if (!DA.GetData(23, ref Transparent_construction_cost)) return;
            double Opaque_construction_embodied_emissions = new double();
			if (!DA.GetData(24, ref Opaque_construction_embodied_emissions)) return;
            double Transparent_construction_embodied_emissions = new double();
			if (!DA.GetData(25, ref Transparent_construction_embodied_emissions)) return;
            

            var Sia_2024_room = new Dictionary<string, object>();

            // TODO

            DA.SetData(0, Sia_2024_room);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.demand_SIAroomcreater.png;


        public override Guid ComponentGuid => new Guid("51596454-227f-404e-8f07-76eef9beaa09"); 
       
    }
}
