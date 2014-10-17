#if false

using Hapil.Members;

namespace Hapil.Operands
{
	internal interface IClosureIdentification
	{
		void Merge(IClosureIdentification other);
		void DefineClosures();
		IAnonymousMethodOperand[] AnonymousMethods { get; }
		IOperand[] MustCloseOverOperands { get; }
		OperandCapture[] Captures { get; }
		bool AnonymousMethodsFound { get; }
		bool ClosuresRequired { get; }
		ClosureDefinition OutermostClosure { get; }
		ClosureDefinition InnermostClosure { get; }
		ClosureDefinition[] ClosuresOuterToInner { get; }
	}
}

#endif