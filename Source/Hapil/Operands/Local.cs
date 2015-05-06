using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Statements;

namespace Hapil.Operands
{
	public class Local<T> : MutableOperand<T>, ILocal, ICanEmitAddress, IScopedOperand, ITransformType, IBindToMethod
	{
		private readonly ILocal m_OriginalLocal;
		private StatementBlock m_HomeStatementBlock;
		private int m_LocalIndex;
		private LocalBuilder m_LocalBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Local(MethodMember ownerMethod)
		{
			if ( StatementScope.Exists )
			{
				m_HomeStatementBlock = StatementScope.Current.StatementBlock;
			}
			else if ( ownerMethod != null )
			{
				m_HomeStatementBlock = ownerMethod.Body;
			}

			if ( ownerMethod != null )
			{
				ownerMethod.RegisterLocal(this, out m_LocalIndex);
			}
			else
			{
				m_LocalIndex = -1;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Local(StatementBlock homeStatementBlock)
		{
			m_OriginalLocal = null;
			m_HomeStatementBlock = homeStatementBlock;
			m_LocalIndex = -1;
			m_LocalBuilder = null;

			if ( homeStatementBlock.OwnerMethod != null )
			{
				homeStatementBlock.OwnerMethod.RegisterLocal(this, out m_LocalIndex);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private Local(ILocal originalLocal, StatementBlock homeStatementBlock, int localIndex, LocalBuilder localBuilder)
		{
			m_OriginalLocal = originalLocal;
			m_HomeStatementBlock = homeStatementBlock;
			m_LocalIndex = localIndex;
			m_LocalBuilder = localBuilder;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IBindToMethod Members

		void IBindToMethod.BindToMethod(MethodMember method)
		{
			if ( m_HomeStatementBlock == null )
			{
				m_HomeStatementBlock = method.Body;
			}

			method.RegisterLocal(this, out m_LocalIndex);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IBindToMethod.ResetBinding()
		{
			if ( m_HomeStatementBlock != null && m_HomeStatementBlock == m_HomeStatementBlock.OwnerMethod.Body )
			{
				m_HomeStatementBlock = null;
			}

			m_LocalBuilder = null;
			m_LocalIndex = -1;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		bool IBindToMethod.IsBound
		{
			get
			{
				return (m_LocalIndex >= 0);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region ILocal Members

		void ILocal.Declare(ILGenerator il)
		{
			m_LocalBuilder = il.DeclareLocal(base.OperandType);
			Debug.Assert(m_LocalIndex <= m_LocalBuilder.LocalIndex, "Local index mismatch after declaring a local.");
            //TODO: understand why 'm_LocalIndex == m_LocalBuilder.LocalIndex' sometimes fails
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		LocalBuilder ILocal.LocalBuilder
		{
			get 
			{
				return GetLocalBuilder();
			}
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
			return new Local<TCast>(this, m_HomeStatementBlock, m_LocalIndex, m_LocalBuilder);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Loc{0}", m_LocalIndex);
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
			il.Emit(OpCodes.Ldloc, GetLocalBuilder());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			il.Emit(OpCodes.Stloc, GetLocalBuilder());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			il.Emit(OpCodes.Ldloca, (short)GetLocalBuilder().LocalIndex);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		private LocalBuilder GetLocalBuilder()
		{
			if ( m_LocalBuilder == null && m_OriginalLocal != null )
			{
				m_LocalBuilder = m_OriginalLocal.LocalBuilder;
			}

			Debug.Assert(m_LocalBuilder != null, "Local was not declared in ILGenerator.");
			return m_LocalBuilder;
		}
	}
}
