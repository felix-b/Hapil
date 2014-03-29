using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Statements
{
	internal interface ILeaveStatement//TODO:redesign : IHappilStatement
	{
		StatementScope HomeScope { get; }
	}
}
