using System.Linq;
using Hive.IO.Building;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestReadRecords()
        {
            var records = Sia2024Record.All();
            Assert.IsTrue(records.Count() > 0);
            Assert.AreEqual(records.First().RoomType, "1.1 Wohnen Mehrfamilienhaus");
        }

        [TestMethod]
        public void TestBuildingUseTypes()
        {
            var useTypes = Sia2024Record.BuildingUseTypes();
            Assert.AreEqual(useTypes.Count(), 12);
        }

        [TestMethod]
        public void TestQualities()
        {
            Assert.AreEqual(Sia2024Record.Qualities().Count(), 3);
        }

        [TestMethod]
        public void TestQualitiesZielwert()
        {
            var useType = Sia2024Record.BuildingUseTypes().First();
            var roomType = Sia2024Record.RoomTypes(useType).First();
            var quality = Sia2024Record.Qualities().Last();
            Assert.AreEqual(quality, "Zielwert");
            var record = Sia2024Record.Lookup(useType, roomType, quality);
            Assert.AreEqual(record.UValueOpaque, 0.1, 0.0000001);
        }

        [TestMethod]
        public void TestJson()
        {
            var record = Sia2024Record.All().First();
            var json = record.ToJson();
            Assert.IsInstanceOfType(json, typeof(string));
            var record2 = Sia2024Record.FromJson(json);
            Assert.IsNotNull(record2);
        }
    }
}