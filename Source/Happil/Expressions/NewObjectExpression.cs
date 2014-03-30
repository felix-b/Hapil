using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Happil.Operands;
using Happil.Statements;

namespace Happil.Expressions
{
	internal class NewObjectExpression<TObject> : ExpressionOperand<TObject>
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
