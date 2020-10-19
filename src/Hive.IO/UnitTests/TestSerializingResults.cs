using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hive.IO;
using Hive.IO.EnergySystems;
using Hive.IO.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UnitTests
{
    [TestClass]
    public class TestSerializingResults
    {
        private static readonly ITraceWriter TraceWriter = new MemoryTraceWriter();
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.All,
            TraceWriter = TraceWriter
        };

        [TestMethod]
        public void TestSerializeGasBoiler()
        {
            var gasBoiler = new GasBoiler(100.0, 200.0, 400.0, 400.0);
            var gasEnergy = new double[Misc.MonthsPerYear];
            var gasPrice = new double[Misc.MonthsPerYear];
            var ghgEmissionsFactor = new double[Misc.MonthsPerYear];
            var heatingGenerated = new double[Misc.MonthsPerYear];
            var supplyTemperature = new double[Misc.MonthsPerYear];
            for (int i = 0; i < Misc.MonthsPerYear; i++)
            {
                gasEnergy[i] = 101.1;
                gasPrice[i] = 202.2;
                ghgEmissionsFactor[i] = 303.3;
                heatingGenerated[i] = 404.4;
                supplyTemperature[i] = 23.45;
            }

            var gas = new Gas(Misc.MonthsPerYear, gasEnergy, gasPrice, ghgEmissionsFactor, 10.9);
            gasBoiler.SetInputOutput(gas, heatingGenerated, supplyTemperature);

            var serialized = JsonConvert.SerializeObject(gasBoiler, Formatting.Indented, JsonSerializerSettings);
            var deserialized = JsonConvert.DeserializeObject<GasBoiler>(serialized, JsonSerializerSettings);
            Assert.AreEqual(gasBoiler.Efficiency, deserialized.Efficiency);
        }

        [TestMethod]
        public void TestSerializeDirectElectricity()
        {
            var directElectricity = new DirectElectricity(100.0, 200.0, 400.0, 400.0);

            var electricEnergy = new double[Misc.MonthsPerYear];
            var electricityPrice = new double[Misc.MonthsPerYear];
            var ghgEmissionsFactor = new double[Misc.MonthsPerYear];
            var finalElectricityDemand = new double[Misc.MonthsPerYear];
            for (int i = 0; i < Misc.MonthsPerYear; i++)
            {
                electricEnergy[i] = 101.1;
                electricityPrice[i] = 202.2;
                ghgEmissionsFactor[i] = 303.3;
                finalElectricityDemand[i] = 404.4;
            }

            var electricityIn = new Electricity(Misc.MonthsPerYear, electricEnergy, electricityPrice, ghgEmissionsFactor, 20.3);


            
            directElectricity.SetInputOutput(electricityIn, finalElectricityDemand);

            var serialized = JsonConvert.SerializeObject(directElectricity, Formatting.Indented, JsonSerializerSettings);
            var deserialized = JsonConvert.DeserializeObject<DirectElectricity>(serialized, JsonSerializerSettings);
            Assert.AreEqual(directElectricity.Efficiency, deserialized.Efficiency);
        }

    }

}
