using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Members
{
    public class NewMethodFactory : MethodFactoryBase
    {
		private const string AnonymousNameSuffix = "<AnonymousMethod>";

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private readonly MethodBuilder m_MethodBuilder;
		private readonly MethodSignature m_Signature;
		private readonly ParameterBuilder[] m_Parameters;
		private readonly ParameterBuilder m_ReturnParameter;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public NewMethodFactory(ClassType type, string methodName, MethodSignature signature)
        {
            m_Signature = signature;
        
            m_MethodBuilder = type.TypeBuilder.DefineMethod(
                type.TakeMemberName(methodName, mustUseThisName: true),
                GetMethodAttributesFor(signature),
                signature.ReturnType,
                signature.ArgumentType);

            m_Parameters = m_Signature.ArgumentType.Select((argType, index) => m_MethodBuilder.DefineParameter(
                index + 1, 
                ParameterAttributes.None, 
                m_Signature.ArgumentName[index])).ToArray();

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
				return MemberKind.VirtualMethod;
			}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static MethodAttributes GetMethodAttributesFor(MethodSignature signature)
        {
            return (
                MethodAttributes.HideBySig |
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.NewSlot);
        }
    }
}
