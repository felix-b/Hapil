using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal static class BinaryOperators
	{
		static BinaryOperators()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorAdd<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
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

		public class OperatorSubtract<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
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

		public class OperatorMultiply<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "*";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorDivide<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "/";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorModulo<T> : IBinaryOperator<T> 
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "%";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLogicalAnd : IBinaryOperator<bool>
		{
			public void Emit(ILGenerator il, IHappilOperand<bool> left, IHappilOperand<bool> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "&&";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLogicalOr : IBinaryOperator<bool>
		{
			public void Emit(ILGenerator il, IHappilOperand<bool> left, IHappilOperand<bool> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "||";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLogicalXor : IBinaryOperator<bool>
		{
			public void Emit(ILGenerator il, IHappilOperand<bool> left, IHappilOperand<bool> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "^^";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseAnd<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "&";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseOr<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "|";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorBitwiseXor<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "^";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLeftShift<T> : IBinaryOperator<T, int>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<int> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "<<";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorRightShift<T> : IBinaryOperator<T, int>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<int> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return ">>";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "==";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorNotEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "!=";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorGreaterThan<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return ">";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLessThan<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "<";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorGreaterThanOrEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return ">=";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorLessThanOrEqual<T> : IBinaryOperator<T>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<T> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "<=";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorCastOrThrow<T> : IBinaryOperator<T, Type>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<Type> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "cast-to";
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorTryCast<T> : IBinaryOperator<T, Type>
		{
			public void Emit(ILGenerator il, IHappilOperand<T> left, IHappilOperand<Type> right)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override string ToString()
			{
				return "as";
			}
		}
	}
}
