using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;
using NUnit.Framework;

namespace Happil.UnitTests.Operands
{
	[TestFixture]
	public class AnonymousMethodIdentificationVisitorTests : ClosureTestFixtureBase
	{
		[Test]
		public void NoExternalOperands_NoClosures()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method(intf => intf.One).Implement(w => {
					var input = w.Local(w.NewArray<int>(1, 2, 3));
					var output = w.Local(input.Where(w.Lambda<int, bool>(x => (x % 2) == 0)));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			WriteMethod("One");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();
			
			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(base.Class));
			Assert.That(base.Class.GetNestedClasses(), Is.Empty);
			Assert.That(anonymousMethod.IsStatic);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void InternalLocalsAndArguments_NoClosures()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method(intf => intf.One).Implement(w => {
					var input = w.Local(w.NewArray<int>(1, 2, 3));
					var output = w.Local(input.Where(w.Delegate<int, bool>((del, x) => {
						var remainder = del.Local(initialValue: x % 2);
						del.Return(remainder == 0);
					})));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			WriteMethod("One");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(base.Class));
			Assert.That(base.Class.GetNestedClasses(), Is.Empty);
			Assert.That(anonymousMethod.IsStatic);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ExternalArgumentAndLocal_ClosureRequired_ExternalsCaptured()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int>(intf => intf.Two).Implement((w, n) => {
					var r = w.Local<int>(initialValueConst: 1);
					var input = w.Local(w.NewArray<int>(1, 2, 3));
					var output = w.Local(input.Where(w.Lambda<int, bool>(x => (x % n) == r)));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			WriteMethod("Two");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();
			var closureClass = base.Class.GetNestedClasses().Single();

			Assert.That(anonymousMethod.OwnerClass, Is.Not.SameAs(base.Class));
			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(closureClass));
			Assert.That(anonymousMethod.IsStatic, Is.False);
			Assert.That(anonymousMethod.IsPublic, Is.True);

			Assert.That(closureClass.IsClosure);
			Assert.That(closureClass.ClosureDefinition, Is.Not.Null);

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Loc0_Int32]", "InstanceField[<hoisted>Arg_n]" },
				closureClass.ClosureDefinition.Captures.ToStringArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ThisReference_NoClosures_ThisCaptured()
		{
			//-- Arrange

			Field<int> remainder;

			DeriveClassFrom<object>()
				.PrimaryConstructor("remainder", out remainder)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method(intf => intf.One).Implement(w => {
					var input = w.Local(w.NewArray<int>(1, 2, 3));
					var output = w.Local(input.Where(w.Lambda<int, bool>(x => (x % 3) == remainder)));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			WriteMethod("One");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(base.Class));
			Assert.That(base.Class.GetNestedClasses(), Is.Empty);
			
			Assert.That(anonymousMethod.IsStatic, Is.False);
			Assert.That(anonymousMethod.IsPublic, Is.False);
			Assert.That(anonymousMethod.HasClosure, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ExternalsAndThis_ClosureRequired_ExternalsAndThisCaptured()
		{
			//-- Arrange

			Field<int> remainder;

			DeriveClassFrom<object>()
				.PrimaryConstructor("remainder", out remainder)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int>(intf => intf.Two).Implement((w, n) => {
					var r = w.Local<int>(initialValueConst: 1);
					var input = w.Local(w.NewArray<int>(1, 2, 3));
					var output = w.Local(input.Where(w.Lambda<int, bool>(x => (x % n) == r + remainder)));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			WriteMethod("Two");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();
			var closureClass = base.Class.GetNestedClasses().Single();

			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(closureClass));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Loc0_Int32]", "InstanceField[<hoisted>Arg_n]", "InstanceField[<hoisted>This]" },
				closureClass.ClosureDefinition.Captures.ToStringArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ThisAndExternalsFromMultipleScopes_ClosureRequired_ThisAndExternalsCaptured()
		{
			//-- Arrange

			Field<int> remainder;

			DeriveClassFrom<object>()
				.PrimaryConstructor("remainder", out remainder)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int>(intf => intf.Two).Implement((w, n) => {
					var input = w.Local(w.NewArray<int>(1, 2, 3));
					var r = w.Local<int>(initialValueConst: 1);
					w.For(from: 1, to: 10, increment: 1).Do((loop, i) => {
						var output = w.Local(input.Where(w.Lambda<int, bool>(x => (x % n) == r + i + remainder)));
					});
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			WriteMethod("Two");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();
			var closureClass = base.Class.GetNestedClasses().Single();

			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(closureClass));

			CollectionAssert.AreEquivalent(
				new[] {
					"InstanceField[<hoisted>Loc2_Int32]", 
					"InstanceField[<hoisted>Loc3_Int32]", 
					"InstanceField[<hoisted>Arg_n]", 
					"InstanceField[<hoisted>This]"
				},
				closureClass.ClosureDefinition.Captures.ToStringArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ThisAndExternalsFromTwoScopes_TwoClosuresDefined()
		{
			//-- Arrange

			Field<int> remainder;

			DeriveClassFrom<object>()
				.PrimaryConstructor("Remainder", out remainder)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int>(intf => intf.Two).Implement((w, n) => {		// arg1[n]
					var input = w.Local(w.NewArray<int>(1, 2, 3));			// local0[new-arr], local1[input]
					var r = w.Local<int>(initialValueConst: 1);				// local2[r]
					w.For(from: 1, to: 10, increment: 1).Do((loop, i) => {  // local3[i]
						var indexMultiply = w.Local(initialValue: i * 2);	// local4[indexMultiply]
						var output = w.Local(input.Where(w.Lambda<int, bool>(x => (x % n) == (r + i + indexMultiply + remainder))));
					});
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			WriteMethod("Two");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();
			var closureClasses = base.Class.GetNestedClasses();

			Assert.That(closureClasses.Length, Is.EqualTo(2));
			Assert.That(closureClasses[0].IsClosure);
			Assert.That(closureClasses[1].IsClosure);

			Assert.That(anonymousMethod.OwnerClass, Is.Not.SameAs(base.Class));
			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(closureClasses[1]));
			Assert.That(anonymousMethod.IsStatic, Is.False);
			Assert.That(anonymousMethod.IsPublic, Is.True);

			CollectionAssert.AreEquivalent(
				new[] {
					"InstanceField[<hoisted>Loc2_Int32]", 
					"InstanceField[<hoisted>Loc3_Int32]", 
					"InstanceField[<hoisted>Arg_n]",
					"InstanceField[<hoisted>This]"
				},
				closureClasses[0].ClosureDefinition.Captures.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] {
					"InstanceField[<hoisted>Loc2_Int32]", 
					"InstanceField[<hoisted>Loc3_Int32]", 
					"InstanceField[<hoisted>Arg_n]",
					"InstanceField[<hoisted>This]",
					"InstanceField[<hoisted>Loc4_Int32]", 
				},
				closureClasses[1].ClosureDefinition.Captures.ToStringArray());
		}
	}
}
