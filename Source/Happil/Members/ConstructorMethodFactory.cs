using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public class ConstructorMethodFactory : MethodFactoryBase
	{
		private readonly ConstructorBuilder m_ConstructorBuilder;
		private readonly MethodSignature m_Signature;
		private readonly ParameterBuilder[] m_Parameters;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ConstructorMethodFactory(ClassType ownerClass, ConstructorBuilder constructorBuilder, MethodSignature signature)
		{
			m_ConstructorBuilder = constructorBuilder;
			m_Signature = signature;

			if ( !signature.IsStatic )
			{
				ownerClass.DefineFactoryMethod(constructorBuilder, signature.ArgumentType);
			}

			m_Parameters = signature.ArgumentName.Select((argName, argIndex) => m_ConstructorBuilder.DefineParameter(
				argIndex + 1,
				ParameterAttributes.None,
				argName)).ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void SetAttribute(CustomAttributeBuilder attribute)
		{
			m_ConstructorBuilder.SetCustomAttribute(attribute);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ILGenerator GetILGenerator()
		{
			return m_ConstructorBuilder.GetILGenerator();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void EmitCallInstruction(ILGenerator generator, OpCode instruction)
		{
			generator.Emit(instruction, m_ConstructorBuilder);
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
				return m_ConstructorBuilder;
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
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string MemberName
		{
			get
			{
				return m_ConstructorBuilder.Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberKind MemberKind
		{
			get
			{
				return (m_ConstructorBuilder.IsStatic ? MemberKind.StaticConstructor : MemberKind.InstanceConstructor);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static ConstructorMethodFactory DefaultConstructor(ClassType type)
		{
			return InstanceConstructor(type, argumentTypes: Type.EmptyTypes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static ConstructorMethodFactory InstanceConstructor(ClassType type, Type[] argumentTypes)
		{
			var resolvedArgumentTypes = argumentTypes.Select(TypeTemplate.Resolve).ToArray();
			var builder = type.TypeBuilder.DefineConstructor(
				MethodAttributes.Public | 
				MethodAttributes.SpecialName | 
				MethodAttributes.RTSpecialName,
				CallingConventions.HasThis,
				resolvedArgumentTypes);
			var signature = new MethodSignature(isStatic: false, argumentTypes: resolvedArgumentTypes);

			return new ConstructorMethodFactory(type, builder, signature);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static ConstructorMethodFactory StaticConstructor(ClassType type)
		{
			var builder = type.TypeBuilder.DefineConstructor(
				MethodAttributes.Private | 
				MethodAttributes.SpecialName | 
				MethodAttributes.RTSpecialName | 
				MethodAttributes.HideBySig | 
				MethodAttributes.Static,
				CallingConventions.Standard,
				Type.EmptyTypes);
			var signature = new MethodSignature(isStatic: true);

			return new ConstructorMethodFactory(type, builder, signature);
		}
	}
}
