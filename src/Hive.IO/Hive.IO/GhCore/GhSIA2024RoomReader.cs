using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;
using Hive.IO.Core;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Hive.IO.Building;
using System.Linq;

namespace Hive.IO.GhCore
{
    public class GhSIA2024RoomReader : GH_Component
    {
        public GhSIA2024RoomReader()
          : base("SIA 2024 Room Reader C#", "Sia2024Reader",
              "Reads room values and properties from a SIA 2024 Json",
              "[hive]", "Demand C#")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Room Json", "RoomJson", "Room type according to sia2024. Should be a dictionary in json format containing all properties. Must comply to hive sia2024 json format", GH_ParamAccess.item);
            pManager.AddNumberParameter("Room Area", "Area", "Room Area in m². If no input is given, default values are taken from SIA2024:2015 (Json dictionary of the parameter above).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Month", "Month", "Calculation month, 1 - 12", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Season", "Season", "Season, either 'winter' or 'summer'. Determines the room temperature setpoints of SIA 2024", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Time constant", "τ [h]", "Zeitkonstante des Gebäudes [h] - time constant, representing thermal latency of the zone", GH_ParamAccess.item);
            pManager.AddNumberParameter("Temperature setpoint", "θ_i [°C]", "Raumlufttemperatur [°C] - temperature setpoint", GH_ParamAccess.item);
            pManager.AddNumberParameter("Calculation period", "t [h]", "Länge der Berechnungsperiode [h] - calculation period, representing usage of zone", GH_ParamAccess.item);
            pManager.AddNumberParameter("Opaque envelope area", "A_op [m^2]", "Aussenwandfläche (opak) [m^2] - oapque envelope surface area", GH_ParamAccess.item);
            pManager.AddNumberParameter("Window area", "A_w [m^2]", "Fensterfläche [m^2] - window surface area", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value opaque", "U_op [W/(m^2K)]", "Wärmedurchgangskoeffizient Aussenwand [W/(m^2K)] - U value of opaque envelope surfaces", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value window", "U_w [W/(m^2K)]", "Wärmedurchgangskoeffizient Fenster [W/(m^2K)] - U value of windows", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volumetric air flow rate", "V̇_e [m3/h]", "Aussenluft-Volumenstrom [m^3/h] - exterior volumetric air flow rate", GH_ParamAccess.item);
            pManager.AddNumberParameter("Infiltration", "V̇_inf [m^3/h]", "Aussenluft-Volumenstrom durch Infiltration [m^3/h] - volume air flow rate through infiltration", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heat recovery", "η_rec [-]", "Nutzungsgrad der Wärmerückgewinnung [-] - heat recovery", GH_ParamAccess.item);
            pManager.AddNumberParameter("Occupation internal heat gains ", "φ_P [W]", "Wärmeabgabe Personen [W] - internal heat gains from occupation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lighting internal heat gains", "φ_L [W]", "Wärmeabgabe Beleuchtung [W] - internal heat gains from lighting", GH_ParamAccess.item);
            pManager.AddNumberParameter("Equipment internal heat gains", "φ_G [W]", "Wärmeabgabe Geräte [W] - internal heat gains from equipment", GH_ParamAccess.item);
            pManager.AddNumberParameter("Occupation load hours ", "t_P [h]", "Vollaststunden Personen [h] - occupation load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lighting load hours", "t_L [h]", "Vollaststunden Beleuchtung [h] - lighting load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("Equipment load hours", "t_A [h]", "Vollaststunden Geräte [h] - equipment load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("g Value", "g [-]", "g-Wert [-] - g-Value windows", GH_ParamAccess.item);
            pManager.AddNumberParameter("g Value with sunscreen", "g_total [-]", "g-Wert total [-] - g-Value total of windows including sunscreen", GH_ParamAccess.item);
            pManager.AddNumberParameter("Reduction factor of solar gains", "f_sh [-]", "Reduktionsfaktor solare Wärmeeinträge [-] - reduction factor of solar gains (proxy for e.g. obstructions)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shading setpoint", "G_T [W/m^2]", "Strahlungsleistung fuer Betaetigung Sonnenschutz [W/m^2] - shading setpoint for activating the sunscreen", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string Room_string = "";
            if (!DA.GetData(0, ref Room_string)) return;
            //Room_Area is assigned below
            int Month = new int();
            if (!DA.GetData(2, ref Month)) return;

            Sia2024Record Room = Sia2024Record.FromJson(Room_string);

            int summerstart = 3;
            int summerend = 10;
            List<int> dayspermonth = new List<int> { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            // length of calculation period (hours per month) [h]
            List<double> t = new List<double> { 744.0, 672.0, 744.0, 720.0, 744.0, 720.0, 744.0, 744.0, 720.0, 744.0, 720.0, 744.0 };

            double Room_Area = new double();
            if (!DA.GetData(1, ref Room_Area))
            {
                Room_Area = Room.FloorArea;
            };

            double Temperature_setpoint;

            string Season = "";
            if (!DA.GetData(3, ref Season))
            {
                if (summerstart <= Month && Month <= summerend)
                {
                    Temperature_setpoint = Room.CoolingSetpoint;
                }
                else
                {
                    Temperature_setpoint = Room.HeatingSetpoint;
                }
            }
            else
            {
                if (Season.ToLower() == "summer")
                {
                    Temperature_setpoint = Room.CoolingSetpoint;
                }
                else
                {
                    Temperature_setpoint = Room.HeatingSetpoint;
                }
            }

            var Time_constant = Room.RoomConstant;
            var Calculation_period = t[Month - 1];
            var A_th = Room.EnvelopeArea;
            var Window_area = A_th * (Room.GlazingRatio / 100.0);
            var Opaque_envelope_area = A_th - Window_area;
            var U_value_opaque = Room.UValueOpaque;
            var U_value_window = Room.UValueTransparent;
            var Volumetric_air_flow_rate = Room.AirChangeRate;
            var Infiltration = Room.Infiltration;
            var Heat_recovery = Room.HeatRecovery;
            var Occupation_internal_heat_gains = Room.OccupantLoads;
            var Lighting_internal_heat_gains = Room.LightingLoads;
            var Equipment_internal_heat_gains = Room.EquipmentLoads;
            var Occupation_load_hours = Room.OccupantYearlyHours;
            var Lighting_load_hours = Room.LightingYearlyHours;
            var Equipment_load_hours = Room.EquipmentYearlyHours;
            var g_Value = Room.GValue;
            var g_Value_with_sunscreen = Room.GValueTotal;
            var Reduction_factor_of_solar_gains = 0.9;  // sia2024, p.12, 1.3.1.9 Reduktion solare Wärmeeinträge
            var Shading_setpoint = Room.ShadingSetpoint;

            var Occupation_load_hours_list = Enumerable.Repeat(Occupation_load_hours, 12).ToList();
            var Lighting_load_hours_list = Enumerable.Repeat(Lighting_load_hours, 12).ToList();
            var Equipment_load_hours_list = Enumerable.Repeat(Equipment_load_hours, 12).ToList();

            for (int i = 0; i < dayspermonth.Count(); i++)
            {
                Occupation_load_hours_list[i] *= dayspermonth[i] / 365.0;
                Lighting_load_hours_list[i] *= dayspermonth[i] / 365.0;
                Equipment_load_hours_list[i] *= dayspermonth[i] / 365.0;
            }

            DA.SetData(0, Time_constant);
            DA.SetData(1, Temperature_setpoint);
            DA.SetData(2, Calculation_period);
            DA.SetData(3, Opaque_envelope_area);
            DA.SetData(4, Window_area);
            DA.SetData(5, U_value_opaque);
            DA.SetData(6, U_value_window);
            DA.SetData(7, Volumetric_air_flow_rate * Room_Area);
            DA.SetData(8, Infiltration * Room_Area);
            DA.SetData(9, Heat_recovery);
            DA.SetData(10, Occupation_internal_heat_gains * Room_Area);
            DA.SetData(11, Lighting_internal_heat_gains * Room_Area);
            DA.SetData(12, Equipment_internal_heat_gains * Room_Area);
            DA.SetData(13, Occupation_load_hours_list[Month - 1]);
            DA.SetData(14, Lighting_load_hours_list[Month - 1]);
            DA.SetData(15, Equipment_load_hours_list[Month - 1]);
            DA.SetData(16, g_Value);
            DA.SetData(17, g_Value_with_sunscreen);
            DA.SetData(18, Reduction_factor_of_solar_gains);
            DA.SetData(19, Shading_setpoint);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Demand_SIARoomReader;

        public override Guid ComponentGuid => new Guid("48162e29-ec57-4882-a7fc-a46fdf26dbd9");
    }
}
