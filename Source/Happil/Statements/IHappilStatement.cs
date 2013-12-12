using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Statements
{
	internal interface IHappilStatement
	{
		void Emit(ILGenerator il);
	}
}
