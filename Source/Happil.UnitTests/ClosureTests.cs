using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Happil.Operands;
using Happil.UnitTests.Assumptions;
using NUnit.Framework;

#if false
 
Closures design:

1. Determine whether an anonynous method accesses operands that must be captured in a closure -. needs closure(s)
2. Determine list of additional operands that need to be captured, when anonynous method is moved to a closure (this + anything else?). 
3. Generate closure class(es) containing captured operands and the anonymous method.
4. Instantiate closure class(es) in the beginning of their corresponding scope(s)
5. Rewrite access to captured operands to access them in the corresponding closure(s)

#endif

namespace Happil.UnitTests
{
	[TestFixture]
	public class ClosureTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CaptureNothing()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int>(intf => intf.Three).Implement(w => {
					var isOdd = w.Local(initialValue: w.Delegate<int, int>((ww, x) => {
						Static.Prop(() => AnonymousMethodInfo).Assign(Static.Func(MethodBase.GetCurrentMethod));
						ww.Return(123 % x);
					}));
					w.Return(isOdd.Invoke(w.Const(100)));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Three();

			//-- Assert

			Assert.That(result, Is.EqualTo(23));
			Assert.That(AnonymousMethodInfo.IsStatic, Is.True);
			Assert.That(AnonymousMethodInfo.DeclaringType, Is.SameAs(obj.GetType()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureField()
		{
			//-- Arrange

			Field<int> intField;

			DeriveClassFrom<object>()
				.Field("m_IntField", out intField)
				.Constructor<int>((w, intFieldValue) => intField.Assign(intFieldValue))
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int>(intf => intf.Three).Implement(w => {
					var isOdd = w.Local(initialValue: w.Delegate<int, int>((ww, x) => {
						Static.Prop(() => AnonymousMethodInfo).Assign(Static.Func(MethodBase.GetCurrentMethod));
						ww.Return(intField % x);
					}));
					w.Return(isOdd.Invoke(w.Const(100)));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor<int>(arg: 123);
			var result = obj.Three();

			//-- Assert

			Assert.That(result, Is.EqualTo(23));
			Assert.That(AnonymousMethodInfo.IsStatic, Is.False);
			Assert.That(AnonymousMethodInfo.DeclaringType, Is.SameAs(obj.GetType()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureLocal()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int>(intf => intf.Three).Implement(w => {
					var numerator = w.Local(123);
					var remainder = w.Local(initialValue: w.Delegate<int, int>((ww, x) => {
						Static.Prop(() => AnonymousMethodInfo).Assign(Static.Func(MethodBase.GetCurrentMethod));
						ww.Return(numerator % x);
					}));
					w.Return(remainder.Invoke(w.Const(100)));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Three();

			//-- Assert

			Assert.That(result, Is.EqualTo(23));
			Assert.That(AnonymousMethodInfo.IsStatic, Is.False);
			Assert.That(AnonymousMethodInfo.DeclaringType, Is.Not.SameAs(obj.GetType()));
			Assert.That(AnonymousMethodInfo.DeclaringType.IsNested);
			//Assert.That(obj.GetType().GetNestedTypes().Contains(AnonymousMethodInfo.DeclaringType));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureLocalAndArgument()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var numerator = w.Local(123);
					var remainder = w.Local(initialValue: w.Delegate<int, int>((ww, x) => {
						Static.Prop(() => AnonymousMethodInfo).Assign(Static.Func(MethodBase.GetCurrentMethod));
						var denominator = ww.Local<int>(initialValue: x + n);
						Static.Void<string, object, object>(Console.WriteLine, ww.Const("x={0} n={1}"), x.CastTo<object>(), n.CastTo<object>());
						Static.Void<string, object, object>(Console.WriteLine, ww.Const("numerator={0} denominator={1}"), numerator.CastTo<object>(), denominator.CastTo<object>());
						ww.Return(numerator % denominator);
					}));
					w.Return(remainder.Invoke(w.Const(1)).Func<string>(x => x.ToString));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Five(99);

			//-- Assert

			Assert.That(result, Is.EqualTo("23"));
			Assert.That(AnonymousMethodInfo.IsStatic, Is.False);
			Assert.That(AnonymousMethodInfo.DeclaringType, Is.Not.SameAs(obj.GetType()));
			Assert.That(AnonymousMethodInfo.DeclaringType.IsNested);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureLocalArgumentAndThisInLambda()
		{
			//-- Arrange

			Field<int> remainder;

			DeriveClassFrom<object>()
				.PrimaryConstructor("remainder", out remainder)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var r = w.Local<int>(initialValueConst: 1);
					var input = w.Local(w.NewArray<int>(100, 123, 200, 223));
					var query = w.Local(initialValue: input.Where(w.Lambda<int, bool>(x => (x % n) == r + remainder)));
					var results = w.Local(initialValue: query.Select(w.Lambda<int, string>(item => item.Func<string>(x => x.ToString))));
					w.Return(Static.Func(String.Join, w.Const(";"), results));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor<int>(22);
			var result = obj.Five(100);

			//-- Assert

			Assert.That(result, Is.EqualTo("123;223"));
			//Assert.That(AnonymousMethodInfo.IsStatic, Is.False);
			//Assert.That(AnonymousMethodInfo.DeclaringType, Is.Not.SameAs(obj.GetType()));
			//Assert.That(AnonymousMethodInfo.DeclaringType.IsNested);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureOperandsFromTwoDifferentScopes()
		{
			//-- Arrange

			Field<int> field;

			DeriveClassFrom<object>()
				.PrimaryConstructor("Field", out field)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, argument) => {				
					var input = w.Local(w.NewArray<int>(100, 101));
					var output = w.Local(w.New<List<string>>());
					w.For(from: 0, to: input.Length(), increment: 1).Do((loop, i) => {			
						var copyOfI = w.Local(initialValue: i);
						output.AddRange(input.Select(w.Lambda<int, string>(item => 
							field.FuncToString() + w.Const(":") +
							argument.FuncToString() + w.Const(":") +
							copyOfI.FuncToString() + w.Const(":") +
							item.FuncToString())));
					});
					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor<int>(22);
			var result = obj.Five(99);

			//-- Assert

			Assert.That(result, Is.EqualTo("22:99:0:100;22:99:0:101;22:99:1:100;22:99:1:101"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureOperandsFromNonConsecutiveScopes()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var input = w.Local(w.NewArray<int>(100, 101));
					var output = w.Local(w.New<List<string>>());
					w.For(from: 0, to: input.Length(), increment: 1).Do((loop, i) => 
						w.Try(() => {
							var copyOfI = w.Local(initialValue: i);
							output.AddRange(input.Select(w.Lambda<int, string>(item =>
								n.FuncToString() + w.Const(":") +
								copyOfI.FuncToString() + w.Const(":") +
								item.FuncToString())));
						})
						.Catch<Exception>(e => { })
					);
					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Five(99);

			//-- Assert

			Assert.That(result, Is.EqualTo("99:0:100;99:0:101;99:1:100;99:1:101"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureOperandsForOneOfTwoAnonymousMethods()
		{
			//-- Arrange

			Field<string> field;

			DeriveClassFrom<object>()
				.PrimaryConstructor("Field", out field)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var input = w.Local(w.NewArray<int>(100, 101));
					var output = w.Local(w.New<List<string>>());

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						n.FuncToString() + w.Const(":") +
						item.FuncToString())));

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						field + w.Const(":") +
						item.FuncToString())));

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor<string>("ABC");
			var result = obj.Five(11);

			//-- Assert

			Assert.That(result, Is.EqualTo("11:100;11:101;ABC:100;ABC:101"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureDifferentOperandsForTwoAnonymousMethods()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var prefix = w.Local("PFX");
					var input = w.Local(w.NewArray<int>(100, 101));
					var output = w.Local(w.New<List<string>>());

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						n.FuncToString() + w.Const(":") +
						item.FuncToString())));

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						prefix + w.Const(":") +
						item.FuncToString())));

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Five(11);

			//-- Assert

			Assert.That(result, Is.EqualTo("11:100;11:101;PFX:100;PFX:101"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureSameOperandsForTwoAnonymousMethods()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var prefix = w.Local("PFX");
					var input = w.Local(w.NewArray<int>(100, 101));
					var output = w.Local(w.New<List<string>>());

					n.Assign(99);

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						n.FuncToString() + w.Const(":") +
						item.FuncToString())));

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						prefix + w.Const(":") +
						n.FuncToString() + w.Const(":") +
						item.FuncToString())));

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Five(11);

			//-- Assert

			Assert.That(result, Is.EqualTo("99:100;99:101;PFX:99:100;PFX:99:101"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NestedAnonymousMethodsInSameClosure()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var input = w.Local(w.NewArray<int>(100, 101, 102, 103));
					var output = w.Local(w.New<List<string>>());

					output.AddRange(
						input.Select(
							w.Lambda<int, string>(item =>
								(item + n).FuncToString().ToCharArray().Select(c => 
									c.CastTo<int>() + n
								)
								.Sum()
								.FuncToString()
							)
						)
					);

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Five(11);

			//-- Assert

			Assert.That(result, Is.EqualTo("180;181;182;183"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NestedAnonymousMethodsInNestedClosures()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var input = w.Local(w.NewArray<int>(100, 101, 102, 103));
					var output = w.Local(w.New<List<string>>());

					output.AddRange(
						input.Select(
							w.Delegate<int, string>((ww, item) => {
								var number = ww.Local(initialValue: item + n);
								var digits = ww.Local(initialValue: number.FuncToString().ToCharArray());
								var sum = ww.Local(initialValue: digits.Select(c => c.CastTo<int>() + number + n).Sum());
								ww.Return(sum.FuncToString());
							})
						)
					);

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();
			var result = obj.Five(11);

			//-- Assert

			Assert.That(result, Is.EqualTo("513;517;521;525"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Manual test")]
		public void RunCompiledExamples()
		{
			var examples = new CompiledExamples.ClosureExamples();

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");

			examples.CapturedField();
			Console.WriteLine("CapturedField: {0}", string.Join(";", CompiledExamples.ClosureExamples.Output));
			Console.WriteLine();

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");

			examples.CapturedLocal();
			Console.WriteLine("CapturedLocal: {0}", string.Join(";", CompiledExamples.ClosureExamples.Output));

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");

			examples.CapturedArgument(y: 200);
			Console.WriteLine("CapturedArgument(y=200): {0}", string.Join(";", CompiledExamples.ClosureExamples.Output));

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");
		
			examples.CapturedFieldAndLocal();
			Console.WriteLine("CapturedFieldAndLocal: {0}", string.Join(";", CompiledExamples.ClosureExamples.Output));

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");

			examples.CapturedFieldAndLocalFromOuterScope();
			Console.WriteLine("CapturedFieldAndLocalFromOuterScope: {0}", string.Join(";", CompiledExamples.ClosureExamples.Output));

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");

			examples.CapturedFieldAndTwoLocalsFromDifferentScopes();
			Console.WriteLine("CapturedFieldAndTwoLocalsFromDifferentScopes: {0}", string.Join(";", CompiledExamples.ClosureExamples.Output));

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");

			examples.CapturedLoopCounter();
			Console.WriteLine("CapturedLoopCounter: {0}", string.Join(";", CompiledExamples.ClosureExamples.Output));

			CompiledExamples.ClosureExamples.Output.Clear();
			Console.WriteLine();
			Console.WriteLine("--------------------------");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static MethodBase AnonymousMethodInfo { get; set; }
	}
}
