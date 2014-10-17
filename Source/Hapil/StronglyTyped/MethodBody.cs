using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Fluent;
using Hapil.Statements;

namespace Hapil.StronglyTyped
{
	internal class MethodBody : IHappilMethodBodyTemplate
	{
		private readonly HappilMethod m_OwnerMethod;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodBody(HappilMethod ownerMethod)
		{
			m_OwnerMethod = ownerMethod;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMethodBodyTemplate Members

		public IHappilIfBody If(IHappilOperand<bool> condition)
		{
			return m_OwnerMethod.AddStatement(new IfStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilIfBody If(IHappilOperand<bool> condition, bool isTautology)
		{
			return m_OwnerMethod.AddStatement(new IfStatement(condition, isTautology));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> Iif<T>(IHappilOperand<bool> condition, IHappilOperand<T> onTrue, IHappilOperand<T> onFalse)
		{
			return new TernaryConditionalOperator<T>(condition, onTrue, onFalse);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> Iif<T>(IHappilOperand<bool> condition, bool isTautology, IHappilOperand<T> onTrue, IHappilOperand<T> onFalse)
		{
			if ( !isTautology )
			{
				return new TernaryConditionalOperator<T>(condition, onTrue, onFalse);
			}
			else
			{
				var scope = StatementScope.Current;
				scope.Consume(condition);
				scope.Consume(onFalse);

				return (HappilOperand<T>)onTrue;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilWhileSyntax While(IHappilOperand<bool> condition)
		{
			return m_OwnerMethod.AddStatement(new WhileStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilDoWhileSyntax Do(Action<IHappilLoopBody> body)
		{
			return m_OwnerMethod.AddStatement(new DoWhileStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(HappilConstant<int> from, IHappilOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(m_OwnerMethod, from, to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IHappilOperand<int> from, HappilConstant<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(m_OwnerMethod, from.CastTo<int>(), to, increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IHappilOperand<int> from, IHappilOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(m_OwnerMethod, from.CastTo<int>(), to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForWhileSyntax For(Action precondition)
		{
			return m_OwnerMethod.AddStatement(new ForStatement(precondition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachInSyntax<T> Foreach<T>(HappilLocal<T> element)
		{
			return m_OwnerMethod.AddStatement(new ForeachStatement<T>(element));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachDoSyntax<T> ForeachElementIn<T>(IHappilOperand<IEnumerable<T>> collection)
		{
			var element = this.Local<T>();
			var statement = new ForeachStatement<T>(element);

			m_OwnerMethod.AddStatement(statement);
			statement.In(collection);

			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilUsingSyntax Using(IHappilOperand<IDisposable> disposable)
		{
			return m_OwnerMethod.AddStatement(new UsingStatement(disposable));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilLockSyntax Lock(IHappilOperand<object> syncRoot, int millisecondsTimeout)
		{
			return m_OwnerMethod.AddStatement(new LockStatement(syncRoot, millisecondsTimeout));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilCatchSyntax Try(Action body)
		{
			return m_OwnerMethod.AddStatement(new TryStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilSwitchSyntax<T> Switch<T>(IHappilOperand<T> value)
		{
			return m_OwnerMethod.AddStatement(new SwitchStatement<T>(value));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilThis<TBase> This<TBase>()
		{
			return new HappilThis<TBase>(m_OwnerMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilLocal<T> Local<T>()
		{
			return new HappilLocal<T>(m_OwnerMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilLocal<T> Local<T>(IHappilOperand<T> initialValue)
		{
			var local = new HappilLocal<T>(m_OwnerMethod);
			local.Assign(initialValue);
			return local;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilLocal<T> Local<T>(T initialValueConst)
		{
			return Local<T>(new HappilConstant<T>(initialValueConst));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilConstant<T> Const<T>(T value)
		{
			return new HappilConstant<T>(value);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> Default<T>()
		{
			var actualType = TypeTemplate.Resolve<T>();

			if ( actualType.IsPrimitive || !actualType.IsValueType )
			{
				return new HappilConstant<T>(default(T));
			}
			else
			{
				return new NewStructExpression<T>();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<TObject> New<TObject>(params IHappilOperand[] constructorArguments)
		{
			if ( TypeTemplate.Resolve<TObject>().IsValueType )
			{
				return new NewStructExpression<TObject>(constructorArguments);
			}
			else
			{
				return new NewObjectExpression<TObject>(constructorArguments);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<TElement[]> NewArray<TElement>(IHappilOperand<int> length)
		{
			return new HappilUnaryExpression<int, TElement[]>(
				ownerMethod: null,
				@operator: new UnaryOperators.OperatorNewArray<TElement>(),
				operand: length);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<TElement[]> NewArray<TElement>(params TElement[] constantValues)
		{
			return NewArray<TElement>(constantValues.Select(v => new HappilConstant<TElement>(v)).ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<TElement[]> NewArray<TElement>(params IHappilOperand<TElement>[] values)
		{
			var arrayLocal = Local<TElement[]>(NewArray<TElement>(Const(values.Length)));

			for ( int i = 0 ; i < values.Length ; i++ )
			{
				arrayLocal.ElementAt(i).Assign(values[i]);
			}

			return arrayLocal;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RaiseEvent(string eventName, IHappilOperand<EventArgs> eventArgs)
		{
			var eventMember = m_OwnerMethod.HappilClass.FindMember<HappilEvent>(eventName);
			eventMember.RaiseEvent(this, eventArgs);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Throw<TException>(string message) where TException : Exception
		{
			m_OwnerMethod.AddStatement(new ThrowStatement(typeof(TException), message));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Throw()
		{
			m_OwnerMethod.AddStatement(new RethrowStatement());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TReturn>> Delegate<TArg1, TReturn>(Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>> body)
		{
			return new HappilAnonymousDelegate<TArg1, TReturn>(m_OwnerMethod.HappilClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TReturn>> Delegate<TArg1, TReturn>(
			ref IHappilDelegate site,
			Action<IHappilMethodBody<TReturn>,
			HappilArgument<TArg1>> body)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Delegate(body);
			}

			return (HappilAnonymousDelegate<TArg1, TReturn>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TArg2, TReturn>> Delegate<TArg1, TArg2, TReturn>(
			Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>, HappilArgument<TArg2>> body)
		{
			return new HappilAnonymousDelegate<TArg1, TArg2, TReturn>(m_OwnerMethod.HappilClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TArg2, TReturn>> Delegate<TArg1, TArg2, TReturn>(
			ref IHappilDelegate site,
			Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>, HappilArgument<TArg2>> body)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Delegate(body);
			}

			return (HappilAnonymousDelegate<TArg1, TArg2, TReturn>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TMethod> MakeDelegate<TTarget, TMethod>(IHappilOperand<TTarget> target, Expression<Func<TTarget, TMethod>> methodSelector)
		{
			var method = Helpers.ResolveMethodFromLambda(methodSelector);
			return new HappilDelegate<TMethod>(target, method);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TResult>> Lambda<TArg1, TResult>(Func<HappilOperand<TArg1>, IHappilOperand<TResult>> expression)
		{
			return Delegate<TArg1, TResult>((m, arg1) => m.Return(expression(arg1)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TResult>> Lambda<TArg1, TResult>(
			ref IHappilDelegate site,
			Func<HappilOperand<TArg1>, IHappilOperand<TResult>> expression)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Lambda(expression);
			}

			return (HappilAnonymousDelegate<TArg1, TResult>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TArg2, TResult>> Lambda<TArg1, TArg2, TResult>(
			ref IHappilDelegate site,
			Func<HappilOperand<TArg1>, HappilOperand<TArg2>, IHappilOperand<TResult>> expression)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Lambda(expression);
			}

			return (HappilAnonymousDelegate<TArg1, TArg2, TResult>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<Func<TArg1, TArg2, TResult>> Lambda<TArg1, TArg2, TResult>(
			Func<HappilOperand<TArg1>, HappilOperand<TArg2>, IHappilOperand<TResult>> expression)
		{
			return Delegate<TArg1, TArg2, TResult>((m, arg1, arg2) => m.Return(expression(arg1, arg2)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void EmitFromLambda(Expression<Action> lambda)
		{
			var callInfo = (MethodCallExpression)lambda.Body;
			var methodInfo = callInfo.Method;
			var arguments = callInfo.Arguments.Select(Helpers.GetLambdaArgumentAsConstant).ToArray();
			var happilExpression = new HappilUnaryExpression<object, object>(
				ownerMethod: m_OwnerMethod,
				@operator: new UnaryOperators.OperatorCall<object>(methodInfo, arguments),
				operand: null);

			//var arguments = new IHappilOperandInternals[callInfo.Arguments.Count];

			//for ( int i = 0 ; i < arguments.Length ; i++ )
			//{
			//	//var argument = callInfo.Arguments[i];


			//	//Expression<Func<object>> argumentLambda = Expression.Lambda<Func<object>>(argument);
			//	//var argumentValueFunc = argumentLambda.Compile();
			//	//var argumentValue = argumentValueFunc();

			//	arguments[i] = Helpers.GetLambdaArgumentAsConstant(callInfo.Arguments[i]);
			//}

			//m_HappilClass.CurrentScope.AddStatement();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ForEachArgument(Action<HappilArgument<TypeTemplate.TArgument>> action)
		{
			var argumentTypes = m_OwnerMethod.GetArgumentTypes();
			var indexBase = (m_OwnerMethod.IsStatic ? 0 : 1);

			for ( byte i = 0 ; i < argumentTypes.Length ; i++ )
			{
				using ( TypeTemplate.CreateScope<TypeTemplate.TArgument>(argumentTypes[i]) )
				{
					var argument = this.Argument<TypeTemplate.TArgument>((byte)(i + indexBase));
					action(argument);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilArgument<T> Argument<T>(byte index)
		{
			return new HappilArgument<T>(m_OwnerMethod, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TypeTemplate.TReturn> Proceed()
		{
			return m_OwnerMethod.Proceed();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return(IHappilOperand<TypeTemplate.TReturn> operand)
		{
			m_OwnerMethod.AddReturnStatement(operand);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return()
		{
			StatementScope.Current.AddStatement(new ReturnStatement());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual MethodInfo MethodInfo
		{
			get
			{
				return m_OwnerMethod.MethodInfo;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual MethodBuilder MethodBuilder
		{
			get
			{
				return m_OwnerMethod.MethodBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual int ArgumentCount
		{
			get
			{
				return m_OwnerMethod.GetArgumentTypes().Length;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual Type ReturnType
		{
			get
			{
				return m_OwnerMethod.ReturnType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilAttributes ReturnAttributes
		{
			get
			{
				return m_OwnerMethod.ReturnAttributes;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected HappilMethod OwnerMethod
		{
			get
			{
				return m_OwnerMethod;
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class MethodBody<TReturn> : MethodBody, IHappilMethodBody<TReturn>
	{
		public MethodBody(HappilMethod ownerMethod) : base(ownerMethod)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMethodBody<TReturn> Members

		public void Return(IHappilOperand<TReturn> operand)
		{
			OwnerMethod.AddReturnStatement(operand.OrNullConstant().CastTo<TypeTemplate.TReturn>());
			//StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(operand.OrNullConstant()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ReturnConst(TReturn constantValue)
		{
			OwnerMethod.AddReturnStatement(new HappilConstant<TReturn>(constantValue).CastTo<TypeTemplate.TReturn>());
			////TODO: verify that current scope belongs to this method
			//StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(new HappilConstant<TReturn>(constantValue)));
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class VoidMethodBody : MethodBody, IVoidHappilMethodBody
	{
		public VoidMethodBody(HappilMethod ownerMethod) : base(ownerMethod)
		{
		}
	}
}
