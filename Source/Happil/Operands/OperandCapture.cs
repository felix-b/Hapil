using System;
using Happil.Members;
using Happil.Statements;

namespace Happil.Operands
{
	internal class OperandCapture
	{
		public OperandCapture(IScopedOperand sourceOperand, StatementBlock sourceOperandHome)
		{
			this.SourceOperand = sourceOperand;
			this.SourceOperandHome = sourceOperandHome;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return (HoistedField != null ? HoistedField.ToString() : SourceOperand.ToString());
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock SourceOperandHome { get; private set; }
		public IScopedOperand SourceOperand { get; private set; }
		public FieldMember HoistedField { get; set; }

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public string Name
		{
			get
			{
				return SourceOperand.CaptureName;
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public Type OperandType
		{
			get
			{
				return SourceOperand.OperandType;
			}
		}
	}
}