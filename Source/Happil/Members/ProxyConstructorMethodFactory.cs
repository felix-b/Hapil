using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Members
{
	internal class ProxyConstructorMethodFactory : ConstructorMethodFactory
	{
		private readonly Func<MethodSignature, RealConstructorMethodFactory> m_RealFactoryFactory;
		private readonly MethodSignature m_Signature;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ProxyConstructorMethodFactory(MethodSignature signature, Func<MethodSignature, RealConstructorMethodFactory> realFactoryFactory)
		{
			m_Signature = signature;
			m_RealFactoryFactory = realFactoryFactory;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void SetAttribute(CustomAttributeBuilder attribute)
		{
			throw NotARealFactoryException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ILGenerator GetILGenerator()
		{
			throw NotARealFactoryException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void EmitCallInstruction(ILGenerator generator, OpCode instruction)
		{
			throw NotARealFactoryException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ConstructorMethodFactory FreezeSignature(MethodSignature finalSignature)
		{
			return m_RealFactoryFactory(finalSignature);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override bool IsFinalSignature
		{
			get
			{
				return false;
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

		public override System.Reflection.MethodInfo Declaration
		{
			get
			{
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override System.Reflection.MethodBase Builder
		{
			get
			{
				throw NotARealFactoryException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ParameterBuilder[] Parameters
		{
			get
			{
				throw NotARealFactoryException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override ParameterBuilder ReturnParameter
		{
			get
			{
				throw NotARealFactoryException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string MemberName
		{
			get
			{
				return (m_Signature.IsStatic ? ConstructorInfo.TypeConstructorName : ConstructorInfo.ConstructorName);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberKind MemberKind
		{
			get
			{
				return (m_Signature.IsStatic ? MemberKind.StaticConstructor : MemberKind.InstanceConstructor);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private Exception NotARealFactoryException()
		{
			return new InvalidOperationException("Requested operation cannot be performed on a constructor member which has no final signature yet.");
		}
	}
}
