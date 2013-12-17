using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Happil.Expressions;

namespace Happil.Fluent
{
	internal class HappilConstructor : HappilMethod, IHappilConstructorBody
	{
		private readonly ConstructorBuilder m_ConstructorBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilConstructor(HappilClass happilClass, Type[] argumentTypes)
			: base(happilClass)
		{
			m_ConstructorBuilder = happilClass.TypeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
				CallingConventions.HasThis,
				argumentTypes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilConstructorBody Members

		public void Base()
		{
			//TODO: handle non-public base constructors
			var baseConstructor = HappilClass.TypeBuilder.BaseType.GetConstructor(new Type[0]);

			if ( baseConstructor == null )
			{
				throw new InvalidOperationException("Base constructor not found.");
			}

			new HappilUnaryExpression<object, object>(
				this, 
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor), 
				@operand: new HappilThis<object>(this));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1>(IHappilOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1, TArg2>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1, TArg2, TArg3>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1>(IHappilOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1, TArg2>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1, TArg2, TArg3>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3)
		{
			throw new NotImplementedException();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ConstructorInfo ConstructorInfo
		{
			get
			{
				return m_ConstructorBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of HappilMethod

		protected override ILGenerator GetILGenerator()
		{
			return m_ConstructorBuilder.GetILGenerator();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override Type GetReturnType()
		{
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override string GetName()
		{
			return ".ctor";
		}

		#endregion
	}
}