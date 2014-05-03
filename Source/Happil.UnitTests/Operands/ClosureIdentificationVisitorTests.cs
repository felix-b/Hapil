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
	public class ClosureIdentificationVisitorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void NoExternalOperands_NoClosure()
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("One", out lambdaAnonymousMethod);

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			Assert.That(visitor.IsClosureRequired, Is.False);
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out lambdaAnonymousMethod);

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);
			
			//-- Assert

			Assert.That(visitor.IsClosureRequired, Is.True);

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				visitor.Externals.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				visitor.Captures.ToStringArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ThisReference_NoClosure_ThisCaptured()
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("One", out lambdaAnonymousMethod);

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			Assert.That(visitor.IsClosureRequired, Is.False);
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

			//-- Act

			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out lambdaAnonymousMethod);

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			Assert.That(visitor.IsClosureRequired, Is.True);

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				visitor.Externals.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]", "this" },
				visitor.Captures.ToStringArray());
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

			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out lambdaAnonymousMethod);

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			Assert.That(visitor.IsClosureRequired, Is.True);

			CollectionAssert.AreEquivalent(
				new[] { "Local2[Int32]", "Local3[Int32]", "Arg1[n]" },
				visitor.Externals.ToStringArray());

			CollectionAssert.AreEquivalent(
				new[] { "Local2[Int32]", "Local3[Int32]", "Arg1[n]", "this" },
				visitor.Captures.ToStringArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Not yet implemented")]
		public void ThisAndExternalsFromMultipleScopes_ScopesIdentifiedAndLinked()
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

			MethodMember methodTwo;
			MethodMember lambdaAnonymousMethod;
			WriteMethods("Two", out methodTwo, out lambdaAnonymousMethod);

			var visitor = new ClosureIdentificationVisitor(lambdaAnonymousMethod);
			lambdaAnonymousMethod.AcceptVisitor(visitor);

			//-- Assert

			CollectionAssert.AreEquivalent(
				new[] { "Local2[Int32]", "Local3[Int32]", "Arg1[n]", "this" },
				visitor.Captures.ToStringArray());
			
			Assert.That(visitor.Captures.Find("this").SourceOperandHome, Is.Null);
			Assert.That(visitor.Captures.Find("Arg1[n]").SourceOperandHome, Is.SameAs(methodTwo.Body));
			Assert.That(visitor.Captures.Find("Local2[Int32]").SourceOperandHome, Is.SameAs(methodTwo.Body));
			Assert.That(visitor.Captures.Find("Local3[Int32]").SourceOperandHome, Is.Not.SameAs(methodTwo.Body));

			Assert.That(methodTwo.Body.ParentBlock, Is.Null);
			Assert.That(visitor.Captures.Find("Local3[Int32]").SourceOperandHome.ParentBlock, Is.SameAs(methodTwo.Body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override bool ShouldSaveAssembly
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void WriteMethods(string implementedMethodName, out MethodMember lambaAnonymousMethod)
		{
			MethodMember implementedMethod;
			WriteMethods(implementedMethodName, out implementedMethod, out lambaAnonymousMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void WriteMethods(string implementedMethodName, out MethodMember implementedMethod, out MethodMember lambaAnonymousMethod)
		{
			implementedMethod = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == implementedMethodName);
			implementedMethod.Write();

			lambaAnonymousMethod = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.IsAnonymous);
			lambaAnonymousMethod.Write();
		}
	}
}
