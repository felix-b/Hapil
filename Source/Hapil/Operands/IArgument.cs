namespace Hapil.Operands
{
	public interface IArgument : IOperand
	{
		bool IsByRef { get; }
		bool IsOut { get; }
		int EmitIndex { get; }
		string Name { get; }
	}
}