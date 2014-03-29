using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public abstract class MethodWriterBase
	{
		private readonly MethodMember m_OwnerMethod;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(MethodMember ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
			ownerMethod.AddWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> Default<T>()
		{
			var actualType = TypeTemplate.Resolve<T>();

			if ( actualType.IsPrimitive || !actualType.IsValueType )
			{
				var constant = Helpers.CreateConstant(actualType, actualType.GetDefaultValue());
				return constant.CastTo<T>();
				//new ConstantOperand<T>(default(T));
			}
			else
			{
				return new NewStructExpression<T>();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember OwnerMethod
		{
			get
			{
				return m_OwnerMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected abstract void Flush();
	}
}
