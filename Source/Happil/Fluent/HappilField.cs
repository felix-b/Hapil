using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public class HappilField<T> : HappilAssignable<T>, IHappilMember
	{
		private readonly string m_Name;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilField(string name)
		{
			m_Name = name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IMember Members

		void IHappilMember.EmitBody()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		string IHappilMember.Name
		{
			get
			{
				return m_Name;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Field{{{0}}}", m_Name);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		protected override void OnEmitTarget(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotImplementedException();
		}
	}
}
