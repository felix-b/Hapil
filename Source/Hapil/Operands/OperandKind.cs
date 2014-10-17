using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Operands
{
	public enum OperandKind
	{
		Constant,
		Local,
		Argument,
		This,
		Field,
		Property,
		ArrayElement,
		Delegate,
		UnaryExpression,
		BinaryExpression,
		TernaryCondition,
		NewObject,
		NewStruct
	}
}
