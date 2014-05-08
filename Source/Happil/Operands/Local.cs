using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Statements;

namespace Happil.Operands
{
	public class Local<T> : MutableOperand<T>, ILocal, ICanEmitAddress, IScopedOperand, ITransformType
	{
		private readonly StatementBlock m_HomeStatementBlock;
		private readonly int m_LocalIndex;
		private LocalBuilder m_LocalBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Local(MethodMember ownerMethod)
		{
			m_HomeStatementBlock = (StatementScope.Exists ? StatementScope.Current.StatementBlock : ownerMethod.Body);
			ownerMethod.RegisterLocal(this, out m_LocalIndex);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private Local(StatementBlock homeStatementBlock, int localIndex, LocalBuilder localBuilder)
		{
			m_HomeStatementBlock = homeStatementBlock;
			m_LocalIndex = localIndex;
			m_LocalBuilder = localBuilder;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region ILocal Members

		void ILocal.Declare(ILGenerator il)
		{
			m_LocalBuilder = il.DeclareLocal(base.OperandType);
			Debug.Assert(m_LocalIndex == m_LocalBuilder.LocalIndex, "Local index mismatch after declaring a local.");
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IScopedOperand Members

		StatementBlock IScopedOperand.HomeStatementBlock
		{
			get
			{
				return m_HomeStatementBlock;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		string IScopedOperand.CaptureName
		{
			get
			{
				return "Loc" + m_LocalIndex + "_" + base.OperandType.Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		bool IScopedOperand.ShouldInitializeHoistedField
		{
			get
			{
				return false;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region ITransformType Members

		Operand<TCast> ITransformType.TransformToType<TCast>()
		{
			return new Local<TCast>(m_HomeStatementBlock, m_LocalIndex, m_LocalBuilder);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Local{0}[{1}]", m_LocalIndex, base.OperandType.Name);
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
