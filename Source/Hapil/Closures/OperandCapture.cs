using System;
using System.Collections.Generic;
using System.Linq;
using Hapil.Closures;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Statements;
using Hapil.Writers;

namespace Hapil.Closures
{
	internal class OperandCapture //: IEquatable<OperandCapture>
	{
		private readonly HashSet<IAnonymousMethodOperand> m_Consumers;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public OperandCapture(IScopedOperand sourceOperand, StatementBlock sourceOperandHome, IAnonymousMethodOperand consumerMethod)
		{
			this.SourceOperand = sourceOperand;
			this.SourceOperandHome = sourceOperandHome;

			m_Consumers = new HashSet<IAnonymousMethodOperand>() {
				consumerMethod
			};
		}

		////-------------------------------------------------------------------------------------------------------------------------------------------------

		//#region IEquatable<OperandCapture> Members

		//public bool Equals(OperandCapture other)
		//{
		//	if ( other != null )
		//	{
		//		return this.SourceOperand.Equals(other.SourceOperand);
		//	}
		//	else
		//	{
		//		return false;
		//	}
		//}

		//#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		//public override bool Equals(object obj)
		//{
		//	return Equals(obj as OperandCapture);
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public override int GetHashCode()
		//{
		//	return this.SourceOperand.GetHashCode();
		//}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public override string ToString()
		{
			return (HoistedField != null ? HoistedField.ToString() : SourceOperand.ToString());
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public void Merge(OperandCapture other)
		{
			if ( this.HoistingClosure != null )
			{
				throw new InvalidOperationException("Cannot merge other captures because current capture was already assigned a closure.");
			}

			if ( other.HoistingClosure != null )
			{
				throw new ArgumentException("Cannot merge specified capture because it was already assigned a closure.");
			}

			if ( other.SourceOperand != this.SourceOperand )
			{
				throw new ArgumentException("Cannot merge specified capture because of source operands mismatch.");
			}

			m_Consumers.UnionWith(other.Consumers);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public void HoistInClosure(ClosureDefinition closure)
		{
			this.HoistingClosure = closure;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public void DefineHoistedField(ClassWriterBase closureClassWriter)
		{
			this.HoistedField = closureClassWriter.DefineField(
				name: "<hoisted>" + this.Name,
				isStatic: false,
				isPublic: true,
				fieldType: this.OperandType);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public void AddConsumer(IAnonymousMethodOperand consumerMethod)
		{
			m_Consumers.Add(consumerMethod);
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock SourceOperandHome { get; private set; }
		public IScopedOperand SourceOperand { get; private set; }
		public FieldMember HoistedField { get; private set; }
		public ClosureDefinition HoistingClosure { get; private set; }

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

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public IAnonymousMethodOperand[] Consumers
		{
			get
			{
				return m_Consumers.ToArray();
			}
		}
	}
}