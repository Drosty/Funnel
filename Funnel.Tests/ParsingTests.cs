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
        [Test(Description="Test the parsing of a delimited file into a list of objects")]
        public void ParseDelimited()
        {
            var starcraftData = File.ReadLines(@"Data\StarcraftData.csv")
                                    .ParseDelimited(',')
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

        [Test(Description = "Test the parsing of a delimited file using explicit columns into a list of objects")]
        public void ParseDelimitedUsingExplicitHeaders()
        {
            var starcraftData = File.ReadLines(@"Data\StarcraftData.csv")
                                    .ParseDelimited(',')
                                    .MapArray("Unit", "Mineral", "Gas", "Supply", "Time", "Type", "Bonus", "Race")
                                    .Into<StarcraftDataItem>()
                                    .ToArray();

            Assert.That(starcraftData.Length, Is.EqualTo(41));

            // One data item has a null value for Mineral
            Assert.That(starcraftData.Count(x => x.Mineral != null), Is.EqualTo(40));
            Assert.That(starcraftData.Count(x => x.Gas > 0), Is.EqualTo(30));
            Assert.That(starcraftData.Count(x => !String.IsNullOrEmpty(x.Supply)), Is.EqualTo(40));
            Assert.That(starcraftData.Count(x => x.Type == "ARMORED BIOLOGICAL"), Is.EqualTo(5));
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

       
    }
}
