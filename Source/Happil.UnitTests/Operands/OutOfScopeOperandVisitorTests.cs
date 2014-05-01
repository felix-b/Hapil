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
	public class OutOfScopeOperandVisitorTests : ClassPerTestCaseFixtureBase
	{
		[Test, Ignore("Not yet implemented")]
		public void CanDetectAllOperandsAreInScope()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method(intf => intf.One).Implement(w => {
					var input = w.Local(w.NewArray<int>(1, 2, 3));
					var output = w.Local(input.Where(w.Lambda<int, bool>(x => (x % 2) == 0)));
				})
				.AllMethods().Throw<NotImplementedException>();

			//-- Act

			classWriter.Flush();
			Class.ForEachMember<MethodMember>(m => {
				if ( m.Name == "One" )
				{
					m.Write();
				}
			});

			var anonymousMethod = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.IsAnonymous);
			anonymousMethod.Write();

			var visitor = new OutOfScopeOperandVisitor();
			anonymousMethod.AcceptVisitor(visitor);
			var foundOperands = visitor.GetOutOfScopeOperands();

			//-- Assert

			Assert.That(foundOperands, Is.Empty);
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
