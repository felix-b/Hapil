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
		private readonly List<MethodWriterBase> m_Writers;
		private readonly StatementBlock m_Statements;
		private readonly TransparentMethodWriter m_TransparentWriter;
		private MethodFactoryBase m_MethodFactory;
		private Type[] m_CachedTemplateTypePairs = null;
		private bool m_HasClosure = false;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal MethodMember(ClassType ownerClass, MethodFactoryBase methodFactory)
			: base(ownerClass, methodFactory.MemberName)
		{
			m_MethodFactory = methodFactory;
			m_Writers = new List<MethodWriterBase>();
			m_Statements = new StatementBlock();
			m_TransparentWriter = new TransparentMethodWriter(this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string BodyToString()
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Local<T> AddLocal<T>()
		{
			return new Local<T>(this);
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

		public override MemberKind Kind
		{
			get
			{
				return m_MethodFactory.MemberKind;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsVoid
		{
			get
			{
				return m_MethodFactory.Signature.IsVoid;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsStatic
		{
			get
			{
				return m_MethodFactory.Signature.IsStatic;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsAnonymous
		{
			get
			{
				return (this.Kind ==  MemberKind.InstanceAnonymousMethod || this.Kind == MemberKind.StaticAnonymousMethod);
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

		internal void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitStatementBlock(m_Statements);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal bool IsClosureRequired()
		{
			//TODO: this is temporary; provide real logic here
			return (this.Kind == MemberKind.StaticAnonymousMethod && !m_HasClosure); 
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void MoveAnonymousMethodToClosure(ClassType closureClass)
		{
			var anonymousMethodFactory = (m_MethodFactory as AnonymousMethodFactory);

			if ( anonymousMethodFactory == null )
			{
				throw new InvalidOperationException("Only anonymous method can be moved to a closure class.");
			}

			base.OwnerClass = closureClass;
			anonymousMethodFactory.MethodMovedToClosure(closureClass);

			m_HasClosure = true;
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

		internal protected MethodFactoryBase MethodFactory
		{
			get
			{
				return m_MethodFactory;
			}
			protected set
			{
				m_MethodFactory = value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal StatementBlock Body
		{
			get
			{
				return m_Statements;
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal bool HasClosure
		{
			get
			{
				return m_HasClosure;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ClassType ClosureClass
		{
			get
			{
				return (m_HasClosure ? OwnerClass : null);
			}
		}
	}
}
