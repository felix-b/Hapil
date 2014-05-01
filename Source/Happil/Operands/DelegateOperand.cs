using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Members;

namespace Happil.Operands
{
	public class DelegateOperand<TDelegate> : Operand<TDelegate>, IDelegateOperand, IAcceptOperandVisitor
	{
		private readonly MethodInfo m_Method;
		private readonly ConstructorInfo m_Constructor;
		private IOperand m_Target;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DelegateOperand(IOperand target, MethodInfo method, Type delegateTypeOverride = null) 
		{
			m_Target = target;
			m_Method = method;

			var effectiveDelegateType = delegateTypeOverride ?? base.OperandType;
			m_Constructor = DelegateShortcuts.GetDelegateConstructor(effectiveDelegateType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.Delegate;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperand(ref m_Target);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format(
				"Delegate[{0}{1}]", 
				m_Method.IsStatic ? m_Method.DeclaringType.Name + "::" : m_Target.ToString() + ".",
				m_Method.Name);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			m_Target.EmitTarget(il);
			m_Target.EmitLoad(il);
			
			il.Emit(OpCodes.Ldftn, m_Method);
			il.Emit(OpCodes.Newobj, m_Constructor);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotSupportedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotSupportedException();
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IDelegateOperand
	{
	}
}
