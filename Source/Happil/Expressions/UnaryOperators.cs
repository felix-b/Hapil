﻿using System;
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
				operand.EmitTarget(il);
				operand.EmitLoad(il);

				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ceq);
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
				operand.EmitTarget(il);
				operand.EmitLoad(il);

				il.Emit(OpCodes.Not);
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
				var overloads = TypeOperators.GetOperators(operand.OperandType);

				if ( overloads.OpUnaryPlus != null )
				{
					Helpers.EmitCall(il, null, overloads.OpUnaryPlus, operand);
				}
				else
				{
					operand.EmitTarget(il);
					operand.EmitLoad(il);
					
					// by default this operator does nothing to the operand
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "+";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorNegation<T> : IUnaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> operand)
			{
				var overloads = TypeOperators.GetOperators(operand.OperandType);

				if ( overloads.OpNegation != null )
				{
					Helpers.EmitCall(il, null, overloads.OpNegation, operand);
				}
				else
				{
					operand.EmitTarget(il);
					operand.EmitLoad(il);

					il.Emit(OpCodes.Neg);
				}
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
			private readonly IHappilOperand[] m_Arguments;
			private bool m_ShouldLeaveValueOnStack;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public OperatorCall(MethodBase method, params IHappilOperand[] arguments)
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
				Helpers.EmitCall(il, operand, m_Method, m_Arguments);

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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorNewArray<TElement> : IUnaryOperator<int>
		{
			public void Emit(ILGenerator il, IHappilOperand<int> lengthOperand)
			{
				lengthOperand.EmitTarget(il);
				lengthOperand.EmitLoad(il);

				il.Emit(OpCodes.Newarr, TypeTemplate.Resolve<TElement>());
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return string.Format("new {0}[]", TypeTemplate.Resolve<TElement>().Name);
			}
		}
	}
}
