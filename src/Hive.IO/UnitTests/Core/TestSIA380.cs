using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Hive.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTests.Core
{
    [TestClass]
    public class TestSIA380
    {
        [TestMethod]
        public void TestSIA380Valid()
        {
            // Arrange
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Core\TestDataSIA380.json");
            var serializer = new JsonSerializer();
            TestDataSIA380 data;
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        data = serializer.Deserialize<TestDataSIA380>(jsonTextReader);
                    }
                }
            }

            GH_Structure<GH_Number> srf_irrad_obstr_tree = ToDataTree(data.srf_irrad_obstr_tree);
            GH_Structure<GH_Number> srf_irrad_unobstr_tree = ToDataTree(data.srf_irrad_unobstr_tree);

            // Act
            var actual = new Sia380();
            actual.Run(JsonConvert.SerializeObject(data.room_properties), JsonConvert.SerializeObject(data.room_schedules), data.floor_area,
                data.T_e_hourly, data.T_i_ub_hourly, data.T_i_lb_hourly,
                data.surface_areas, data.surface_type,
                srf_irrad_obstr_tree, srf_irrad_unobstr_tree,
                data.g_value, data.g_value_total, data.setpoint_shading,
                data.run_obstructed_simulation, data.hourly,
                data.use_adaptive_comfort, data.use_natural_ventilation, data.use_fixed_time_constant);

            // Assert
            var expected = new Expected();

            CollectionAssert.AreEqual(expected.Q_Heat, actual.Q_Heat);
            CollectionAssert.AreEqual(expected.Q_Cool, actual.Q_Cool);
            CollectionAssert.AreEqual(expected.Q_Elec, actual.Q_Elec);
            CollectionAssert.AreEqual(expected.Q_dhw, actual.Q_DHW);
            CollectionAssert.AreEqual(expected.Q_i, actual.Q_i_out);
            CollectionAssert.AreEqual(expected.Q_V, actual.Q_V_out);
            CollectionAssert.AreEqual(expected.Q_T, actual.Q_T_out);
            CollectionAssert.AreEqual(expected.Q_s, actual.Q_s_out);

            for (int window = 0; window < expected.Q_s_tr_tree.Count(); window++)
            {
                CollectionAssert.AreEqual(expected.Q_s_tr_tree[window],
                    actual.Q_s_tr_tree.Branch(new[] { 0, 0, window }));
            }

        }

        private GH_Structure<GH_Number> ToDataTree(List<List<double>> srf_irrad_obstr_tree)
        {
            GH_Structure<GH_Number> tree = new GH_Structure<GH_Number>();

            for (int window = 0; window < srf_irrad_obstr_tree?.Count; window++)
            {
                for (int qs = 0; qs < srf_irrad_obstr_tree[window]?.Count; qs++)
                {
                    tree.Insert(new GH_Number(srf_irrad_obstr_tree[window][qs]), new GH_Path(0, 0, window), qs);
                }
            }
            return tree;
        }
    }

    #region Expected

    /// <summary>
    /// Results taken from the original python sia380 component, using the same testdata as used here.
    /// </summary>
    public class Expected
    {
        public double[][] Q_s_tr_tree { 
            get {
                var tree = new double[2][];
                tree[0] = new double[] {
                    179207.18105028008,
                    274382.14292155986,
                    423311.8212192197,
                    540275.6378331198,
                    634716.6806543003,
                    630120.0007597009,
                    662570.5163411598,
                    595263.3286175599,
                    475753.5976565006,
                    351494.5070148599,
                    196812.60567608013,
                    154619.11616679994
                };
                tree[1] = new double[] {
                    198078.65344712004,
                    264021.36826612003,
                    422436.2471165197,
                    487130.3958357202,
                    647002.1166130195,
                    647687.33675722,
                    674345.077591061,
                    573815.3103861004,
                    453878.3776286801,
                    339257.2299515401,
                    222473.95296691984,
                    186311.0618942198
                };
                return tree;
            } 
        }

        public double[] Q_T_op = new double[] {
            1494.20801623148,
            1264.508488224,
            1078.8618600956204,
            845.5229673805202,
            395.4756635098369,
            329.54882643307235,
            320.2004297452356,
            297.7768146379289,
            344.2583907532855,
            807.6560118033798,
            1196.04354947502,
            1408.7463661670201
        };

        public double[] Q_V = new double[]
        {
            415.55081699999994,
            351.6696,
            300.0398355000001,
            235.14648300000002,
            281.07237826683655,
            234.21687083985282,
            227.57278036229366,
            211.63587346993293,
            244.67124922070724,
            224.6153894999999,
            332.6289704999999,
            391.7832704999999
        };

        public double[] Q_T = new double[] {
            2381.3499375314,
            2015.27309232,
            1719.4042564591007,
            1347.5272811286002,
            630.2776697514669,
            525.2087184080316,
            510.3099870220597,
            474.57301207951565,
            548.6516525206451,
            1287.1779379858997,
            1906.1591163261,
            2245.1479543861
        };
        public double[] Q_Heat = new double[] {
            2175.651534392863,
            1704.9725764515074,
            1240.861467421732,
            830.0144984036497,
            0.0,
            0.0,
            0.0,
            0.0,
            0.0,
            871.0376552918278,
            1634.843991166537,
            2045.5886588222259
        };
        public double[] Q_Elec = new double[] {
            277.5476712328767,
            250.68821917808222,
            277.5476712328767,
            268.59452054794525,
            277.5476712328767,
            268.59452054794525,
            277.5476712328767,
            277.5476712328767,
            268.59452054794525,
            277.5476712328767,
            268.59452054794525,
            277.5476712328767
        };
        public double[] Q_T_tr = new double[] {
            887.14192129992,
            750.764604096,
            640.5423963634803,
            502.00431374808005,
            234.8020062416299,
            195.65989197495932,
            190.10955727682415,
            176.79619744158674,
            204.39326176735977,
            479.5219261825198,
            710.11556685108,
            836.4015882190799
        };
        public double[] Q_Cool = new double[] {
            0.0,
            0.0,
            0.0,
            0.0,
            -791.7275437695644,
            -926.1483236115023,
            -1020.3916210684155,
            -904.2285479747596,
            -544.0756488862941,
            0.0,
            0.0,
            0.0
        };
        public double[] Q_dhw = new double[] {
            302.6958904109589,
            273.4027397260274,
            302.6958904109589,
            292.93150684931504,
            302.6958904109589,
            292.93150684931504,
            302.6958904109589,
            302.6958904109589,
            292.93150684931504,
            302.6958904109589,
            292.93150684931504,
            302.6958904109589
        };
        public double[] Q_i = new[] {
            327.76633434107987,
            274.14356450385446,
            258.9068418193219,
            213.84834775788303,
            421.358794520548,
            407.7665753424658,
            421.358794520548,
            421.358794520548,
            407.7665753424658,
            242.77086672033377,
            297.7658937082636,
            326.8673688686999
        };

        public double[] Q_s = new[] {
            293.4828857974571,
            387.82655136463785,
            519.6757827180467,
            538.8109179670676,
            1281.7187972673196,
            1277.8073375169208,
            1336.9155939322206,
            1169.0786390036603,
            929.6319752851806,
            397.98480547373777,
            306.17820195129923,
            264.47519719517436
        };
    }

    #endregion

    // TODO use the proper classes instead...
    #region Test Data Struct

    public struct RoomProperties
    {
        public string description { get; set; }
        public double Zeitkonstante { get; set; }

        [JsonProperty("Waermespeicherfaehigkeit des Raumes")]
        public double WaermespeicherfaehigkeitDesRaumes { get; set; }

        [JsonProperty("Raumlufttemperatur Auslegung Kuehlung (Sommer)")]
        public double RaumlufttemperaturAuslegungKuehlungSommer { get; set; }

        [JsonProperty("Raumlufttemperatur Auslegung Heizen (Winter)")]
        public double RaumlufttemperaturAuslegungHeizenWinter { get; set; }

        [JsonProperty("Raumlufttemperatur Auslegung Kuehlung (Sommer) - Absenktemperatur")]
        public double RaumlufttemperaturAuslegungKuehlungSommerAbsenktemperatur { get; set; }

        [JsonProperty("Raumlufttemperatur Auslegung Heizen (Winter) - Absenktemperatur")]
        public double RaumlufttemperaturAuslegungHeizenWinterAbsenktemperatur { get; set; }
        public double Nettogeschossflaeche { get; set; }

        [JsonProperty("Thermische Gebaeudehuellflaeche")]
        public double ThermischeGebaeudehuellflaeche { get; set; }
        public double Glasanteil { get; set; }

        [JsonProperty("U-Wert opake Bauteile")]
        public double UWertOpakeBauteile { get; set; }

        [JsonProperty("U-Wert Fenster")]
        public double UWertFenster { get; set; }

        [JsonProperty("Gesamtenergiedurchlassgrad Verglasung")]
        public double GesamtenergiedurchlassgradVerglasung { get; set; }

        [JsonProperty("Gesamtenergiedurchlassgrad Verglasung und Sonnenschutz")]
        public double GesamtenergiedurchlassgradVerglasungUndSonnenschutz { get; set; }

        [JsonProperty("Strahlungsleistung fuer Betaetigung Sonnenschutz")]
        public double StrahlungsleistungFuerBetaetigungSonnenschutz { get; set; }

        [JsonProperty("Abminderungsfaktor fuer Fensterrahmen")]
        public double AbminderungsfaktorFuerFensterrahmen { get; set; }

        [JsonProperty("Aussenluft-Volumenstrom (pro NGF)")]
        public double AussenluftVolumenstromProNGF { get; set; }

        [JsonProperty("Aussenluft-Volumenstrom durch Infiltration")]
        public double AussenluftVolumenstromDurchInfiltration { get; set; }

        [JsonProperty("Temperatur-Aenderungsgrad der Waermerueckgewinnung")]
        public double TemperaturAenderungsgradDerWaermerueckgewinnung { get; set; }

        [JsonProperty("Waermeeintragsleistung Personen (bei 24.0 deg C, bzw. 70 W)")]
        public double WaermeeintragsleistungPersonenBei240DegCBzw70W { get; set; }

        [JsonProperty("Waermeeintragsleistung der Raumbeleuchtung")]
        public double WaermeeintragsleistungDerRaumbeleuchtung { get; set; }

        [JsonProperty("Waermeeintragsleistung der Geraete")]
        public double WaermeeintragsleistungDerGeraete { get; set; }

        [JsonProperty("Jaehrlicher Waermebedarf fuer Warmwasser")]
        public double JaehrlicherWaermebedarfFuerWarmwasser { get; set; }

        [JsonProperty("Vollaststunden pro Jahr (Personen)")]
        public double VollaststundenProJahrPersonen { get; set; }

        [JsonProperty("Jaehrliche Vollaststunden der Raumbeleuchtung")]
        public double JaehrlicheVollaststundenDerRaumbeleuchtung { get; set; }

        [JsonProperty("Jaehrliche Vollaststunden der Geraete")]
        public double JaehrlicheVollaststundenDerGeraete { get; set; }

        [JsonProperty("Kosten opake Bauteile")]
        public double KostenOpakeBauteile { get; set; }

        [JsonProperty("Kosten transparente Bauteile")]
        public double KostenTransparenteBauteile { get; set; }

        [JsonProperty("Emissionen opake Bauteile")]
        public double EmissionenOpakeBauteile { get; set; }

        [JsonProperty("Emissionen transparente Bauteile")]
        public double EmissionenTransparenteBauteile { get; set; }

        [JsonProperty("U-Wert Boeden")]
        public double UWertBoeden { get; set; }

        [JsonProperty("U-Wert Daecher")]
        public double UWertDaecher { get; set; }

        [JsonProperty("U-Wert Waende")]
        public double UWertWaende { get; set; }

        [JsonProperty("Kosten Boeden")]
        public double KostenBoeden { get; set; }

        [JsonProperty("Kosten Daecher")]
        public double KostenDaecher { get; set; }

        [JsonProperty("Kosten Waende")]
        public double KostenWaende { get; set; }

        [JsonProperty("Emissionen Boeden")]
        public double EmissionenBoeden { get; set; }

        [JsonProperty("Emissionen Daecher")]
        public double EmissionenDaecher { get; set; }

        [JsonProperty("Emissionen Waende")]
        public double EmissionenWaende { get; set; }
    }

    public struct OccupancySchedule
    {
        public List<double> DailyProfile { get; set; }
        public double Default { get; set; }
    }

    public struct DeviceSchedule
    {
        public List<double> DailyProfile { get; set; }
        public double Default { get; set; }
    }

    public struct LightingSchedule
    {
        public List<double> DailyProfile { get; set; }
        public double Default { get; set; }
    }

    public struct SetpointSchedule
    {
        public List<double> DailyProfile { get; set; }
        public double Default { get; set; }
    }

    public struct RoomSchedules
    {
        public string RoomType { get; set; }
        public List<double> YearlyProfile { get; set; }
        public int DaysOffPerWeek { get; set; }
        public int DaysUsedPerYear { get; set; }
        public OccupancySchedule OccupancySchedule { get; set; }
        public DeviceSchedule DeviceSchedule { get; set; }
        public LightingSchedule LightingSchedule { get; set; }
        public SetpointSchedule SetpointSchedule { get; set; }
    }

    public struct TestDataSIA380
    {
        public bool run_obstructed_simulation { get; set; }
        public bool use_adaptive_comfort { get; set; }
        public bool use_natural_ventilation { get; set; }
        public bool use_fixed_time_constant { get; set; }
        public bool hourly { get; set; }
        public double g_value { get; set; }
        public double g_value_total { get; set; }
        public int setpoint_shading { get; set; }
        public double floor_area { get; set; }
        public List<double> surface_areas { get; set; }
        public List<string> surface_type { get; set; }
        public RoomProperties room_properties { get; set; }
        public RoomSchedules room_schedules { get; set; }
        public List<double> T_e_hourly { get; set; }
        public List<double> T_i_ub_hourly { get; set; }
        public List<double> T_i_lb_hourly { get; set; }
        public List<List<double>> srf_irrad_obstr_tree { get; set; }
        public List<List<double>> srf_irrad_unobstr_tree { get; set; }
    }

    #endregion
}
