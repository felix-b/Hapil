using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hapil.Statements
{
	public interface ILoopBody
	{
		void Continue();
		void Break();
	}
}
