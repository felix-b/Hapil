using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class StringShortcutsTests : NUnitEmittedTypesTestBase
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
					m.Return(str.Format(m.Const("ABC"), m.Const(123)));
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
					m.Return(str.Format(Static.Prop(() => CultureInfo.CurrentCulture), m.Const("ABC"), m.Const(123)));
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

		[Test]
		public void TestInsert()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string, string>(cls => cls.DoBinaryTest).Implement((m, strA, strB) => {
					m.Return(strA.Insert(m.Const(3), strB));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoBinaryTest("ABCDEF", "xyz");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABCxyzDEF"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLastIndexOf()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.LastIndexOf(m.Const("123")));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("123-123-123");

			//-- Assert

			Assert.That(result, Is.EqualTo(8));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLastIndexOfWithStartIndexAndCount()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.IndexOf(m.Const("123"), m.Const(7), m.Const(10)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result1 = obj.DoIntTest("123-123-123-GHI-JKL");
			var result2 = obj.DoIntTest("123-123-DEF-GHI-JKL");

			//-- Assert

			Assert.That(result1, Is.EqualTo(8));
			Assert.That(result2, Is.EqualTo(-1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLastIndexOfAny()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.LastIndexOfAny(m.NewArray<char>('1', '2', '3')));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoIntTest("ABC-123-DEF");

			//-- Assert

			Assert.That(result, Is.EqualTo(6));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLastIndexOfAnyWithStartIndexAndCount()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, int>(cls => cls.DoIntTest).Implement((m, str) => {
					m.Return(str.LastIndexOfAny(m.NewArray<char>('1', '2', '3'), m.Const(8), m.Const(8)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result1 = obj.DoIntTest("123-123-DEF");
			var result2 = obj.DoIntTest("123-ABC-DEF");

			//-- Assert

			Assert.That(result1, Is.EqualTo(6));
			Assert.That(result2, Is.EqualTo(2));
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

		[Test]
		public void TestPadLeft()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.PadLeft(m.Const(5)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("ABC");

			//-- Assert

			Assert.That(result, Is.EqualTo("  ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestPadRight()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.PadRight(m.Const(5)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("ABC");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABC  "));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestRemove()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Remove(m.Const(5)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("ABCDEFGH");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABCDE"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestRemoveWithCount()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Remove(m.Const(5), m.Const(3)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("ABCDEFGHIJKL");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABCDEIJKL"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestReplaceChar()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Replace(m.Const('$'), m.Const('@')));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("$abc$def");

			//-- Assert

			Assert.That(result, Is.EqualTo("@abc@def"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestReplaceString()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Replace(m.Const("$"), m.Const("@")));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("$abc$def");

			//-- Assert

			Assert.That(result, Is.EqualTo("@abc@def"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSplitByConstChars()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string[]>(cls => cls.DoSplitTest).Implement((m, str) => {
					m.Return(str.Split(',', ';'));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoSplitTest("ab,cd;ef");

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "ab", "cd", "ef" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSplitByConstCharsWithCountAndOptions()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string[]>(cls => cls.DoSplitTest).Implement((m, str) => {
					m.Return(str.Split(m.NewArray<char>(',', ';'), count: m.Const(3), options: m.Const(StringSplitOptions.RemoveEmptyEntries)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoSplitTest("ab,,cd;;ef,ij;kl");

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "ab", "cd", "ef,ij;kl" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSplitByConstString()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string[]>(cls => cls.DoSplitTest).Implement((m, str) => {
					m.Return(str.Split(",", ";"));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoSplitTest("ab,cd;ef");

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "ab", "cd", "ef" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSplitByConstStringsWithCountAndOptions()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string[]>(cls => cls.DoSplitTest).Implement((m, str) => {
					m.Return(str.Split(m.NewArray<string>(",", ";"), count: m.Const(3), options: m.Const(StringSplitOptions.RemoveEmptyEntries)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoSplitTest("ab,,cd;;ef,ij;kl");

			//-- Assert

			Assert.That(result, Is.EqualTo(new[] { "ab", "cd", "ef,ij;kl" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestStartsWith()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, bool>(cls => cls.DoBooleanTest).Implement((m, str) => {
					m.Return(str.StartsWith(m.Const("ABC")));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result1 = obj.DoBooleanTest("ABCDEF");
			var result2 = obj.DoBooleanTest("BCDEFG");

			//-- Assert

			Assert.That(result1, Is.True);
			Assert.That(result2, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSubstring()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Substring(m.Const(1)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("ABCDEF");

			//-- Assert

			Assert.That(result, Is.EqualTo("BCDEF"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestSubstringWithLength()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Substring(startIndex: m.Const(1), length: m.Const(3)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("ABCDEF");

			//-- Assert

			Assert.That(result, Is.EqualTo("BCD"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestToCharArray()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, char[]>(cls => cls.DoCharArrayTest).Implement((m, str) => {
					m.Return(str.ToCharArray());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoCharArrayTest("ABC");

			//-- Assert

			Assert.That(result, Is.EqualTo(new char[] { 'A', 'B', 'C' }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestToCharArrayWithStartIndexAndLength()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, char[]>(cls => cls.DoCharArrayTest).Implement((m, str) => {
					m.Return(str.ToCharArray(startIndex: m.Const(2), length: m.Const(3)));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoCharArrayTest("ABCDEF");

			//-- Assert

			Assert.That(result, Is.EqualTo(new char[] { 'C', 'D', 'E' }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestToLower()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.ToLower());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("ABC");

			//-- Assert

			Assert.That(result, Is.EqualTo("abc"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestToUpper()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.ToUpper());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("abc");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrim()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Trim());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("  ABC  ");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimWithCharOperandArray()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Trim('^', '$'));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("^^  ABC  $$");

			//-- Assert

			Assert.That(result, Is.EqualTo("  ABC  "));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimWithCharArrayOperand()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.Trim(m.NewArray<char>('^', '$')));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("^^  ABC  $$");

			//-- Assert

			Assert.That(result, Is.EqualTo("  ABC  "));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimEnd()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.TrimEnd());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("  ABC  ");

			//-- Assert

			Assert.That(result, Is.EqualTo("  ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimEndWithCharOperandArray()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.TrimEnd('^', '$'));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("^^  ABC  $$");

			//-- Assert

			Assert.That(result, Is.EqualTo("^^  ABC  "));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimEndWithCharArrayOperand()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.TrimEnd(m.NewArray<char>('^', '$')));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("^^  ABC  $$");

			//-- Assert

			Assert.That(result, Is.EqualTo("^^  ABC  "));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimStart()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.TrimStart());
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("  ABC  ");

			//-- Assert

			Assert.That(result, Is.EqualTo("ABC  "));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimStartWithCharOperandArray()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.TrimStart('^', '$'));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("^^  ABC  $$");

			//-- Assert

			Assert.That(result, Is.EqualTo("  ABC  $$"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTrimStartWithCharArrayOperand()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StringTester>()
				.DefaultConstructor()
				.Method<string, string>(cls => cls.DoTest).Implement((m, str) => {
					m.Return(str.TrimStart(m.NewArray<char>('^', '$')));
				});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.StringTester>().UsingDefaultConstructor();
			var result = obj.DoTest("^^  ABC  $$");

			//-- Assert

			Assert.That(result, Is.EqualTo("  ABC  $$"));
		}
	}
}
