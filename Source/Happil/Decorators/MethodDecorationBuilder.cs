using System;
using System.Collections.Generic;
using Happil.Operands;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Decorators
{
	public class MethodDecorationBuilder
	{
		private readonly DecoratingMethodWriter m_OwnerWriter;
		private Action<MethodWriterBase> m_OnBefore;
		private Action<MethodWriterBase, Argument<TypeTemplate.TArgument>> m_OnInputArgument;
		private Action<MethodWriterBase> m_OnReturnVoid;
		private Action<MethodWriterBase, LocalOperand<TypeTemplate.TReturn>> m_OnReturnValue;
		private Action<MethodWriterBase, Argument<TypeTemplate.TArgument>> m_OnOutputArgument;
		private Action<MethodWriterBase> m_OnSuccess;
		private readonly List<Action<MethodWriterBase, IHappilCatchSyntax>> m_OnCatchExceptions;
		private Action<MethodWriterBase> m_OnFailure;
		private Action<MethodWriterBase> m_OnAfter;
		private bool m_IsEmpty;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal MethodDecorationBuilder(DecoratingMethodWriter ownerWriter)
		{
			m_OwnerWriter = ownerWriter;
			m_OnCatchExceptions = new List<Action<MethodWriterBase, IHappilCatchSyntax>>();
			m_IsEmpty = true;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			m_OwnerWriter.Attribute<TAttribute>(values);
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnBefore(Action<MethodWriterBase> decoration)
		{
			m_OnBefore = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnInputArgument(Action<MethodWriterBase, Argument<TypeTemplate.TArgument>> decoration)
		{
			m_OnInputArgument = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnReturnVoid(Action<MethodWriterBase> decoration)
		{
			m_OnReturnVoid = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnReturnValue(Action<MethodWriterBase, LocalOperand<TypeTemplate.TReturn>> decoration)
		{
			m_OnReturnValue = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnOutputArgument(Action<MethodWriterBase, Argument<TypeTemplate.TArgument>> decoration)
		{
			m_OnOutputArgument = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnSuccess(Action<MethodWriterBase> decoration)
		{
			m_OnSuccess = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnException<TException>(Action<MethodWriterBase, Operand<TException>> decoration)
			where TException : Exception
		{
			m_OnCatchExceptions.Add((method, tryCatchStatement) => tryCatchStatement.Catch<TException>(excepion => decoration(method, excepion)));
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnFailure(Action<MethodWriterBase> decoration)
		{
			m_OnFailure = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnAfter(Action<MethodWriterBase> decoration)
		{
			m_OnAfter = decoration;
			m_IsEmpty = false;
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal void ApplyDecoration()
		{
			var successLocal = (m_OnFailure != null ? m_OwnerWriter.Local<bool>(initialValueConst: false) : null);

			if ( m_OnBefore != null )
			{
				m_OnBefore(m_OwnerWriter);
			}

			InspectInputArguments();

			var tryBlock = m_OwnerWriter.Try(
				() => {
					var returnValueExpression = m_OwnerWriter.Proceed<TypeTemplate.TReturn>();

					if ( m_OnFailure != null )
					{
						successLocal.Assign(true);
					}

					InspectReturnValue(returnValueExpression);
					InspectOutputArguments();
				});

			foreach ( var exceptionHandler in m_OnCatchExceptions )
			{
				exceptionHandler(m_OwnerWriter, tryBlock);
			}

			tryBlock.Finally(() => {
				if ( m_OnFailure != null )
				{
					m_OwnerWriter.If(!successLocal).Then(() => m_OnFailure(m_OwnerWriter));
				}

				if ( m_OnAfter != null )
				{
					m_OnAfter(m_OwnerWriter);
				}
			});
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal bool IsEmpty
		{
			get
			{
				return m_IsEmpty;
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private void InspectInputArguments()
		{
			if ( m_OnInputArgument != null )
			{
				m_OwnerWriter.ForEachArgument(arg => {
					if ( !arg.IsOut )
					{
						m_OnInputArgument(m_OwnerWriter, arg);
					}
				});
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private void InspectReturnValue(Operand<TypeTemplate.TReturn> returnValueExpression)
		{
			if ( !ReferenceEquals(returnValueExpression, null) )
			{
				var returnValue = m_OwnerWriter.Local<TypeTemplate.TReturn>(initialValue: returnValueExpression);

				if ( m_OnReturnValue != null )
				{
					m_OnReturnValue(m_OwnerWriter, returnValue);
				}

				if ( m_OnSuccess != null )
				{
					m_OnSuccess(m_OwnerWriter);
				}

				m_OwnerWriter.AddReturnStatement(returnValue);
			}
			else
			{
				if ( m_OnReturnVoid != null )
				{
					m_OnReturnVoid(m_OwnerWriter);
				}

				if ( m_OnSuccess != null )
				{
					m_OnSuccess(m_OwnerWriter);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void InspectOutputArguments()
		{
			if ( m_OnOutputArgument != null )
			{
				m_OwnerWriter.ForEachArgument(arg => {
					if ( arg.IsByRef || arg.IsOut )
					{
						m_OnOutputArgument(m_OwnerWriter, arg);
					}
				});
			}
		}
	}
}