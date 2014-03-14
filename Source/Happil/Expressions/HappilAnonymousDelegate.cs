using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;
using Happil.Statements;

namespace Happil.Expressions
{
	internal class HappilAnonymousDelegate<TArg1, TReturn> : HappilOperand<Func<TArg1, TReturn>>, IHappilDelegate
	{
		private readonly HappilMethod<TReturn> m_Method;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAnonymousDelegate(HappilClass happilClass, Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>> body)
			: base(ownerMethod: null)
		{
			m_Method = new HappilMethod<TReturn>(happilClass, "<Anonymous>", typeof(TReturn), new[] { typeof(TArg1) });

			happilClass.AddUndeclaredMember(m_Method);
			m_Method.AddBodyDefinition(() => {
				body(m_Method, new HappilArgument<TArg1>(m_Method, index: 0));
			});
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Ldftn, m_Method.MethodBuilder);
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

	internal class HappilAnonymousDelegate<TArg1, TArg2, TReturn> : HappilOperand<Func<TArg1, TArg2, TReturn>>, IHappilDelegate
	{
		private readonly HappilMethod<TReturn> m_Method;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilAnonymousDelegate(HappilClass happilClass, Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>, HappilArgument<TArg2>> body)
			: base(ownerMethod: null)
		{
			m_Method = new HappilMethod<TReturn>(happilClass, "<Anonymous>", typeof(TReturn), new[] { typeof(TArg1), typeof(TArg2) });
			m_Method.AddBodyDefinition(() => {
				body(m_Method, new HappilArgument<TArg1>(m_Method, index: 0), new HappilArgument<TArg2>(m_Method, index: 1));
			});

			happilClass.AddUndeclaredMember(m_Method);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Ldftn, m_Method.MethodBuilder);
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
