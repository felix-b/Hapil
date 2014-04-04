using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Happil;

namespace Happil.UnitTests
{
	[TestFixture]
	public class DictionaryShortcutsTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestGetItem()
		{
			//-- Arrange

			var inputDictionary = new Dictionary<int, string> {
				{ 123, "ABC" },
				{ 456, "DEF" },
				{ 789, "GHI" }
			};

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>, string>(cls => cls.DoStringTest).Implement((m, dictionary) => {
					m.Return(dictionary.Item(m.Const(456)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			var result = tester.DoStringTest(inputDictionary);

			//-- Assert

			Assert.That(result, Is.EqualTo("DEF"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSetItem()
		{
			//-- Arrange

			var inputDictionary = new Dictionary<int, string>();

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>>(cls => cls.DoTest).Implement((m, dictionary) => {
					dictionary.Item(m.Const(123)).Assign("ABC");
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			tester.DoTest(inputDictionary);

			//-- Assert

			Assert.That(inputDictionary[123], Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestAdd()
		{
			//-- Arrange

			var inputDictionary = new Dictionary<int, string>();

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>>(cls => cls.DoTest).Implement((m, dictionary) => {
					dictionary.Add(m.Const(123), m.Const("ABC"));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			tester.DoTest(inputDictionary);

			//-- Assert

			Assert.That(inputDictionary[123], Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestClear()
		{
			//-- Arrange

			var inputDictionary = new Dictionary<int, string>() {
				{ 123, "ABC"},
				{ 456, "DEF"}
			};

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>>(cls => cls.DoTest).Implement((m, dictionary) => {
					dictionary.Clear();
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			tester.DoTest(inputDictionary);

			//-- Assert

			Assert.That(inputDictionary.Count, Is.EqualTo(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestContainsKey()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>, bool>(cls => cls.DoBooleanTest).Implement((m, dictionary) => {
					m.Return(dictionary.ContainsKey(m.Const(123)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanTest(new Dictionary<int, string> { { 123, "ABC"} });
			var result2 = tester.DoBooleanTest(new Dictionary<int, string> { { 456, "DEF" } });

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestContainsValue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>, bool>(cls => cls.DoBooleanTest).Implement((m, dictionary) => {
					m.Return(dictionary.ContainsValue(m.Const("ABC")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			var result1 = tester.DoBooleanTest(new Dictionary<int, string> { { 123, "ABC" } });
			var result2 = tester.DoBooleanTest(new Dictionary<int, string> { { 456, "DEF" } });

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestRemove()
		{
			//-- Arrange

			var inputDictionary = new Dictionary<int, string>() {
				{ 123, "ABC"},
				{ 456, "DEF"},
				{ 789, "GHI"}
			};

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>>(cls => cls.DoTest).Implement((m, dictionary) => {
					dictionary.Remove(m.Const(456));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			tester.DoTest(inputDictionary);

			//-- Assert

			Assert.That(inputDictionary.Keys, Is.EqualTo(new[] { 123, 789 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTryGetValue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>, string>(cls => cls.DoStringTest).Implement((m, dictionary) => {
					var value = m.Local<string>();
					m.If(dictionary.TryGetValue(m.Const(123), value)).Then(() => {
						m.Return(value);
					})
					.Else(() => {
						m.Return((string)null);
					});
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			var result1 = tester.DoStringTest(new Dictionary<int, string> { { 123, "ABC" } });
			var result2 = tester.DoStringTest(new Dictionary<int, string> { { 456, "DEF" } });

			//-- Assert

			Assert.That(result1, Is.EqualTo("ABC"));
			Assert.That(result2, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCount()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>, int>(cls => cls.DoIntTest).Implement((m, dictionary) => {
					m.Return(dictionary.Count());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			var result = tester.DoIntTest(new Dictionary<int, string> { { 123, "ABC" }, { 456, "DEF" }, { 789, "GHI" } });

			//-- Assert

			Assert.That(result, Is.EqualTo(3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestKeys()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>, int[]>(cls => cls.DoKeysTest).Implement((m, dictionary) => {
					m.Return(dictionary.Keys().ToArray());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			var result = tester.DoKeysTest(new Dictionary<int, string> { { 123, "ABC" }, { 456, "DEF" }, { 789, "GHI" } });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { 123, 456, 789 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestValues()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>, string[]>(cls => cls.DoValuesTest).Implement((m, dictionary) => {
					m.Return(dictionary.Values().ToArray());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.DictionaryTester>().UsingDefaultConstructor();
			var result = tester.DoValuesTest(new Dictionary<int, string> { { 123, "ABC" }, { 456, "DEF" }, { 789, "GHI" } });

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "ABC", "DEF", "GHI" }));
		}
	}
}
