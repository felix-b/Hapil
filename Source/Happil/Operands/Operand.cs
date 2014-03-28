using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Members;

namespace Happil.Operands
{
	public abstract class Operand<T> : IOperand<T>, IOperandEmitter
	{
		private readonly Type m_OperandType;
		private TypeMemberCache m_TypeMembers;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Operand()
		{
			m_OperandType = TypeTemplate.Resolve<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IOperand Members

		public IOperand<TOther> CastTo<TOther>()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type OperandType
		{
			get
			{
				return m_OperandType;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IOperandEmitter Members

		void IOperandEmitter.EmitTarget(ILGenerator il)
		{
			OnEmitTarget(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IOperandEmitter.EmitLoad(ILGenerator il)
		{
			OnEmitLoad(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IOperandEmitter.EmitStore(ILGenerator il)
		{
			OnEmitStore(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IOperandEmitter.EmitAddress(ILGenerator il)
		{
			OnEmitAddress(il);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual bool HasTarget
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual bool IsMutable
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual bool CanEmitAddress
		{
			get
			{
				return false;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeMemberCache Members
		{
			get
			{
				if ( m_TypeMembers == null )
				{
					m_TypeMembers = TypeMemberCache.Of(this.OperandType);
				}

				return m_TypeMembers;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void OnEmitTarget(ILGenerator il);
		protected abstract void OnEmitLoad(ILGenerator il);
		protected abstract void OnEmitStore(ILGenerator il);
		protected abstract void OnEmitAddress(ILGenerator il);

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		
	}
}
