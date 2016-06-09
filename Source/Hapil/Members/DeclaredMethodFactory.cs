using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Writers;

namespace Hapil.Members
{
	public class DeclaredMethodFactory : MethodFactoryBase
	{
	    private readonly ClassType m_ClassType;
	    private readonly MethodInfo m_Declaration;
		private readonly MethodBuilder m_MethodBuilder;
		private readonly MethodSignature m_Signature;
		private readonly ParameterBuilder[] m_Parameters;
		private readonly ParameterBuilder m_ReturnParameter;
        private MethodInfo m_InvokableBaseMethod;
	    private bool m_InvokableBaseMethodLookupComplete;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DeclaredMethodFactory(ClassType type, MethodInfo declaration, InterfaceImplementationKind implementationKind)
		{
		    var proposedName = (
                implementationKind == InterfaceImplementationKind.Explicit && declaration.DeclaringType != null
		        ? declaration.DeclaringType + "." + declaration.Name
		        : declaration.Name);

		    m_ClassType = type;
		    m_Declaration = declaration;
			m_MethodBuilder = type.TypeBuilder.DefineMethod(
                type.TakeMemberName(proposedName, mustUseThisName: true),
                GetMethodAttributesFor(declaration, implementationKind),
				declaration.ReturnType,
				declaration.GetParameters().Select(p => p.ParameterType).ToArray());

		    if ( implementationKind == InterfaceImplementationKind.Explicit )
		    {
		        type.TypeBuilder.DefineMethodOverride(m_MethodBuilder, declaration);
		    }

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
        
        public override MethodInfo InvokableBaseMethod
	    {
            get
            {
                if ( !m_InvokableBaseMethodLookupComplete )
                {
                    m_InvokableBaseMethod = TryFindInvokableBaseMethod();
                    m_InvokableBaseMethodLookupComplete = true;
                }

                return m_InvokableBaseMethod;
            }
	    }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    private MethodInfo TryFindInvokableBaseMethod()
	    {
	        var declaration = m_Declaration;

	        if ( declaration != null && declaration.DeclaringType != null )
	        {
	            for ( Type baseType = TypeTemplate.Resolve(m_ClassType.BaseType) ; baseType != null ; baseType = baseType.BaseType )
	            {
	                var foundBaseMethod = TryFindInvokableBaseMethodInType(declaration, baseType);

	                if ( foundBaseMethod != null )
	                {
                        if (!foundBaseMethod.IsAbstract && foundBaseMethod.DeclaringType != null && !foundBaseMethod.DeclaringType.IsInterface)
                        {
                            return foundBaseMethod;
                        }

                        return null;
	                }
	            }
	        }

	        return null;
	    }

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    private static MethodInfo TryFindInvokableBaseMethodInType(MethodInfo declaration, Type baseType)
	    {
	        if ( declaration.DeclaringType == baseType )
	        {
	            return declaration;
	        }

	        var baseMethod = TypeMemberCache.Of(baseType)
	            .ImplementableMethods.Where(m => m.Name == declaration.Name)
	            .OfSignature(declaration.ReturnType, declaration.GetParameters().Select(p => p.ParameterType).ToArray())
	            .FirstOrDefault();

	        if ( baseMethod != null && baseMethod.DeclaringType == baseType )
	        {
	            return baseMethod;
	        }

	        return null;
	    }

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static MethodAttributes GetMethodAttributesFor(MethodInfo declaration, InterfaceImplementationKind implementationKind)
		{
			var attributes = MethodAttributes.Virtual | MethodAttributes.HideBySig;

            switch ( implementationKind )
            {
                case InterfaceImplementationKind.Explicit:
                    attributes |= (MethodAttributes.Private);
                    break;
                case InterfaceImplementationKind.ImplicitVirtual:
                    attributes |= (MethodAttributes.Public);
                    break;
                case InterfaceImplementationKind.ImplicitNonVirtual:
                    attributes |= (MethodAttributes.Public);
                    break;
            }

            if ( declaration != null && declaration.DeclaringType != null && declaration.DeclaringType.IsInterface )
            {
				attributes |= MethodAttributes.NewSlot;
            }

            return attributes;
		}
	}
}
