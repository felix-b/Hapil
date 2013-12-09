using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	internal class HappilMethod : IHappilMember
	{
		private readonly HappilClass m_HappilClass;
		private readonly MethodBuilder m_MethodBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilMethod(HappilClass happilClass, MethodBuilder methodBuilder)
		{
			m_HappilClass = happilClass;
			m_MethodBuilder = methodBuilder;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

		public IHappilMember[] Flatten()
		{
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string Name
		{
			get
			{
				return m_MethodBuilder.Name;
			}
		}

		#endregion
	}
}
