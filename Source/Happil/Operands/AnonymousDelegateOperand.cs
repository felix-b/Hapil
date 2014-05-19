using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using Happil.Closures;
using Happil.Members;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Operands
{
	internal abstract class AnonymousDelegateOperand<TDelegate> : Operand<TDelegate>, IDelegateOperand, IAnonymousMethodOperand, IAcceptOperandVisitor
		where TDelegate : class
	{
		private readonly ClassType m_OwnerClass;
		private readonly StatementBlock m_HomeScopeBlock;
		private readonly StatementBlock m_Statements;
		private MethodSignature m_Signature;
		private Local<TDelegate> m_CallSite = null;
		private MethodMember m_Method = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected AnonymousDelegateOperand(ClassType ownerClass, Type[] argumentTypes, Type returnType)
		{
			m_OwnerClass = ownerClass;
			m_HomeScopeBlock = StatementScope.Current.StatementBlock;
			m_Statements = new StatementBlock();
			m_Signature = new MethodSignature(
				isStatic: true,
				isPublic: false,
				argumentTypes: argumentTypes.Select(TypeTemplate.Resolve).ToArray(),
				returnType: returnType != null ? TypeTemplate.Resolve(returnType) : null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAnonymousMethodOperand Members

		public void CreateAnonymousMethod(ClassType ownerClass, ClosureDefinition closure, bool isStatic, bool isPublic)
		{
			Debug.Assert(m_Method == null, "CreateAnonymousMethod was already called.");

			var methodFactory = AnonymousMethodFactory.Create(
				ownerClass,
				argumentTypes: m_Signature.ArgumentType,
				returnType: m_Signature.ReturnType,
				isStatic: isStatic,
				isPublic: isPublic);

			m_Method = new MethodMember(ownerClass, methodFactory, closure);
			m_Method.SetBody(m_Statements);
			ownerClass.AddMember(m_Method);

			m_Signature = m_Method.Signature;

			var operandBinder = new BindToMethodOperandVisitor(m_Method);
			m_Method.AcceptVisitor(operandBinder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void WriteCallSite()
		{
			Debug.Assert(m_Method != null, "CreateAnonymousMethod was not called.");

			if ( !NeedCallSite() )
			{
				return;
			}

			var writer = m_HomeScopeBlock.OwnerMethod.TransparentWriter;
			m_CallSite = writer.Local<TDelegate>();

			using ( new StatementScope(m_HomeScopeBlock.EnclosingLoopStatement.HomeScope.StatementBlock, StatementScope.RewriteMode.On) )
			{
				m_CallSite.Assign(writer.Const<TDelegate>(null));
			}

			using ( new StatementScope(m_HomeScopeBlock, StatementScope.RewriteMode.On) )
			{
				writer.If(m_CallSite == writer.Const<TDelegate>(null)).Then(() =>
					m_CallSite.Assign(writer.New<TDelegate>(GetTargetOperand(), writer.MethodOf(m_Method)))
					);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementBlock Statements
		{
			get
			{
				return m_Statements;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember AnonymousMethod
		{
			get
			{
				return m_Method;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAcceptOperandVisitor Members

		void IAcceptOperandVisitor.AcceptVisitor(OperandVisitorBase visitor)
		{
			if ( m_Method == null ) // the statements belong to host method until the anonymous method is created
			{
				visitor.VisitStatementBlock(m_Statements);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			if ( m_CallSite.IsDefined() )
			{
				return m_CallSite.ToString();
			}
			else if ( m_Method != null )
			{
				var target = (m_Method.IsStatic ? "" : (m_Method.HasClosure ? m_Method.Closure.ClosureInstanceReference.ToString() + "." : "this."));

				return string.Format(
					"{0}({1}{2})",
					OperandType.FriendlyName(),
					target,
					m_Method.Name);
			}
			else
			{
				return string.Format("Delegate{0}{1}", m_Signature, m_Statements);
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override OperandKind Kind
		{
			get
			{
				return OperandKind.Delegate;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void WriteMethodBody(MethodWriterBase methodWriter)
		{
			using ( StatementScope.Stash() )
			{
				using ( new StatementScope(m_OwnerClass, methodWriter, statementBlock: m_Statements) )
				{
					methodWriter.Flush();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			if ( m_CallSite.IsDefined() )
			{
				m_CallSite.EmitTarget(il);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			if ( m_CallSite.IsDefined() )
			{
				m_CallSite.EmitLoad(il);
			}
			else
			{
				var target = GetTargetOperand();
				var delegateConstructor = DelegateShortcuts.GetDelegateConstructor(OperandType);

				target.EmitTarget(il);
				target.EmitLoad(il);

				il.Emit(OpCodes.Ldftn, (MethodBuilder)m_Method.MethodFactory.Builder);
				il.Emit(OpCodes.Newobj, delegateConstructor);
			}
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private IOperand GetTargetOperand()
		{
			if ( m_Method.HasClosure )
			{
				return m_Method.Closure.ClosureInstanceReference;
			}
			else if ( m_Method.IsStatic )
			{
				return new Constant<object>(null);
			}
			else
			{
				return new ThisOperand<object>(m_OwnerClass);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private bool NeedCallSite()
		{
			if ( m_Method.HasClosure )
			{
				return (m_HomeScopeBlock.EnclosingLoopStatement != m_Method.Closure.HostScopeBlock.EnclosingLoopStatement);
			}
			else
			{
				return (m_HomeScopeBlock.EnclosingLoopStatement != null);
			}
		}
	}
}