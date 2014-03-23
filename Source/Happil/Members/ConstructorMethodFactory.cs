using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public class ConstructorMethodFactory : MethodFactory
	{
		public ConstructorMethodFactory(TypeBuilder type, Type[] argumentTypes)
		{
			
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void EmitCallInstruction(ILGenerator generator, OpCode instruction)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MethodSignature Signature
		{
			get { throw new NotImplementedException(); }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MethodBase Builder
		{
			get { throw new NotImplementedException(); }
		}
	}
}
