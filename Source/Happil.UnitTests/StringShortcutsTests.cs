using System;
using System.Collections.Generic;
using System.Globalization;
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
		public void TestConcat()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string, string>(cls => cls.DoBinaryTest).Implement((m, str1, str2) => {
					m.Return(str1.Concat(str2));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoBinaryTest("ABC", "DEF");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABCDEF"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestConcatWithArrayOfOperands()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string, string>(cls => cls.DoBinaryTest).Implement((m, str1, str2) => {
					m.Return(str1.Concat(m.Const("=^"), str2, m.Const("$")));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoBinaryTest("ABC", "DEF");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABC=^DEF$"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCopy()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Copy());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var input = "ABC";
			var output = obj.DoTest(input);

			//-- Assert

			Assert.That(output, Is.EqualTo("ABC"));
			Assert.That(output, Is.Not.SameAs(input));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestCopyTo()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, char[]>(cls => cls.DoCharArrayTest).Implement((m, str) => {
					var array = m.Local(m.NewArray<char>(m.Const(3)));
					str.CopyTo(m.Const(1), array, m.Const(0), array.Length());
					m.Return(array);
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoCharArrayTest("0123456789");

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { '1', '2', '3' }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestEndsWith()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, bool>(cls => cls.DoBooleanTest).Implement((m, str) => {
					m.Return(str.EndsWith(m.Const("Z")));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result1 = obj.DoBooleanTest("XYZ");
			var result2 = obj.DoBooleanTest("XY");

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestStringEquals()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, bool>(cls => cls.DoBooleanTest).Implement((m, str) => {
					m.Return(str.StringEquals(m.Const("ABC")));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result1 = obj.DoBooleanTest("ABC");
			var result2 = obj.DoBooleanTest("DEF");

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFormatWithOperandArray()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Format(m.Const("ABC"), m.Const(123).CastTo<object>()));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("STR={0},NUM={1}");

			//-- Assert

			Assert.That(result, Is.EqualTo("STR=ABC,NUM=123"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFormatWithArrayOperand()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Format(m.NewArray<object>("ABC", 123)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("STR={0},NUM={1}");

			//-- Assert

			Assert.That(result, Is.EqualTo("STR=ABC,NUM=123"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFormatWithOperandArrayAndProvider()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Format(Static.Prop(() => CultureInfo.CurrentCulture), m.Const("ABC"), m.Const(123).CastTo<object>()));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("STR={0},NUM={1}");

			//-- Assert

			Assert.That(result, Is.EqualTo("STR=ABC,NUM=123"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFormatWithArrayOperandAndProvider()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Format(Static.Prop(() => CultureInfo.CurrentCulture), m.NewArray<object>("ABC", 123)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("STR={0},NUM={1}");

			//-- Assert

			Assert.That(result, Is.EqualTo("STR=ABC,NUM=123"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestStringGetHashCode()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.StringGetHashCode());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("ABC");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABC".GetHashCode()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIndexOf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.IndexOf(m.Const("123")));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("ABC-123-DEF");

			//-- Assert

			Assert.That(result, Is.EqualTo(4));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIndexOfWithStartIndexAndCount()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.IndexOf(m.Const("123"), m.Const(1), m.Const(10)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("123-123-DEF");

			//-- Assert

			Assert.That(result, Is.EqualTo(4));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIndexOfAny()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.IndexOfAny(m.NewArray<char>('1','2','3')));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("ABC-123-DEF");

			//-- Assert

			Assert.That(result, Is.EqualTo(4));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestIndexOfAnyWithStartIndexAndCount()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.IndexOfAny(m.NewArray<char>('1', '2', '3'), m.Const(3), m.Const(5)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("123-123-DEF");

			//-- Assert

			Assert.That(result, Is.EqualTo(4));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------



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
