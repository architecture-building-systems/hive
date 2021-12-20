using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using Grasshopper;
using Hive.IO;
using System.Collections;
using Grasshopper.Kernel.Data;

//using dt = datatree;

namespace Hive.Core
{
    public class Sia380
    {
        /// <summary>
        /// Monthly heating, cooling and electricity demand calculation according to SIA 380 with some modifications.
        /// Equations from 'Interaktion Struktu, Klima und Wärmebedarf_HS20.xlsx', 19.10.2020
        ///
        /// heating and cooling demand: SIA 380.1
        /// thermal balance depends on individual surfaces, i.e.each building surface(both transparent and opaque) can have
        /// individual proporties(U-value, g-value, infiltration, etc.)
        ///
        /// temperature set-point must be an input, e.g.from an adaptive thermal comfort module or taken from the room properties(SIA 2024).
        ///
        /// electricity demand: currently simply by using sqm and internal loads for lighting and equipment
        /// </summary>

        public Sia380()
        {

        }

        #region Constants
        readonly int MonthsPerYear = Misc.MonthsPerYear;
        readonly int DaysPerWeek = Misc.DaysPerWeek;
        readonly int[] DAYS_PER_MONTH = Misc.DaysPerMonth;
        readonly int DAYS_PER_YEAR = Misc.DaysPerYear;
        readonly int HOURS_PER_DAY = Misc.HoursPerDay;
        readonly int[] HOURS_PER_MONTH = Misc.HoursPerMonth;
        readonly int HOURS_PER_YEAR = Misc.HoursPerYear;
        const double SECONDS_PER_HOUR = 3600.0;
        const double CELCIUS_TO_KELVIN = 273.15;
        const double GRAVITATIONAL_CONSTANT = 9.8;
        const double RHO = 1.2;
        const double C_P = 1005;
        #endregion

        #region Parameters
        private readonly double H_V;
        private readonly bool use_natural_ventilation;
        private bool use_fixed_time_constant;
        private double floor_area;
        Dictionary<string, double> UValues;
        double TauFixed;
        double Cm;
        double H_V_no_heat_recovery;
        double Vdot_nat_vent_constant;
        double H_T_op;
        double H_T_tr;
        double H_T;
        #endregion Parameters

        #region Results
        public double[] Q_Heat { get; private set; }
        public double[] Q_Cool { get; private set; }
        public double[] Q_Elec { get; private set; }
        public double[] Q_DHW { get; private set; }

        public double[] Q_V_out { get; private set; }
        public double[] Q_T_out { get; private set; }

        public double[] Q_i_out { get; private set; }
        public double[] Q_s_out { get; private set; }

        public double[] Q_T_opaque_out { get; private set; }
        public double[] Q_T_transparent_out { get; private set; }

        public DataTree<double> Q_s_tr_tree { get; private set; }
        #endregion Results

        // 
        //     Computes monthly heating, cooling and electricity demand for a thermal zone, based on SIA 380.1
        //     :param room_properties: room properties in json format
        //     :param room_schedules: room schedules in json format
        //     :param floor_area: Floor area of the room in m2
        //     :param T_e_hourly: Hourly ambient air temperature in degree Celsius
        //     :param T_i_ub_hourly: Upper bound for temperature setpoints, hourly.
        //     :param T_i_lb_hourly: Lower bound for temperature setpoints, hourly.
        //     :param surface_areas: building surface areas that are used for fabric gains/losses computation
        //     :param surface_type: indicator if this surface is transparent or not. if yes, it will be assigned the transparent construction from room properties. 'opaque' for opaque or 'transp' for transparent
        //     :param srf_irrad_obstr_tree: hourly solar irradiation in W/m2 per building surface. Jagged array [months_per_year][num_srfs] or [hours_per_year][num_srfs]. Must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
        //     :param srf_irrad_unobstr_tree: hourly solar irradiation in W/m2 per building surface. Jagged array [months_per_year][num_srfs] or [hours_per_year][num_srfs]. Must be in correct order with the next 2 parameters 'surface_areas' and 'surface_is_transparent'
        //     :param g_value: G value of windows.
        //     :param g_value_total: G value total including sunscreen ('Sonnenschutz') of windows
        //     :param setpoint_shading: Shading setpoint for activating sunscreen of windows, in W/m^2
        //     :param run_obstructed_simulation: Boolean to indicate if an obstructed solar simulation is conducted. True if yes.
        //     :param hourly: Boolean to indicate if hourly values should be returned instead of monthly. True if yes.
        //     :param use_adaptive_comfort: Boolean to indicate if adaptive comfort should be used instead of fixed setpoints. True if yes. Defaults to yes if setpoints_ub and setpoints_lb are null.
        //     :param use_natural_ventilation: Boolean to indicate if natural ventilation should be considered (True) or not (False).
        //     :param use_fixed_time_constant: Boolean to indicate if fixed time constant should be uesd from SIA 2024 (True) or dynamically calculated based on room capacitance (False).
        //    
        //     :return: Monthly or hourly cooling, heating and electricity loads for a thermal zone
        //     
        public void Run(
            Dictionary<string, object> room_properties,
            Dictionary<string, object> room_schedules,
            double floor_area,
            double[] T_e_hourly,
            List<double> T_i_ub_hourly,
            List<double> T_i_lb_hourly,
            double[] surface_areas,
            string[] surface_type,
            DataTree<double> srf_irrad_obstr_tree,
            DataTree<double> srf_irrad_unobstr_tree,
            double g_value,
            double g_value_total,
            double setpoint_shading,
            bool run_obstructed_simulation,
            bool hourly,
            bool use_adaptive_comfort,
            bool use_natural_ventilation,
            bool use_fixed_time_constant,
            bool debug = false)
        {
            // List of equations

            // Heizwärmebedarf (in Wh):
            // Q_H = Q_T + Q_V - eta_g * (Q_i + Q_s)

            // Ausnutzungsgrad für Wärmegewinne (in -):
            // eta_g = (1 - gamma^a) / (1 - gamma^(a+1))
            // a = 1 + tau / 15

            // Wärmeeintrag/-verlust-Verhältnis (in -):
            // gamma = (Q_i + Q_s) / (Q_T + Q_V)

            // Zeitkonstante (in h):
            // tau = C_A_e * A / (H_V + H_T)

            //Transmissionswärmeverluste (in Wh):
            //Q_T = H_T * (T_i - T_e) * t

            //Transmissions-Wärmetransferkoeffizient (in W/K):
            //H_T = A_op * U_op + A_w * U_w

            //Interne Wärmeeinträge (in Wh):
            //Q_i = Phi_P * t_P + Phi_L * t_L + Phi_A * t_A

            //Lüftungswärmeverluste (in Wh):
            //Q_V = H_V * (T_i - T_e) * t

            //Lüftungs-Wärmetransferkoefizient (in W/K):
            //H_V = Vdot_th * rho * c_p

            //Thermisch wirksamer Aussenluftvolumenstrom (in m^3/h):
            //Vdot_th = Vdot_e * (1 - eta_rec) + Vdot_inf
            //";
            //        @"
            //Nomenclature:

            //[Q_H] = Wh (Heizwärmebedarf)
            //[Q_T] = Wh (Transmissionswärmeverluste)
            //[Q_V] = Wh (Lüftungswärmeverluste)
            //[Q_i] = Wh (interne Wärmeeinträge)
            //[Q_s] = Wh (solare Wärmeeinträge)

            //[H_V] = W/K (Lüftungs-Wärmetransferkoefizient) 
            //[H_T] = W/K (Transmissions-Wärmetransferkoeffizient)

            //[Phi_P] = W (Wärmeabgabe der Personen)
            //[Phi_L] = W (Wärmeabgabe der Beleuchtung)
            //[Phi_A] = W (Wärmeabgabe der Geräte)
            //[t_P] = h (Volllaststunden Personen)
            //[t_L] = h (Volllaststunden Beleuchtung)
            //[t_A] = h (Volllaststunden Geräte)    

            //[gamma] = - (Wärmeeintrag/-verlust-Verhältnis)
            //[tau] = h (Zeitkonstante des Gebäudes)
            //[C_A_e] = W/(m^2K) (Waermespeicherfaehigkeit pro Energiebezugsflaeche (SIA 2024: Waermespeicherfaehigkeit des Raumes))
            //[t] = h (Länge der Berechnungsperiode)

            //[Vdot_th] = m^3/h (Thermisch wirksamer Aussenluftvolumenstrom)
            //[Vdot_e] = m^3/h (Aussenluftvolumenstrom durch Lüftung)
            //[Vdot_inf] = m^3/h (Aussenluftvolumenstrom durch Infiltration)

            //[eta_rec] = - (Nutzungsgrad der Wärmerückgewinnung) 
            //[eta_g] = - (Ausnutzungsgrad für Wärmegewinne)      

            //[rho] = kg/m^3 (Luftdichte)
            //[c_p] = J/(kgK) (Spez. Wärmekapazität von Luft)

            //[T_i] = K oder °C (Raumlufttemperatur)
            //[T_e] = K oder °C (Aussenlufttemperatur)

            //[A_op] = m^2 (opaque surface area) 
            //[A_w] = m^2 (windows surface area)
            //[U_op] = W/(m^2K) (U-value opaque surface)
            //[U_w] = W/(m^2K) (U-value window surface)  
            //";
            //        @"
            //SIA 2024 variables:

            //tau     Zeitkonstante des Gebäudes [h]
            //C_m     Waermespeicherfaehigkeit des Raumes [Wh/m2K]
            //theta_e Aussenlufttemperatur
            //theta_i Raumlufttemperatur
            //t       Länge der Berechnungsperiode [h]
            //A_th    Thermische Gebäudehüllfläche [m2] 
            //A_w     Fensterfläche [m2]                      !!!!!! f_g in sia2024 - Glasanteil in [%]
            //U_op    Wärmedurchgangskoeffizient Aussenwand [W/m2K]
            //U_w     Wärmedurchgangskoeffizient Fenster [W/m2K]
            //Vdot_e_spec  Aussenluft-Volumenstrom [m3/m2h]
            //Vdot_inf_spec Aussenluft-Volumenstrom durch Infiltration [m3/m2h]
            //eta_rec Nutzungsgrad der Wärmerückgewinnung [-]
            //phi_P   Wärmeabgabe Personen [W/m2]
            //phi_L   Wärmeabgabe Beleuchtung [W/m2]
            //phi_A   Wärmeabgabe Geräte [W/m2]
            //t_P     Vollaststunden Personen [h]
            //t_L     Vollaststunden Beleuchtung [h]
            //t_A     Vollaststunden Geräte [h]
            //g       g-Wert [-]
            //f_sh    Reduktionsfaktor solare Wärmeeinträge [-]
            //I       description"": ""Solare Strahlung [Wh/m2]
            //";
            //        @"
            //cases according to SIA 2024:2015.
            //The norm defines following ROOM and/or BUILDING types (p.7-8):
            //    _____________________________________________________________________________________
            //          | abbr.     | description                                    |    code SIA 380
            //    ______|___________|________________________________________________|_________________
            //    - 1.1   mfh:        multi-family house (Mehrfamilienhaus)                   HNF1
            //    - 1.2   efh:        single family house (Einfamilienhaus)                   HNF1
            //    - 2.1   ...:        Hotelroom                                               HNF1
            //    - 2.2   ...:        Hotel lobby                                             HNF1
            //    - ...   ...:
            //    - 3.1   office:     small office space (Einzel,- Gruppenbüro)               HNF2
            //    - ...   ...:
            //    - 4.1   school:     school room (Schulzimmer)                               HNF5
            //    - ...   ...:
            //    _____________________________________________________________________________________
            //


            double[] Q_dhw;
            double Vdot_nat_vent_constant;
            List<List<double>> Q_s_tr_per_surface_hourly;
            double[] T_i_lb;
            double[] T_i_ub;

            double[] t_P_hourly = new double[HOURS_PER_YEAR];
            double[] t_A_hourly = new double[HOURS_PER_YEAR];
            double[] t_L_hourly = new double[HOURS_PER_YEAR];
            double[] t_S_hourly = new double[HOURS_PER_YEAR];

            double Phi_P_multiplier = 0.0;
            double Phi_L_multiplier = 0.0;
            double Phi_A_multiplier = 0.0;

            double Phi_P_hourly = 0.0;
            double Phi_A_hourly = 0.0;
            double Phi_L_hourly = 0.0;

            //if (debug)
            //{
            //    DEBUG_MODE = true;
            //}

            // PARSE INPUTS

            // Assert inputs hourly
            var adaptive_comfort = use_adaptive_comfort && (T_i_ub_hourly != null && T_i_lb_hourly != null);

            if (adaptive_comfort)
            {
                if (T_i_ub_hourly.Count() != HOURS_PER_YEAR) throw new ArgumentException($"'setpoints_ub' (Setpoints upper bound) must be hourly. Length was {T_i_ub_hourly.Count}");
                if (T_i_lb_hourly.Count() != HOURS_PER_YEAR) throw new ArgumentException($"'setpoints_lb' (Setpoints lower bound) must be hourly. Length was {T_i_lb_hourly.Count}");
            }

            if (T_e_hourly.Count() != HOURS_PER_YEAR) throw new ArgumentException($"'T_e' (Ambient temperature) must be hourly. Length was {T_e_hourly.Count}");
            if (run_obstructed_simulation && srf_irrad_obstr_tree == null) throw new ArgumentException("Q_s_tree is None but solar gains set to run using obstructued solar gains.");

            // read room properties from sia2024
            room_properties = cleanDictForNaN(room_properties);

            // f_sh = 0.9  # sia2024, p.12, 1.3.1.9 Reduktion solare Wärmeeinträge
            // g = room_properties["Gesamtenergiedurchlassgrad Verglasung"]
            TauFixed = (double)room_properties["Zeitkonstante"];
            Cm = (double)room_properties["Waermespeicherfaehigkeit des Raumes"];
            // U_value_opaque = room_properties["U-Wert opake Bauteile"]
            // U_value_transparent = room_properties["U-Wert Fenster"]
            var U_value_floors = (double)room_properties["U-Wert Boeden"];
            var U_value_roofs = (double)room_properties["U-Wert Daecher"];
            var U_value_walls = (double)room_properties["U-Wert Waende"];
            var U_value_windows = (double)room_properties["U-Wert Fenster"];
            UValues = new Dictionary<string, double> {
                { "floor", U_value_floors},
                { "roof", U_value_roofs},
                { "wall", U_value_walls},
                { "window", U_value_windows}
            };
            var Vdot_e_spec = (double)room_properties["Aussenluft-Volumenstrom (pro NGF)"];
            var Vdot_inf_spec = (double)room_properties["Aussenluft-Volumenstrom durch Infiltration"];
            var eta_rec = (double)room_properties["Temperatur-Aenderungsgrad der Waermerueckgewinnung"];
            var Phi_P_per_m2 = (double)room_properties["Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)"];
            var Phi_L_per_m2 = (double)room_properties["Waermeeintragsleistung der Raumbeleuchtung"];
            var Phi_A_per_m2 = (double)room_properties["Waermeeintragsleistung der Geraete"];
            var Q_dhw_year = (double)room_properties["Jaehrlicher Waermebedarf fuer Warmwasser"];
            var Q_dhw_hourly = Q_dhw_year / HOURS_PER_YEAR * floor_area;
            var Q_dhw_monthly = (from h in HOURS_PER_MONTH
                                 select (Q_dhw_hourly * h)).ToList();

            // assign room properties to individual surfaces
            //    surface_type = ["opaque", "opaque", "transp", "transp"]
            //    surface_areas = [44.0, 62.3, 4.0, 5.2]
            var num_surfaces = surface_type.Count();
            
            // Average out the hours of occupancy, lighting, appliances
            var P_total_hours = (double)room_properties["Vollaststunden pro Jahr (Personen)"];
            var L_total_hours = (double)room_properties["Jaehrliche Vollaststunden der Raumbeleuchtung"];
            var A_total_hours = (double)room_properties["Jaehrliche Vollaststunden der Geraete"];
            
            if (hourly)
            {
                var _tup_1 = get_hourly_schedules(room_schedules);
                t_P_hourly = _tup_1.Item1;
                t_A_hourly = _tup_1.Item2;
                t_L_hourly = _tup_1.Item3;
                t_S_hourly = _tup_1.Item4;
                
                // Multiplier to adjust kWh/m2 based on how many yearly hours from SIA 2024 vs how many yearly hours from schedule generator
                Phi_P_multiplier = P_total_hours / t_P_hourly.Sum();
                Phi_L_multiplier = L_total_hours / t_L_hourly.Sum();
                Phi_A_multiplier = A_total_hours / t_A_hourly.Sum();
            }

            // transforming daily sia2024 data to monthly
            var DAYS_PER_YEAR_double = (double)DAYS_PER_YEAR;
            var t_P = DAYS_PER_MONTH.Select(d => P_total_hours * d / DAYS_PER_YEAR_double).ToArray();
            var t_L = DAYS_PER_MONTH.Select(d => L_total_hours * d / DAYS_PER_YEAR_double).ToArray();
            var t_A = DAYS_PER_MONTH.Select(d => A_total_hours * d / DAYS_PER_YEAR_double).ToArray();

            // IF setpoints given monthly for hourly simulation, they stay the same for entire month regardless of T_e.
            // Use fixed setpoints / setbacks from room properties if no adaptive comfort
            if (adaptive_comfort)
            {
                T_i_ub = get_monthly_avg(T_i_ub_hourly);
                T_i_lb = get_monthly_avg(T_i_lb_hourly);
            }
            else
            {
                var setpoint_ub = (double)room_properties["Raumlufttemperatur Auslegung Kuehlung (Sommer)"];
                var setpoint_lb = (double)room_properties["Raumlufttemperatur Auslegung Heizen (Winter)"];
                T_i_ub = Enumerable.Repeat(setpoint_ub, MonthsPerYear).ToArray();
                T_i_lb = Enumerable.Repeat(setpoint_lb, MonthsPerYear).ToArray();
                if (hourly)
                {
                    T_i_ub_hourly = new List<double>();
                    T_i_lb_hourly = new List<double>();
                    var setback_ub = (double)room_properties["Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur"];
                    var setback_lb = (double)room_properties["Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur"];
                    foreach (var day in Enumerable.Range(0, DAYS_PER_YEAR))
                    {
                        foreach (var hour in Enumerable.Range(0, HOURS_PER_DAY))
                        {
                            if (t_S_hourly[hour] == 1)
                            {
                                T_i_ub_hourly.Add(setpoint_ub);
                                T_i_lb_hourly.Add(setpoint_lb);
                            }
                            else if (t_S_hourly[hour] == 0.5)
                            {
                                T_i_ub_hourly.Add(setback_ub);
                                T_i_lb_hourly.Add(setback_lb);
                            }
                            else
                            {
                                // TODO should be NaN? For now keep setpoint
                                T_i_ub_hourly.Add(setpoint_ub);
                                T_i_lb_hourly.Add(setpoint_lb);
                            }
                        }
                    }
                }
            }

            // Monthly averaged T_e
            var T_e = get_monthly_avg(T_e_hourly);

            // Windows
            var windows_areas = new List<double>();
            for (int i = 0; i < surface_areas.Count(); i++)
            {
                if (surface_type[i] == "window") windows_areas.Add(surface_areas[i]);
            }
            int windows_count = windows_areas.Count;

            // Solar
            // formatting the grasshopper tree that contains solar irradiation time series for each window
            // could be changed later to also include solar irradiation on opaque surfaces...
            // ...would need to be adapted in the 'for surface in range(num_surfaces):' loop as well then
            object Q_s_tr_per_surface = null;
            object Q_s_tr_per_surface_jagged = null;
            object Q_s_tr_per_surface_jagged_hourly = null;

            if (windows_count == 0 || srf_irrad_obstr_tree.Branch(0).Count == 0 && srf_irrad_unobstr_tree.BranchCount == 0)
            {
                Q_s_tr_per_surface = new DataTree<double>();
                //{
                //    new List<object> {
                //        0.0
                //    } * windows_count
                //} * MonthsPerYear;

                Q_s_tr_per_surface_hourly = Enumerable.Repeat(Enumerable.Repeat(0.0, windows_count).ToList(), HOURS_PER_YEAR).ToList();
            }
            else
            {
                if (srf_irrad_unobstr_tree.BranchCount != windows_count)
                    throw new ArgumentException($"The number of branches for the solar radiation tree ({srf_irrad_unobstr_tree.BranchCount}) does not match the number of windows ({windows_count})");
                // Monthly
                Q_s_tr_per_surface_jagged = calculate_Q_s(run_obstructed_simulation, srf_irrad_obstr_tree, srf_irrad_unobstr_tree, g_value, g_value_total, setpoint_shading, windows_areas, hourly: false);
                // Transpose to per timestep
                Q_s_tr_per_surface = transpose_jagged_2D_array(Q_s_tr_per_surface_jagged);
                if (hourly)
                {
                    Q_s_tr_per_surface_jagged_hourly = calculate_Q_s(run_obstructed_simulation, srf_irrad_obstr_tree, srf_irrad_unobstr_tree, g_value, g_value_total, setpoint_shading, windows_areas, hourly: true);
                    // Transpose to per timestep
                    Q_s_tr_per_surface_hourly = transpose_jagged_2D_array(Q_s_tr_per_surface_jagged_hourly);
                }
            }

            // Outputs
            // calculations from Illias Excel sheet:
            var time_range = hourly ? HOURS_PER_YEAR : MonthsPerYear;
            Q_i_out = new double[time_range];
            Q_s_out = new double[time_range];
            Q_V_out = new double[time_range];
            Q_T_out = new double[time_range];
            Q_T_opaque_out = new double[time_range];
            Q_T_transparent_out = new double[time_range];
            Q_Heat = new double[time_range];
            Q_Cool = new double[time_range];
            Q_Elec = new double[time_range];

            var Phi_P = Phi_P_per_m2 * floor_area;
            var Phi_L = Phi_L_per_m2 * floor_area;
            var Phi_A = Phi_A_per_m2 * floor_area;
            if (hourly)
            {
                Phi_P_hourly = Phi_P * Phi_P_multiplier;
                Phi_L_hourly = Phi_L * Phi_L_multiplier;
                Phi_A_hourly = Phi_A * Phi_A_multiplier;
            }

            // External air flowrate (thermisch wirksamer Aussenluftvolumenstrom)
            var Vdot_e = Vdot_e_spec * floor_area;
            var Vdot_inf = Vdot_inf_spec * floor_area;
            var Vdot_th = Vdot_e * (1.0 - eta_rec) + Vdot_inf;
            var Vdot_th_no_heat_recovery = Vdot_e + Vdot_inf;

            // Ventilation heat loss coefficient (Lüftungs-Wärmetransferkoeffizient), H_V
            // m3/h to m3/s, hence divide by 3600
            var H_V = Vdot_th / SECONDS_PER_HOUR * RHO * C_P;
            var H_V_no_heat_recovery = Vdot_th_no_heat_recovery / SECONDS_PER_HOUR * RHO * C_P;

            // Transmission heat transfer coefficient (Transmissions-Wärmetransferkoeffizient), H_T
            var H_T = 0.0;
            var H_T_op = 0.0;
            var H_T_tr = 0.0;
            foreach (var _tup_3 in zip(surface_type, surface_areas))
            {
                var s_type = _tup_3.Item1;
                var s_area = _tup_3.Item2;
                var h_t = s_area * UValues[s_type];
                if (s_type == "window")
                {
                    H_T_tr += h_t;
                }
                else
                {
                    H_T_op += h_t;
                }
                H_T += h_t;
            }

            // Natural ventilation (no infiltration ? TODO)
            if (use_natural_ventilation && windows_count > 0)
            {
                // TODO assume fixed height of 1.5m as has significant influence on natural ventilation calc.
                var h = 1.5;
                var w = windows_areas.Sum() / h;
                // Precalculate constant part to reduce repetitive computation
                Vdot_nat_vent_constant = calc_nat_vent_Vdot_constant(h, w);
            }
            else
            {
                // no windows
                Vdot_nat_vent_constant = 0;
            }
            
            // For each month, compute gains and losses of the zone
            // for some variables, we compute upper and lower bounds (subscripts ub & lb)
            // furthermore, cooling and heating demand requires us to compute certain variables differently.
            // See equation sheet 'EK1_Formelsammlung_HS20.pdf'
            var end_hour = 0;
            foreach (var month in Enumerable.Range(0, MonthsPerYear))
            {
                if (hourly)
                {
                    // Get the eta_g for the month
                    var eta_g_t = calculate_step(month, T_e, T_i_ub, T_i_lb, 
                        t_P, t_A, t_L, 
                        Phi_P, Phi_A, Phi_L, 
                        Q_s_tr_per_surface, run_hourly: false, eta_g_t: null, only_return_eta_g: true);
                    
                    foreach (var hour in Enumerable.Range(0, HOURS_PER_MONTH[month]))
                    {
                        // Calculate hourly heat flows and demands for that month
                        calculate_step(hour + end_hour, 
                            T_e_hourly, T_i_ub_hourly.ToArray(), T_i_lb_hourly.ToArray(), 
                            t_P_hourly, t_A_hourly, t_L_hourly, 
                            Phi_P_hourly, Phi_A_hourly, Phi_L_hourly, 
                            Q_s_tr_per_surface_hourly, run_hourly: true, eta_g_t: eta_g_t);
                    }
                    
                    end_hour += HOURS_PER_MONTH[month];
                }
                else
                {
                    // Calculate only monthly heat flows and demands
                    calculate_step(month, 
                        T_e, T_i_ub, T_i_lb, 
                        t_P, t_A, t_L, 
                        Phi_P, Phi_A, Phi_L, 
                        Q_s_tr_per_surface, run_hourly: false);
                }
            }

            // Prepare solar datatrees
            if (hourly && Q_s_tr_per_surface_jagged_hourly != null)
            {
                if (DEBUG_MODE)
                {
                    Q_s_tr_tree = Q_s_tr_per_surface_jagged_hourly;
                }
                else
                {
                    Q_s_tr_tree = th.list_to_tree(Q_s_tr_per_surface_jagged_hourly, source: new List<object> {
                        0,
                        0
                    });
                }
            }
            else if (!hourly && Q_s_tr_per_surface_jagged != null)
            {
                if (DEBUG_MODE)
                {
                    Q_s_tr_tree = Q_s_tr_per_surface_jagged;
                }
                else
                {
                    Q_s_tr_tree = th.list_to_tree(Q_s_tr_per_surface_jagged, source: new List<object> {
                        0,
                        0
                    });
                }
            }
            else
            {
                Q_s_tr_tree = null;
            }

            // in kWh per month or per hour
            Q_DHW = hourly ? Enumerable.Repeat(Q_dhw_hourly, HOURS_PER_YEAR).ToArray() : Q_dhw_monthly.ToArray();

            // Finalise results

            Q_Heat = ToKWh(Q_Heat);
            //Q_DHW;
            Q_Cool = ToKWh(Q_Cool);
            Q_Elec = ToKWh(Q_Elec);
            Q_T_out = ToKWh(Q_T_out);
            Q_V_out = ToKWh(Q_V_out);
            Q_i_out = ToKWh(Q_i_out);
            Q_s_out = ToKWh(Q_s_out);
            Q_T_opaque_out = ToKWh(Q_T_opaque_out);
            Q_T_transparent_out = ToKWh(Q_T_transparent_out);
            //Q_s_tr_tree;
        }


        Tuple<double, double, double, double, double, double> calculate_step(int t,
                double[] T_e, double[] T_i_ub, double[] T_i_lb,
                double[] t_P, double[] t_A, double[] t_L,
                double Phi_P, double Phi_A, double Phi_L,
                DataTree<double> Q_s_tr_per_surface,
                bool run_hourly = false,
                Tuple<double,double,double,double,double,double> eta_g_t = null, bool only_return_eta_g = false)
        {
            double eta_g_cooling_no_hr;
            double eta_g_heating_no_hr;
            double eta_g_cooling;
            double eta_g_heating;
            double eta_g;

            double tau_no_hr;
            double tau;

            double Q_V;

            // Nat Vent - assign 0.0 so compiler is happy with all the if(nat_vent) stuff.
            double Vdot_nat_vent_ub = 0.0;
            double Vdot_nat_vent_lb = 0.0;
            double H_V_ub_nat_vent = 0.0;
            double H_V_lb_nat_vent = 0.0;
            double Q_V_lb_nat_vent = 0.0;
            double Q_V_ub_nat_vent = 0.0;
            double tau_cooling_nat_vent = 0.0;
            double tau_heating_nat_vent = 0.0;
            double gamma_lb_nat_vent = 0.0;
            double gamma_ub_nat_vent = 0.0;
            double eta_g_cooling_nat_vent = 0.0;
            double eta_g_heating_nat_vent = 0.0;

            int Q_K_index;
            double Q_K;
            int Q_H_index;
            double Q_H;

            var num_timesteps = run_hourly ? 1 : HOURS_PER_MONTH[t];
            var deltaT_ub = (T_i_ub[t] - T_e[t]) * num_timesteps;
            var deltaT_lb = (T_i_lb[t] - T_e[t]) * num_timesteps;

            // Ventilation losses (Lüftungswärmeverluste)
            // Q_V = H_V * (T_i - T_e) * t
            // we compute with and without heat recovery, ub and lb, and take the respectively best (lowest demand)
            // this assumes, the ventilation system operates ideally and does not, for example, keep warm air in summer
            var Q_V_ub = H_V * deltaT_ub;
            var Q_V_lb = H_V * deltaT_lb;
            var Q_V_ub_no_hr = H_V_no_heat_recovery * deltaT_ub;
            var Q_V_lb_no_hr = H_V_no_heat_recovery * deltaT_lb;

            // Natural ventilation losses (No infiltration or mechanical)
            // Calculates natural ventilation, for now based on single rectangular opening in single zone !
            // V_dot = c_d * H * W * 1/3 * sqrt(g * H * (T_i - T_e)/T_e)
            // H_V_nat_vent = V_dot * rho * c_p
            // Q_V_nat_vent = H_V_nat_vent * (T_i - T_e) * t
            // Assumes:
            //   - Ta and Ti constant  
            //   - No wind influence (only driven by temperature differences) 
            //   - Single zone and single sided ventilation
            if (use_natural_ventilation)
            {
                Vdot_nat_vent_ub = calc_nat_vent_Vdot(Vdot_nat_vent_constant, T_i_ub[t], T_e[t]);
                Vdot_nat_vent_lb = calc_nat_vent_Vdot(Vdot_nat_vent_constant, T_i_lb[t], T_e[t]);
                H_V_ub_nat_vent = calc_nat_vent_H_V(Vdot_nat_vent_ub);
                H_V_lb_nat_vent = calc_nat_vent_H_V(Vdot_nat_vent_lb);
                Q_V_ub_nat_vent = H_V_ub_nat_vent * deltaT_ub;
                Q_V_lb_nat_vent = H_V_lb_nat_vent * deltaT_lb;
            }

            // Internal loads (interne Wärmeeinträge)
            // Q_i = Phi_P * t_P + Phi_L * t_L + Phi_A * t_A
            var Q_i = Phi_P * t_P[t] + Phi_L * t_L[t] + Phi_A * t_A[t];

            // Transmission heat losses, Q_T, both transparent and opaque
            var Q_T_op_ub = H_T_op * deltaT_ub;
            var Q_T_op_lb = H_T_op * deltaT_lb;
            var Q_T_tr_ub = H_T_tr * deltaT_ub;
            var Q_T_tr_lb = H_T_tr * deltaT_lb;

            // Transmission losses (Transmissionswärmeverluste), Q_T, upper and lower bounds for all surfaces
            var Q_T_ub = Q_T_tr_ub + Q_T_op_ub;
            var Q_T_lb = Q_T_tr_lb + Q_T_op_lb;

            // solar gains (solare Wärmeeinträge), Q_s, (PER SURFACE)
            // unobstructed or obstructed, both using SolarModel.dll and GHSolar.gha
            var Q_s_tr = Q_s_tr_per_surface[t].Sum();
            var Q_s = Q_s_tr;

            if (eta_g_t == null)
            {
                // Heatgains/-losses ratio (Wärmeeintrag/-verlust Verhältnis), gamma
                // calculating for different cases, heating / cooling, upper / lower bounds, with / without heat recovery
                var gamma_lb = calc_gamma(Q_i, Q_s, Q_T_lb, Q_V_lb);
                var gamma_ub = calc_gamma(Q_i, Q_s, Q_T_ub, Q_V_ub);
                var gamma_lb_no_hr = calc_gamma(Q_i, Q_s, Q_T_lb, Q_V_lb_no_hr);
                var gamma_ub_no_hr = calc_gamma(Q_i, Q_s, Q_T_ub, Q_V_ub_no_hr);
                if (use_natural_ventilation)
                {
                    gamma_lb_nat_vent = calc_gamma(Q_i, Q_s, Q_T_lb, Q_V_lb_nat_vent);
                    gamma_ub_nat_vent = calc_gamma(Q_i, Q_s, Q_T_ub, Q_V_ub_nat_vent);
                }
                // time constant of the building (Zeitkonstante), tau
                // TODO where does H_T come from...
                if (use_fixed_time_constant)
                {
                    tau = TauFixed;
                    tau_no_hr = TauFixed;
                    tau_heating_nat_vent = TauFixed;
                    tau_cooling_nat_vent = TauFixed;
                }
                else
                {
                    tau = calc_tau(Cm, floor_area, H_V, H_T);
                    tau_no_hr = calc_tau(Cm, floor_area, H_V_no_heat_recovery, H_T);
                    if (use_natural_ventilation)
                    {
                        tau_heating_nat_vent = calc_tau(Cm, floor_area, H_V_lb_nat_vent, H_T);
                        tau_cooling_nat_vent = calc_tau(Cm, floor_area, H_V_ub_nat_vent, H_T);
                    }
                }
                // for debugging, to compare fixed tau
                // tau = 126.0
                // tau_no_hr = tau
                // tau_heating_nat_vent = tau
                // tau_cooling_nat_vent = tau
                // usage of heat gains (Ausnutzungsgrad für Wärmegewinne), eta_g
                eta_g_heating = calc_eta_g(gamma_lb, tau);
                eta_g_cooling = calc_eta_g(gamma_ub, tau, cooling: true);
                eta_g_heating_no_hr = calc_eta_g(gamma_lb_no_hr, tau_no_hr);
                eta_g_cooling_no_hr = calc_eta_g(gamma_ub_no_hr, tau_no_hr, cooling: true);
                if (use_natural_ventilation)
                {
                    eta_g_heating_nat_vent = calc_eta_g(gamma_lb_nat_vent, tau_heating_nat_vent);
                    eta_g_cooling_nat_vent = calc_eta_g(gamma_ub_nat_vent, tau_cooling_nat_vent, cooling: true);
                }
                else
                {
                    // assigned but won't be used later
                    eta_g_heating_nat_vent = 1.0;
                    eta_g_cooling_nat_vent = 1.0;
                }
            }
            else
            {
                // TODO struct instead of tuple...
                eta_g_heating = eta_g_t.Item1;
                eta_g_cooling = eta_g_t.Item2;
                eta_g_heating_no_hr = eta_g_t.Item3;
                eta_g_cooling_no_hr = eta_g_t.Item4;
                eta_g_heating_nat_vent = eta_g_t.Item5;
                eta_g_cooling_nat_vent = eta_g_t.Item6;
            }
            if (only_return_eta_g)
            {
                return Tuple.Create(eta_g_heating, eta_g_cooling, eta_g_heating_no_hr, eta_g_cooling_no_hr, eta_g_heating_nat_vent, eta_g_cooling_nat_vent);
            }
            else
            {
                // calculate demand
                // heating demand (Heizwärmebedarf), Q_H
                // Q_H = Q_T + Q_V - eta_g * (Q_i + Q_s)
                // calculating different cases with/without heat recovery, with/without natural ventilation (hr)
                double Q_H_ = Q_T_lb + Q_V_lb - eta_g_heating * (Q_i + Q_s);
                double Q_H_no_hr = Q_T_lb + Q_V_lb_no_hr - eta_g_heating_no_hr * (Q_i + Q_s);
                double Q_H_nat_vent = Q_T_lb + Q_V_lb_nat_vent - eta_g_heating_nat_vent * (Q_i + Q_s);
                var Q_H_options = negatives_to_zero(new List<double> {
                        Q_H_,
                        Q_H_no_hr,
                        Q_H_nat_vent
                    });
                // cooling demand (Kältebedarf), Q_K
                // calculating different cases with/without heat recovery, with/without natural ventilation (hr)
                var Q_K_ = Q_i + Q_s - eta_g_cooling * (Q_T_ub + Q_V_ub);
                var Q_K_no_hr = Q_i + Q_s - eta_g_cooling_no_hr * (Q_T_ub + Q_V_ub_no_hr);
                var Q_K_nat_vent = Q_i + Q_s - eta_g_cooling_nat_vent * (Q_T_ub + Q_V_ub_nat_vent);
                var Q_K_options = negatives_to_zero(new List<double> {
                        Q_K_,
                        Q_K_no_hr,
                        Q_K_nat_vent
                    });
                // take smaller value of both comfort set points and remember the index
                if (use_natural_ventilation)
                {
                    (Q_H, Q_H_index) = min_and_index(Q_H_options);
                    (Q_K, Q_K_index) = min_and_index(Q_K_options);
                }
                else
                {
                    (Q_H, Q_H_index) = min_and_index(Q_H_, Q_H_no_hr);
                    (Q_K, Q_K_index) = min_and_index(Q_K_, Q_K_no_hr);
                }
#if debug
                if (Q_K > 0.01 && Q_K_index == 2)
                {
                    Console.WriteLine(t.ToString() + " cooled by nat_vent T_e = " + T_e[t].ToString() + " C");
                }
#endif
                // either subtract heating from cooling, but then also account for that in losses/gains by subtracting those too
                // or just take the higher load of both and then take the corresponding losses/gains
                // demand = Q_K + Q_H  # sometimes we have both cooling and heating. so subtract one from another
                // if demand < 0:
                //     Q_Cool[month] = demand * -1
                //     Q_Heat[month] = 0
                // else:
                //     Q_Cool[month] = 0
                //     Q_Heat[month] = demand
                if (Q_K > Q_H)
                {
                    // cooling
                    // make cooling negative
                    Q_Cool[t] = -Q_K;
                    Q_Heat[t] = 0.0;
                    eta_g = new[] { eta_g_cooling, eta_g_cooling_no_hr, eta_g_cooling_nat_vent }[Q_K_index];
                    Q_V = new[] { Q_V_ub, Q_V_ub_no_hr, Q_V_ub_nat_vent }[Q_K_index];
                    Q_T_out[t] = Q_T_ub * eta_g;
                    Q_T_opaque_out[t] = Q_T_op_ub * eta_g;
                    Q_T_transparent_out[t] = Q_T_tr_ub * eta_g;
                    Q_V_out[t] = Q_V * eta_g;
                    Q_i_out[t] = Q_i;
                    Q_s_out[t] = Q_s;
                }
                else
                {
                    // heating
                    Q_Cool[t] = 0.0;
                    Q_Heat[t] = Q_H;
                    eta_g = new[] {
                            eta_g_heating,
                            eta_g_heating_no_hr,
                            eta_g_heating_nat_vent
                        }[Q_H_index];
                    Q_V = new[] {
                            Q_V_lb,
                            Q_V_lb_no_hr,
                            Q_V_lb_nat_vent
                        }[Q_H_index];
                    Q_T_out[t] = Q_T_lb;
                    Q_T_opaque_out[t] = Q_T_op_lb;
                    Q_T_transparent_out[t] = Q_T_tr_lb;
                    Q_V_out[t] = Q_V;
                    Q_i_out[t] = Q_i * eta_g;
                    Q_s_out[t] = Q_s * eta_g;
                }
                // TODO lighting and utility loads. simplification, because utility and lighting have efficiencies (inefficiencies are heat loads). 
                // Need efficiency or electricity use separately
                // use sia380 reduction factors?
                Q_Elec[t] = Phi_L * t_L[t] + Phi_A * t_A[t];

                return null;
            }
        }

        Dictionary<string, object> cleanDictForNaN(Dictionary<string, object> d)
        {
            foreach (var k in d.Keys)
            {
                var v = d[k];
                if ((v is double vDouble && double.IsNaN(vDouble))
                    || (v is string vString && vString.Equals("NaN")))
                {
                    d[k] = 0.0;
                }
            }
            return d;
        }

        double[] ToKWh(IEnumerable<double> Q)
        {
            return Q.Select(x => x / 1000.0).ToArray();
        }

        Tuple<double, int> min_and_index(params double[] sequence)
        {
            var minValue = sequence[0];
            int index = 0;
            for (int i = 0; i < sequence.Count(); i++)
            {
                if (sequence[i] < minValue)
                {
                    minValue = sequence[i]; // TODO is it a reference??
                    index = i;
                }
            }
            return Tuple.Create(minValue, index);
        }

        double[] negatives_to_zero<T>(T values) where T : IEnumerable<double>
        {
            return values.Select(v => v < 0.0 ? 0.0 : v).ToArray();
        }

        // 
        //     Wärmeeintrag/-verlust Verhältnis. Ratio of gains and losses of a zone
        //     :param Q_i: internal gains
        //     :param Q_s: solar gains
        //     :param Q_T: transmission heat losses
        //     :param Q_V: ventilation heat losses
        //     :return: float. ratio of gains and losses
        //     
        double calc_gamma(double Q_i, double Q_s, double Q_T, double Q_V)
        {
            return (Q_i + Q_s) / (Q_T + Q_V);
        }

        /// <summary>
        /// Zeitkonstante. Time constant of a zone
        /// </summary>
        /// <param name="C">specific heat capacity per netto floor area</param>
        /// <param name="A">netto floor area</param>
        /// <param name="H_V">transmission heat losses</param>
        /// <param name="H_T">ventilation heat losses</param>
        /// <returns>time constant of the zone</returns>
        double calc_tau(double C, double A, double H_V, double H_T)
        {
            return C * A / (H_V + H_T);
        }

        // usage of heat gains (Ausnutzungsgrad für Wärmegewinne), eta_g
        // 
        //     Ausnutzungsgrad für Wärmegewinne/-verluste. A variable that describes thermal losses/gains of a zone
        //     :param gamma: Wärmeeintrag/-verlust Verhältnis. Ratio between gains and losses (Q_i + Q_s) / (Q_T + Q_V)
        //     :param tau: Zeitkonstante. Time constant of the zone, describing thermal latency (C * A) / ( H_T + H_V). Or coefficients from SIA 2024
        //     :param cooling: Boolean to indicate whether this function should return eta_g for the cooling case. Default is False
        //     :return: float. Value for the degree of thermal losses/gains of a zone
        //     
        double calc_eta_g(double gamma, double tau, bool cooling = false)
        {
            double a;
            double eta_g;

            if (gamma < 0.0)
            {
                eta_g = 1.0;
            }
            else if (gamma == 1.0)
            {
                a = 1.0 + tau / 15.0;
                eta_g = cooling ? a / (a + 1.0) : a / (a + 1.0);
            }
            else
            {
                a = 1.0 + tau / 15.0;
                eta_g = cooling
                    ? (1.0 - Math.Pow(gamma, -a)) / (1.0 - Math.Pow(gamma, -a + 1.0))
                    : (1.0 - Math.Pow(gamma, a)) / (1.0 - Math.Pow(gamma, a + 1.0));
            }
            return eta_g;
        }

        List<object> calculate_Q_s(
            bool run_obstr,
            DataTree<double> tree_obstr,
            DataTree<double> tree_unobstr,
            double g_value,
            double g_value_total,
            double setpoint,
            List<double> win_areas,
            bool hourly = false)
        {
            var tree = tree_obstr;
            if (run_obstr == false)
            {
                tree = tree_unobstr;
            }
            var Q_array = new List<object>();
            foreach (var i in Enumerable.Range(0, tree.BranchCount))
            {
                var row = new List<double>();
                var win_area = win_areas[i];
                var branch = tree.Branch(i);
                foreach (var j in Enumerable.Range(0, branch.Count))
                {
                    var irrad = branch[j] / win_area;
                    if (irrad > setpoint)
                    {
                        irrad *= g_value_total;
                    }
                    else
                    {
                        irrad *= g_value;
                    }
                    row.Add(irrad * win_area);
                }
                if (hourly)
                {
                    Q_array.Add(row);
                }
                else
                {
                    Q_array.Add(get_monthly_sum(row));
                }
            }
            return Q_array;
        }

        // Precalculates the constant part of natural ventilation calc 
        //             _______________________________
        //     
        //     V_dot = c_d * H * W * 1/3 * sqrt(g * H * (T_i - T_e)/T_e)
        //             _______________________________
        //     
        //     c_d = 0.25   [-]         discharge coefficient, value for large single openings such as open doors and windows
        //     g = 9.8     [m/s^2]     graviational constant
        //     
        //     :param H: Height of window opening [m]
        //     :param W: Width of window opening [m]
        //     
        double calc_nat_vent_Vdot_constant(double H, double W)
        {
            return 0.25 * H * W * 0.333333 * Math.Sqrt(GRAVITATIONAL_CONSTANT * H);
        }

        // Calculates air flow from buoyancy driven natural ventilation.
        //     
        //     V_dot = c_d * H * W * 1/3 * sqrt(g * H * (T_i - T_e)/T_e)
        //     
        //     :param T_i: Indoor temperature [C]
        //     :param T_e: Outdoor temperature [C]
        //     
        double calc_nat_vent_Vdot(double Vdot_nat_vent_constant, double T_i, double T_e)
        {
            return Vdot_nat_vent_constant * Math.Sqrt(Math.Abs(T_i - T_e) / (T_e + CELCIUS_TO_KELVIN));
        }

        // Calculates the coefficient of natural ventilation H_V_nat_vent
        //     
        //     H_V_nat_vent = V_dot_nat_vent * rho * c_p
        //     
        //     rho = 1.2   [kg/m^3]    Specific heat capacity of air   
        //     c_p = 1005  [J/kgK]     Average air density             
        //     
        //     :param T_i: Indoor temperature [C]
        //     :param T_e: Outdoor temperature [C]
        //     
        double calc_nat_vent_H_V(double Vdot_nat_vent)
        {
            return Vdot_nat_vent * RHO * C_P;
        }

        // UTILS
        double[] get_monthly_sum(IEnumerable<double> hourly) => Misc.GetCumulativeMonthlyValue(hourly.ToArray());
        //{
        //    var monthly = new List<object>();
        //    foreach (var month in Enumerable.Range(0, MonthsPerYear))
        //    {
        //        var start = Convert.ToInt32(HOURS_PER_DAY * DAYS_PER_MONTH[0::month].Sum());
        //        var end = Convert.ToInt32(HOURS_PER_DAY * DAYS_PER_MONTH[0::(month + 1)].Sum());
        //        monthly.append(hourly[start::end].Sum());
        //    }
        //    return monthly;
        //}

        double[] get_monthly_avg(IEnumerable<double> hourly) => Misc.GetAverageMonthlyValue(hourly.ToArray());
        //{
        //    var monthly = new List<object>();
        //    foreach (var month in Enumerable.Range(0, MonthsPerYear))
        //    {
        //        var start = Convert.ToInt32(HOURS_PER_DAY * DAYS_PER_MONTH[0::month].Sum());
        //        var end = Convert.ToInt32(HOURS_PER_DAY * DAYS_PER_MONTH[0::(month + 1)].Sum());
        //        monthly.append(hourly[start::end].Sum() / (end - start));
        //    }
        //    return monthly;
        //}

        List<List<double>> transpose_jagged_2D_array(List<List<double>> array)
        {
            var transposed_array = new List<List<double>>();
            var len_d1 = array.Count;
            var len_d2 = array[0].Count;
            foreach (var i in Enumerable.Range(0, len_d2))
            {
                var transposed_row = new List<double>();
                foreach (var j in Enumerable.Range(0, len_d1))
                {
                    transposed_row.Add(array[j][i]);
                }
                transposed_array.Add(transposed_row); // TODO check this
            }
            return transposed_array;
        }

        // 
        //     Converts the encoded schedules for occupancy, devices, lighting, and setpoints into hourly schedules.
        //     :param: The schedules JSON.
        //     :returns: Hourly schedules for occupancy (P), devices (A), lighting (L), and setpoints (S)
        //     
        public Tuple<double[], double[], double[], double[]> get_hourly_schedules(Dictionary<string,object> room_schedules)
        {
            var P_schedule = (Dictionary<string, object>)room_schedules["OccupancySchedule"];
            var A_schedule = (Dictionary<string, object>)room_schedules["DeviceSchedule"];
            var L_schedule = (Dictionary<string, object>)room_schedules["LightingSchedule"];
            var S_schedule = (Dictionary<string, object>)room_schedules["SetpointSchedule"];

            var YEAR_INTIAL_WEEKDAY = 0;

            // Other asserts are captured in schema validation
            Debug.Assert(365 - (double)room_schedules["DaysOffPerWeek"] * 52 == (double)room_schedules["DaysUsedPerYear"]);

            var P_daily_occupied = (double[])P_schedule["DailyProfile"];
            var A_daily_occupied = (double[])A_schedule["DailyProfile"];
            var L_daily_occupied = (double[])L_schedule["DailyProfile"];
            var S_daily_occupied = (double[])S_schedule["DailyProfile"];

            var P_daily_unoccupied = Enumerable.Repeat((double)P_schedule["Default"], HOURS_PER_DAY);
            var A_daily_unoccupied = Enumerable.Repeat((double)A_schedule["Default"], HOURS_PER_DAY);
            var L_daily_unoccupied = Enumerable.Repeat((double)L_schedule["Default"], HOURS_PER_DAY);
            var S_daily_unoccupied = Enumerable.Repeat((double)S_schedule["Default"], HOURS_PER_DAY);

            var weekdays_on = DaysPerWeek - (int)room_schedules["DaysOffPerWeek"];
            var weekday = YEAR_INTIAL_WEEKDAY;

            // TODO assert props, expand daily profiles and yearly to hourly timeseries
            var P_hourly = new List<double>();
            var A_hourly = new List<double>();
            var L_hourly = new List<double>();
            var S_hourly = new List<double>();
            foreach (var _tup_1 in DAYS_PER_MONTH.Select((_p_1, _p_2) => Tuple.Create(_p_2, _p_1)))
            {
                var month = _tup_1.Item1;
                var month_days = _tup_1.Item2;
                // TODO assumes days off / holidays all at once rather 
                // than peppered through month. More appropriate for schools / summer / winter
                // but not really for national holidays...
                var days_on = Convert.ToInt32(month_days * ((double[])room_schedules["YearlyProfile"])[month]);
                foreach (var day in Enumerable.Range(0, month_days))
                {
                    if (weekday == DaysPerWeek)
                    {
                        weekday = 0;
                    }
                    var skip = weekday >= weekdays_on || day > days_on;
                    if (skip)
                    {
                        // unoccupied
                        P_hourly.AddRange(P_daily_unoccupied);
                        A_hourly.AddRange(A_daily_unoccupied);
                        L_hourly.AddRange(L_daily_unoccupied);
                        S_hourly.AddRange(S_daily_unoccupied);
                    }
                    else
                    {
                        // occupied
                        P_hourly.AddRange(P_daily_occupied);
                        A_hourly.AddRange(A_daily_occupied);
                        L_hourly.AddRange(L_daily_occupied);
                        S_hourly.AddRange(S_daily_occupied);
                    }
                    weekday += 1;
                }
            }
            return Tuple.Create(P_hourly.ToArray(), A_hourly.ToArray(), L_hourly.ToArray(), S_hourly.ToArray());
        }

        //public static object test()
        //{
        //    // Arrange
        //    var testdir = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(@__file__)), "..\\..\\..\\tests\\Hive.Core\\sia380"));
        //    var path_testdata = os.path.join(testdir, "testdata.json");
        //    var path_results = os.path.join(testdir, "results.json");
        //    using (var f = open(path_testdata, "r"))
        //    {
        //        kwargs = json.loads(f.read(), encoding: "utf-8");
        //    }
        //    kwargs["srf_irrad_obstr_tree"] = dt.DataTree(new List<object> {
        //        new List<object> {
        //            0.0
        //        } * 8760
        //    });
        //    kwargs["srf_irrad_unobstr_tree"] = dt.DataTree(kwargs["srf_irrad_unobstr_tree"]);
        //    kwargs["debug"] = true;
        //    // Act
        //    var results = main(kwargs);
        //    // Assert
        //    var _tup_1 = results;
        //    var Q_Heat = _tup_1.Item1;
        //    var Q_dhw = _tup_1.Item2;
        //    var Q_Cool = _tup_1.Item3;
        //    var Q_Elec = _tup_1.Item4;
        //    var Q_T = _tup_1.Item5;
        //    var Q_V = _tup_1.Item6;
        //    var Q_i = _tup_1.Item7;
        //    var Q_s = _tup_1.Item8;
        //    var Q_T_op = _tup_1.Item9;
        //    var Q_T_tr = _tup_1.Item10;
        //    var Q_s_tr_tree = _tup_1.Item11;
        //    // with open(os.path.join(testdir, "results_variable_tau.json"), 'w') as f:
        //    //     data = {
        //    //         "Q_Heat": Q_Heat, 
        //    //         "Q_dhw": Q_dhw, 
        //    //         "Q_Cool": Q_Cool,
        //    //         "Q_Elec": Q_Elec,
        //    //         "Q_T": Q_T,
        //    //         "Q_V": Q_V,
        //    //         "Q_i": Q_i,
        //    //         "Q_s": Q_s,
        //    //         "Q_T_op": Q_T_op,
        //    //         "Q_T_tr": Q_T_tr,
        //    //         "Q_s_tr_tree": Q_s_tr_tree 
        //    //     }
        //    //     json.dump(data, f, indent=4)
        //    Console.WriteLine("Q_Cool = " + Q_Cool.Sum().ToString());
        //    Console.WriteLine("Q_Heat = " + Q_Heat.Sum().ToString());
        //    // t_start = 168 * HOURS_PER_DAY
        //    // t_end = t_start + 7 * HOURS_PER_DAY
        //    // import matplotlib.pyplot as plt
        //    // X = range(t_start, t_end)
        //    // plt.plot(X, Q_Cool[t_start:t_end], label='Q_Cool')
        //    // plt.plot(X, Q_Heat[t_start:t_end], label='Q_Heat')
        //    // plt.plot(X, Q_dhw[t_start:t_end], label='Q_dhw')
        //    // plt.plot(X, Q_T[t_start:t_end], ':c', label='Q_T')
        //    // plt.plot(X, Q_V[t_start:t_end], ':b', label='Q_V')
        //    // plt.plot(X, Q_i[t_start:t_end], ':g', label='Q_i')
        //    // plt.plot(X, Q_s[t_start:t_end], ':r', label='Q_s')
        //    // plt.plot(X, kwargs['T_e_hourly'][t_start:t_end])
        //    // plt.plot(X, kwargs['T_i_ub_hourly'][t_start:t_end])
        //    // plt.plot(X, kwargs['T_i_lb_hourly'][t_start:t_end])
        //    // plt.show()
        //}
    }
}