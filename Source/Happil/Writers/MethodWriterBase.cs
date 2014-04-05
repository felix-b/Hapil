using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Writers
{
	public abstract class MethodWriterBase : MemberWriterBase
	{
		private readonly MethodMember m_OwnerMethod;
		private AttributeWriter m_ReturnAttributeWriter;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(MethodMember ownerMethod)
			: this(ownerMethod, attachToOwner: true)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(MethodMember ownerMethod, bool attachToOwner)
		{
			m_OwnerMethod = ownerMethod;

			if ( attachToOwner )
			{
				ownerMethod.AddWriter(this);
			}
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

		public Operand<T> Const<T>(T constantValue)
		{
			return new ConstantOperand<T>(constantValue);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public LocalOperand<T> Local<T>()
		{
			return new LocalOperand<T>(m_OwnerMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public LocalOperand<T> Local<T>(IOperand<T> initialValue)
		{
			var local = new LocalOperand<T>(m_OwnerMethod);
			local.Assign(initialValue);
			return local;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public LocalOperand<T> Local<T>(T initialValueConst)
		{
			return Local<T>(new ConstantOperand<T>(initialValueConst));
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

		public IHappilIfBody If(IOperand<bool> condition)
		{
			return OwnerMethod.AddStatement(new IfStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilIfBody If(IOperand<bool> condition, bool isTautology)
		{
			return OwnerMethod.AddStatement(new IfStatement(condition, isTautology));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> Iif<T>(IOperand<bool> condition, IOperand<T> onTrue, IOperand<T> onFalse)
		{
			return new TernaryConditionalOperator<T>(condition, onTrue, onFalse);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<T> Iif<T>(IOperand<bool> condition, bool isTautology, IOperand<T> onTrue, IOperand<T> onFalse)
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

				return (Operand<T>)onTrue;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilWhileSyntax While(IOperand<bool> condition)
		{
			return OwnerMethod.AddStatement(new WhileStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilDoWhileSyntax Do(Action<ILoopBody> body)
		{
			return OwnerMethod.AddStatement(new DoWhileStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(Operand<int> from, IOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(OwnerMethod, from, to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IOperand<int> from, Operand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(OwnerMethod, from.CastTo<int>(), to, increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IOperand<int> from, IOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(OwnerMethod, from.CastTo<int>(), to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForWhileSyntax For(Action precondition)
		{
			return OwnerMethod.AddStatement(new ForStatement(precondition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachInSyntax<T> Foreach<T>(LocalOperand<T> element)
		{
			return OwnerMethod.AddStatement(new ForeachStatement<T>(element));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachDoSyntax<T> ForeachElementIn<T>(IOperand<IEnumerable<T>> collection)
		{
			var element = this.Local<T>();
			var statement = new ForeachStatement<T>(element);

			OwnerMethod.AddStatement(statement);
			statement.In(collection);

			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilUsingSyntax Using(IOperand<IDisposable> disposable)
		{
			return OwnerMethod.AddStatement(new UsingStatement(disposable));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilLockSyntax Lock(IOperand<object> syncRoot, int millisecondsTimeout)
		{
			return OwnerMethod.AddStatement(new LockStatement(syncRoot, millisecondsTimeout));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilCatchSyntax Try(Action body)
		{
			return OwnerMethod.AddStatement(new TryStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilSwitchSyntax<T> Switch<T>(IOperand<T> value)
		{
			return OwnerMethod.AddStatement(new SwitchStatement<T>(value));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ThisOperand<TBase> This<TBase>()
		{
			return new ThisOperand<TBase>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RaiseEvent(string eventName, IOperand<EventArgs> eventArgs)
		{
			var eventMember = m_OwnerMethod.OwnerClass.GetMemberByName<EventMember>(eventName);

			using ( eventMember.CreateTypeTemplateScope() )
			{
				If(eventMember.BackingField.AsOperand<TypeTemplate.TEventHandler>() != Const<TypeTemplate.TEventHandler>(null)).Then(() => {
					eventMember.BackingField.AsOperand<TypeTemplate.TEventHandler>().Invoke(This<TypeTemplate.TBase>(), eventArgs);
				});
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand<TObject> New<TObject>(params IOperand[] constructorArguments)
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

		public IOperand<TElement[]> NewArray<TElement>(IOperand<int> length)
		{
			return new UnaryExpressionOperand<int, TElement[]>(
				@operator: new UnaryOperators.OperatorNewArray<TElement>(),
				operand: length);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand<TElement[]> NewArray<TElement>(params TElement[] constantValues)
		{
			return NewArray<TElement>(constantValues.Select(v => new ConstantOperand<TElement>(v)).ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IOperand<TElement[]> NewArray<TElement>(params IOperand<TElement>[] values)
		{
			var arrayLocal = Local<TElement[]>(NewArray<TElement>(Const(values.Length)));

			for ( int i = 0 ; i < values.Length ; i++ )
			{
				arrayLocal.ElementAt(i).Assign(values[i]);
			}

			return arrayLocal;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Throw<TException>(string message) where TException : Exception
		{
			OwnerMethod.AddStatement(new ThrowStatement(typeof(TException), message));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Throw()
		{
			OwnerMethod.AddStatement(new RethrowStatement());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TReturn>> Delegate<TArg1, TReturn>(Action<FunctionMethodWriter<TReturn>, Argument<TArg1>> body)
		{
			return new HappilAnonymousDelegate<TArg1, TReturn>(OwnerMethod.OwnerClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TReturn>> Delegate<TArg1, TReturn>(
			ref IHappilDelegate site,
			Action<FunctionMethodWriter<TReturn>,
			Argument<TArg1>> body)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Delegate(body);
			}

			return (HappilAnonymousDelegate<TArg1, TReturn>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TReturn>> Delegate<TArg1, TArg2, TReturn>(
			Action<FunctionMethodWriter<TReturn>, Argument<TArg1>, Argument<TArg2>> body)
		{
			return new HappilAnonymousDelegate<TArg1, TArg2, TReturn>(m_OwnerMethod.OwnerClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TReturn>> Delegate<TArg1, TArg2, TReturn>(
			ref IHappilDelegate site,
			Action<FunctionMethodWriter<TReturn>, Argument<TArg1>, Argument<TArg2>> body)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Delegate(body);
			}

			return (HappilAnonymousDelegate<TArg1, TArg2, TReturn>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TMethod> MakeDelegate<TTarget, TMethod>(IOperand<TTarget> target, Expression<Func<TTarget, TMethod>> methodSelector)
		{
			var method = Helpers.ResolveMethodFromLambda(methodSelector);
			return new HappilDelegate<TMethod>(target, method);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TResult>> Lambda<TArg1, TResult>(Func<Operand<TArg1>, IOperand<TResult>> expression)
		{
			return Delegate<TArg1, TResult>((m, arg1) => m.Return(expression(arg1)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TResult>> Lambda<TArg1, TResult>(
			ref IHappilDelegate site,
			Func<Operand<TArg1>, IOperand<TResult>> expression)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Lambda(expression);
			}

			return (HappilAnonymousDelegate<TArg1, TResult>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TResult>> Lambda<TArg1, TArg2, TResult>(
			Func<Operand<TArg1>, Operand<TArg2>, IOperand<TResult>> expression)
		{
			return Delegate<TArg1, TArg2, TResult>((m, arg1, arg2) => m.Return(expression(arg1, arg2)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TResult>> Lambda<TArg1, TArg2, TResult>(
			ref IHappilDelegate site,
			Func<Operand<TArg1>, Operand<TArg2>, IOperand<TResult>> expression)
		{
			if ( site == null )
			{
				site = (IHappilDelegate)Lambda(expression);
			}

			return (HappilAnonymousDelegate<TArg1, TArg2, TResult>)site;
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public void EmitFromLambda(Expression<Action> lambda)
		//{
		//	var callInfo = (MethodCallExpression)lambda.Body;
		//	var methodInfo = callInfo.Method;
		//	var arguments = callInfo.Arguments.Select(Helpers.GetLambdaArgumentAsConstant).ToArray();
		//	var happilExpression = new HappilUnaryExpression<object, object>(
		//		ownerMethod: this,
		//		@operator: new UnaryOperators.OperatorCall<object>(methodInfo, arguments),
		//		operand: null);

		//	//var arguments = new IHappilOperandInternals[callInfo.Arguments.Count];

		//	//for ( int i = 0 ; i < arguments.Length ; i++ )
		//	//{
		//	//	//var argument = callInfo.Arguments[i];


		//	//	//Expression<Func<object>> argumentLambda = Expression.Lambda<Func<object>>(argument);
		//	//	//var argumentValueFunc = argumentLambda.Compile();
		//	//	//var argumentValue = argumentValueFunc();

		//	//	arguments[i] = Helpers.GetLambdaArgumentAsConstant(callInfo.Arguments[i]);
		//	//}

		//	//m_HappilClass.CurrentScope.AddStatement();
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ForEachArgument(Action<Argument<TypeTemplate.TArgument>> action)
		{
			//TODO:redesign - does this method work correctly?
			var argumentTypes = OwnerMethod.Signature.ArgumentType;
			var indexBase = (OwnerMethod.Signature.IsStatic ? 0 : 1);

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

		public MethodMember OwnerMethod
		{
			get
			{
				return m_OwnerMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AttributeWriter ReturnAttributes
		{
			get
			{
				if ( m_ReturnAttributeWriter == null )
				{
					m_ReturnAttributeWriter = new AttributeWriter();
				}

				return m_ReturnAttributeWriter;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( m_OwnerMethod.MethodFactory.ReturnParameter != null && m_ReturnAttributeWriter != null )
			{
				foreach ( var attribute in m_ReturnAttributeWriter.GetAttributes() )
				{
					m_OwnerMethod.MethodFactory.ReturnParameter.SetCustomAttribute(attribute);
				}
			}

			base.Flush();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal void AddAttributes(Func<MethodMember, AttributeWriter> attributeWriterFactory)
		{
			if ( attributeWriterFactory != null )
			{
				AttributeWriter.Include(attributeWriterFactory(m_OwnerMethod));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void SetCustomAttribute(CustomAttributeBuilder attribute)
		{
			m_OwnerMethod.MethodFactory.SetAttribute(attribute);
		}
	}
}
