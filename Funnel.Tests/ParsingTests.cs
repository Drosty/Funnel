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
        }

    }
}
