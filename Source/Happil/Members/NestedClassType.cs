using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Happil.Closures;
using Happil.Operands;

namespace Happil.Members
{
	public class NestedClassType : ClassType
	{
		private readonly ClassType m_ContainingClass;
		private readonly ClosureDefinition m_ClosureDefinition;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal NestedClassType(ClassType containingClass, string classFullName, Type baseType, ClosureDefinition closureDefinition = null)
			: base(containingClass.Module, null, classFullName, baseType, containingClass)
		{
			m_ContainingClass = containingClass;
			m_ClosureDefinition = closureDefinition;
			m_ContainingClass.AddNestedClass(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override TypeBuilder CreateTypeBuilder(DynamicModule module, string classFullName, Type baseType, ClassType containingClass)
		{
			return containingClass.TypeBuilder.DefineNestedType(
				containingClass.TakeMemberName(classFullName, mustUseThisName: false), 
				TypeAttributes.NestedPrivate, 
				baseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override bool IsClosure
		{
			get
			{
				return (m_ClosureDefinition != null);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override ClosureDefinition ClosureDefinition
		{
			get
			{
				return m_ClosureDefinition;
			}
		}
	}
}
