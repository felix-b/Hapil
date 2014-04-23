using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil
{
	public class CodeGenerationException : Exception
	{
		public CodeGenerationException(string message)
			: base(message)
		{
		}
	}
}
