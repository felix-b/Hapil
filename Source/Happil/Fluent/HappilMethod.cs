using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Statements;

namespace Happil.Fluent
{
	internal class HappilMethod : IHappilMethodBody, IHappilMember
	{
		private readonly HappilClass m_HappilClass;
		private readonly MethodBuilder m_MethodBuilder;
		private readonly List<IHappilStatement> m_Statements;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilMethod(HappilClass happilClass, MethodInfo declaration)
		{
			m_HappilClass = happilClass;
			m_MethodBuilder = happilClass.TypeBuilder.DefineMethod(
				happilClass.TakeMemberName(declaration.Name),
				GetMethodAttributesFor(declaration));
			m_Statements = new List<IHappilStatement>();

			//TODO: copy parameters and return type definition from declaration
			
			happilClass.TypeBuilder.DefineMethodOverride(m_MethodBuilder, declaration);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMethodBodyBase Members

		public HappilLocal<T> Local<T>(string name)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilLocal<T> Local<T>(string name, HappilOperand<T> initialValue)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Throw<TException>(string message) where TException : Exception
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void EmitFromLambda(Expression<Action> lambda)
		{
			throw new NotImplementedException();
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

		public MethodInfo MethodInfo
		{
			get { throw new NotImplementedException(); }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int ArgumentCount
		{
			get { throw new NotImplementedException(); }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type ReturnType
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMethodBody Members

		public void Return(IHappilOperand<object> operand)
		{
			throw new NotImplementedException();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

		void IHappilMember.EmitBody()
		{
			var il = m_MethodBuilder.GetILGenerator();

			foreach ( var statement in m_Statements )
			{
				statement.Emit(il);
			}

			if ( m_MethodBuilder.ReturnType == null )
			{
				il.Emit(OpCodes.Ret);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		string IHappilMember.Name
		{
			get
			{
				return m_MethodBuilder.Name;
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
			throw new NotImplementedException();
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IVoidHappilMethodBody Members

		public void Return()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
