using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	public class HappilLocal<T> : HappilAssignable<T>
	{
		private readonly string m_Name;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilLocal(string name, HappilMethod ownerMethod)
			: base(ownerMethod)
		{
			m_Name = name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Arg<{0}>{{{1}}}", typeof(T).Name, m_Name);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
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
