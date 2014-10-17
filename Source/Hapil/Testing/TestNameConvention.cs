using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Testing
{
	public class TestNameConvention : ImplementationConvention
	{
		private readonly EmittedTypesTestBase m_TestClass;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TestNameConvention(EmittedTypesTestBase testClass)
			: base(Will.InspectDeclaration)
		{
			m_TestClass = testClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of ImplementationConvention

		protected override void OnInspectDeclaration(ObjectFactoryContext context)
		{
			context.ClassFullName = m_TestClass.TestCaseClassName;
		}

		#endregion
	}
}
