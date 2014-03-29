using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class ConstructorWriter : MethodWriterBase
	{
		public ConstructorWriter(MethodMember ownerMethod)
			: base(ownerMethod)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base()
		{
			var baseConstructor = FindBaseConstructor();

			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor),
				@operand: new ThisOperand<object>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1>(IOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1, TArg2>(IOperand<TArg1> arg1, IOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1, TArg2, TArg3>(IOperand<TArg1> arg1, IOperand<TArg2> arg2, IOperand<TArg3> arg3)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1>(IOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1, TArg2>(IOperand<TArg1> arg1, IOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1, TArg2, TArg3>(IOperand<TArg1> arg1, IOperand<TArg2> arg2, IOperand<TArg3> arg3)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ConstructorInfo FindBaseConstructor(params Type[] argumentTypes)
		{
			var constructor = OwnerMethod.OwnerClass.BaseType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				binder: null,
				types: argumentTypes,
				modifiers: null);

			if ( constructor != null )
			{
				return constructor;
			}

			throw new InvalidOperationException("Base constructor not found.");
		}
	}
}
