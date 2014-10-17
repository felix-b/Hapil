using Hapil.Closures;
using Hapil.Members;
using Hapil.Statements;

namespace Hapil.Operands
{
	internal interface IAnonymousMethodOperand
	{
		void CreateAnonymousMethod(ClassType ownerClass, ClosureDefinition closure, bool isStatic, bool isPublic);
		void WriteCallSite();
		StatementBlock Statements { get; }
		MethodMember AnonymousMethod { get; }
	}
}