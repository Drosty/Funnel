﻿using System;
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

            Assert.That(starcraftDataItems.Length, Is.EqualTo(3));

            Assert.That(starcraftDataItems[2].Gas, Is.EqualTo(3));
            Assert.That(starcraftDataItems[1].Supply, Is.EqualTo("2"));
        }
        
        [Test(Description = "Test reflecting into a dynamic object")]
        public void ReflectIntoDynamics()
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

            var starcraftDataItems = items.Reflect().IntoDynamic().ToArray();

            Assert.That(starcraftDataItems.Length, Is.EqualTo(3));

            Assert.That(starcraftDataItems[2].Gas, Is.EqualTo(3));
            Assert.That(starcraftDataItems[1].Supply, Is.EqualTo("2"));
        }

        [Test(Description = "Test reflection using explicit mapping")]
        public void ReflectSingleWithExplicitMapping()
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

            var starcraftDataItem = testObject
                .ReflectSingle()
                .AddExplicitMapping("Unit","Other")
                .IntoSingle<StarcraftDataItem>();

            Assert.That(starcraftDataItem.Gas, Is.EqualTo(1));
            Assert.That(starcraftDataItem.Mineral, Is.EqualTo(2));
            Assert.That(starcraftDataItem.Race, Is.EqualTo("Protoss"));
            Assert.That(starcraftDataItem.Supply, Is.EqualTo("1"));
            Assert.That(starcraftDataItem.Time, Is.EqualTo("20"));
            Assert.That(starcraftDataItem.Other, Is.EqualTo("Test"));

            Assert.That(starcraftDataItem.Bonus, Is.EqualTo(null));
            Assert.That(starcraftDataItem.Type, Is.EqualTo(null));
        }

        [Test(Description = "Test into dynamic using explicit mapping")]
        public void ReflectIntoDynamicWithExplicitMapping()
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

            var starcraftDataItem = testObject
                .ReflectSingle()
                .AddExplicitMapping("Unit", "Other")
                .IntoDynamicSingle();

            Assert.That(starcraftDataItem.Gas, Is.EqualTo(1));
            Assert.That(starcraftDataItem.Mineral, Is.EqualTo(2));
            Assert.That(starcraftDataItem.Race, Is.EqualTo("Protoss"));
            Assert.That(starcraftDataItem.Supply, Is.EqualTo("1"));
            Assert.That(starcraftDataItem.Time, Is.EqualTo("20"));
            Assert.That(starcraftDataItem.Other, Is.EqualTo("Test"));

            Assert.That(starcraftDataItem.Bonus, Is.EqualTo(null));
            Assert.That(starcraftDataItem.Type, Is.EqualTo(null));
        }
    }
}
