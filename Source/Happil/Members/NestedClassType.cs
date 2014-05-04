using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Members
{
	public class NestedClassType : ClassType
	{
		private readonly ClassType m_ContainingClass;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal NestedClassType(ClassType containingClass, string classFullName, Type baseType)
			: base(containingClass.Module, null, classFullName, baseType, containingClass)
		{
			m_ContainingClass = containingClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override TypeBuilder CreateTypeBuilder(DynamicModule module, string classFullName, Type baseType, ClassType containingClass)
		{
			return containingClass.TypeBuilder.DefineNestedType(
				containingClass.TakeMemberName(classFullName, mustUseThisName: false), 
				TypeAttributes.NestedPrivate, 
				baseType);
		}
	}
}
