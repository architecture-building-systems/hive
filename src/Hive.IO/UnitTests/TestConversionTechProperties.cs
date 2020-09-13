using System.Collections.Generic;
using Hive.IO.Forms;
using Hive.IO.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test loading the data from conversion_technology_defaults...
    /// </summary>
    [TestClass]
    public class TestConversionTechProperties
    {
        [TestMethod]
        public void TestLoadingConversionTechnologyDefaults()
        {
            Dictionary<string, ConversionTechDefaults> defaults = null;
            var records = JsonResource.ReadRecords(ConversionTechDefaults.ResourceName, ref defaults);
            Assert.IsTrue(records.Count > 0);
        }
    }
}