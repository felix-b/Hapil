using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Happil
{
	internal class UniqueNameSet
	{
		private readonly HashSet<string> m_NamesInUse;
		private int m_UniqueNumericSuffix;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public UniqueNameSet()
		{
			m_NamesInUse = new HashSet<string>();
			m_UniqueNumericSuffix = 0;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string TakeUniqueName(string proposedName)
		{
			string uniqueName;

			if ( m_NamesInUse.Add(proposedName) )
			{
				uniqueName = proposedName;
			}
			else
			{
				uniqueName = proposedName + Interlocked.Increment(ref m_UniqueNumericSuffix);
				m_NamesInUse.Add(uniqueName);
			}

			return uniqueName;
		}
	}
}
