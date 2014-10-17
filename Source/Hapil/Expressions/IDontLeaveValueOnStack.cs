using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hapil.Expressions
{
	/// <summary>
	/// Implemented by operators that do not leave resulting value on the stack, by default.
	/// </summary>
	internal interface IDontLeaveValueOnStack
	{
		/// <summary>
		/// Overrides default behavior and forces the operator to leave resulting value on the stack.
		/// </summary>
		void ForceLeaveFalueOnStack();
	}
}
