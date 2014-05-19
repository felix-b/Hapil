using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
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
		private readonly ClassType m_OwnerClass;
		private readonly MethodMember m_OwnerMethod;
		private MethodWriterModes m_Mode;
		private AttributeWriter m_ReturnAttributeWriter;
		private IMutableOperand m_ReturnValueLocal;
		private LabelStatement m_LeaveLabel;
		private MethodWriterBase[] m_InnerWriters;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(MethodMember ownerMethod)
			: this(ownerMethod, MethodWriterModes.Normal, attachToOwner: true)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(ClassType ownerClass)
			: this(null, MethodWriterModes.Normal, attachToOwner: true)
		{
			m_OwnerClass = ownerClass;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MethodWriterBase(MethodMember ownerMethod, MethodWriterModes mode, bool attachToOwner)
		{
			m_OwnerMethod = ownerMethod;
			m_Mode = mode;
			m_ReturnValueLocal = null;
			m_InnerWriters = null;

			if ( ownerMethod != null )
			{
				m_OwnerClass = ownerMethod.OwnerClass;

				if ( attachToOwner )
				{
					ownerMethod.AddWriter(this);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			ValidateNotAnonymousMethod();

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
			return new Constant<T>(constantValue);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Local<T> Local<T>()
		{
			return new Local<T>(m_OwnerMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Local<T> Local<T>(IOperand<T> initialValue)
		{
			var local = new Local<T>(m_OwnerMethod);
			local.Assign(initialValue);
			return local;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Local<T> Local<T>(T initialValueConst)
		{
			return Local<T>(new Constant<T>(initialValueConst));
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
			return AddStatement(new IfStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilIfBody If(IOperand<bool> condition, bool isTautology)
		{
			return AddStatement(new IfStatement(condition, isTautology));
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
			return AddStatement(new WhileStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilDoWhileSyntax Do(Action<ILoopBody> body)
		{
			return AddStatement(new DoWhileStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(Operand<int> from, IOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(this, from, to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IOperand<int> from, Operand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(this, from.CastTo<int>(), to, increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IOperand<int> from, IOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(this, from.CastTo<int>(), to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(Operand<int> from, Operand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(this, from, to, increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForWhileSyntax For(Action precondition)
		{
			return AddStatement(new ForStatement(precondition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachInSyntax<T> Foreach<T>(Local<T> element)
		{
			return AddStatement(new ForeachStatement<T>(element));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachDoSyntax<T> ForeachElementIn<T>(IOperand<IEnumerable<T>> collection)
		{
			var element = this.Local<T>();
			var statement = new ForeachStatement<T>(element);

			AddStatement(statement);
			statement.In(collection);

			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilUsingSyntax Using(IOperand<IDisposable> disposable)
		{
			return AddStatement(new UsingStatement(disposable));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilLockSyntax Lock(IOperand<object> syncRoot, int millisecondsTimeout)
		{
			return AddStatement(new LockStatement(syncRoot, millisecondsTimeout));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilCatchSyntax Try(Action body)
		{
			return AddStatement(new TryStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilSwitchSyntax<T> Switch<T>(IOperand<T> value)
		{
			return AddStatement(new SwitchStatement<T>(value));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ThisOperand<TBase> This<TBase>()
		{
			return new ThisOperand<TBase>(OwnerClass);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RaiseEvent(string eventName, IOperand<EventArgs> eventArgs)
		{
			var eventMember = m_OwnerClass.GetMemberByName<EventMember>(eventName);

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
			return NewArray<TElement>(constantValues.Select(v => new Constant<TElement>(v)).ToArray());
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
			AddStatement(new ThrowStatement(typeof(TException), message));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Throw()
		{
			AddStatement(new RethrowStatement());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TReturn>> Delegate<TArg1, TReturn>(Action<FunctionMethodWriter<TReturn>, Argument<TArg1>> body)
		{
			return new AnonymousFuncOperand<TArg1, TReturn>(OwnerClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TReturn>> Delegate<TArg1, TReturn>(
			ref IDelegateOperand site,
			Action<FunctionMethodWriter<TReturn>,
			Argument<TArg1>> body)
		{
			if ( site == null )
			{
				site = (IDelegateOperand)Delegate(body);
			}

			return (AnonymousFuncOperand<TArg1, TReturn>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TReturn>> Delegate<TArg1, TArg2, TReturn>(
			Action<FunctionMethodWriter<TReturn>, Argument<TArg1>, Argument<TArg2>> body)
		{
			return new AnonymousFuncOperand<TArg1, TArg2, TReturn>(m_OwnerClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TReturn>> Delegate<TArg1, TArg2, TReturn>(
			ref IDelegateOperand site,
			Action<FunctionMethodWriter<TReturn>, Argument<TArg1>, Argument<TArg2>> body)
		{
			if ( site == null )
			{
				site = (IDelegateOperand)Delegate(body);
			}

			return (AnonymousFuncOperand<TArg1, TArg2, TReturn>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TDelegate> MakeDelegate<TTarget, TDelegate>(IOperand<TTarget> target, Expression<Func<TTarget, TDelegate>> methodSelector)
		{
			var method = Helpers.ResolveMethodFromLambda(methodSelector);
			return new DelegateOperand<TDelegate>(target, method);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TMethod> MakeDelegate<TTarget, TMethod, TDelegate>(IOperand<TTarget> target, Expression<Func<TTarget, TMethod>> methodSelector)
		{
			var method = Helpers.ResolveMethodFromLambda(methodSelector);
			return new DelegateOperand<TMethod>(target, method, delegateTypeOverride: TypeTemplate.Resolve<TDelegate>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TResult>> Lambda<TArg1, TResult>(Func<Operand<TArg1>, IOperand<TResult>> expression)
		{
			return Delegate<TArg1, TResult>((m, arg1) => m.Return(expression(arg1)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TResult>> Lambda<TArg1, TResult>(
			ref IDelegateOperand site,
			Func<Operand<TArg1>, IOperand<TResult>> expression)
		{
			if ( site == null )
			{
				site = (IDelegateOperand)Lambda(expression);
			}

			return (AnonymousFuncOperand<TArg1, TResult>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TResult>> Lambda<TArg1, TArg2, TResult>(
			Func<Operand<TArg1>, Operand<TArg2>, IOperand<TResult>> expression)
		{
			return Delegate<TArg1, TArg2, TResult>((m, arg1, arg2) => m.Return(expression(arg1, arg2)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TArg1, TArg2, TResult>> Lambda<TArg1, TArg2, TResult>(
			ref IDelegateOperand site,
			Func<Operand<TArg1>, Operand<TArg2>, IOperand<TResult>> expression)
		{
			if ( site == null )
			{
				site = (IDelegateOperand)Lambda(expression);
			}

			return (AnonymousFuncOperand<TArg1, TArg2, TResult>)site;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<IntPtr> MethodOf(MethodInfo method)
		{
			return new MethodOf(method);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<IntPtr> MethodOf(MethodMember method)
		{
			return new MethodOf(method);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<IntPtr> MethodOf<TTarget, TMethod>(IOperand<TTarget> target, Expression<Func<TTarget, TMethod>> methodSelector)
		{
			var method = Helpers.ResolveMethodFromLambda(methodSelector);
			return new MethodOf(method);
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
			ValidateNotAnonymousMethod();

			//TODO: refactor to reuse the overloaded method
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

		//TODO: refactor to reuse the overloaded method
		public void ForEachArgument(Action<Argument<TypeTemplate.TArgument>, int> action)
		{
			ValidateNotAnonymousMethod();

			var argumentTypes = OwnerMethod.Signature.ArgumentType;

			for ( int index = 0 ; index < argumentTypes.Length ; index++ )
			{
				using ( TypeTemplate.CreateScope<TypeTemplate.TArgument>(argumentTypes[index]) )
				{
					var argument = this.Argument<TypeTemplate.TArgument>(position: index + 1);
					action(argument, index);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TResult> Proceed<TResult>()
		{
			ValidateNotAnonymousMethod();

			if ( !m_Mode.HasFlag(MethodWriterModes.Decorator) )
			{
				throw new InvalidOperationException("Proceed is only allowed in the Decorator mode.");
			}

			var returnValueLocal = (m_OwnerMethod.Signature.IsVoid ? null : m_OwnerMethod.AddLocal<TResult>());
			AddStatement(new ProceedStatement(m_OwnerMethod, m_InnerWriters, returnValueLocal));

			return returnValueLocal;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<TResult> PropagateCall<TResult>(IOperand target)
		{
			ValidateNotAnonymousMethod();

			Local<TResult> returnValueLocal = null;

			if ( !OwnerMethod.IsVoid )
			{
				returnValueLocal = Local<TResult>();
			}

			StatementScope.Current.AddStatement(new PropagateCallStatement(OwnerMethod, target, returnValueLocal));

			if ( !OwnerMethod.IsVoid )
			{
				AddReturnStatement<TResult>(returnValueLocal);
			}

			return returnValueLocal;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RawIL(Action<ILGenerator> script)
		{
			StatementScope.Current.AddStatement(new RawILStatement(script));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType OwnerClass
		{
			get
			{
				return m_OwnerClass;
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

		public MethodWriterModes Mode
		{
			get
			{
				return m_Mode;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsDecorator
		{
			get
			{
				return m_Mode.HasFlag(MethodWriterModes.Decorator);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsDecorated
		{
			get
			{
				return m_Mode.HasFlag(MethodWriterModes.Decorated);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			WriteReturnAttributesIfAny();
			base.Flush();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal void AddAttributes(Func<MethodMember, AttributeWriter> attributeWriterFactory)
		{
			ValidateNotAnonymousMethod();

			if ( attributeWriterFactory != null )
			{
				AttributeWriter.Include(attributeWriterFactory(m_OwnerMethod));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void SetCustomAttribute(CustomAttributeBuilder attribute)
		{
			ValidateNotAnonymousMethod();
			m_OwnerMethod.MethodFactory.SetAttribute(attribute);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TStatement AddStatement<TStatement>(TStatement statement) where TStatement : StatementBase
		{
			StatementScope.Current.AddStatement(statement);
			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal void AddReturnStatement()
		{
			if ( IsDecorated )
			{
				StatementScope.Current.AddStatement(new GotoStatement(m_LeaveLabel));
			}
			else
			{
				StatementScope.Current.AddStatement(new ReturnStatement());
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal void AddReturnStatement<TReturn>(IOperand<TReturn> returnValue)
		{
			if ( IsDecorated )
			{
				m_ReturnValueLocal.Assign(returnValue);
				StatementScope.Current.AddStatement(new GotoStatement(m_LeaveLabel));
			}
			else
			{
				StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(returnValue));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void SetupDecoratedMode(IMutableOperand returnValueLocal, LabelStatement leaveLabel)
		{
			m_Mode |= MethodWriterModes.Decorated;
			m_ReturnValueLocal = returnValueLocal;
			m_LeaveLabel = leaveLabel;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void SetupDecoratorMode(MethodWriterBase[] innerWriters)
		{
			m_Mode |= MethodWriterModes.Decorator;
			m_InnerWriters = innerWriters;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected MethodWriterBase[] InnerWriters
		{
			get
			{
				return m_InnerWriters;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void WriteReturnAttributesIfAny()
		{
			if ( m_ReturnAttributeWriter == null )
			{
				return;
			}

			ValidateNotAnonymousMethod();

			if ( m_OwnerMethod.MethodFactory.ReturnParameter != null )
			{
				foreach ( var attribute in m_ReturnAttributeWriter.GetAttributes() )
				{
					m_OwnerMethod.MethodFactory.ReturnParameter.SetCustomAttribute(attribute);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
	
		private void ValidateNotAnonymousMethod()
		{
			if ( m_OwnerMethod == null )
			{
				throw new InvalidOperationException("Requested operation is not available within anonymous delegate or lambda expression.");
			}
		}
	}
}
