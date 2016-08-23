using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;

namespace Hapil.UnitTests
{
    internal static class ShouldAssertionsExtensions
    {
        public static void ShouldBeAtomicDictionaryEquivalentTo<TKey, TValue>(this AtomicDictionary<TKey, TValue> dictionaryUnderTest, IDictionary<TKey, TValue> expected)
        {
            dictionaryUnderTest.Count.Should().Be(expected.Count, "dictionary.Count");
            dictionaryUnderTest.Keys.Should().BeEquivalentTo(expected.Keys, "dictionary.Keys");

            var actualKeys = dictionaryUnderTest.Keys.ToArray();

            for ( int i = 0 ; i < actualKeys.Length ; i++ )
            {
                var key = actualKeys[i];
                var actualValue = dictionaryUnderTest[key];
                var expectedValue = expected[key];

                actualValue.Should().Be(expectedValue, "dictionary[{0}]", key);
            }
            
            dictionaryUnderTest.ToArray().Should().BeEquivalentTo(expected.ToArray(), "dictionary.ToArray()");

            foreach ( var key in expected.Keys )
            {
                dictionaryUnderTest.Contains(new KeyValuePair<TKey, TValue>(key, expected[key])).Should().BeTrue("dictionary.Contains({{{0},{1}}})", key, expected[key]);
                dictionaryUnderTest.ContainsKey(key).Should().BeTrue("dictionary.ContainsKey({0})", key);
                dictionaryUnderTest[key].Should().Be(expected[key], "dictionary[{0}]", key);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static void ShouldContainNothingFrom<TKey, TValue>(this AtomicDictionary<TKey, TValue> dictionaryUnderTest, IDictionary<TKey, TValue> unexpected)
        {
            foreach (var kvp in unexpected)
            {
                dictionaryUnderTest.Contains(kvp).Should().BeFalse("dictionary.Contains({{{0},{1}}})", kvp.Key, kvp.Value);
                dictionaryUnderTest.ContainsKey(kvp.Key).Should().BeFalse("dictionary.ContainsKey({0})", kvp.Key);

                var copyOfKvp = kvp;
                dictionaryUnderTest
                    .Invoking(
                        d => {
                            var value = d[copyOfKvp.Key];
                        })
                    .ShouldThrow<KeyNotFoundException>("dictionary[{0}]", kvp.Key);
            }
        }
    }
}
