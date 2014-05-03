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
	public class ClosureDefinitionTests : ClosureTestFixtureBase
	{
		[Test]
		public void CanCloseOverExternalOperandAndLocal()
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

			//-- Assert

			
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
