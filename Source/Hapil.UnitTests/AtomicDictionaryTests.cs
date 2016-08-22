using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class AtomicDictionaryTests
    {
        [Test]
        public void EmptyDictionary()
        {
            //-- arrange & act

            var dictionary = new AtomicDictionary<int, string>();

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string> { });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string> { { 123, "ABC" } });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EmptyDictionary_SetItem_Added()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>();

            //-- act

            dictionary[123] = "ABC";

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string> { { 123, "ABC" } });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string> { { 456, "DEF" } });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EmptyDictionary_Add_Added()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>();

            //-- act

            dictionary.Add(123, "ABC");

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string> { { 123, "ABC" } });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string> { { 456, "DEF" } });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EmptyDictionary_AddKeyValuePair_Added()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>();

            //-- act

            dictionary.Add(new KeyValuePair<int, string>(123, "ABC"));

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string> { { 123, "ABC" } });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string> { { 456, "DEF" } });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EmptyDictionary_TryAdd_Added()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>();

            //-- act

            var added = dictionary.TryAdd(123, "ABC");

            //-- assert

            added.Should().Be(true);
            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string> { { 123, "ABC" } });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string> { { 456, "DEF" } });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EmptyDictionary_GetOrAdd_Added()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>();

            //-- act

            bool added;
            var value = dictionary.GetOrAdd(
                key: 123, 
                valueFactory: k => {
                    k.Should().Be(123);
                    return "ABC";
                },
                wasAdded: out added);

            //-- assert

            added.Should().Be(true);
            value.Should().Be("ABC");
            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string> { { 123, "ABC" } });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string> { { 456, "DEF" } });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary()
        {
            //-- arrange & act

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string>() {
                { 789, "GHI" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_SetItem_Changed()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            dictionary[789] = "GHI";

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "GHI" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_AddNewKey_Added()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            dictionary.Add(789, "GHI");

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "GHI" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_AddNewKeyValuePair_Added()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            dictionary.Add(new KeyValuePair<int, string>(789, "GHI"));

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "GHI" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_AddExistingKey_Throws()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            dictionary
                .Invoking(d => d.Add(456, "GHI"))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("*already added: 456*");

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_AddExistingKeyValuePair_Throws()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            dictionary
                .Invoking(d => d.Add(456, "GHI"))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("*already added: 456*");

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            });
        }
        
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_RemoveExistingKey_Removed()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            var removed = dictionary.Remove(456);

            //-- assert

            removed.Should().BeTrue();
            
            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" }
            });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string>() {
                { 456, "DEF" }
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_RemoveExistingKeyValuePair_Removed()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            var removed = dictionary.Remove(new KeyValuePair<int, string>(456, "DEF"));

            //-- assert

            removed.Should().BeTrue();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" }
            });
            dictionary.ShouldContainNothingFrom(new Dictionary<int, string>() {
                { 456, "DEF" }
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_RemoveNonExistentKeyValuePair_NoChange()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            var removed = dictionary.Remove(new KeyValuePair<int, string>(789, "GHI"));

            //-- assert

            removed.Should().BeFalse();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_RemoveNonExistentKey_NoChange()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            var removed = dictionary.Remove(789);

            //-- assert

            removed.Should().BeFalse();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NonEmptyDictionary_Clear_Emptied()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            dictionary.Clear();

            //-- assert

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() { });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void GetOrAdd_NewKey_ValueFactoryInvoked()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var valueFactoryCount = 0;

            //-- act

            bool added;
            var value = dictionary.GetOrAdd(
                789,
                valueFactory: k => {
                    valueFactoryCount++;
                    k.Should().Be(789);
                    return "GHI";
                },
                wasAdded: out added);

            //-- assert

            added.Should().BeTrue();
            valueFactoryCount.Should().Be(1);

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "GHI" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void GetOrAdd_ExistingKey_ValueFactoryNotInvoked()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var valueFactoryCount = 0;

            //-- act

            bool added;
            var value = dictionary.GetOrAdd(
                key: 456,
                valueFactory: k => {
                    valueFactoryCount++;
                    k.Should().Be(789);
                    return "DEF";
                },
                wasAdded: out added);

            //-- assert

            added.Should().BeFalse();
            valueFactoryCount.Should().Be(0);

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AddOrUpdate_NewKey_AddValueFactoryInvoked()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var addValueFactoryCount = 0;
            bool added;

            //-- act

            var value = dictionary.AddOrUpdate(
                key: 789,
                addValueFactory: k => {
                    addValueFactoryCount++;
                    k.Should().Be(789);
                    return "NEW!";
                },
                updateValueFactory: (k, v) => {
                    Assert.Fail("unexpected invocation of updateValueFactory.");
                    return null;
                },
                wasAdded: out added);

            //-- assert

            value.Should().Be("NEW!");
            added.Should().BeTrue();
            addValueFactoryCount.Should().Be(1);

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "NEW!" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AddOrUpdate_ExistingKey_UpdateValueFactoryInvoked()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var updateValueFactoryCount = 0;
            bool added;

            //-- act

            var value = dictionary.AddOrUpdate(
                key: 456, 
                addValueFactory: k => {
                    Assert.Fail("unexpected invocation of addValueFactory.");
                    return null;
                },
                updateValueFactory: (k, v) => {
                    updateValueFactoryCount++;
                    k.Should().Be(456);
                    return "UPDATED!";
                },
                wasAdded: out added);

            //-- assert

            value.Should().Be("UPDATED!");
            added.Should().BeFalse();
            updateValueFactoryCount.Should().Be(1);

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "UPDATED!" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void TryUpdate_ComparisonValueMatch_ValueUpdated()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            var updated = dictionary.TryUpdate(456, newValue: "UPDATED!", comparisonValue: "DEF");

            //-- assert

            updated.Should().BeTrue();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "UPDATED!" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void TryUpdate_ComparisonValueMismatch_ValueNotUpdated()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            //-- act

            var updated = dictionary.TryUpdate(456, newValue: "UPDATED!", comparisonValue: "GHI");

            //-- assert

            updated.Should().BeFalse();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void GetOrAdd_ValueFactoryThrows_NothingAddedAndExceptionThrown()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var valueFactoryCount = 0;
            var added = false;
            string value = null;

            Action action = () => {
                value = dictionary.GetOrAdd(
                    key: 789,
                    valueFactory: k => {
                        valueFactoryCount++;
                        k.Should().Be(789);
                        throw new TestValueFactoryException("TEST1");
                    },
                    wasAdded: out added);
            };

            //-- act

            action.ShouldThrow<TestValueFactoryException>().WithMessage("TEST1");

            //-- assert

            valueFactoryCount.Should().Be(1);
            added.Should().BeFalse();
            value.Should().BeNull();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void GetOrAdd_ValueFactoryThrowsOnce_CanAddLater()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var valueFactoryCount = 0;
            var added = false;
            string value = null;

            Action action = () => {
                value = dictionary.GetOrAdd(
                    key: 789,
                    valueFactory: k => {
                        k.Should().Be(789);
                        if ( valueFactoryCount++ == 0 )
                        {
                            throw new TestValueFactoryException("TEST1");
                        }
                        return "ADDED!";
                    },
                    wasAdded: out added);
            };

            //-- act

            action.ShouldThrow<TestValueFactoryException>().WithMessage("TEST1");
            action.ShouldNotThrow();

            //-- assert

            valueFactoryCount.Should().Be(2);
            added.Should().BeTrue();
            value.Should().Be("ADDED!");

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "ADDED!" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AddOrUpdate_AddValueFactoryThrows_NothingAddedAndExceptionThrown()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var addValueFactoryCount = 0;
            var added = false;
            string value = null;

            Action action = () => {
                value = dictionary.AddOrUpdate(
                    key: 789,
                    addValueFactory: k => {
                        addValueFactoryCount++;
                        k.Should().Be(789);
                        throw new TestValueFactoryException("TEST1");
                    },
                    updateValueFactory: (k, v) => {
                        Assert.Fail("unexpected invocation of updateValueFactory.");
                        return null;
                    },
                    wasAdded: out added);
            };

            //-- act

            action.ShouldThrow<TestValueFactoryException>().WithMessage("TEST1");

            //-- assert

            addValueFactoryCount.Should().Be(1);
            added.Should().BeFalse();
            value.Should().BeNull();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AddOrUpdate_AddValueFactoryThrows_CanBeAddedLater()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var addValueFactoryCount = 0;
            var added = false;
            string value = null;

            Action action = () => {
                value = dictionary.AddOrUpdate(
                    key: 789,
                    addValueFactory: k => {
                        k.Should().Be(789);
                        if ( addValueFactoryCount++ == 0 )
                        {
                            throw new TestValueFactoryException("TEST1");
                        }
                        return "ADDED!";
                    },
                    updateValueFactory: (k, v) => {
                        Assert.Fail("unexpected invocation of updateValueFactory.");
                        return null;
                    },
                    wasAdded: out added);
            };

            //-- act

            action.ShouldThrow<TestValueFactoryException>().WithMessage("TEST1");
            action.ShouldNotThrow();

            //-- assert

            addValueFactoryCount.Should().Be(2);
            added.Should().BeTrue();
            value.Should().Be("ADDED!");

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "ADDED!" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AddOrUpdate_UpdateValueFactoryThrows_NothingUpdatedAndExceptionThrown()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var updateValueFactoryCount = 0;
            var added = false;
            string value = null;

            Action action = () => {
                value = dictionary.AddOrUpdate(
                    key: 456,
                    addValueFactory: k => {
                        Assert.Fail("unexpected invocation of addValueFactory.");
                        return null;
                    },
                    updateValueFactory: (k, v) => {
                        updateValueFactoryCount++;
                        k.Should().Be(456);
                        throw new TestValueFactoryException("TEST1");
                    },
                    wasAdded: out added);
            };

            //-- act

            action.ShouldThrow<TestValueFactoryException>().WithMessage("TEST1");

            //-- assert

            updateValueFactoryCount.Should().Be(1);
            added.Should().BeFalse();
            value.Should().BeNull();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AddOrUpdate_UpdateValueFactoryThrows_CanBeUpdatedLater()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var updateValueFactoryCount = 0;
            var added = false;
            string value = null;

            Action action = () =>
            {
                value = dictionary.AddOrUpdate(
                    key: 456,
                    addValueFactory: k => {
                        Assert.Fail("unexpected invocation of addValueFactory.");
                        return null;
                    },
                    updateValueFactory: (k, v) => {
                        k.Should().Be(456);
                        if ( updateValueFactoryCount++ == 0 )
                        {
                            throw new TestValueFactoryException("TEST1");
                        }
                        return "UPDATED!";
                    },
                    wasAdded: out added);
            };

            //-- act

            action.ShouldThrow<TestValueFactoryException>().WithMessage("TEST1");
            action.ShouldNotThrow();

            //-- assert

            updateValueFactoryCount.Should().Be(2);
            added.Should().BeFalse();
            value.Should().Be("UPDATED!");

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "UPDATED!" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void GetOrAdd_ConcurrentRequestsForSameNewKey_ValueFactoryInvokedOnlyOnce()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var valueFactoryCount = 0;
            var inFactory = new ManualResetEvent(initialState: false);

            Func<object, Tuple<string, bool>> action = (state) => {
                if ( (int)state == 1 )
                {
                    inFactory.WaitOne();
                }
                bool added;
                var value = dictionary.GetOrAdd(
                    key: 789,
                    valueFactory: k => {
                        k.Should().Be(789);
                        Interlocked.Increment(ref valueFactoryCount);
                        inFactory.Set();
                        Thread.Sleep(1000);
                        return "ADDED!";
                    },
                    wasAdded: out added);
                return new Tuple<string, bool>(value, added);
            };

            var clock = Stopwatch.StartNew();

            //-- act

            var task1 = Task.Factory.StartNew(action, 0);
            var task2 = Task.Factory.StartNew(action, 1);

            task1.Wait();
            task2.Wait();

            var value1 = task1.Result.Item1;
            var added1 = task1.Result.Item2;

            var value2 = task2.Result.Item1;
            var added2 = task2.Result.Item2;

            //-- assert

            valueFactoryCount.Should().Be(1);
            value1.Should().Be("ADDED!");
            added1.Should().BeTrue();
            value2.Should().BeSameAs(value1);
            added2.Should().BeFalse();

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 789, "ADDED!" },
            });
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void GetOrAdd_ConcurrentRequestsForDifferentNewKeys_NoContention()
        {
            //-- arrange

            var dictionary = new AtomicDictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" }
            };

            var valueFactoryCount = 0;
            var inFactory = new ManualResetEvent(initialState: false);

            Func<object, Tuple<string, bool, TimeSpan>> action = (state) => {
                var clock = Stopwatch.StartNew();
                var intState = (int)state;
                if (intState == 2)
                {
                    inFactory.WaitOne();
                }
                bool added;
                var value = dictionary.GetOrAdd(
                    key: 111 * intState,
                    valueFactory: k => {
                        k.Should().Be(111 * intState);
                        Interlocked.Increment(ref valueFactoryCount);
                        inFactory.Set();
                        if ( intState == 1 )
                        {
                            Thread.Sleep(1000);
                        }
                        return "ADDED-" + k;
                    },
                    wasAdded: out added);
                return new Tuple<string, bool, TimeSpan>(value, added, clock.Elapsed);
            };

            //-- act

            var task1 = Task.Factory.StartNew(action, 1);
            var task2 = Task.Factory.StartNew(action, 2);

            task1.Wait();
            task2.Wait();

            var value1 = task1.Result.Item1;
            var added1 = task1.Result.Item2;
            var time1 = task1.Result.Item3;

            var value2 = task2.Result.Item1;
            var added2 = task2.Result.Item2;
            var time2 = task2.Result.Item3;

            //-- assert

            valueFactoryCount.Should().Be(2);
            value1.Should().Be("ADDED-111");
            added1.Should().BeTrue();
            value2.Should().Be("ADDED-222");
            added2.Should().BeTrue();

            time1.TotalSeconds.Should().BeGreaterThan(1.0d);
            time2.TotalSeconds.Should().BeLessThan(0.5d);

            dictionary.ShouldBeAtomicDictionaryEquivalentTo(new Dictionary<int, string>() {
                { 123, "ABC" },
                { 456, "DEF" },
                { 111, "ADDED-111" },
                { 222, "ADDED-222" },
            });

            Console.WriteLine("time1={0}, time2={1}", time1.TotalSeconds, time2.TotalSeconds);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class TestValueFactoryException : Exception
        {
            public TestValueFactoryException(string message)
                : base(message)
            {
            }
        }
    }
}
