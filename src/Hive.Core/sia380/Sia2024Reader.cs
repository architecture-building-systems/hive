
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.GhDemand
{
    public class Sia2024Reader : GH_Component
    {
        public Sia2024Reader()
          : base("SIA 2024 Room Reader", "Sia2024Reader",
              "Reads room values and properties from a SIA 2024 Json",
              "[hive]", "Demand")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddStringParameter("Room Json", "RoomJson", "Room type according to sia2024. Should be a dictionary in json format containing all properties. Must comply to hive sia2024 json format", GH_ParamAccess.item);
            pManager.AddNumberParameter("Room Area", "Area", "Room Area in mÂ². If no input is given, default values are taken from SIA2024:2015 (Json dictionary of the parameter above).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Month", "Month", "Calculation month, 1 - 12", GH_ParamAccess.item);
            pManager.AddTextParameter("Season", "Season", "Season, either 'winter' or 'summer'. Determines the room temperature setpoints of SIA 2024", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Time constant", "Ï„ [h]", "Zeitkonstante des GebÃ¤udes [h] - time constant, representing thermal latency of the zone", GH_ParamAccess.item);
            pManager.AddNumberParameter("Temperature setpoint", "Î¸_i [Â°C]", "Raumlufttemperatur [Â°C] - temperature setpoint", GH_ParamAccess.item);
            pManager.AddNumberParameter("Calculation period", "t [h]", "LÃ¤nge der Berechnungsperiode [h] - calculation period, representing usage of zone", GH_ParamAccess.item);
            pManager.AddNumberParameter("Opaque envelope area", "A_op [m^2]", "AussenwandflÃ¤che (opak) [m^2] - oapque envelope surface area", GH_ParamAccess.item);
            pManager.AddNumberParameter("Window area", "A_w [m^2]", "FensterflÃ¤che [m^2] - window surface area", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value opaque", "U_op [W/(m^2K)]", "WÃ¤rmedurchgangskoeffizient Aussenwand [W/(m^2K)] - U value of opaque envelope surfaces", GH_ParamAccess.item);
            pManager.AddNumberParameter("U value window", "U_w [W/(m^2K)]", "WÃ¤rmedurchgangskoeffizient Fenster [W/(m^2K)] - U value of windows", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volumetric air flow rate", "V̇_e [m3/h]", "Aussenluft-Volumenstrom [m^3/h] - exterior volumetric air flow rate", GH_ParamAccess.item);
            pManager.AddNumberParameter("Infiltration", "V̇_inf [m^3/h]", "Aussenluft-Volumenstrom durch Infiltration [m^3/h] - volume air flow rate through infiltration", GH_ParamAccess.item);
            pManager.AddNumberParameter("Heat recovery", "Î·_rec [-]", "Nutzungsgrad der WÃ¤rmerÃ¼ckgewinnung [-] - heat recovery", GH_ParamAccess.item);
            pManager.AddNumberParameter("Occupation internal heat gains ", "Ï†_P [W]", "WÃ¤rmeabgabe Personen [W] - internal heat gains from occupation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lighting internal heat gains", "Ï†_L [W]", "WÃ¤rmeabgabe Beleuchtung [W] - internal heat gains from lighting", GH_ParamAccess.item);
            pManager.AddNumberParameter("Equipment internal heat gains", "Ï†_G [W]", "WÃ¤rmeabgabe GerÃ¤te [W] - internal heat gains from equipment", GH_ParamAccess.item);
            pManager.AddNumberParameter("Occupation load hours ", "t_P [h]", "Vollaststunden Personen [h] - occupation load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lighting load hours", "t_L [h]", "Vollaststunden Beleuchtung [h] - lighting load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("Equipment load hours", "t_A [h]", "Vollaststunden GerÃ¤te [h] - equipment load hours", GH_ParamAccess.item);
            pManager.AddNumberParameter("g Value", "g [-]", "g-Wert [-] - g-Value windows", GH_ParamAccess.item);
            pManager.AddNumberParameter("g Value with sunscreen", "g_total [-]", "g-Wert total [-] - g-Value total of windows including sunscreen", GH_ParamAccess.item);
            pManager.AddNumberParameter("Reduction factor of solar gains", "f_sh [-]", "Reduktionsfaktor solare WÃ¤rmeeintrÃ¤ge [-] - reduction factor of solar gains (proxy for e.g. obstructions)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shading setpoint", "G_T [W/m^2]", "Strahlungsleistung fuer Betaetigung Sonnenschutz [W/m^2] - shading setpoint for activating the sunscreen", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Dictionary<string, object> Room_Json = new Dictionary<string, object>();
			if (!DA.GetData(0, ref Room_Json)) return;
            double Room_Area = new double();
			if (!DA.GetData(1, ref Room_Area)) return;
            int Month = DA.GetData(2, ref Month) ?? 1;
            string Season = new string();
			if (!DA.GetData(3, ref Season)) return;
            

            var Time_constant = new double();
            var Temperature_setpoint = new double();
            var Calculation_period = new double();
            var Opaque_envelope_area = new double();
            var Window_area = new double();
            var U_value_opaque = new double();
            var U_value_window = new double();
            var Volumetric_air_flow_rate = new double();
            var Infiltration = new double();
            var Heat_recovery = new double();
            var Occupation_internal_heat_gains_ = new double();
            var Lighting_internal_heat_gains = new double();
            var Equipment_internal_heat_gains = new double();
            var Occupation_load_hours_ = new double();
            var Lighting_load_hours = new double();
            var Equipment_load_hours = new double();
            var g_Value = new double();
            var g_Value_with_sunscreen = new double();
            var Reduction_factor_of_solar_gains = new double();
            var Shading_setpoint = new double();

            // TODO

            DA.SetData(0, Time_constant);
            DA.SetData(1, Temperature_setpoint);
            DA.SetData(2, Calculation_period);
            DA.SetData(3, Opaque_envelope_area);
            DA.SetData(4, Window_area);
            DA.SetData(5, U_value_opaque);
            DA.SetData(6, U_value_window);
            DA.SetData(7, Volumetric_air_flow_rate);
            DA.SetData(8, Infiltration);
            DA.SetData(9, Heat_recovery);
            DA.SetData(10, Occupation_internal_heat_gains_);
            DA.SetData(11, Lighting_internal_heat_gains);
            DA.SetData(12, Equipment_internal_heat_gains);
            DA.SetData(13, Occupation_load_hours_);
            DA.SetData(14, Lighting_load_hours);
            DA.SetData(15, Equipment_load_hours);
            DA.SetData(16, g_Value);
            DA.SetData(17, g_Value_with_sunscreen);
            DA.SetData(18, Reduction_factor_of_solar_gains);
            DA.SetData(19, Shading_setpoint);
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.demand_SIAroomreader.png;


        public override Guid ComponentGuid => new Guid("99dd97f1-b633-44e9-866c-6b6dc3bb8ae0"); 
       
    }
}
