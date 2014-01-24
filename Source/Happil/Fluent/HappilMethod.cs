using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Happil.Expressions;
using Happil.Selectors;
using Happil.Statements;

namespace Happil.Fluent
{
	internal class HappilMethod : IHappilMethodBodyTemplate, IHappilMember
	{
		private readonly HappilClass m_HappilClass;
		private readonly MethodBuilder m_MethodBuilder;
		private readonly MethodInfo m_Declaration;
		private readonly List<IHappilStatement> m_Statements;
		private readonly Type[] m_ArgumentTypes;
		private Type[] m_TemplateActualTypePairs = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilMethod(HappilClass happilClass, MethodInfo declaration)
			: this(happilClass)
		{
			m_Declaration = declaration;
			m_MethodBuilder = happilClass.TypeBuilder.DefineMethod(
				happilClass.TakeMemberName(declaration.Name),
				GetMethodAttributesFor(declaration),
				declaration.ReturnType, 
				declaration.GetParameters().Select(p => p.ParameterType).ToArray());

			happilClass.TypeBuilder.DefineMethodOverride(m_MethodBuilder, declaration);
			m_ArgumentTypes = declaration.GetParameters().Select(p => p.ParameterType).ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected HappilMethod(HappilClass happilClass)
		{
			m_HappilClass = happilClass;
			m_Statements = new List<IHappilStatement>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMethodBodyBase Members

		public IHappilIfBody If(IHappilOperand<bool> condition)
		{
			return AddStatement(new IfStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<T> Iif<T>(IHappilOperand<bool> condition, IHappilOperand<T> onTrue, IHappilOperand<T> onFalse)
		{
			return new TernaryConditionalOperator<T>(condition, onTrue, onFalse);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilWhileSyntax While(IHappilOperand<bool> condition)
		{
			return AddStatement(new WhileStatement(condition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(HappilConstant<int> from, IHappilOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(this, from, to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IHappilOperand<int> from, HappilConstant<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(this, from.CastTo<int>(), to, increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilForShortSyntax For(IHappilOperand<int> from, IHappilOperand<int> to, int increment = 1)
		{
			return new HappilForShortSyntax(this, from.CastTo<int>(), to.CastTo<int>(), increment);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForWhileSyntax For(Action precondition)
		{
			return AddStatement(new ForStatement(precondition));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachInSyntax<T> Foreach<T>(HappilLocal<T> element)
		{
			return AddStatement(new ForeachStatement<T>(element));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilForeachDoSyntax<T> ForeachElementIn<T>(IHappilOperand<IEnumerable<T>> collection)
		{
			var element = this.Local<T>();
			var statement = new ForeachStatement<T>(element);
			
			AddStatement(statement);
			statement.In(collection);

			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		public IHappilUsingSyntax Using(IHappilOperand<IDisposable> disposable)
		{
			return AddStatement(new UsingStatement(disposable));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilLockSyntax Lock(IHappilOperand<object> syncRoot, int millisecondsTimeout)
		{
			return AddStatement(new LockStatement(syncRoot, millisecondsTimeout));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilCatchSyntax Try(Action body)
		{
			return AddStatement(new TryStatement(body));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilSwitchSyntax<T> Switch<T>(IHappilOperand<T> value)
		{
			return AddStatement(new SwitchStatement<T>(value));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<TBase> This<TBase>()
		{
			return new HappilThis<TBase>(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilLocal<T> Local<T>()
		{
			return new HappilLocal<T>(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilLocal<T> Local<T>(IHappilOperand<T> initialValue)
		{
			var local = new HappilLocal<T>(this);
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

		public HappilConstant<T> Default<T>()
		{
			return new HappilConstant<T>(default(T));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilConstant<object> DefaultOf(Type type)
		{
			return new HappilConstant<object>(type.GetDefaultValue());
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

		public void EmitFromLambda(Expression<Action> lambda)
		{
			var callInfo = (MethodCallExpression)lambda.Body;
			var methodInfo = callInfo.Method;
			var arguments = callInfo.Arguments.Select(Helpers.GetLambdaArgumentAsConstant).ToArray();
			var happilExpression = new HappilUnaryExpression<object, object>(
				ownerMethod: this, 
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

		public HappilArgument<T> Argument<T>(string name)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilArgument<T> Argument<T>(int index)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual MethodInfo MethodInfo
		{
			get
			{
				if ( m_Declaration != null )
				{
					return (MethodInfo)m_MethodBuilder;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual MethodBuilder MethodBuilder
		{
			get
			{
				if ( m_Declaration != null )
				{
					return m_MethodBuilder;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual int ArgumentCount
		{
			get
			{
				if ( m_Declaration != null )
				{
					return m_Declaration.GetParameters().Length;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual Type ReturnType
		{
			get
			{
				if ( m_Declaration != null )
				{
					return m_Declaration.ReturnType;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMethodBodyTemplate Members

		public void Return(IHappilOperand<TypeTemplate.TReturn> operand)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<TypeTemplate.TReturn>(operand));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Return()
		{
			StatementScope.Current.AddStatement(new ReturnStatement());
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

		void IHappilMember.EmitBody()
		{
			var il = GetILGenerator();

			foreach ( var statement in m_Statements )
			{
				statement.Emit(il);
			}

			if ( IsVoidMethod() )
			{
				il.Emit(OpCodes.Ret);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		IDisposable IHappilMember.CreateTypeTemplateScope()
		{
			if ( m_TemplateActualTypePairs == null )
			{
				m_TemplateActualTypePairs = BuildTemplateActualTypePairs();
			}

			return TypeTemplate.CreateScope(m_TemplateActualTypePairs);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		string IHappilMember.Name
		{
			get
			{
				return GetName();
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			var text = new StringBuilder();
			text.Append("{");

			foreach ( var statement in m_Statements )
			{
				text.Append(statement.ToString() + ";");
			}

			text.Append("}");
			return text.ToString();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope CreateBodyScope()
		{
			return new StatementScope(m_HappilClass, this, m_Statements);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass HappilClass
		{
			get
			{
				return m_HappilClass;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual ILGenerator GetILGenerator()
		{
			return m_MethodBuilder.GetILGenerator();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected virtual Type GetReturnType()
		{
			return m_MethodBuilder.ReturnType;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected virtual Type[] GetArgumentTypes()
		{
			return m_ArgumentTypes;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual string GetName()
		{
			return m_MethodBuilder.Name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual Type[] BuildTemplateActualTypePairs()
		{
			var parameterTypes = m_Declaration.GetParameters().Select(p => p.ParameterType).ToArray();
			var pairs = new Type[(1 + parameterTypes.Length) * 2];

			pairs[0] = typeof(TypeTemplate.TReturn);
			pairs[1] = m_MethodBuilder.ReturnType;

			TypeTemplate.BuildArgumentsTypePairs(parameterTypes, pairs, arrayStartIndex: 2);

			return pairs;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected bool IsVoidMethod()
		{
			var returnType = GetReturnType();
			return (returnType == null || returnType == typeof(void));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TStatement AddStatement<TStatement>(TStatement statement) where TStatement : IHappilStatement
		{
			StatementScope.Current.AddStatement(statement);
			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static MethodAttributes GetMethodAttributesFor(MethodInfo declaration)
		{
			var attributes =
				MethodAttributes.Final |
				MethodAttributes.HideBySig |
				MethodAttributes.Public |
				MethodAttributes.Virtual;

			if ( declaration.DeclaringType.IsInterface )
			{
				attributes |= MethodAttributes.NewSlot;
			}

			return attributes;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class HappilMethod<TReturn> : HappilMethod, IHappilMethodBody<TReturn>
	{
		public HappilMethod(HappilClass happilClass, MethodInfo declaration)
			: base(happilClass, declaration)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMethodBody<TReturn> Members

		public void Return(IHappilOperand<TReturn> operand)
		{
			//TODO: verify that current scope belongs to this method
			StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(operand.OrNullConstant()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ReturnConst(TReturn constantValue)
		{
			//TODO: verify that current scope belongs to this method
			StatementScope.Current.AddStatement(new ReturnStatement<TReturn>(new HappilConstant<TReturn>(constantValue)));
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class VoidHappilMethod : HappilMethod, IVoidHappilMethodBody
	{
		public VoidHappilMethod(HappilClass happilClass, MethodInfo declaration)
			: base(happilClass, declaration)
		{
		}
	}
}
