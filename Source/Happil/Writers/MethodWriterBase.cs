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
		private AttributeWriter m_ReturnAttributeWriter;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(MethodMember ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
			ownerMethod.AddWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new AttributeArgumentWriter<TAttribute>(values);
			m_OwnerMethod.MethodFactory.SetAttribute(builder.GetAttributeBuilder());
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

		public Argument<T> Argument<T>(int position)
		{
			return new Argument<T>(m_OwnerMethod, (byte)position);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg1<T>()
		{
			return new Argument<T>(m_OwnerMethod, 1);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg2<T>()
		{
			return new Argument<T>(m_OwnerMethod, 2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg3<T>()
		{
			return new Argument<T>(m_OwnerMethod, 3);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg4<T>()
		{
			return new Argument<T>(m_OwnerMethod, 4);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg5<T>()
		{
			return new Argument<T>(m_OwnerMethod, 5);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg6<T>()
		{
			return new Argument<T>(m_OwnerMethod, 6);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg7<T>()
		{
			return new Argument<T>(m_OwnerMethod, 7);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Argument<T> Arg8<T>()
		{
			return new Argument<T>(m_OwnerMethod, 8);
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

		protected AttributeWriter GetReturnAttributeWriter()
		{
			if ( m_ReturnAttributeWriter == null )
			{
				m_ReturnAttributeWriter = new AttributeWriter(attr => m_OwnerMethod.MethodFactory.ReturnParameter.SetCustomAttribute(attr));
			}

			return m_ReturnAttributeWriter;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal abstract void Flush();
	}
}
