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

			OutputValue = null;

			var inputDictionary = new Dictionary<int, string> {
				{ 123, "ABC" },
				{ 456, "DEF" },
				{ 789, "GHI" }
			};

			DeriveClassFrom<DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>>(cls => cls.DoTest).Implement((m, dictionary) => {
					Static.Prop(() => OutputValue).Assign(dictionary.Item(m.Const(456)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<DictionaryTester>().UsingDefaultConstructor();
			tester.DoTest(inputDictionary);

			//-- Assert

			Assert.That(OutputValue, Is.EqualTo("DEF"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSetItem()
		{
			//-- Arrange

			var inputDictionary = new Dictionary<int, string>();

			DeriveClassFrom<DictionaryTester>()
				.DefaultConstructor()
				.Method<IDictionary<int, string>>(cls => cls.DoTest).Implement((m, dictionary) => {
					dictionary.Item(m.Const(123)).AssignConst("ABC");
				});

			//-- Act

			var tester = CreateClassInstanceAs<DictionaryTester>().UsingDefaultConstructor();
			tester.DoTest(inputDictionary);

			//-- Assert

			Assert.That(inputDictionary[123], Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static string OutputValue { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class DictionaryTester
		{
			public abstract void DoTest(IDictionary<int, string> dictionary);
		}
	}
}
