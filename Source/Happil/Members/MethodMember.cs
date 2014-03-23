using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Writers;

namespace Happil.Members
{
	public class MethodMember : MemberBase
	{
		private readonly MethodInfo m_MethodDeclaration = null;
		//private readonly MethodBuilder m_MethodBuilder = null;
		//private readonly List<MethodWriterBase> m_Writers = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal MethodMember(ClassType ownerClass, string name, MethodInfo methodDeclaration)
			: base(ownerClass, name)
		{
			//m_MethodDeclaration = methodDeclaration;
			//m_MethodBuilder = ownerClass.TypeBuilder.DefineMethod(
			//	ownerClass.TakeMemberName(methodDeclaration.Name),
			//	GetMethodAttributesFor(methodDeclaration),
			//	declaration.ReturnType,
			//	declaration.GetParameters().Select(p => p.ParameterType).ToArray());

			////if ( !declaration.IsSpecialName )
			////{
			//happilClass.TypeBuilder.DefineMethodOverride(m_MethodBuilder, declaration);
			////}

		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberInfo MemberDeclaration
		{
			get
			{
				return m_MethodDeclaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo MethodDeclaration
		{
			get
			{
				return m_MethodDeclaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Write()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Compile()
		{
			throw new NotImplementedException();
		}
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodAttributes GetMethodAttributesFor(MethodInfo declaration)
		{
			var attributes =
				MethodAttributes.Final |
				MethodAttributes.HideBySig |
				MethodAttributes.Public |
				MethodAttributes.Virtual;

			if ( declaration != null && declaration.DeclaringType != null && declaration.DeclaringType.IsInterface )
			{
				attributes |= MethodAttributes.NewSlot;
			}

			return attributes;
		}
	}
}
