using System;
using System.Collections.Generic;
using System.Linq;
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
	}
}
