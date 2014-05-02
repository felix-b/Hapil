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
	public class OperandVisitorBaseTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanReturnEmptyListIfNoOperandsFound()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method(intf => intf.One).Implement(w => Static.Void(Console.WriteLine))
				.AllMethods().Throw<NotImplementedException>()
				.Flush();
			
			var method = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == "One");
			method.Write();

			//-- Act

			var visitor = new TestVisitor();
			method.AcceptVisitor(visitor);
			var visitedOperands = visitor.GetVisitedOperands();

			//-- Assert

			Assert.That(visitedOperands, Is.Empty);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanVisitLocalsAndArguments()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var s = w.Local<string>();
					s.Assign(n.Func<string>(x => x.ToString));
					w.Return(s);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			var method = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == "Five");
			method.Write();

			//-- Act

			var visitor = new TestVisitor();
			method.AcceptVisitor(visitor);
			
			var visitedOperandStrings = visitor.GetVisitedOperands().Select(op => op.ToString()).ToArray();

			//-- Assert

			CollectionAssert.Contains(visitedOperandStrings, "Local0[String]");
			CollectionAssert.Contains(visitedOperandStrings, "Arg1[n]");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanVisitThisAndFields()
		{
			//-- Arrange

			Field<int> fieldOne;

			DeriveClassFrom<object>()
				.Field("m_One", out fieldOne)
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					fieldOne.Assign(n);
					w.Return(fieldOne.Func<string>(x => x.ToString));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			var method = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == "Five");
			method.Write();

			//-- Act

			var visitor = new TestVisitor();
			method.AcceptVisitor(visitor);

			var visitedOperandStrings = visitor.GetVisitedOperands().Select(op => op.ToString()).ToArray();

			//-- Assert

			CollectionAssert.Contains(visitedOperandStrings, "this");
			CollectionAssert.Contains(visitedOperandStrings, "this.Field[m_One]");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanFilterOperands()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var s = w.Local<string>();
					s.Assign(n.Func<string>(x => x.ToString));
					w.Return(s);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			var method = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == "Five");
			method.Write();

			//-- Act

			var visitor = new TestVisitor(OperandKind.Argument);
			method.AcceptVisitor(visitor);

			var visitedOperandStrings = visitor.GetVisitedOperands().Select(op => op.ToString()).ToArray();

			//-- Assert

			Assert.That(visitedOperandStrings, Is.EqualTo(new[] { "Arg1[n]" }));
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

		private class TestVisitor : OperandVisitorBase
		{
			private readonly HashSet<IOperand> m_VisitedOperands = new HashSet<IOperand>();

			//--------------------------------------------------------------------------------------------------------------------------------------------------

			public TestVisitor(params OperandKind[] filter)
				: base(filter)
			{
			}

			//--------------------------------------------------------------------------------------------------------------------------------------------------

			public IOperand[] GetVisitedOperands()
			{
				return m_VisitedOperands.ToArray();
			}

			//--------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnVisitOperand(ref IOperand operand)
			{
				m_VisitedOperands.Add(operand);
			}
		}
	}
}
