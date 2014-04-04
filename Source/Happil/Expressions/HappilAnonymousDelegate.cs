using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Operands;
using Happil.Members;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Expressions
{
	//TODO:redesign: rename
	internal class HappilAnonymousDelegate<TArg1, TReturn> : Operand<Func<TArg1, TReturn>>, IHappilDelegate
	{
		private readonly MethodMember m_Method;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAnonymousDelegate(ClassType classType, Action<FunctionMethodWriter<TReturn>, Argument<TArg1>> body)
		{
			var methodFactory = AnonymousMethodFactory.InstanceMethod(classType, new[] { typeof(TArg1) }, typeof(TReturn));
			m_Method = new MethodMember(classType, methodFactory);

			classType.AddMember(m_Method);
			var writer = new FunctionMethodWriter<TReturn>(
				m_Method, 
				w => body(w, w.Arg1<TArg1>()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_0);
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

		static HappilAnonymousDelegate()
		{
			s_DelegateConstructor = typeof(Func<TArg1, TReturn>).GetConstructor(new[] { typeof(object), typeof(IntPtr) });
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class HappilAnonymousDelegate<TArg1, TArg2, TReturn> : Operand<Func<TArg1, TArg2, TReturn>>, IHappilDelegate
	{
		private readonly MethodMember m_Method;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAnonymousDelegate(ClassType classType, Action<FunctionMethodWriter<TReturn>, Argument<TArg1>, Argument<TArg2>> body)
		{
			var methodFactory = AnonymousMethodFactory.InstanceMethod(classType, new[] { typeof(TArg1), typeof(TArg2) }, typeof(TReturn));
			m_Method = new MethodMember(classType, methodFactory);

			classType.AddMember(m_Method);
			var writer = new FunctionMethodWriter<TReturn>(
				m_Method, 
				w => body(w, w.Arg1<TArg1>(), w.Arg2<TArg2>()));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_0);
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

		static HappilAnonymousDelegate()
		{
			s_DelegateConstructor = typeof(Func<TArg1, TArg2, TReturn>).GetConstructor(new[] { typeof(object), typeof(IntPtr) });
		}
	}
}
