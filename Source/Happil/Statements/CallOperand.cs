//using System;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using Happil.Fluent;

//namespace Happil.Statements
//{
//	internal class CallOperand<TReturn> : HappilOperand<TReturn>
//	{
//		private readonly IHappilOperandInternals m_Target;
//		private readonly MethodBase m_Method;
//		private readonly IHappilOperandInternals[] m_Arguments;

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		public CallOperand(HappilMethod ownerMethod, IHappilOperandInternals target, MethodBase method, params IHappilOperandInternals[] arguments)
//			: base(ownerMethod)
//		{
//			m_Target = target;
//			m_Method = method;
//			m_Arguments = arguments;
//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
//		protected override void OnEmitTarget(ILGenerator il)
//		{
//			if ( m_Target != null )
//			{
//				m_Target.EmitTarget(il);
//				m_Target.EmitLoad(il);
//			}
//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		protected override void OnEmitLoad(ILGenerator il)
//		{
//			Helpers.EmitCall(il, null, m_Method, m_Arguments);
//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		protected override void OnEmitStore(ILGenerator il)
//		{
//			throw new NotSupportedException();
//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		protected override void OnEmitAddress(ILGenerator il)
//		{
//			throw new NotSupportedException();
//		}
//	}
//}
