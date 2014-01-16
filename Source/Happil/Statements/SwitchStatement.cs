﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class SwitchStatement<T> : IHappilStatement, IHappilSwitchSyntax<T>
	{
		private readonly IHappilOperand<T> m_Value;
		private readonly SortedDictionary<T, CaseBlock> m_CasesByValue;
		private CaseBlock m_Default;
		private Label m_DefaultLabel;
		private Label m_EndLabel;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public SwitchStatement(IHappilOperand<T> value)
		{
			m_Value = value;
			m_CasesByValue = new SortedDictionary<T, CaseBlock>();
			m_Default = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			m_EndLabel = il.DefineLabel();
			m_DefaultLabel = (m_Default != null ? il.DefineLabel() : m_EndLabel);

			foreach ( var block in m_CasesByValue.Values )
			{
				block.Label = il.DefineLabel();
			}

			if ( !CanEmitJumpTable() || !TryEmitJumpTable(il) )
			{
				throw new NotImplementedException("Switch without jump table is not yet implemented");
			}

			il.MarkLabel(m_EndLabel);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilSwitchSyntax<T> Members

		public IHappilSwitchCaseSyntax<T> Case(T constantValue)
		{
			if ( m_CasesByValue.ContainsKey(constantValue) )
			{
				throw new ArgumentException("There is already case for specified value", paramName: "constantValue");
			}

			var block = new CaseBlock(this, constantValue);
			m_CasesByValue.Add(constantValue, block);

			return block;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Default(Action body)
		{
			if ( m_Default != null )
			{
				throw new InvalidOperationException("Default case was already specified.");
			}

			m_Default = new CaseBlock(this, default(T));
			m_Default.Do(body);
		}

		#endregion

		private bool CanEmitJumpTable()
		{
			return (typeof(T).IsPrimitive || typeof(T).IsEnum);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private bool TryEmitJumpTable(ILGenerator il)
		{
			var caseBlocks = m_CasesByValue.Values.ToArray();
			var jumpTableLength = caseBlocks[caseBlocks.Length - 1].Int64 - caseBlocks[0].Int64 + 1;

			if ( jumpTableLength > caseBlocks.Length * 2 + 1 || jumpTableLength >= UInt32.MaxValue )
			{
				return false;
			}

			var jumpTable = new CaseBlock[jumpTableLength];
			var adjustment = -caseBlocks[0].Int64;
			var caseIndex = 0;

			for ( int jumpIndex = 0 ; jumpIndex < jumpTableLength ; jumpIndex++ )
			{
				if ( jumpIndex == caseBlocks[caseIndex].Int64 + adjustment )
				{
					jumpTable[jumpIndex] = caseBlocks[caseIndex++];
				}
			}

			m_Value.EmitTarget(il);
			m_Value.EmitLoad(il);

			if ( adjustment != 0 )
			{
				new HappilConstant<int>(Math.Abs((int)adjustment)).EmitLoad(il);
				il.Emit(adjustment < 0 ? OpCodes.Sub : OpCodes.Add);
			}

			var jumpTableLabels = jumpTable.Select(b => b != null ? b.Label : m_DefaultLabel).ToArray();
			il.Emit(OpCodes.Switch, jumpTableLabels);

			if ( m_Default != null )
			{
				il.MarkLabel(m_DefaultLabel);
				m_Default.Emit(il, m_EndLabel);
			}
			else
			{
				il.Emit(OpCodes.Br, m_EndLabel);
			}

			for ( int i = 0 ; i < caseBlocks.Length ; i++ )
			{
				il.MarkLabel(caseBlocks[i].Label);
				caseBlocks[i].Emit(il, m_EndLabel);
			}

			return true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class CaseBlock : IHappilSwitchCaseSyntax<T>
		{
			private readonly SwitchStatement<T> m_OwnerStatement;
			private readonly T m_Value;
			private readonly List<IHappilStatement> m_Body;
			private readonly long m_Int64;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public CaseBlock(SwitchStatement<T> ownerStatement, T value)
			{
				m_OwnerStatement = ownerStatement;
				m_Value = value;
				m_Body = new List<IHappilStatement>();

				if ( typeof(T).IsPrimitive || typeof(T).IsEnum )
				{
					m_Int64 = (long)Convert.ChangeType(value, typeof(long));
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilSwitchCaseSyntax<T> Members

			public IHappilSwitchSyntax<T> Do(Action body)
			{
				using ( new StatementScope(m_Body) )
				{
					body();
				}

				return m_OwnerStatement;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public virtual void Emit(ILGenerator il, Label endLabel)
			{
				foreach ( var statement in m_Body )
				{
					statement.Emit(il);
				}

				il.Emit(OpCodes.Br, endLabel);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T Value
			{
				get { return m_Value; }
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public long Int64
			{
				get
				{
					return m_Int64;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Label Label { get; set; }
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilSwitchSyntax<T>
	{
		IHappilSwitchCaseSyntax<T> Case(T constantValue);
		void Default(Action body);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilSwitchCaseSyntax<T>
	{
		IHappilSwitchSyntax<T> Do(Action body);
	}
}
