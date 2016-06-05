using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Statements;
using TT = Hapil.TypeTemplate;

namespace Hapil.Writers
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

		public IHapilIfBody If(IOperand<bool> condition)
		{
			return AddStatement(new IfStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilIfBody If(IOperand<bool> condition, bool isTautology)
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

		public IHapilWhileSyntax While(IOperand<bool> condition)
		{
			return AddStatement(new WhileStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilDoWhileSyntax Do(Action<ILoopBody> body)
		{
			return AddStatement(new DoWhileStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HapilForShortSyntax For(Operand<int> from, IOperand<int> to, int increment = 1)
		{
			return new HapilForShortSyntax(this, from, to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HapilForShortSyntax For(IOperand<int> from, Operand<int> to, int increment = 1)
		{
			return new HapilForShortSyntax(this, from.CastTo<int>(), to, increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HapilForShortSyntax For(IOperand<int> from, IOperand<int> to, int increment = 1)
		{
			return new HapilForShortSyntax(this, from.CastTo<int>(), to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HapilForShortSyntax For(Operand<int> from, Operand<int> to, int increment = 1)
		{
			return new HapilForShortSyntax(this, from, to, increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilForWhileSyntax For(Action precondition)
		{
			return AddStatement(new ForStatement(precondition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilForeachInSyntax<T> Foreach<T>(Local<T> element)
		{
			return AddStatement(new ForeachStatement<T>(element));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilForeachDoSyntax<T> ForeachElementIn<T>(IOperand<IEnumerable<T>> collection)
		{
			var element = this.Local<T>();
			var statement = new ForeachStatement<T>(element);

			AddStatement(statement);
			statement.In(collection);

			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilUsingSyntax Using(IOperand<IDisposable> disposable)
		{
			return AddStatement(new UsingStatement(disposable));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilLockSyntax Lock(IOperand<object> syncRoot, int millisecondsTimeout)
		{
			return AddStatement(new LockStatement(syncRoot, millisecondsTimeout));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilCatchSyntax Try(Action body)
		{
			return AddStatement(new TryStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHapilSwitchSyntax<T> Switch<T>(IOperand<T> value)
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

		public Operand<Action> Delegate(Action<VoidMethodWriter> body)
		{
			return new AnonymousActionOperand(OwnerClass, body);
		}
		public Operand<Action<TA1>> Delegate<TA1>(Action<VoidMethodWriter, Argument<TA1>> body)
		{
			return new AnonymousActionOperand<TA1>(OwnerClass, body);
		}
		public Operand<Action<TA1, TA2>> Delegate<TA1, TA2>(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
		{
			return new AnonymousActionOperand<TA1, TA2>(m_OwnerClass, body);
		}
		public Operand<Action<TA1, TA2, TA3>> Delegate<TA1, TA2, TA3>(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
		{
			return new AnonymousActionOperand<TA1, TA2, TA3>(m_OwnerClass, body);
		}
		public Operand<Action<TA1, TA2, TA3, TA4>> Delegate<TA1, TA2, TA3, TA4>(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
		{
			return new AnonymousActionOperand<TA1, TA2, TA3, TA4>(m_OwnerClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TReturn>> Delegate<TReturn>(Action<FunctionMethodWriter<TReturn>> body)
		{
			return new AnonymousFuncOperand<TReturn>(OwnerClass, body);
		}
		public Operand<Func<TA1, TReturn>> Delegate<TA1, TReturn>(Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
		{
			return new AnonymousFuncOperand<TA1, TReturn>(OwnerClass, body);
		}
		public Operand<Func<TA1, TA2, TReturn>> Delegate<TA1, TA2, TReturn>(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body)
		{
			return new AnonymousFuncOperand<TA1, TA2, TReturn>(m_OwnerClass, body);
		}
		public Operand<Func<TA1, TA2, TA3, TReturn>> Delegate<TA1, TA2, TA3, TReturn>(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
		{
			return new AnonymousFuncOperand<TA1, TA2, TA3, TReturn>(m_OwnerClass, body);
		}
		public Operand<Func<TA1, TA2, TA3, TA4, TReturn>> Delegate<TA1, TA2, TA3, TA4, TReturn>(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
		{
			return new AnonymousFuncOperand<TA1, TA2, TA3, TA4, TReturn>(m_OwnerClass, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Operand<Func<TResult>> Lambda<TResult>(Func<IOperand<TResult>> expression)
		{
			return Delegate<TResult>(w => w.Return(expression()));
		}
		public Operand<Func<TA1, TResult>> Lambda<TA1, TResult>(Func<Operand<TA1>, IOperand<TResult>> expression)
		{
			return Delegate<TA1, TResult>((w, arg1) => w.Return(expression(arg1)));
		}
		public Operand<Func<TA1, TA2, TResult>> Lambda<TA1, TA2, TResult>(Func<Operand<TA1>, Operand<TA2>, IOperand<TResult>> expression)
		{
			return Delegate<TA1, TA2, TResult>((w, arg1, arg2) => w.Return(expression(arg1, arg2)));
		}
		public Operand<Func<TA1, TA2, TA3, TResult>> Lambda<TA1, TA2, TA3, TResult>(Func<Operand<TA1>, Operand<TA2>, Operand<TA3>, IOperand<TResult>> expression)
		{
			return Delegate<TA1, TA2, TA3, TResult>((w, arg1, arg2, arg3) => w.Return(expression(arg1, arg2, arg3)));
		}
		public Operand<Func<TA1, TA2, TA3, TA4, TResult>> Lambda<TA1, TA2, TA3, TA4, TResult>(Func<Operand<TA1>, Operand<TA2>, Operand<TA3>, Operand<TA4>, IOperand<TResult>> expression)
		{
			return Delegate<TA1, TA2, TA3, TA4, TResult>((w, arg1, arg2, arg3, arg4) => w.Return(expression(arg1, arg2, arg3, arg4)));
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
		//	var happilExpression = new HapilUnaryExpression<object, object>(
		//		ownerMethod: this,
		//		@operator: new UnaryOperators.OperatorCall<object>(methodInfo, arguments),
		//		operand: null);

		//	//var arguments = new IHapilOperandInternals[callInfo.Arguments.Count];

		//	//for ( int i = 0 ; i < arguments.Length ; i++ )
		//	//{
		//	//	//var argument = callInfo.Arguments[i];


		//	//	//Expression<Func<object>> argumentLambda = Expression.Lambda<Func<object>>(argument);
		//	//	//var argumentValueFunc = argumentLambda.Compile();
		//	//	//var argumentValue = argumentValueFunc();

		//	//	arguments[i] = Helpers.GetLambdaArgumentAsConstant(callInfo.Arguments[i]);
		//	//}

		//	//m_HapilClass.CurrentScope.AddStatement();
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

        public void ProceedToBase()
        {
            var arguments = new IOperand[this.OwnerMethod.Signature.ArgumentCount];

            this.ForEachArgument((arg, index) => {
                arguments[index] = arg;
            });

            if (this.OwnerMethod.IsVoid)
            {
                InternalBase(arguments);
            }
            else
            {
                AddReturnStatement(InternalBase(arguments));
            }
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

        protected MethodInfo GetValidBaseMethod(IOperand[] arguments)
        {
            var baseMethod = OwnerMethod.MethodDeclaration;

            if (baseMethod == null || baseMethod.IsAbstract || baseMethod.DeclaringType == null || baseMethod.DeclaringType.IsInterface)
            {
                throw new InvalidOperationException("Current method has no base which can be invoked.");
            }

            if (arguments.Length != OwnerMethod.Signature.ArgumentCount)
            {
                throw new ArgumentException("Number of arguments does not match method signature.");
            }

            for (int i = 0 ; i < OwnerMethod.Signature.ArgumentCount ; i++)
            {
                var parameterType = GetValidatableType(OwnerMethod.Signature.ArgumentUnderlyingType[i]);
                var argumentType = GetValidatableType(arguments[i].OperandType);

                if (!parameterType.IsAssignableFrom(argumentType))
                {
                    throw new ArgumentException(string.Format(
                        "Argument at index {0}: argument of type '{1}' cannot be assigned to parameter of type '{2}'.", 
                        i, 
                        argumentType.FriendlyName(),
                        parameterType.FriendlyName()));
                }
            }

            return baseMethod;
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

	    protected internal IOperand<TT.TReturn> InternalBase(params IOperand[] arguments)
        {
            var baseMethod = GetValidBaseMethod(arguments);

            using (TT.CreateScope<TT.TBase>(baseMethod.DeclaringType))
            {
                if (baseMethod.IsVoid())
                {
                    AddStatement(new CallStatement(This<TT.TBase>(), baseMethod, disableVirtual: true, arguments: arguments));
                    return null;
                }
                else
                {
                    var @operator = new UnaryOperators.OperatorCall<TT.TBase>(baseMethod, disableVirtual: true, arguments: arguments);
                    return new UnaryExpressionOperand<TT.TBase, TT.TReturn>(@operator, This<TT.TBase>());
                }
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

	    private Type GetValidatableType(Type type)
	    {
	        var resolvedType = TypeTemplate.Resolve(type);
            var nonRefType = (resolvedType.IsByRef ? resolvedType.GetElementType() : resolvedType);
            var resolvedNonRefType = TypeTemplate.Resolve(nonRefType);

            return resolvedNonRefType;
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
