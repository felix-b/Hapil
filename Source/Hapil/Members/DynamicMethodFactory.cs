using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Members
{
    public class DynamicMethodFactory : MethodFactoryBase
    {
        private readonly Type[] m_ArgumentTypes;
        private readonly Type m_ReturnType;
        private readonly MethodSignature m_Signature;
        private readonly DynamicMethod m_DynamicMethod;
        private readonly ParameterBuilder[] m_Parameters;
        private readonly ParameterBuilder m_ReturnParameter;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public DynamicMethodFactory(DynamicModule module, string name, Type[] argumentTypes, Type returnType)
        {
            var resolvedArgumentTypes = argumentTypes.Select(TypeTemplate.Resolve).ToArray();
            var resolvedReturnType = (returnType != null ? TypeTemplate.Resolve(returnType) : null);

            m_ArgumentTypes = argumentTypes;
            m_ReturnType = returnType;
            m_Signature = new MethodSignature(
                isStatic: true, 
                isPublic: true,
                argumentTypes: argumentTypes, 
                argumentNames: null, 
                returnType: returnType);

            m_DynamicMethod = new DynamicMethod(
                name,
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                returnType ?? typeof(void),
                argumentTypes,
                module.ModuleBuilder,
                skipVisibility: false);

            m_Parameters = resolvedArgumentTypes.Select((argType, argIndex) => m_DynamicMethod.DefineParameter(
                argIndex + 1,
                ParameterAttributes.None,
                "arg" + (argIndex + 1).ToString())).ToArray();

            if ( !m_Signature.IsVoid )
            {
                m_ReturnParameter = m_DynamicMethod.DefineParameter(0, ParameterAttributes.Retval, parameterName: null);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Overrides of MethodFactoryBase

        public override void SetAttribute(CustomAttributeBuilder attribute)
        {
            throw new NotSupportedException();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override ILGenerator GetILGenerator()
        {
            return m_DynamicMethod.GetILGenerator();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void EmitCallInstruction(ILGenerator generator, OpCode instruction)
        {
            generator.Emit(instruction, m_DynamicMethod);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override ConstructorMethodFactory FreezeSignature(MethodSignature finalSignature)
        {
            throw new InvalidOperationException("Current method member already has a final signature.");
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

        public override bool IsFinalSignature
        {
            get
            {
                return true;
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
                return m_DynamicMethod;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override ParameterBuilder[] Parameters
        {
            get { throw new NotImplementedException(); }
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
                return m_DynamicMethod.Name;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override MemberKind MemberKind
        {
            get
            {
                return MemberKind.StaticAnonymousMethod;
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public DynamicMethod DynamicMethod
        {
            get
            {
                return m_DynamicMethod;
            }
        }
    }
}
