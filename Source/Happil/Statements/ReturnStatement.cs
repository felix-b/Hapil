using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	/// <summary>
	/// Return statement for void methods
	/// </summary>
	internal class ReturnStatement : IHappilStatement, ILeaveStatement
	{
		private readonly TryStatement m_ExceptionStatement;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ReturnStatement()
		{
			m_ExceptionStatement = StatementScope.Current.InheritedExceptionStatement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Emit(ILGenerator il)
		{
			il.Emit(OpCodes.Ret);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope HomeScope
		{
			get
			{
				return StatementScope.Current.Root;
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Return statement for non-void methods
	/// </summary>
	/// <typeparam name="T">
	/// The type of the return value.
	/// </typeparam>
	internal class ReturnStatement<T> : IHappilStatement, ILeaveStatement
	{
		private readonly IHappilOperand<T> m_Operand;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ReturnStatement(IHappilOperand<T> operand)
		{
			m_Operand = operand;
			StatementScope.Current.Consume(operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Emit(ILGenerator il)
		{
			m_Operand.EmitTarget(il);
			m_Operand.EmitLoad(il);

			il.Emit(OpCodes.Ret);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope HomeScope
		{
			get
			{
				return StatementScope.Current.Root;
			}
		}
	}
}
