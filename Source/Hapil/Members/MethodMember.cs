using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Closures;
using Hapil.Operands;
using Hapil.Statements;
using Hapil.Writers;

namespace Hapil.Members
{
	public class MethodMember : MemberBase
	{
		private readonly ClosureDefinition m_Closure;
		private readonly List<MethodWriterBase> m_Writers;
		private readonly List<ILocal> m_Locals;
		private readonly TransparentMethodWriter m_TransparentWriter;
	    private ITransformType m_ReturnValueLocal;
        private Label? m_ReturnLabel;
        private IDisposable m_EffectiveTypeTemplates;
        private StatementBlock m_Statements;
		private MethodFactoryBase m_MethodFactory;
		private Type[] m_CachedTemplateTypePairs = null;
		private bool m_LocalsDeclared = false;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal MethodMember(ClassType ownerClass, MethodFactoryBase methodFactory)
			: this(ownerClass, methodFactory, closure: null)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal MethodMember(ClassType ownerClass, MethodFactoryBase methodFactory, ClosureDefinition closure)
			: base(ownerClass, methodFactory.MemberName)
		{
			m_MethodFactory = methodFactory;
			m_Closure = closure;
			m_Writers = new List<MethodWriterBase>();
			m_Statements = new StatementBlock();
			m_Locals = new List<ILocal>();
			m_TransparentWriter = new TransparentMethodWriter(this);
		    m_EffectiveTypeTemplates = TypeTemplate.Save();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string GetMethodText()
		{
			return (
				base.Name + 
				m_MethodFactory.Signature.ToString() + 
				m_Statements.ToString());
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

		public bool IsPublic
		{
			get
			{
				return m_MethodFactory.Signature.IsPublic;
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

		internal void SetBody(StatementBlock body)
		{
			body.BindToMethod(this);
			m_Statements = body;
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

		internal void RegisterLocal(ILocal local, out int localIndex)
		{
			localIndex = m_Locals.Count;
			m_Locals.Add(local);

			if ( m_LocalsDeclared )
			{
				local.Declare(this.MethodFactory.GetILGenerator());
			}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        internal Operand<T> GetReturnValueLocal<T>()
        {
            if ( m_ReturnValueLocal == null )
            {
                m_ReturnValueLocal = new Local<T>(this);
            }

            return m_ReturnValueLocal.TransformToType<T>();
        }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Label AddLabel()
		{
			return m_MethodFactory.GetILGenerator().DefineLabel();
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        internal Label GetMethodReturnLabel()
        {
            if ( !m_ReturnLabel.HasValue )
            {
                m_ReturnLabel = AddLabel();
            }

            return m_ReturnLabel.Value;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AcceptVisitor(OperandVisitorBase visitor)
		{
			visitor.VisitStatementBlock(m_Statements);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal bool IdentifyAnonymousMethods(out IAnonymousMethodIdentification identification)
		{
			var visitor = new AnonymousMethodIdentificationVisitor(this);
			AcceptVisitor(visitor);

			identification = visitor;
			return identification.AnonymousMethodsFound;
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
            using ( TypeTemplate.Restore(m_EffectiveTypeTemplates) )
		    {
		        var writersArray = m_Writers.ToArray();

		        using ( new StatementScope(OwnerClass, m_TransparentWriter, m_Statements) )
		        {
		            foreach ( var writer in writersArray )
		            {
		                writer.Flush();
		            }
		        }

		        new AnonymousMethodWriter(this).Flush();
            }
        }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Compile()
		{
		    using ( TypeTemplate.Restore(m_EffectiveTypeTemplates) )
		    {
		        var il = m_MethodFactory.GetILGenerator();

		        foreach ( var local in m_Locals )
		        {
		            local.Declare(il);
		        }

		        m_LocalsDeclared = true;

		        foreach ( var statement in m_Statements )
		        {
		            statement.Emit(il, this);
		        }

		        EmitMethodReturn(il);
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

		internal virtual bool HasClosure
		{
			get
			{
				return (m_Closure != null);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal virtual ClosureDefinition Closure
		{
			get
			{
				return m_Closure;
			}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    private void EmitMethodReturn(ILGenerator il)
	    {
	        if ( m_ReturnLabel.HasValue )
	        {
	            il.MarkLabel(m_ReturnLabel.Value);
	        }

	        if ( !Signature.IsVoid )
	        {
	            var returnValue = (m_ReturnValueLocal as IOperand);

                //if ( returnValue == null )
                //{
                //    throw new CodeGenerationException(string.Format(
                //        "Return statement is missing in method '{0}.{1}'.", 
                //        this.Name, this.OwnerClass.TypeBuilder.Name));
                //}

	            if ( returnValue != null )
	            {
	                returnValue.EmitLoad(il);
	            }
	        }

	        il.Emit(OpCodes.Ret);
	    }
	}
}
