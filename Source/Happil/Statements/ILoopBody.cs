using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Statements
{
	public interface ILoopBody
	{
		void Continue();
		void Break();
	}
}
