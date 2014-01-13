using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Statements
{
	internal interface ILeaveStatement : IHappilStatement
	{
		StatementScope HomeScope { get; }
	}
}
