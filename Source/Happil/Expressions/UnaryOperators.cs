using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

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
			public void Emit(ILGenerator il, IOperand<bool> operand)
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
			public void Emit(ILGenerator il, IOperand<T> operand)
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
			public void Emit(ILGenerator il, IOperand<T> operand)
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
			public void Emit(ILGenerator il, IOperand<T> operand)
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

		public class OperatorIncrement<T> : IUnaryOperator<T>, IDontLeaveValueOnStack
		{
			private readonly UnaryOperatorPosition m_Position;
			private readonly bool m_Positive;
			private readonly MethodMember m_OwnerMethod;
			private bool m_ShouldLeaveValueOnStack;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public OperatorIncrement(UnaryOperatorPosition position, bool positive)
			{
				m_Position = position;
				m_Positive = positive;
				m_OwnerMethod = StatementScope.Current.OwnerMethod;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IDontLeaveValueOnStack Members

			public void ForceLeaveFalueOnStack()
			{
				m_ShouldLeaveValueOnStack = true;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Emit(ILGenerator il, IOperand<T> operand)
			{
				if ( m_Position == UnaryOperatorPosition.Prefix || !m_ShouldLeaveValueOnStack )
				{
					EmitPrefixVersion(il, operand);
				}
				else
				{
					EmitPostfixVersion(il, operand);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return (m_Positive ? "++" : "--");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void EmitPostfixVersion(ILGenerator il, IOperand<T> operand)
			{
				operand.EmitTarget(il);

				EmitIncrement(il, operand);

				if ( operand.HasTarget )
				{
					var temp = m_OwnerMethod.AddLocal<T>();

					temp.EmitStore(il);
					operand.EmitTarget(il);
					temp.EmitLoad(il);
				}

				operand.EmitStore(il);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void EmitPrefixVersion(ILGenerator il, IOperand<T> operand)
			{
				if ( operand.HasTarget )
				{
					operand.EmitTarget(il);
					il.Emit(OpCodes.Dup);
				}

				EmitIncrement(il, operand);

				if ( operand.HasTarget && m_ShouldLeaveValueOnStack )
				{
					var temp = m_OwnerMethod.AddLocal<T>();

					temp.EmitStore(il);
					operand.EmitStore(il);
					temp.EmitLoad(il);
				}
				else
				{
					operand.EmitStore(il);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void EmitIncrement(ILGenerator il, IOperand<T> operand)
			{
				var overloads = TypeOperators.GetOperators(operand.OperandType);

				operand.EmitLoad(il);

				if ( m_Position == UnaryOperatorPosition.Postfix && m_ShouldLeaveValueOnStack )
				{
					il.Emit(OpCodes.Dup);
				}

				var overloadedOperator = (m_Positive ? overloads.OpIncrement : overloads.OpDecrement);

				if ( overloadedOperator != null )
				{
					il.Emit(OpCodes.Call, overloadedOperator);
				}
				else
				{
					Helpers.EmitConvertible(il, 1);
					Helpers.EmitCast(il, typeof(int), operand.OperandType);

					il.Emit(m_Positive ? OpCodes.Add : OpCodes.Sub);
				}

				if ( m_Position == UnaryOperatorPosition.Prefix && m_ShouldLeaveValueOnStack )
				{
					il.Emit(OpCodes.Dup);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorCall<T> : IUnaryOperator<T>, IDontLeaveValueOnStack, IAcceptOperandVisitor
		{
			private readonly MethodBase m_Method;
			private readonly IOperand[] m_Arguments;
			private bool m_ShouldLeaveValueOnStack;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public OperatorCall(MethodBase method, params IOperand[] arguments)
			{
				m_Method = method;
				m_Arguments = arguments;
				m_ShouldLeaveValueOnStack = false;

				foreach ( var arg in m_Arguments )
				{
					StatementScope.Current.Consume(arg);
				}
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

			#region IAcceptOperandVisitor Members

			void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
			{
				visitor.VisitOperandArray(m_Arguments);
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Emit(ILGenerator il, IOperand<T> operand)
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
				var argumentString = string.Join(",", m_Arguments.Select(a => a.ToString()));
				return string.Format("->{0}({1})", m_Method.Name, argumentString);
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
			public void Emit(ILGenerator il, IOperand<int> lengthOperand)
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
