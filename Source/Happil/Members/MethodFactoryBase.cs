﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public abstract class MethodFactoryBase
	{
		public abstract void SetAttribute(CustomAttributeBuilder attribute);
		public abstract ILGenerator GetILGenerator();
		public abstract void EmitCallInstruction(ILGenerator generator, OpCode instruction);
		public abstract MethodSignature Signature { get; }
		public abstract MethodInfo Declaration { get; }
		public abstract MethodBase Builder { get; }
		public abstract ParameterBuilder[] Parameters { get; }
		public abstract ParameterBuilder ReturnParameter { get; }
		public abstract string MemberName { get; }
	}
}
