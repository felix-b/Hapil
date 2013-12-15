﻿using System.Reflection.Emit;
using Happil.Fluent;

namespace Happil.Expressions
{
	internal interface IUnaryOperator<T> : IOperator
	{
		void Emit(ILGenerator il, IHappilOperand<T> operand);
	}
}
