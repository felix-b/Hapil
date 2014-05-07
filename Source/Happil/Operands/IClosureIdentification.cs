using Happil.Members;

namespace Happil.Operands
{
	internal interface IClosureIdentification
	{
		MethodMember HostMethod { get; }
		IOperand[] Externals { get; }
		OperandCapture[] Captures { get; }
		bool ClosuresRequired { get; }
		ClosureDefinition OutermostClosure { get; }
		ClosureDefinition InnermostClosure { get; }
		ClosureDefinition[] ClosuresOuterToInner { get; }
	}
}