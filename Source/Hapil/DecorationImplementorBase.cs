using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Fluent;
using Hapil.Statements;

namespace Hapil
{
	public abstract class DecorationImplementorBase : IDecorationImplementor
	{
		#region IDecorationImplementor Members

		public void ImplementDecoration<TBase>(IHappilClassBody<TBase> classDefinition)
		{
			classDefinition.AllMethods().ForEach(info => InternalOnMethod(classDefinition, info));
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnField(HappilField field)
		{
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public virtual void OnProperty(HappilProperty<TypeTemplate.TProperty> property)
		//{
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnEvent(HappilEvent @event)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnMethod(MethodInfo info, MethodDecorationBuilder decoration)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void InternalOnMethod<TBase>(IHappilClassBody<TBase> classDefinition, MethodInfo info)
		{
			var decoration = new MethodDecorationBuilder();
			OnMethod(info, decoration);
			
			if ( !decoration.IsEmpty )
			{
				classDefinition.Method(info).Decorate(
					attributes: decoration.GetAttributes,
					body: decoration.ApplyBody);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class MethodDecorationBuilder
		{
			private readonly HappilAttributes m_Attributes;
			private Action<IHappilMethodBodyBase> m_OnBefore;
			private Action<IHappilMethodBodyBase, HappilArgument<TypeTemplate.TArgument>> m_OnInputArgument;
			private Action<IHappilMethodBodyBase> m_OnReturnVoid;
			private Action<IHappilMethodBodyBase, HappilLocal<TypeTemplate.TReturn>> m_OnReturnValue;
			private Action<IHappilMethodBodyBase, HappilArgument<TypeTemplate.TArgument>> m_OnOutputArgument;
			private Action<IHappilMethodBodyBase> m_OnSuccess;
			private readonly List<Action<IHappilMethodBodyTemplate, IHappilCatchSyntax>> m_OnCatchExceptions;
			private Action<IHappilMethodBodyBase> m_OnFailure;
			private Action<IHappilMethodBodyBase> m_OnAfter;
			private bool m_IsEmpty;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder()
			{
				m_Attributes = new HappilAttributes();
				m_OnCatchExceptions = new List<Action<IHappilMethodBodyTemplate, IHappilCatchSyntax>>();
				m_IsEmpty = true;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder SetAttribute<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null) 
				where TAttribute : Attribute
			{
				m_Attributes.Set<TAttribute>(values);
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnBefore(Action<IHappilMethodBodyBase> decoration)
			{
				m_OnBefore = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnInputArgument(Action<IHappilMethodBodyBase, HappilArgument<TypeTemplate.TArgument>> decoration)
			{
				m_OnInputArgument = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnReturnVoid(Action<IHappilMethodBodyBase> decoration)
			{
				m_OnReturnVoid = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnReturnValue(Action<IHappilMethodBodyBase, HappilLocal<TypeTemplate.TReturn>> decoration)
			{
				m_OnReturnValue = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnOutputArgument(Action<IHappilMethodBodyBase, HappilArgument<TypeTemplate.TArgument>> decoration)
			{
				m_OnOutputArgument = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnSuccess(Action<IHappilMethodBodyBase> decoration)
			{
				m_OnSuccess = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnException<TException>(Action<IHappilMethodBodyTemplate, HappilOperand<TException>> decoration)
				where TException : Exception
			{
				m_OnCatchExceptions.Add((method, tryCatchStatement) => tryCatchStatement.Catch<TException>(excepion => decoration(method, excepion)));
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnFailure(Action<IHappilMethodBodyBase> decoration)
			{
				m_OnFailure = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodDecorationBuilder OnAfter(Action<IHappilMethodBodyBase> decoration)
			{
				m_OnAfter = decoration;
				m_IsEmpty = false;
				return this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal void ApplyBody(IHappilMethodBodyTemplate method)
			{
				var successLocal = (m_OnFailure != null ? method.Local<bool>(initialValueConst: false) : null);

				if ( m_OnBefore != null )
				{
					m_OnBefore(method);
				}

				InspectInputArguments(method);

				var tryBlock = method.Try(
					() => {
						var returnValueExpression = method.Proceed();

						if ( m_OnFailure != null )
						{
							successLocal.AssignConst(true);
						}

						InspectReturnValue(method, returnValueExpression);
						InspectOutputArguments(method);
					});

				foreach ( var exceptionHandler in m_OnCatchExceptions )
				{
					exceptionHandler(method, tryBlock);
				}

				tryBlock.Finally(() => {
					if ( m_OnFailure != null )
					{
						method.If(!successLocal).Then(() => m_OnFailure(method));
					}

					if ( m_OnAfter != null )
					{
						m_OnAfter(method);
					}
				});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal IHappilAttributes GetAttributes(IHappilMethodBodyBase method)
			{
				return m_Attributes;
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

			private void InspectInputArguments(IHappilMethodBodyTemplate method)
			{
				if ( m_OnInputArgument != null )
				{
					method.ForEachArgument(arg => {
						if ( !arg.IsOut )
						{
							m_OnInputArgument(method, arg);
						}
					});
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
			
			private void InspectReturnValue(IHappilMethodBodyTemplate method, HappilOperand<TypeTemplate.TReturn> returnValueExpression)
			{
				if ( !object.ReferenceEquals(returnValueExpression, null) )
				{
					var returnValue = method.Local<TypeTemplate.TReturn>(initialValue: returnValueExpression);

					if ( m_OnReturnValue != null )
					{
						m_OnReturnValue(method, returnValue);
					}

					if ( m_OnSuccess != null )
					{
						m_OnSuccess(method);
					}

					method.Return(returnValue);
				}
				else
				{
					if ( m_OnReturnVoid != null )
					{
						m_OnReturnVoid(method);
					}

					if ( m_OnSuccess != null )
					{
						m_OnSuccess(method);
					}
				}
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------
			
			private void InspectOutputArguments(IHappilMethodBodyTemplate method)
			{
				if ( m_OnOutputArgument != null )
				{
					method.ForEachArgument(arg => {
						if ( arg.IsByRef || arg.IsOut )
						{
							m_OnOutputArgument(method, arg);
						}
					});
				}
			}
		}
	}
}
