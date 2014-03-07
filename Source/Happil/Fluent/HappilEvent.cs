using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Happil.Fluent
{
	internal class HappilEvent : IHappilMember
	{
		private const MethodAttributes ContainedMethodAttributes = 
			MethodAttributes.Public | 
			MethodAttributes.NewSlot | 
			MethodAttributes.Final | 
			MethodAttributes.HideBySig | 
			MethodAttributes.SpecialName | 
			MethodAttributes.Virtual;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private readonly HappilClass m_HappilClass;
		private readonly EventInfo m_Declaration;
		private readonly EventBuilder m_EventBuilder;
		private readonly HappilField<TypeTemplate.TEventHandler> m_BackingField;
		private readonly VoidHappilMethod m_AddMethod;
		private readonly VoidHappilMethod m_RemoveMethod;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilEvent(HappilClass happilClass, EventInfo declaration)
		{
			m_HappilClass = happilClass;
			m_Declaration = declaration;
			m_EventBuilder = happilClass.TypeBuilder.DefineEvent(declaration.Name, declaration.Attributes, declaration.EventHandlerType);

			using ( CreateTypeTemplateScope() )
			{
				m_BackingField = new HappilField<TypeTemplate.TEventHandler>(happilClass, "m_" + declaration.Name + "EventHandler");

				m_AddMethod = new VoidHappilMethod(happilClass, declaration.GetAddMethod(), ContainedMethodAttributes);
				m_EventBuilder.SetAddOnMethod(m_AddMethod.MethodBuilder);

				m_RemoveMethod = new VoidHappilMethod(happilClass, declaration.GetRemoveMethod(), ContainedMethodAttributes);
				m_EventBuilder.SetRemoveOnMethod(m_RemoveMethod.MethodBuilder);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

		public void EmitBody()
		{
			((IHappilMember)m_AddMethod).EmitBody();
			((IHappilMember)m_RemoveMethod).EmitBody();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IDisposable CreateTypeTemplateScope()
		{
			return TypeTemplate.CreateScope(typeof(TypeTemplate.TEventHandler), m_Declaration.EventHandlerType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		MemberInfo IHappilMember.Declaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string Name
		{
			get
			{
				return m_Declaration.Name;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void DefineDefaultAddRemoveMethods()
		{
			using ( CreateTypeTemplateScope() )
			{
				using ( m_AddMethod.CreateBodyScope() )
				{
					DefineAddMethodBody(m_AddMethod, new HappilArgument<TypeTemplate.TEventHandler>(m_AddMethod, 1));
				}

				using ( m_RemoveMethod.CreateBodyScope() )
				{
					DefineRemoveMethodBody(m_RemoveMethod, new HappilArgument<TypeTemplate.TEventHandler>(m_RemoveMethod, 1));
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RaiseEvent(IHappilMethodBodyBase m, IHappilOperand args)
		{
			using ( CreateTypeTemplateScope() )
			{
				m.If(m_BackingField != m.Const<TypeTemplate.TEventHandler>(null)).Then(() => {
					m_BackingField.Invoke(m.This<TypeTemplate.TBase>(), args);
				});
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilField<TypeTemplate.TEventHandler> BackingField
		{
			get
			{
				return m_BackingField;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void DefineAddMethodBody(IVoidHappilMethodBody m, HappilArgument<TypeTemplate.TEventHandler> value)
		{
			var oldHandler = m.Local<TypeTemplate.TEventHandler>();
			var newHandler = m.Local<TypeTemplate.TEventHandler>();
			var lastHandler = m.Local<TypeTemplate.TEventHandler>(initialValue: m_BackingField);

			m.Do(loop => {
				oldHandler.Assign(lastHandler);

				newHandler.Assign(Static.Func(Delegate.Combine,
					oldHandler.CastTo<Delegate>(),
					value.CastTo<Delegate>()).CastTo<TypeTemplate.TEventHandler>());

				lastHandler.Assign(Static.GenericFunc((x, y, z) => Interlocked.CompareExchange(ref x, y, z),
					m_BackingField,
					newHandler,
					oldHandler));
			}).While(lastHandler != oldHandler);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void DefineRemoveMethodBody(IVoidHappilMethodBody m, HappilArgument<TypeTemplate.TEventHandler> value)
		{
			var oldHandler = m.Local<TypeTemplate.TEventHandler>();
			var newHandler = m.Local<TypeTemplate.TEventHandler>();
			var lastHandler = m.Local<TypeTemplate.TEventHandler>(initialValue: m_BackingField);

			m.Do(loop => {
				oldHandler.Assign(lastHandler);

				newHandler.Assign(Static.Func(Delegate.Remove, 
					oldHandler.CastTo<Delegate>(), 
					value.CastTo<Delegate>()).CastTo<TypeTemplate.TEventHandler>());

				lastHandler.Assign(Static.GenericFunc((x, y, z) => Interlocked.CompareExchange(ref x, y, z),
					m_BackingField,
					newHandler,
					oldHandler));
			}).While(lastHandler != oldHandler);
		}
	}
}
