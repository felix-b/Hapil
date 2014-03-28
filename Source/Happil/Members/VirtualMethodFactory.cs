using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public class VirtualMethodFactory : MethodFactoryBase
	{
		private readonly MethodInfo m_Declaration;
		private readonly MethodBuilder m_MethodBuilder;
		private readonly MethodSignature m_Signature;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public VirtualMethodFactory(ClassType type, MethodInfo declaration)
		{
			m_Declaration = declaration;
			m_MethodBuilder = type.TypeBuilder.DefineMethod(
				type.TakeMemberName(declaration.Name),
				GetMethodAttributesFor(declaration),
				declaration.ReturnType,
				declaration.GetParameters().Select(p => p.ParameterType).ToArray());

			type.TypeBuilder.DefineMethodOverride(m_MethodBuilder, declaration);
			m_Signature = new MethodSignature(declaration);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ILGenerator GetILGenerator()
		{
			return m_MethodBuilder.GetILGenerator();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void EmitCallInstruction(ILGenerator generator, OpCode instruction)
		{
			generator.Emit(instruction, m_MethodBuilder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MethodSignature Signature
		{
			get
			{
				return m_Signature;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MethodInfo Declaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MethodBase Builder
		{
			get
			{
				return m_MethodBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string MemberName
		{
			get
			{
				return m_MethodBuilder.Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodAttributes GetMethodAttributesFor(MethodInfo declaration)
		{
			const MethodAttributes attributes = 
				MethodAttributes.Final |
				MethodAttributes.HideBySig |
				MethodAttributes.Public |
				MethodAttributes.Virtual;

			if ( declaration != null && declaration.DeclaringType != null && declaration.DeclaringType.IsInterface )
			{
				return (attributes | MethodAttributes.NewSlot);
			}
			else
			{
				return attributes;
			}
		}
	}
}
