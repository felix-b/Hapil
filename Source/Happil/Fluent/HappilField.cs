using System;
using System.Collections.Generic;
using System.Linq;
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

		public IHappilMember[] Flatten()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string Name
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
		
		////-------------------------------------------------------------------------------------------------------------------------------------------------

		//public static HappilField<T> operator ++(HappilField<T> x)
		//{
		//	return x;
		//}

		////-------------------------------------------------------------------------------------------------------------------------------------------------

		//public static HappilField<T> operator --(HappilField<T> x)
		//{
		//	return x;
		//}
	}
}
