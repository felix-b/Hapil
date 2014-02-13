using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class StringShortcutsTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestCharAt()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, char>(cls => cls.DoCharTest).Implement((m, str) => {
					m.Return(str.CharAt(m.Const(2)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoCharTest("ABCD");

			//-- Assert

			Assert.That(result, Is.EqualTo('C'));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCompare()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string, int>(cls => cls.DoIntBinaryTest).Implement((m, str1, str2) => {
					m.Return(str1.Compare(str2));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result1 = obj.DoIntBinaryTest("ABC", "ABC");
			var result2 = obj.DoIntBinaryTest("ABC", "DEF");
			var result3 = obj.DoIntBinaryTest("ABC", "abc");

			//-- Assert

			Assert.That(result1, Is.EqualTo(0));
			Assert.That(result2, Is.LessThan(0));
			Assert.That(result3, Is.Not.EqualTo(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCompareIgnoreCase()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string, int>(cls => cls.DoIntBinaryTest).Implement((m, str1, str2) => {
					m.Return(str1.Compare(str2, ignoreCase: true));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result1 = obj.DoIntBinaryTest("ABC", "ABC");
			var result2 = obj.DoIntBinaryTest("ABC", "abc");
			var result3 = obj.DoIntBinaryTest("ABC", "DEF");

			//-- Assert

			Assert.That(result1, Is.EqualTo(0));
			Assert.That(result2, Is.EqualTo(0));
			Assert.That(result3, Is.LessThan(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLength()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.Length());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("ABCD");

			//-- Assert

			Assert.That(result, Is.EqualTo(4));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------


	}
}
