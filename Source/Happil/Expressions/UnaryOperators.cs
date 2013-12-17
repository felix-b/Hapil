using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal static class UnaryOperators
	{
		static UnaryOperators()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLogicalNot : IUnaryOperator<bool>
		{
			public void Emit(ILGenerator il, IHappilOperand<bool> operand)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "!";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseNot<T> : IUnaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> operand)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "~";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorPlus<T> : IUnaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> operand)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "+";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorMinus<T> : IUnaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> operand)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "-";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorPlusPlus<T> : IUnaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> operand)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "++";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorMinusMinus<T> : IUnaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> operand)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "--";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorCall<T> : IUnaryOperator<T>, IDontLeaveValueOnStack
		{
			private readonly MethodBase m_Method;
			private readonly IHappilOperandInternals[] m_Arguments;
			private bool m_ShouldLeaveValueOnStack;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public OperatorCall(MethodBase method, params IHappilOperandInternals[] arguments)
			{
				m_Method = method;
				m_Arguments = arguments;
				m_ShouldLeaveValueOnStack = false;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
 
			#region IDontLeaveValueOnStack Members

			public void ForceLeaveFalueOnStack()
			{
				if ( IsVoidCall() )
				{
					throw new InvalidOperationException("This call does not return a value.");
				}

				m_ShouldLeaveValueOnStack = true;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Emit(ILGenerator il, IHappilOperand<T> operand)
			{
				((IHappilOperandInternals)operand).EmitTarget(il);
				((IHappilOperandInternals)operand).EmitLoad(il);

				foreach ( var argument in m_Arguments )
				{
					argument.EmitTarget(il);
					argument.EmitLoad(il);
				}

				var methodInfo = (m_Method as MethodInfo);
				var constructorInfo = (m_Method as ConstructorInfo);
				var callOpcode = (m_Method.IsVirtual || m_Method.DeclaringType.IsInterface ? OpCodes.Callvirt : OpCodes.Call);

				if ( methodInfo != null )
				{
					il.Emit(callOpcode, methodInfo);
				}
				else
				{
					il.Emit(callOpcode, constructorInfo);
				}

				if ( !IsVoidCall() && !m_ShouldLeaveValueOnStack )
				{
					il.Emit(OpCodes.Pop);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "->";
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
			
			private bool IsVoidCall()
			{
				if ( m_Method is ConstructorInfo )
				{
					return true;
				}

				var returnType = ((MethodInfo)m_Method).ReturnType;
				return (returnType == null || returnType == typeof(void));
			}
		}
	}
}
