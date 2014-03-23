using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public abstract class ClassWriterBase
	{
		private readonly ClassType m_ClassType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ClassWriterBase(ClassType classType)
		{
			m_ClassType = classType;
			classType.AddWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected abstract void Flush();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		public ClassType ClassType
		{
			get { return m_ClassType; }
		}
	}
}
