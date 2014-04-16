using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;
using Happil.Statements;
using Happil.Writers;

namespace Happil.Decorators
{
	public class EventDecorationBuilder
	{
		private readonly EventMember m_OwnerEvent;
		private MethodDecorationBuilder m_OnAdd;
		private MethodDecorationBuilder m_OnRemove;
		private MethodDecorationBuilder m_OnRaise;
		private MethodMember m_InterceptorMethod;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal EventDecorationBuilder(EventMember ownerEvent)
		{
			m_OwnerEvent = ownerEvent;
			m_OnAdd = null;
			m_OnRemove = null;
			m_OnRaise = null;
			m_InterceptorMethod = null;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public EventDecorationBuilder Attribute<TAttribute>(Action<AttributeArgumentWriter<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var attributes = new AttributeWriter();
			attributes.Set<TAttribute>(values);
			m_OwnerEvent.AddAttributes(p => attributes);
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnAdd()
		{
			if ( m_OnAdd == null )
			{
				m_OnAdd = new DecoratingMethodWriter(m_OwnerEvent.AddMethod).DecorationBuilder;
			}

			return m_OnAdd;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnRemove()
		{
			if ( m_OnRemove == null )
			{
				m_OnRemove = new DecoratingMethodWriter(m_OwnerEvent.RemoveMethod).DecorationBuilder;
			}

			return m_OnRemove;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder OnRaise()
		{
			if ( m_OnRaise == null )
			{
				BuildRaiseInterceptors();
				m_OnRaise = new DecoratingMethodWriter(m_InterceptorMethod).DecorationBuilder;
			}

			return m_OnRaise;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private void BuildRaiseInterceptors()
		{
			using ( m_OwnerEvent.CreateTypeTemplateScope() )
			{
				MethodInfo closureFactoryMethod;
				FieldMember closureHandlerField;

				BuildRaiseInterceptingClosure(out closureFactoryMethod, out closureHandlerField);
				BuildAddInterceptingDecoration(closureFactoryMethod, closureHandlerField);
				BuildRemoveInterceptingDecoration();
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		private void BuildRaiseInterceptingClosure(out MethodInfo factoryMethod, out FieldMember closureHandlerField)
		{
			var closureKey = new TypeKey(baseType: typeof(object));
			var closureClass = m_OwnerEvent.OwnerClass.Module.DefineClass(typeof(object), closureKey, "EventInterceptorClosure");
			var closureWriter = new ImplementationClassWriter<object>(closureClass);

			Field<TypeTemplate.TEventHandler> handlerField;
			closureWriter.Field("handler", out handlerField, isPublic: true);
			closureWriter.DefaultConstructor();

			var reflectionCache = DelegateShortcuts.GetReflectionCache(m_OwnerEvent.EventDeclaration.EventHandlerType);
			var interceptorMethodFactory = AnonymousMethodFactory.FromMethodInfo(closureClass, reflectionCache.Invoke);
			m_InterceptorMethod = new MethodMember(closureClass, interceptorMethodFactory);
			closureClass.AddMember(m_InterceptorMethod);

			var interceptorMethodWriter = new VoidMethodWriter(m_InterceptorMethod, w => 
				handlerField.Invoke(w.Arg1<object>(), w.Arg2<TypeTemplate.TEventArgs>())
			);

			closureClass.Compile();
			factoryMethod = closureClass.GetFactoryMethods()[0].Method;
			closureHandlerField = handlerField;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void BuildAddInterceptingDecoration(MethodInfo closureFactoryMethod, FieldMember closureHandlerField)
		{
			var decoration = new DecoratingMethodWriter(m_OwnerEvent.AddMethod).DecorationBuilder;
			var handlerDelegateConstructor = DelegateShortcuts.GetDelegateConstructor(m_OwnerEvent.EventDeclaration.EventHandlerType);

			decoration.OnBefore(w => w.RawIL(il => {
				var closure = il.DeclareLocal(closureFactoryMethod.ReflectedType);
				il.Emit(OpCodes.Call, closureFactoryMethod);
				il.Emit(OpCodes.Stloc, closure);
				il.Emit(OpCodes.Ldloc, closure);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stfld, closureHandlerField.FieldBuilder);
				il.Emit(OpCodes.Ldloc, closure);
				il.Emit(OpCodes.Ldftn, (MethodInfo)m_InterceptorMethod.MethodFactory.Builder);
				il.Emit(OpCodes.Newobj, handlerDelegateConstructor);
				il.Emit(OpCodes.Starg_S, 1);
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void BuildRemoveInterceptingDecoration()
		{
			//TODO: lookup interceptor closure by original handler + remove closure handler instead of original one
		}

	}
}
