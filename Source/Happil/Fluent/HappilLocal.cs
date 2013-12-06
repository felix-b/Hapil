using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilLocal<T> : HappilAssignable<T>
	{
		private readonly string m_Name;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilLocal(string name)
		{
			m_Name = name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return string.Format("Arg<{0}>{{{1}}}", typeof(T).Name, m_Name);
		}
	}
}
