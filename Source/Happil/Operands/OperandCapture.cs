using Happil.Statements;

namespace Happil.Operands
{
	internal class OperandCapture
	{
		public OperandCapture(IOperand sourceOperand, StatementBlock sourceOperandHome)
		{
			this.SourceOperand = sourceOperand;
			this.SourceOperandHome = sourceOperandHome;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return (RewrittenOperand != null ? RewrittenOperand.ToString() : SourceOperand.ToString());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock SourceOperandHome { get; private set; }
		public IOperand SourceOperand { get; private set; }
		public IOperand RewrittenOperand { get; private set; }
	}
}