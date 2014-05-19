using Happil.Closures;
using Happil.Members;
using Happil.Statements;

namespace Happil.Operands
{
	internal interface IAnonymousMethodOperand
	{
		void CreateAnonymousMethod(ClassType ownerClass, ClosureDefinition closure, bool isStatic, bool isPublic);
		void WriteCallSite();
		StatementBlock Statements { get; }
		MethodMember AnonymousMethod { get; }
	}
}