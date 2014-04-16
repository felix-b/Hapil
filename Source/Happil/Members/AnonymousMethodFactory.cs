using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public class AnonymousMethodFactory : MethodFactoryBase
	{
		private readonly MethodBuilder m_MethodBuilder;
		private readonly MethodSignature m_Signature;
		private readonly ParameterBuilder[] m_Parameters;
		private readonly ParameterBuilder m_ReturnParameter;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private AnonymousMethodFactory(ClassType type, Type[] argumentTypes, Type returnType, bool isStatic, bool isPublic)
		{
			var resolvedArgumentTypes = argumentTypes.Select(TypeTemplate.Resolve).ToArray();
			var resolvedReturnType = (returnType != null ? TypeTemplate.Resolve(returnType) : null);
			var methodAttributes = (MethodAttributes.Final | MethodAttributes.HideBySig | GetMethodModifierAttributes(isStatic, isPublic));

			m_MethodBuilder = type.TypeBuilder.DefineMethod(
				type.TakeMemberName("AnonymousMethod"),
				methodAttributes,
				resolvedReturnType,
				resolvedArgumentTypes);

			m_Signature = new MethodSignature(isStatic, resolvedArgumentTypes, returnType: resolvedReturnType);

			m_Parameters = resolvedArgumentTypes.Select((argType, argIndex) => m_MethodBuilder.DefineParameter(
				argIndex + 1,
				ParameterAttributes.None, 
				"arg" + (argIndex + 1).ToString())).ToArray();

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
				return null;
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
				return (m_MethodBuilder.IsStatic ? MemberKind.StaticAnonymousMethod : MemberKind.InstanceAnonymousMethod);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static AnonymousMethodFactory InstanceMethod(ClassType type, Type[] argumentTypes, Type returnType)
		{
			return new AnonymousMethodFactory(type, argumentTypes, returnType, isStatic: false, isPublic: false);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static AnonymousMethodFactory StaticMethod(ClassType type, Type[] argumentTypes, Type returnType)
		{
			return new AnonymousMethodFactory(type, argumentTypes, returnType, isStatic: true, isPublic: false);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static AnonymousMethodFactory FromMethodInfo(ClassType type, MethodInfo methodInfo)
		{
			return new AnonymousMethodFactory(
				type, 
				argumentTypes: methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(),
				returnType: methodInfo.ReturnType,
				isStatic: methodInfo.IsStatic,
				isPublic: methodInfo.IsPublic);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		private static MethodAttributes GetMethodModifierAttributes(bool isStatic, bool isPublic)
		{
			return (
				(isStatic ? MethodAttributes.Static : (MethodAttributes)0) |
				(isPublic ? MethodAttributes.Public : MethodAttributes.Private));
		}
	}
}
