using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Statements;

namespace Happil.Operands
{
	public class Local<T> : MutableOperand<T>, ICanEmitAddress, IScopedOperand
	{
		private readonly StatementBlock m_HomeStatementBlock;
		private readonly LocalBuilder m_LocalBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Local(MethodMember ownerMethod)
		{
			m_HomeStatementBlock = (StatementScope.Exists ? StatementScope.Current.StatementBlock : ownerMethod.Body);
			m_LocalBuilder = ownerMethod.MethodFactory.GetILGenerator().DeclareLocal(TypeTemplate.Resolve<T>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IScopedOperand Members

		StatementBlock IScopedOperand.HomeStatementBlock
		{
			get
			{
				return m_HomeStatementBlock;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Local{0}[{1}]", m_LocalBuilder.LocalIndex, m_LocalBuilder.LocalType.Name);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.Local;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldloc, m_LocalBuilder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(OpCodes.Stloc, m_LocalBuilder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(OpCodes.Ldloca, (short)m_LocalBuilder.LocalIndex);
		}
	}
}
