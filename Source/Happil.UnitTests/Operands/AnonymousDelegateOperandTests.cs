using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Happil.Operands;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression

namespace Happil.UnitTests.Operands
{
	[TestFixture]
	public class AnonymousDelegateOperandTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void Action0Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method(x => x.TestAction).Implement(w => {
					testerField.Void<Action>(x => x.Acton0, w.Delegate(ww => logField.Add(ww.Const("Action0"))));		
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			obj.TestAction();

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] { "Action0" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Action1Argument()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method(x => x.TestAction).Implement(w => {
					testerField.Void(x => x.Acton1, w.Delegate<int>((ww, n) => 
						logField.Add(ww.Const("Action1({0})").Format(n))
					));
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			obj.TestAction();

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] { "Action1(123)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Action2Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method(x => x.TestAction).Implement(w => {
					testerField.Void(x => x.Acton2, w.Delegate<int, string>((ww, n, s) =>
						logField.Add(ww.Const("Action2({0},{1})").Format(n, s))
					));
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			obj.TestAction();

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] { "Action2(123,ABC)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Action3Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method(x => x.TestAction).Implement(w => {
					testerField.Void(x => x.Acton3, w.Delegate<int, string, char>((ww, n, s, c) =>
						logField.Add(ww.Const("Action3({0},{1},{2})").Format(n, s, c))
					));
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			obj.TestAction();

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] { "Action3(123,ABC,@)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Action4Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method(x => x.TestAction).Implement(w => {
					testerField.Void(x => x.Acton4, w.Delegate<int, string, char, string>((ww, n, s, c, s2) =>
						logField.Add(ww.Const("Action4({0},{1},{2},{3})").Format(n, s, c, s2))
					));
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			obj.TestAction();

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] { "Action4(123,ABC,@,DEF)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ActionAsGenericDelegate()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method(x => x.TestAction).Implement(w => {
					testerField.Void(
						x => (a, c, s) => x.GenericActon(a, c, s),
						w.Delegate<char, string>((ww, c, s) => logField.Add(ww.Const("GenericActon<char,string>({0},{1})").Format(c, s))),
						w.Const('c'),
						w.Const("S"));
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			obj.TestAction();

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] { "GenericActon<char,string>(c,S)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Func0Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int>, int>(x => x.Func0, w.Delegate<int>(ww => {
						logField.Add(ww.Const("Func0()"));
						ww.Return(999);
					})));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo(999));
			Assert.That(log, Is.EqualTo(new[] { "Func0()" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Func1Argument()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, string>, string>(x => x.Func1, w.Delegate<int, string>((ww, n) => {
						logField.Add(ww.Const("Func1<int,string>({0})").Format(n));
						ww.Return("ZZZ");
					})));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo("ZZZ"));
			Assert.That(log, Is.EqualTo(new[] { "Func1<int,string>(123)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Func2Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, char, string>, string>(x => x.Func2, w.Delegate<int, char, string>((ww, n, c) => {
						logField.Add(ww.Const("Func2<int,char,string>({0},{1})").Format(n,c));
						ww.Return("ZZZ");
					})));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo("ZZZ"));
			Assert.That(log, Is.EqualTo(new[] { "Func2<int,char,string>(123,@)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Func3Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, string, char, int>, int>(x => x.Func3, w.Delegate<int, string, char, int>((ww, n, s, c) => {
						logField.Add(ww.Const("Func3<int,string,char,int>({0},{1},{2})").Format(n, s, c));
						ww.Return(987);
					})));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo(987));
			Assert.That(log, Is.EqualTo(new[] { "Func3<int,string,char,int>(123,ABC,@)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Func4Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, string, char, string, char>, char>(x => x.Func4, w.Delegate<int, string, char, string, char>((ww, n, s, c, s2) => {
						logField.Add(ww.Const("Func4<int,string,char,string,char>({0},{1},{2},{3})").Format(n, s, c, s2));
						ww.Return('#');
					})));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo('#'));
			Assert.That(log, Is.EqualTo(new[] { "Func4<int,string,char,string,char>(123,ABC,@,DEF)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FuncAsGenericDelegate()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, char, string>, int, char, string>(
						x => (f, n, c) => x.GenericFunc(f, n, c), 
						w.Delegate<int, char, string>((ww, n, c) => {
							logField.Add(ww.Const("GenericFunc<int,char,string>({0},{1})").Format(n, c));
							ww.Return("ZYX");
						}),
						w.Const(555),
						w.Const('#')));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo("ZYX"));
			Assert.That(log, Is.EqualTo(new[] { "GenericFunc<int,char,string>(555,#)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Lambda0Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int>, int>(
						x => x.Func0, 
						w.Lambda(() => w.Const(321))));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo(321));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Lambda1Argument()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, string>, string>(
						x => x.Func1, 
						w.Lambda<int, string>(n => "S" + n.FuncToString())));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo("S123"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Lambda2Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, char, string>, string>(
						x => x.Func2, 
						w.Lambda<int, char, string>((n, c) => c.FuncToString() + n.FuncToString())));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo("@123"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Lambda3Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, string, char, int>, int>(
						x => x.Func3, 
						w.Lambda<int, string, char, int>((n, s, c) => n + s.Length())));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo(126));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Lambda4Arguments()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, string, char, string, char>, char>(
						x => x.Func4, 
						w.Lambda<int, string, char, string, char>((n, s, c, s2) => (n + s.Length() + c.CastTo<int>() + s2.Length()).CastTo<char>())));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo((char)(123 + 3 + '@' + 3)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void LambdaAsGenericDelegate()
		{
			//-- Arrange

			Field<List<string>> logField;
			Field<DelegateTester> testerField;

			DeriveClassFrom<object>()
				.ImplementInterface<AncestorRepository.ITester>()
				.PrimaryConstructor("Log", out logField, "Tester", out testerField)
				.Method<object>(x => x.TestFunc).Implement(w => {
					var value = w.Local(initialValue: testerField.Func<Func<int, char, string>, int, char, string>(
						x => (f, n, c) => x.GenericFunc(f, n, c),
						w.Lambda<int, char, string>((n, c) => n.FuncToString() + c.FuncToString()),
						w.Const(555),
						w.Const('#')));
					w.Return(value.CastTo<object>());
				})
				.AllMethods().Throw<NotImplementedException>();

			var log = new List<string>();
			var tester = new DelegateTester();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.ITester>().UsingConstructor(log, tester);
			var result = obj.TestFunc();

			//-- Assert

			Assert.That(result, Is.EqualTo("555#"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class DelegateTester
		{
			public void Acton0(Action action)
			{
				action();
			}
			public void Acton1(Action<int> action)
			{
				action(123);
			}
			public void Acton2(Action<int, string> action)
			{
				action(123, "ABC");
			}
			public void Acton3(Action<int, string, char> action)
			{
				action(123, "ABC", '@');
			}
			public void Acton4(Action<int, string, char, string> action)
			{
				action(123, "ABC", '@', "DEF");
			}
			public void GenericActon<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
			{
				action(t1, t2);
			}
			public int Func0(Func<int> function)
			{
				return function();
			}
			public string Func1(Func<int, string> function)
			{
				return function(123);
			}
			public string Func2(Func<int, char, string> function)
			{
				return function(123, '@');
			}
			public int Func3(Func<int, string, char, int> function)
			{
				return function(123, "ABC", '@');
			}
			public char Func4(Func<int, string, char, string, char> function)
			{
				return function(123, "ABC", '@', "DEF");
			}
			public TReturn GenericFunc<T1, T2, TReturn>(Func<T1, T2, TReturn> function, T1 t1, T2 t2)
			{
				return function(t1, t2);
			}
		}
	}
}

// ReSharper restore ConvertToLambdaExpression
