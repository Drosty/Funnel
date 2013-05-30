using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funnel.Tests.Models;
using NUnit.Framework;

namespace Funnel.Tests
{
    [TestFixture]
    public class MappingTests
    {
        private List<StarcraftModel> _testArray = null;
        private StarcraftModel _testItem = null;
        [SetUp]
        public void Setup()
        {
            _testItem = new StarcraftModel
            {
                Gas = 1,
                Mineral = 2,
                Race = "Protoss",
                Supply = "1",
                Time = "20",
                Unit = "Test"
            };
            _testArray = new List<StarcraftModel>()
                {
                    new StarcraftModel
                        {
                            Gas = 1,
                            Mineral = 2,
                            Race = "Protoss",
                            Supply = "1",
                            Time = "20",
                            Unit = "Test"
                        },
                    new StarcraftModel
                        {
                            Gas = 2,
                            Mineral = 3,
                            Race = "Protoss",
                            Supply = "2",
                            Time = "21",
                            Unit = "Test 1"
                        },
                    new StarcraftModel
                        {
                            Gas = 3,
                            Mineral = 4,
                            Race = "Protoss",
                            Supply = "2",
                            Time = "22",
                            Unit = "Test 2"
                        },
                };
        }
        [Test(Description = "Test reflecting an object into another")]
        public void ReflectIntoSingle()
        {


            var starcraftDataItem = _testItem.Funnel().Into<StarcraftDataItem>();

            Assert.That(starcraftDataItem.Gas, Is.EqualTo(1));
            Assert.That(starcraftDataItem.Mineral, Is.EqualTo(2));
            Assert.That(starcraftDataItem.Race, Is.EqualTo("Protoss"));
            Assert.That(starcraftDataItem.Supply, Is.EqualTo("1"));
            Assert.That(starcraftDataItem.Time, Is.EqualTo("20"));
            Assert.That(starcraftDataItem.Unit, Is.EqualTo("Test"));

            Assert.That(starcraftDataItem.Bonus, Is.EqualTo(null));
            Assert.That(starcraftDataItem.Type, Is.EqualTo(null));
        }

        [Test(Description = "Test reflecting an IEnumerable of objects into an IEnumerable of another object")]
        public void ReflectInto()
        {


            var starcraftDataItems = _testArray.FunnelArray().IntoMany<StarcraftDataItem>().ToArray();

            Assert.That(starcraftDataItems.Length, Is.EqualTo(3));

            Assert.That(starcraftDataItems[2].Gas, Is.EqualTo(3));
            Assert.That(starcraftDataItems[1].Supply, Is.EqualTo("2"));
        }
        
        [Test(Description = "Test reflecting into a dynamic object")]
        public void ReflectIntoDynamics()
        {

            var starcraftDataItems = _testArray.FunnelArray().IntoDynamics().ToArray();

            Assert.That(starcraftDataItems.Length, Is.EqualTo(3));

            Assert.That(starcraftDataItems[2].Gas, Is.EqualTo(3));
            Assert.That(starcraftDataItems[1].Supply, Is.EqualTo("2"));
        }

        [Test(Description = "Test reflection using explicit column mapping")]
        public void ReflectSingleWithExplicitMapping()
        {

            var starcraftDataItem = _testItem
                .Funnel()
                .AddExplicitMapping("Unit","Other")
                .Into<StarcraftDataItem>();

            Assert.That(starcraftDataItem.Gas, Is.EqualTo(1));
            Assert.That(starcraftDataItem.Mineral, Is.EqualTo(2));
            Assert.That(starcraftDataItem.Race, Is.EqualTo("Protoss"));
            Assert.That(starcraftDataItem.Supply, Is.EqualTo("1"));
            Assert.That(starcraftDataItem.Time, Is.EqualTo("20"));
            Assert.That(starcraftDataItem.Other, Is.EqualTo("Test"));

            Assert.That(starcraftDataItem.Bonus, Is.EqualTo(null));
            Assert.That(starcraftDataItem.Type, Is.EqualTo(null));
        }

        [Test(Description = "Test into dynamic using explicit column mapping")]
        public void ReflectIntoDynamicWithExplicitColumnMapping()
        {

            var starcraftDataItem = _testItem
                .Funnel()
                .AddExplicitMapping("Unit", "Other")
                .IntoDynamic();

            Assert.That(starcraftDataItem.Gas, Is.EqualTo(1));
            Assert.That(starcraftDataItem.Mineral, Is.EqualTo(2));
            Assert.That(starcraftDataItem.Race, Is.EqualTo("Protoss"));
            Assert.That(starcraftDataItem.Supply, Is.EqualTo("1"));
            Assert.That(starcraftDataItem.Time, Is.EqualTo("20"));

            Assert.That(starcraftDataItem.Other, Is.EqualTo("Test"));

        }
    }
}
