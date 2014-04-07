using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Members
{
	public class MethodMember : MemberBase
	{
		private readonly MethodFactoryBase m_MethodFactory;
		private readonly List<MethodWriterBase> m_Writers;
		private readonly List<StatementBase> m_Statements;
		private readonly TransparentMethodWriter m_TransparentWriter;
		private Type[] m_CachedTemplateTypePairs = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal MethodMember(ClassType ownerClass, MethodFactoryBase methodFactory)
			: base(ownerClass, methodFactory.MemberName)
		{
			m_MethodFactory = methodFactory;
			m_Writers = new List<MethodWriterBase>();
			m_Statements = new List<StatementBase>();
			m_TransparentWriter = new TransparentMethodWriter(this);
		}

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

		public LocalOperand<T> AddLocal<T>()
		{
			return new LocalOperand<T>(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberInfo MemberDeclaration
		{
			get
			{
				return m_MethodFactory.Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo MethodDeclaration
		{
			get
			{
				return m_MethodFactory.Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string Name
		{
			get
			{
				return m_MethodFactory.MemberName;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSignature Signature
		{
			get
			{
				return m_MethodFactory.Signature;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddWriter(MethodWriterBase writer)
		{
			if ( writer.IsDecorator )
			{
				writer.SetupDecoratorMode(innerWriters: m_Writers.ToArray());
				m_Writers.Clear();
			}
			
			m_Writers.Add(writer);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TStatement AddStatement<TStatement>(TStatement statement) where TStatement : StatementBase
		{
			StatementScope.Current.AddStatement(statement);
			return statement;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Label AddLabel()
		{
			return m_MethodFactory.GetILGenerator().DefineLabel();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override IDisposable CreateTypeTemplateScope()
		{
			if ( m_CachedTemplateTypePairs == null )
			{
				m_CachedTemplateTypePairs = Signature.BuildTemplateTypePairs();
			}

			return TypeTemplate.CreateScope(m_CachedTemplateTypePairs);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Write()
		{
			using ( new StatementScope(OwnerClass, this, m_Statements) )
			{
				foreach ( var writer in m_Writers )
				{
					writer.Flush();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Compile()
		{
			var il = m_MethodFactory.GetILGenerator();

			foreach ( var statement in m_Statements )
			{
				statement.Emit(il);
			}

			if ( Signature.IsVoid )
			{
				il.Emit(OpCodes.Ret);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal MethodFactoryBase MethodFactory
		{
			get
			{
				return m_MethodFactory;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TransparentMethodWriter TransparentWriter
		{
			get
			{
				return m_TransparentWriter;
			}
		}
	}
}
