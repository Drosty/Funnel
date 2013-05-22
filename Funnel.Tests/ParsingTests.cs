using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funnel.Tests.Models;
using NUnit.Framework;
using Funnel;

namespace Funnel.Tests
{
    [TestFixture]
    public class ParsingTests
    {
        [Test(Description="Test the parsing of a CSV file into a list of objects")]
        public void ParseCSV()
        {
            var starcraftData = File.ReadLines(@"Data\StarcraftData.csv")
                                    .ParseCsv(',')
                                    .MapArrayUsingHeader()
                                    .Into<StarcraftDataItem>()
                                    .ToArray();

            Assert.That(starcraftData.Length, Is.EqualTo(41));

            // One data item has a null value for Mineral
            Assert.That(starcraftData.Count(x=>x.Mineral!=null),Is.EqualTo(40));
            Assert.That(starcraftData.Count(x=>x.Gas>0),Is.EqualTo(30));
            Assert.That(starcraftData.Count(x=>!String.IsNullOrEmpty(x.Supply)),Is.EqualTo(40));
            Assert.That(starcraftData.Count(x=>x.Type=="ARMORED BIOLOGICAL"),Is.EqualTo(5));
        }

        [Test(Description = "Test the parsing of a fixed width file into a list of objects")]
        public void ParseFixedWidth()
        {
            var starcraftData = File.ReadLines(@"Data\StarcraftData.prn")
                                    .ParseFixedWidth(12,8,8,8,8,21,8,8)
                                    .MapArrayUsingHeader()
                                    .Into<StarcraftDataItem>()
                                    .ToArray();

            Assert.That(starcraftData.Length, Is.EqualTo(41));

            // One data item has a null value for Mineral
            Assert.That(starcraftData.Count(x => x.Mineral != null), Is.EqualTo(40));
            Assert.That(starcraftData.Count(x => x.Gas > 0), Is.EqualTo(30));
            Assert.That(starcraftData.Count(x => !String.IsNullOrEmpty(x.Supply)), Is.EqualTo(40));
            Assert.That(starcraftData.Count(x=>x.Type=="ARMORED BIOLOGICAL"),Is.EqualTo(5));
        }

        [Test(Description = "Test reflecting an object into another")]
        public void ReflectIntoSingle()
        {
            var testObject = new StarcraftModel
                    {
                        Gas = 1,
                        Mineral = 2,
                        Race = "Protoss",
                        Supply = "1",
                        Time = "20",
                        Unit = "Test"
                    };

            var starcraftDataItem = testObject.ReflectSingle().IntoSingle<StarcraftDataItem>();

            Assert.That(starcraftDataItem.Gas,Is.EqualTo(1));
            Assert.That(starcraftDataItem.Mineral,Is.EqualTo(2));
            Assert.That(starcraftDataItem.Race,Is.EqualTo("Protoss"));
            Assert.That(starcraftDataItem.Supply,Is.EqualTo("1"));
            Assert.That(starcraftDataItem.Time,Is.EqualTo("20"));
            Assert.That(starcraftDataItem.Unit,Is.EqualTo("Test"));
            
            Assert.That(starcraftDataItem.Bonus,Is.EqualTo(null));
            Assert.That(starcraftDataItem.Type,Is.EqualTo(null));
        }

        [Test(Description = "Test reflecting an IEnumerable of objects into an IEnumerable of another object")]
        public void ReflectInto()
        {
            var items = new List<StarcraftModel>()
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

            var starcraftDataItems = items.Reflect().Into<StarcraftDataItem>().ToArray();

            Assert.That(starcraftDataItems.Length,Is.EqualTo(3));

            Assert.That(starcraftDataItems[2].Gas,Is.EqualTo(3));
            Assert.That(starcraftDataItems[1].Supply,Is.EqualTo("2"));
        }
    }
}
