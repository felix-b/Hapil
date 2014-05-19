using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Operands
{
	internal class MethodOf : Operand<IntPtr>
	{
		private readonly MethodInfo m_MethodInfo;
		private readonly MethodMember m_MethodMember;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodOf(MethodInfo method)
		{
			m_MethodInfo = method;
			m_MethodMember = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodOf(MethodMember method)
		{
			m_MethodMember = method;
			m_MethodInfo = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format("Method[{0}]", (m_MethodMember != null ? m_MethodMember.Name : m_MethodInfo.Name));
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.Constant;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			var methodInfo = (m_MethodInfo ?? (MethodInfo)m_MethodMember.MethodFactory.Builder);
			il.Emit(OpCodes.Ldftn, methodInfo);
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
}
