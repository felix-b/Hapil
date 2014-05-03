using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.UnitTests.Operands
{
	public abstract class ClosureTestFixtureBase : ClassPerTestCaseFixtureBase
	{
		protected void WriteMethods(string implementedMethodName, out MethodMember lambaAnonymousMethod)
		{
			MethodMember implementedMethod;
			WriteMethods(implementedMethodName, out implementedMethod, out lambaAnonymousMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void WriteMethods(string implementedMethodName, out MethodMember implementedMethod, out MethodMember lambaAnonymousMethod)
		{
			implementedMethod = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == implementedMethodName);
			implementedMethod.Write();

			lambaAnonymousMethod = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.IsAnonymous);
			lambaAnonymousMethod.Write();
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
