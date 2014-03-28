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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private AnonymousMethodFactory(ClassType type, Type[] argumentTypes, Type returnType, bool isStatic)
		{
			var methodAttributes = (
				isStatic ? 
				MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.Static :
				MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.Private);

			m_MethodBuilder = type.TypeBuilder.DefineMethod(
				type.TakeMemberName("AnonymousMethod"),
				methodAttributes,
				returnType,
				argumentTypes);
			
			m_Signature = new MethodSignature(isStatic, argumentTypes, returnType);
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

		public override string MemberName
		{
			get
			{
				return m_MethodBuilder.Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static AnonymousMethodFactory InstanceMethod(ClassType type, Type[] argumentTypes, Type returnType)
		{
			return new AnonymousMethodFactory(type, argumentTypes, returnType, isStatic: false);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static AnonymousMethodFactory StaticMethod(ClassType type, Type[] argumentTypes, Type returnType)
		{
			return new AnonymousMethodFactory(type, argumentTypes, returnType, isStatic: true);
		}
	}
}
