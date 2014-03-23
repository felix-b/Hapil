using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public abstract class MethodFactory
	{
		public abstract void EmitCallInstruction(ILGenerator generator, OpCode instruction);
		public abstract MethodSignature Signature { get; }
		public abstract MethodBase Builder { get; }
	}
}
