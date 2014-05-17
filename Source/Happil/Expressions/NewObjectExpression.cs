using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Expressions
{
	internal class NewObjectExpression<TObject> : ExpressionOperand<TObject>, IAcceptOperandVisitor
	{
		private readonly Type m_ObjectType;
		private readonly ConstructorInfo m_Constructor;
		private readonly IOperand[] m_ConstructorArguments;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public NewObjectExpression(IOperand[] constructorArguments)
		{
			m_ObjectType = TypeTemplate.Resolve<TObject>();
			m_ConstructorArguments = constructorArguments;

			var argumentTypes = constructorArguments.Select(arg => arg.OperandType).ToArray();
			m_Constructor = m_ObjectType.GetConstructor(argumentTypes); //TODO: use MemberTypeCache for this

			if ( m_Constructor == null )
			{
				throw new ArgumentException("Could not find constructor with specified argument types.");
			}

			var scope = StatementScope.Current;

			foreach ( var argument in constructorArguments.Reverse() )
			{
				scope.Consume(argument);
			}

			scope.RegisterExpressionStatement(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public NewObjectExpression(ConstructorInfo constructor, IOperand[] constructorArguments)
		{
			m_ObjectType = TypeTemplate.Resolve<TObject>();
			m_ConstructorArguments = constructorArguments;
			m_Constructor = constructor;

			var scope = StatementScope.Current;

			foreach ( var argument in constructorArguments.Reverse() )
			{
				scope.Consume(argument);
			}

			scope.RegisterExpressionStatement(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAcceptOperandVisitor Members

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitOperandArray(m_ConstructorArguments);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format(
				"NewObj[{0}]({1})", 
				m_ObjectType.FriendlyName(), 
				string.Join(",", m_ConstructorArguments.Select(a => a.ToString())));
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.NewObject;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// no target
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			foreach ( var argument in m_ConstructorArguments )
			{
				argument.EmitTarget(il);
				argument.EmitLoad(il);
			}

			il.Emit(OpCodes.Newobj, m_Constructor);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotSupportedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotSupportedException();
		}
	}
}
