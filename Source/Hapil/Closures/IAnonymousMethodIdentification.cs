using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Closures
{
	internal interface IAnonymousMethodIdentification
	{
		//void Merge(IAnonymousMethodIdentification other);
		void DefineClosures();
		AnonymousMethodScope GetAnonymousMethodScope(IAnonymousMethodOperand anonymousMethod);
		IAnonymousMethodOperand[] AnonymousMethods { get; }
		IOperand[] MustCloseOverOperands { get; }
		OperandCapture[] Captures { get; }
		bool AnonymousMethodsFound { get; }
		bool ClosuresRequired { get; }
		ClosureDefinition OutermostClosure { get; }
		ClosureDefinition InnermostClosure { get; }
		ClosureDefinition[] ClosuresOuterToInner { get; }
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal enum AnonymousMethodScope
	{
		Static = 0,
		Instance = 1,
		Closure = 2
	}
}
