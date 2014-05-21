using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Happil.Members;

namespace Happil.UnitTests
{
	public abstract class ClosureTestFixtureBase : NUnitEmittedTypesTestBase
	{
		protected MethodMember WriteMethod(string methodName)
		{
			var method = base.Class.GetAllMembers().OfType<MethodMember>().Single(m => m.Name == methodName);
			method.Write();
			return method;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodMember[] FindAnonymousMethods()
		{
			var foundMethods = new List<MethodMember>();
			FindAnonymousMethods(base.Class, foundMethods);
			return foundMethods.ToArray();
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

		private void FindAnonymousMethods(ClassType classType, List<MethodMember> foundMethods)
		{
			classType.ForEachMember<MethodMember>(foundMethods.Add, predicate: m => m.IsAnonymous);

			foreach ( var nestedClass in classType.GetNestedClasses() )
			{
				FindAnonymousMethods(nestedClass, foundMethods);
			}
		}
	}
}
