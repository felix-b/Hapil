using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Hapil.Members
{
	public class VirtualMethodFactory : MethodFactoryBase
	{
		private readonly MethodInfo m_Declaration;
		private readonly MethodBuilder m_MethodBuilder;
		private readonly MethodSignature m_Signature;
		private readonly ParameterBuilder[] m_Parameters;
		private readonly ParameterBuilder m_ReturnParameter;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public VirtualMethodFactory(ClassType type, MethodInfo declaration)
		{
			m_Declaration = declaration;
			m_MethodBuilder = type.TypeBuilder.DefineMethod(
				type.TakeMemberName(declaration.Name, mustUseThisName: true),
				GetMethodAttributesFor(declaration),
				declaration.ReturnType,
				declaration.GetParameters().Select(p => p.ParameterType).ToArray());

			type.TypeBuilder.DefineMethodOverride(m_MethodBuilder, declaration);
			m_Signature = new MethodSignature(declaration);

			m_Parameters = m_Declaration.GetParameters().Select(p => m_MethodBuilder.DefineParameter(p.Position + 1, p.Attributes, p.Name)).ToArray();

			if ( !m_Signature.IsVoid )
			{
				m_ReturnParameter = m_MethodBuilder.DefineParameter(0, ParameterAttributes.Retval, strParamName: null);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void SetAttribute(CustomAttributeBuilder attribute)
		{
			m_MethodBuilder.SetCustomAttribute(attribute);
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

		public override ConstructorMethodFactory FreezeSignature(MethodSignature finalSignature)
		{
			throw new InvalidOperationException("Current method member already has a final signature.");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool IsFinalSignature
		{
			get
			{
				return true;
			}
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

		public override ParameterBuilder[] Parameters
		{
			get
			{
				return m_Parameters;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ParameterBuilder ReturnParameter
		{
			get
			{
				return m_ReturnParameter;
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

		public override MemberKind MemberKind
		{
			get
			{
				return MemberKind.VirtualMethod;
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
