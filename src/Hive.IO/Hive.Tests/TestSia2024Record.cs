using System.Linq;
using Hive.IO.Building;
using NUnit.Framework;

namespace Hive.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestReadRecords()
        {
            var records = Sia2024Record.ReadRecords();
            Assert.IsNotEmpty(records);
            Assert.AreEqual(records[0].Quality, "Standardwert");
        }

        [Test]
        public void TestBuildingUseTypes()
        {
            var useTypes = Sia2024Record.BuildingUseTypes();
            Assert.AreEqual(useTypes.Count(), 12);
        }

        [Test]
        public void TestQualities()
        {
            Assert.AreEqual(Sia2024Record.Qualities().Count(), 3);
        }

        [Test]
        public void TestToString()
        {
            var record = Sia2024Record.ReadRecords().First();
            Assert.IsInstanceOf(typeof(string), record.ToString());
        }
    }
}