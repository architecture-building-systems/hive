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
            var records = Sia2024Record.All();
            Assert.IsNotEmpty(records);
            Assert.AreEqual(records.First().RoomType, "1.1 Wohnen Mehrfamilienhaus");
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
        public void TestJson()
        {
            var record = Sia2024Record.All().First();
            var json = record.ToJson();
            Assert.IsInstanceOf(typeof(string), json);
            var record2 = Sia2024Record.FromJson(json);
            Assert.NotNull(record2);
        }
    }
}