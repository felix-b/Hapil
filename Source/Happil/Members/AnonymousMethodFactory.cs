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
		private readonly MethodMember m_HostMethod;
		private readonly string m_MethodName;
		private MethodSignature m_Signature;
		private MethodAttributes m_MethodAttributes;
		private MethodBuilder m_MethodBuilder;
		private ParameterBuilder[] m_Parameters;
		private ParameterBuilder m_ReturnParameter;
		private ClassType m_ClassType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private AnonymousMethodFactory(MethodMember hostMethod, Type[] argumentTypes, Type returnType, bool isStatic, bool isPublic)
		{
			m_HostMethod = hostMethod;
			m_ClassType = hostMethod.OwnerClass;
			m_MethodAttributes = (MethodAttributes.Final | MethodAttributes.HideBySig | GetMethodModifierAttributes(isStatic, isPublic));
			m_MethodName = m_ClassType.TakeMemberName("AnonymousMethod");

			var resolvedArgumentTypes = argumentTypes.Select(TypeTemplate.Resolve).ToArray();
			var resolvedReturnType = (returnType != null ? TypeTemplate.Resolve(returnType) : null);
			
			m_Signature = new MethodSignature(isStatic, isPublic, resolvedArgumentTypes, returnType: resolvedReturnType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void SetAttribute(CustomAttributeBuilder attribute)
		{
			EnsureMethodBuilderCreated();
			m_MethodBuilder.SetCustomAttribute(attribute);		
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ILGenerator GetILGenerator()
		{
			EnsureMethodBuilderCreated();
			return m_MethodBuilder.GetILGenerator();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void EmitCallInstruction(ILGenerator generator, OpCode instruction)
		{
			EnsureMethodBuilderCreated();
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
				EnsureMethodBuilderCreated();
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
				EnsureMethodBuilderCreated();
				return m_ReturnParameter;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string MemberName
		{
			get
			{
				return m_MethodName;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberKind MemberKind
		{
			get
			{
				return (m_MethodAttributes.HasFlag(MethodAttributes.Static) ? MemberKind.StaticAnonymousMethod : MemberKind.InstanceAnonymousMethod);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember HostMethod
		{
			get
			{
				return m_HostMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void MethodMovedToClosure(ClassType closureClass)
		{
			if ( m_MethodBuilder != null )
			{
				throw new InvalidOperationException("Cannot change class type because MethodBuilder is already created.");
			}

			m_ClassType = closureClass;
			m_MethodAttributes = (m_MethodAttributes & ~(MethodAttributes.Private)) | MethodAttributes.Public;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void ChangeMethodAttributes(bool? isPublic = null, bool? isStatic = null)
		{
			if ( m_MethodBuilder != null )
			{
				throw new InvalidOperationException("Cannot change method attributes because MethodBuilder is already created.");
			}

			var clearFlags = (MethodAttributes)0;
			var setFlags = (MethodAttributes)0;

			if ( isPublic.HasValue )
			{
				clearFlags |= (MethodAttributes.Private | MethodAttributes.Public);
				setFlags |= isPublic.Value ? MethodAttributes.Public : MethodAttributes.Private;
			}

			if ( isStatic.HasValue )
			{
				clearFlags |= MethodAttributes.Static;

				if ( isStatic.Value )
				{
					setFlags |= MethodAttributes.Static;
				}
			}

			m_MethodAttributes = (m_MethodAttributes & ~clearFlags) | setFlags;
			m_Signature = m_Signature.ChangeModifiers(isPublic, isStatic);
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static AnonymousMethodFactory InstanceMethod(ClassType type, Type[] argumentTypes, Type returnType)
		//{
		//	return new AnonymousMethodFactory(type, argumentTypes, returnType, isStatic: false, isPublic: false);
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static AnonymousMethodFactory StaticMethod(MethodMember hostMethod, Type[] argumentTypes, Type returnType)
		{
			return new AnonymousMethodFactory(hostMethod, argumentTypes, returnType, isStatic: true, isPublic: false);
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static AnonymousMethodFactory FromMethodInfo(ClassType type, MethodInfo methodInfo)
		//{
		//	return new AnonymousMethodFactory(
		//		type, 
		//		argumentTypes: methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(),
		//		returnType: methodInfo.ReturnType,
		//		isStatic: methodInfo.IsStatic,
		//		isPublic: methodInfo.IsPublic);
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void EnsureMethodBuilderCreated()
		{
			if ( m_MethodBuilder == null )
			{
				CreateMethodBuilder();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void CreateMethodBuilder()
		{
			m_MethodBuilder = m_ClassType.TypeBuilder.DefineMethod(
				m_MethodName,
				m_MethodAttributes,
				m_Signature.ReturnType,
				m_Signature.ArgumentType);

			m_Parameters = m_Signature.ArgumentType.Select((argType, argIndex) =>
				m_MethodBuilder.DefineParameter(argIndex + 1, ParameterAttributes.None, "arg" + (argIndex + 1).ToString())
			).ToArray();

			if ( !m_Signature.IsVoid )
			{
				m_ReturnParameter = m_MethodBuilder.DefineParameter(0, ParameterAttributes.Retval, strParamName: null);
			}
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
