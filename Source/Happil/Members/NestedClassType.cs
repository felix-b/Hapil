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
		private readonly ClassType m_ContainingType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal NestedClassType(ClassType containingType, string classFullName, Type baseType)
			: base(containingType.Module, null, classFullName, baseType)
		{
			m_ContainingType = containingType;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override TypeBuilder CreateTypeBuilder(DynamicModule module, string classFullName, Type baseType)
		{
			return m_ContainingType.TypeBuilder.DefineNestedType(
				TakeMemberName(classFullName, mustUseThisName: false), 
				TypeAttributes.NestedPrivate, 
				baseType);
		}
	}
}
