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
			var statement = new IfStatement(condition);
			StatementScope.Current.AddStatement(statement);
			return statement;
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

		public HappilConstant<object> Default(Type type)
		{
			return new HappilConstant<object>(type.GetDefaultValue());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Throw<TException>(string message) where TException : Exception
		{
			throw new NotImplementedException();
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

		public void Return(IHappilOperand<TypeTemplate> operand)
		{
			StatementScope.Current.AddStatement(new ReturnStatement<object>(operand));
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

		protected virtual Type GetReturnType()
		{
			return m_MethodBuilder.ReturnType;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual string GetName()
		{
			return m_MethodBuilder.Name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected bool IsVoidMethod()
		{
			var returnType = GetReturnType();
			return (returnType == null || returnType == typeof(void));
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
