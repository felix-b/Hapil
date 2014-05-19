using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Closures;
using Happil.Expressions;
using Happil.Members;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Operands
{
	internal class AnonymousFuncOperand<TArg1, TReturn> : Operand<Func<TArg1, TReturn>>, IDelegateOperand, IAnonymousMethodOperand, IAcceptOperandVisitor
	{
		private readonly ClassType m_OwnerClass;
		private readonly StatementBlock m_HomeScopeBlock;
		private readonly StatementBlock m_Statements;
		private MethodSignature m_Signature;
		private Local<Func<TArg1, TReturn>> m_CallSite = null;
		private MethodMember m_Method = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AnonymousFuncOperand(ClassType ownerClass, Action<FunctionMethodWriter<TReturn>, Argument<TArg1>> body)
		{
			//var methodFactory = AnonymousMethodFactory.StaticMethod(ownerMethod, new[] { typeof(TArg1) }, typeof(TReturn));
			//m_Method = new MethodMember(ownerMethod.OwnerClass, methodFactory);
			//ownerMethod.OwnerClass.AddMember(m_Method);

			m_OwnerClass = ownerClass;
			m_HomeScopeBlock = StatementScope.Current.StatementBlock;
			m_Statements = new StatementBlock();
			m_Signature = new MethodSignature(
				isStatic: true, 
				isPublic: false, 
				argumentTypes: new[] { TypeTemplate.Resolve<TArg1>() },
				returnType: TypeTemplate.Resolve<TReturn>());

			using ( StatementScope.Stash() )
			{
				var writer = new FunctionMethodWriter<TReturn>(
					m_Method,
					script: w => body(w, w.Arg1<TArg1>()),
					mode: MethodWriterModes.Normal,
					attachToOwner: false);

				using ( new StatementScope(ownerClass, writer, statementBlock: m_Statements) )
				{
					body(writer, new Argument<TArg1>(m_Statements, index: 1, isByRef: false, isOut: false));
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IAnonymousMethodOperand Members

		public void CreateAnonymousMethod(ClassType ownerClass, ClosureDefinition closure, bool isStatic, bool isPublic)
		{
			Debug.Assert(m_Method == null, "CreateAnonymousMethod was already called.");

			var methodFactory = AnonymousMethodFactory.Create(
				ownerClass,
				argumentTypes: new[] { typeof(TArg1) },
				returnType: typeof(TReturn),
				isStatic: isStatic,
				isPublic: isPublic);
				
			m_Method = new MethodMember(ownerClass, methodFactory, closure);
			m_Method.SetBody(m_Statements);
			ownerClass.AddMember(m_Method);

			m_Signature = m_Method.Signature;

			//var operandUnbinder = new UnbindFromMethodOperandVisitor();
			//m_Method.AcceptVisitor(operandUnbinder);

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
			m_CallSite = writer.Local<Func<TArg1, TReturn>>();

			using ( new StatementScope(m_HomeScopeBlock.EnclosingLoopStatement.HomeScope.StatementBlock, StatementScope.RewriteMode.On) )
			{
				m_CallSite.Assign(writer.Const<Func<TArg1, TReturn>>(null));
			}

			using ( new StatementScope(m_HomeScopeBlock, StatementScope.RewriteMode.On) )
			{
				writer.If(m_CallSite == writer.Const<Func<TArg1, TReturn>>(null)).Then(() =>
					m_CallSite.Assign(writer.New<Func<TArg1, TReturn>>(GetTargetOperand(), writer.MethodOf(m_Method)))
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
					"Func<{0},{1}>({2}{3})",
					m_Signature.ArgumentType[0].FriendlyName(),
					m_Signature.ReturnType.FriendlyName(),
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

				target.EmitTarget(il);
				target.EmitLoad(il);

				//if ( m_Method.HasClosure )
				//{
				//	var target = m_Method.Closure.ClosureInstanceReference;

				//	target.EmitTarget(il);
				//	target.EmitLoad(il);
				//}
				//else
				//{
				//	il.Emit(m_Method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0);
				//}

				il.Emit(OpCodes.Ldftn, (MethodBuilder)m_Method.MethodFactory.Builder);
				il.Emit(OpCodes.Newobj, s_DelegateConstructor);
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConstructorInfo s_DelegateConstructor;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static AnonymousFuncOperand()
		{
			s_DelegateConstructor = DelegateShortcuts.GetDelegateConstructor(typeof(Func<TArg1, TReturn>));
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TArg1, TArg2, TReturn> : Operand<Func<TArg1, TArg2, TReturn>>, IDelegateOperand
	{
		private readonly MethodMember m_Method;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AnonymousFuncOperand(ClassType ownerClass, Action<FunctionMethodWriter<TReturn>, Argument<TArg1>, Argument<TArg2>> body)
		{
			var methodFactory = AnonymousMethodFactory.Create(ownerClass, new[] { typeof(TArg1), typeof(TArg2) }, typeof(TReturn), isStatic: true, isPublic: false);

			//var methodFactory = (
			//	ownerMethod.IsStatic
			//	? AnonymousMethodFactory.StaticMethod(ownerMethod.OwnerClass, new[] { typeof(TArg1), typeof(TArg2) }, typeof(TReturn))
			//	: AnonymousMethodFactory.InstanceMethod(ownerMethod.OwnerClass, new[] { typeof(TArg1), typeof(TArg2) }, typeof(TReturn)));

			m_Method = new MethodMember(ownerClass, methodFactory);

			ownerClass.AddMember(m_Method);
			var writer = new FunctionMethodWriter<TReturn>(
				m_Method, 
				w => body(w, w.Arg1<TArg1>(), w.Arg2<TArg2>()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format("Func[{0}]", m_Method);
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

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			if ( m_Method.HasClosure )
			{
				var target = m_Method.Closure.ClosureInstanceReference;

				target.EmitTarget(il);
				target.EmitLoad(il);
			}
			else
			{
				il.Emit(m_Method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0);
			}

			il.Emit(OpCodes.Ldftn, (MethodBuilder)m_Method.MethodFactory.Builder);
			il.Emit(OpCodes.Newobj, s_DelegateConstructor);
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

		private static readonly ConstructorInfo s_DelegateConstructor;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static AnonymousFuncOperand()
		{
			s_DelegateConstructor = typeof(Func<TArg1, TArg2, TReturn>).GetConstructor(new[] { typeof(object), typeof(IntPtr) });
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal interface IAnonymousMethodOperand
	{
		void CreateAnonymousMethod(ClassType ownerClass, ClosureDefinition closure, bool isStatic, bool isPublic);
		void WriteCallSite();
		StatementBlock Statements { get; }
		MethodMember AnonymousMethod { get; }
	}
}
