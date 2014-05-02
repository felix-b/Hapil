using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Operands;
using NUnit.Framework;

namespace Happil.UnitTests.Operands
{
	[TestFixture]
	public class ExternalOperandVisitorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanDetectAllOperandsAreInScope()
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

			var method = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == "One");
			method.Write();

			var anonymousMethod = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.IsAnonymous);
			anonymousMethod.Write();

			var visitor = new ExternalOperandVisitor(anonymousMethod);
			anonymousMethod.AcceptVisitor(visitor);
			var foundOperands = visitor.GetExternalOperands();

			//-- Assert

			Assert.That(foundOperands, Is.Empty);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanDetectExternalArgumentsAndLocals()
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

			var method = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == "Two");
			method.Write();

			var anonymousMethod = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.IsAnonymous);
			anonymousMethod.Write();

			var visitor = new ExternalOperandVisitor(anonymousMethod);
			anonymousMethod.AcceptVisitor(visitor);
			
			var foundOperandStrings = visitor.GetExternalOperands().Select(op => op.ToString()).ToArray();

			//-- Assert

			CollectionAssert.AreEquivalent(
				new[] { "Local0[Int32]", "Arg1[n]" },
				foundOperandStrings);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override bool ShouldSaveAssembly
		{
			get
			{
				return false;
			}
		}
	}
}
