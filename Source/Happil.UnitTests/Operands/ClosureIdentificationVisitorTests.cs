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
	public class ClosureIdentificationVisitorTests : ClosureTestFixtureBase
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("One", out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.False);
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("One", out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.False);
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			visitor.DefineClosures();

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.True);

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				visitor.Externals.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				visitor.Captures.ToStringArray());

			Assert.That(
				visitor.ClosuresOuterToInner.SelectMany(closure => closure.Captures).All(capture => capture.HoistingClosure != null),
				"At least one captured operand was not hoisted");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ExternalArgumentAndLocal_ClosureRequired_OneClosureDefined()
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

			MethodMember methodTwo;
			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out methodTwo, out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			visitor.DefineClosures();

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.True);
			Assert.That(visitor.InnermostClosure, Is.Not.Null);
			Assert.That(visitor.OutermostClosure, Is.SameAs(visitor.InnermostClosure));
			Assert.That(visitor.InnermostClosure.ScopeBlock, Is.SameAs(methodTwo.Body));

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				visitor.OutermostClosure.Captures.ToStringArray());

			Assert.That(
				visitor.ClosuresOuterToInner.SelectMany(closure => closure.Captures).All(capture => capture.HoistingClosure != null),
				"At least one captured operand was not hoisted");
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("One", out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.False);
			Assert.That(visitor.Externals.ToStringArray(), Is.Empty);
			Assert.That(visitor.Captures.ToStringArray(), Is.EqualTo(new[] { "this" }));
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			visitor.DefineClosures();

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.True);

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				visitor.Externals.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]", "this" },
				visitor.Captures.ToStringArray());

			Assert.That(
				visitor.ClosuresOuterToInner.SelectMany(closure => closure.Captures).All(capture => capture.HoistingClosure != null),
				"At least one captured operand was not hoisted");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ExternalsAndThis_ClosureRequired_OneClosureDefined()
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

			MethodMember methodTwo;
			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out methodTwo, out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			visitor.DefineClosures();

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.True);
			Assert.That(visitor.InnermostClosure, Is.Not.Null);
			Assert.That(visitor.OutermostClosure, Is.SameAs(visitor.InnermostClosure));
			Assert.That(visitor.InnermostClosure.ScopeBlock, Is.SameAs(methodTwo.Body));

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]", "this" },
				visitor.OutermostClosure.Captures.ToStringArray());

			Assert.That(
				visitor.ClosuresOuterToInner.SelectMany(closure => closure.Captures).All(capture => capture.HoistingClosure != null), 
				"At least one captured operand was not hoisted");
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			visitor.DefineClosures();

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.True);

			CollectionAssert.AreEquivalent(
				new[] { "Local2[Int32]", "Local3[Int32]", "Arg1[n]" },
				visitor.Externals.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] { "Local2[Int32]", "Local3[Int32]", "Arg1[n]", "this" },
				visitor.Captures.ToStringArray());

			Assert.That(
				visitor.ClosuresOuterToInner.SelectMany(closure => closure.Captures).All(capture => capture.HoistingClosure != null),
				"At least one captured operand was not hoisted");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ThisAndExternalsFromTwoScopes_ScopesIdentifiedAndLinked()
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

			MethodMember methodTwo;
			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out methodTwo, out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			visitor.DefineClosures();

			//-- Assert

			CollectionAssert.AreEquivalent(
				new[] { "Arg1[n]", "Local2[Int32]", "Local3[Int32]", "Local4[Int32]", "this" },
				visitor.Captures.ToStringArray());
			
			Assert.That(visitor.Captures.Find("this").SourceOperandHome, Is.Null, "'this' reference");
			Assert.That(visitor.Captures.Find("Arg1[n]").SourceOperandHome, Is.SameAs(methodTwo.Body), "'n' argument");
			Assert.That(visitor.Captures.Find("Local2[Int32]").SourceOperandHome, Is.SameAs(methodTwo.Body), "'r' variable");
			Assert.That(visitor.Captures.Find("Local3[Int32]").SourceOperandHome, Is.SameAs(methodTwo.Body), "'i' loop counter variable");
			Assert.That(visitor.Captures.Find("Local4[Int32]").SourceOperandHome, Is.Not.SameAs(methodTwo.Body), "'indexMultiply' variable");

			Assert.That(methodTwo.Body.ParentBlock, Is.Null);
			Assert.That(visitor.Captures.Find("Local4[Int32]").SourceOperandHome.ParentBlock, Is.SameAs(methodTwo.Body));
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

			MethodMember methodTwo;
			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out methodTwo, out lambdaAnonymousMethod);

			//-- Act

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			visitor.DefineClosures();

			//-- Assert

			Assert.That(visitor.ClosuresRequired, Is.True);
			Assert.That(visitor.InnermostClosure, Is.Not.Null);
			Assert.That(visitor.OutermostClosure, Is.Not.Null);
			Assert.That(visitor.OutermostClosure, Is.Not.SameAs(visitor.InnermostClosure));
			Assert.That(visitor.OutermostClosure.ScopeBlock, Is.SameAs(methodTwo.Body));
			Assert.That(visitor.OutermostClosure.ChildCount, Is.EqualTo(1));
			Assert.That(visitor.OutermostClosure.Children, Is.EqualTo(new[] { visitor.InnermostClosure }));
			Assert.That(visitor.InnermostClosure.Parent, Is.SameAs(visitor.OutermostClosure));
			Assert.That(visitor.InnermostClosure.ScopeBlock.ParentBlock, Is.SameAs(methodTwo.Body));

			CollectionAssert.AreEquivalent(
				new[] { "Arg1[n]", "Local2[Int32]", "Local3[Int32]", "this" },
				visitor.OutermostClosure.Captures.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] { "Arg1[n]", "Local2[Int32]", "Local3[Int32]", "this", "Local4[Int32]" }, // parent captures are pulled by child closure
				visitor.InnermostClosure.Captures.ToStringArray());

			Assert.That(
				visitor.ClosuresOuterToInner.SelectMany(closure => closure.Captures).All(capture => capture.HoistingClosure != null),
				"At least one captured operand was not hoisted");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override bool SuppressAutomaticClosures
		{
			get
			{
				return true;
			}
		}
	}
}
