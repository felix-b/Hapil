using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Selectors;

namespace Happil.Fluent
{
	internal class HappilClassBody<TBase> : 
		IHappilClassBody<TBase>, 
		IHappilClassDefinition, 
		IHappilClassDefinitionInternals
	{
		private readonly HappilClass m_HappilClass;
		private readonly Type m_ReflectedType;
		private readonly MemberInfo[] m_ImplementableMembers;
		private readonly MethodInfo[] m_ImplementableMethods;
		private readonly PropertyInfo[] m_ImplementableProperties;
		private readonly EventInfo[] m_ImplementableEvents;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClassBody(HappilClass happilClass)
		{
			m_HappilClass = happilClass;
			m_ReflectedType = TypeTemplate.Resolve(typeof(TBase));

			var members = TypeMembers.Of(m_ReflectedType);
			
			m_ImplementableMembers = members.ImplementableMembers;
			m_ImplementableMethods = members.ImplementableMethods.Where(m => !m.IsSpecialName).ToArray();
			m_ImplementableProperties = members.ImplementableProperties;
			m_ImplementableEvents = members.ImplementableEvents;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilClassDefinition Members

		public Type BaseType
		{
			get
			{
				return m_HappilClass.TypeBuilder.BaseType;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilClassDefinitionInternals Members

		public HappilClass HappilClass
		{
			get
			{
				return m_HappilClass;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilClassBody<TBase> Members

		public IHappilClassBody<T> AsBase<T>()
		{
			return m_HappilClass.GetBody<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> ImplementInterface(Type interfaceType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members)
		{
			m_HappilClass.ImplementInterface(interfaceType);
			return m_HappilClass.GetBody<TBase>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TInterface> ImplementInterface<TInterface>(params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members)
		{
			m_HappilClass.ImplementInterface(typeof(TInterface));
			return m_HappilClass.GetBody<TInterface>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> Attribute<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new HappilAttributeBuilder<TAttribute>(values);
			m_HappilClass.TypeBuilder.SetCustomAttribute(builder.GetAttributeBuilder());
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldAccessOperand<T> Field<T>(string name)
		{
			var field = DefineField<T>(name, isStatic: false);
			return field.AsOperand<T>(); //TODO: check that T is compatible with field type
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> Field<T>(string name, out FieldAccessOperand<T> field)
		{
			field = this.Field<T>(name);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> Field<T>(string name, IHappilAttributes attributes, out FieldAccessOperand<T> field)
		{
			var fieldMember = DefineField<T>(name, isStatic: false);
			fieldMember.SetAttributes(attributes as HappilAttributes);
			field = fieldMember.AsOperand<T>();
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldAccessOperand<T> StaticField<T>(string name)
		{
			var fieldMember = DefineField<T>(name, isStatic: true);
			return fieldMember.AsOperand<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> StaticField<T>(string name, out FieldAccessOperand<T> field)
		{
			field = this.StaticField<T>(name);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> StaticField<T>(string name, IHappilAttributes attributes, out FieldAccessOperand<T> field)
		{
			var fieldMember = DefineField<T>(name, isStatic: true);
			fieldMember.SetAttributes(attributes as HappilAttributes);
			field = fieldMember.AsOperand<T>();
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> DefaultConstructor(IHappilAttributes attributes = null)
		{
			return DefineConstructor(attributes, ctor => ctor.Base());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> StaticConstructor(
			Action<IHappilConstructorBody> body,
			IHappilAttributes attributes = null)
		{
			var constructorMember = HappilConstructor.CreateStaticConstructor(m_HappilClass);

			constructorMember.SetAttributes(attributes as HappilAttributes);
			constructorMember.AddBodyDefinition(() => {
				body(constructorMember);
			});

			m_HappilClass.AddUndeclaredMember(constructorMember);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> Constructor(
			Action<IHappilConstructorBody> body,
			IHappilAttributes attributes = null)
		{
			return DefineConstructor(attributes, body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> Constructor<TArg1>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>> body,
			IHappilAttributes attributes = null)
		{
			return DefineConstructor(
				attributes,
				(ctor) => body(ctor, new HappilArgument<TArg1>(ctor, 1)),
				typeof(TArg1));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> Constructor<TArg1, TArg2>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>> body,
			IHappilAttributes attributes = null)
		{
			return DefineConstructor(
				attributes,
				(ctor) => body(ctor, new HappilArgument<TArg1>(ctor, 1), new HappilArgument<TArg2>(ctor, 2)),
				typeof(TArg1), typeof(TArg2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> Constructor<TArg1, TArg2, TArg3>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body,
			IHappilAttributes attributes = null)
		{
			return DefineConstructor(
				attributes,
				(ctor) => body(ctor, new HappilArgument<TArg1>(ctor, 1), new HappilArgument<TArg2>(ctor, 2), new HappilArgument<TArg3>(ctor, 3)),
				typeof(TArg1), typeof(TArg2), typeof(TArg3));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void<TBase> Method(Expression<Func<TBase, Action>> method)
		{
			return new MethodSelectors.Void<TBase>(this, Helpers.GetMethodInfoArrayFromLambda(method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void1Arg<TBase, TArg1> Method<TArg1>(Expression<Func<TBase, Action<TArg1>>> method)
		{
			return new MethodSelectors.Void1Arg<TBase, TArg1>(this, Helpers.GetMethodInfoArrayFromLambda(method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void2Args<TBase, TArg1, TArg2> Method<TArg1, TArg2>(Expression<Func<TBase, Action<TArg1, TArg2>>> method)
		{
			return new MethodSelectors.Void2Args<TBase, TArg1, TArg2>(this, Helpers.GetMethodInfoArrayFromLambda(method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void3Args<TBase, TArg1, TArg2, TArg3> Method<TArg1, TArg2, TArg3>(Expression<Func<TBase, Action<TArg1, TArg2, TArg3>>> method)
		{
			return new MethodSelectors.Void3Args<TBase, TArg1, TArg2, TArg3>(this, Helpers.GetMethodInfoArrayFromLambda(method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions<TBase, TReturn> Method<TReturn>(Expression<Func<TBase, Func<TReturn>>> function)
		{
			return new MethodSelectors.Functions<TBase, TReturn>(this, Helpers.GetMethodInfoArrayFromLambda(function));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions1Arg<TBase, TArg1, TReturn> Method<TArg1, TReturn>(Expression<Func<TBase, Func<TArg1, TReturn>>> function)
		{
			return new MethodSelectors.Functions1Arg<TBase, TArg1, TReturn>(this, Helpers.GetMethodInfoArrayFromLambda(function));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions2Args<TBase, TArg1, TArg2, TReturn> Method<TArg1, TArg2, TReturn>(Expression<Func<TBase, Func<TArg1, TArg2, TReturn>>> function)
		{
			return new MethodSelectors.Functions2Args<TBase, TArg1, TArg2, TReturn>(this, Helpers.GetMethodInfoArrayFromLambda(function));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions3Args<TBase, TArg1, TArg2, TArg3, TReturn> Method<TArg1, TArg2, TArg3, TReturn>(
			Expression<Func<TBase, Func<TArg1, TArg2, TArg3, TReturn>>> function)
		{
			return new MethodSelectors.Functions3Args<TBase, TArg1, TArg2, TArg3, TReturn>(this, Helpers.GetMethodInfoArrayFromLambda(function));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Untyped<TBase> Method(MethodInfo methodInfo)
		{
			return new MethodSelectors.Untyped<TBase>(this, new[] { methodInfo });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Untyped<TBase> AllMethods(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Untyped<TBase>(
				this, 
				m_ImplementableMethods.SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void<TBase> VoidMethods(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Void<TBase>(
				this, 
				m_ImplementableMethods.OfSignature(typeof(void)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void1Arg<TBase, TArg1> VoidMethods<TArg1>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Void1Arg<TBase, TArg1>(
				this, 
				m_ImplementableMethods.OfSignature(typeof(void), typeof(TArg1)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void2Args<TBase, TArg1, TArg2> VoidMethods<TArg1, TArg2>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Void2Args<TBase, TArg1, TArg2>(
				this, 
				m_ImplementableMethods.OfSignature(typeof(void), typeof(TArg1), typeof(TArg2)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Void3Args<TBase, TArg1, TArg2, TArg3> VoidMethods<TArg1, TArg2, TArg3>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Void3Args<TBase, TArg1, TArg2, TArg3>(
				this,
				m_ImplementableMethods.OfSignature(typeof(void), typeof(TArg1), typeof(TArg2), typeof(TArg3)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions<TBase, TReturn> NonVoidMethods<TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Functions<TBase, TReturn>(
				this, 
				m_ImplementableMethods.OfSignature(typeof(TReturn)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions1Arg<TBase, TArg1, TReturn> NonVoidMethods<TArg1, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Functions1Arg<TBase, TArg1, TReturn>(
				this,
				m_ImplementableMethods.OfSignature(typeof(TReturn), typeof(TArg1)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions2Args<TBase, TArg1, TArg2, TReturn> NonVoidMethods<TArg1, TArg2, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Functions2Args<TBase, TArg1, TArg2, TReturn>(
				this,
				m_ImplementableMethods.OfSignature(typeof(TReturn), typeof(TArg1), typeof(TArg2)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelectors.Functions3Args<TBase, TArg1, TArg2, TArg3, TReturn> NonVoidMethods<TArg1, TArg2, TArg3, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelectors.Functions3Args<TBase, TArg1, TArg2, TArg3, TReturn>(
				this,
				m_ImplementableMethods.OfSignature(typeof(TReturn), typeof(TArg1), typeof(TArg2), typeof(TArg3)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Untyped<TBase> AllProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Untyped<TBase>(this, m_ImplementableProperties.SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Untyped<TBase> ReadOnlyProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Untyped<TBase>(
				this, 
				m_ImplementableProperties.Where(p => p.CanRead && !p.CanWrite).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Untyped<TBase> ReadWriteProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Untyped<TBase>(
				this,
				m_ImplementableProperties.Where(p => p.CanRead && p.CanWrite).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Typed<TBase, TProperty> Property<TProperty>(Expression<Func<TBase, TProperty>> property)
		{
			return new PropertySelectors.Typed<TBase, TProperty>(this, Helpers.GetPropertyInfoArrayFromLambda(property));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Indexer1Arg<TBase, TIndex, TProperty> This<TIndex, TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Indexer1Arg<TBase, TIndex, TProperty>(
				this,
				m_ImplementableProperties.OfSignature(typeof(TProperty), typeof(TIndex)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Indexer2Args<TBase, TIndex1, TIndex2, TProperty> This<TIndex1, TIndex2, TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Indexer2Args<TBase, TIndex1, TIndex2, TProperty>(
				this,
				m_ImplementableProperties.OfSignature(typeof(TProperty), typeof(TIndex1), typeof(TIndex2)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Typed<TBase, TProperty> Properties<TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Typed<TBase, TProperty>(
				this,
				m_ImplementableProperties.OfSignature(typeof(TProperty)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Indexer1Arg<TBase, TIndex, TProperty> Properties<TIndex, TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Indexer1Arg<TBase, TIndex, TProperty>(
				this,
				m_ImplementableProperties.OfSignature(typeof(TProperty), typeof(TIndex)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelectors.Indexer2Args<TBase, TIndex1, TIndex2, TProperty> Properties<TIndex1, TIndex2, TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelectors.Indexer2Args<TBase, TIndex1, TIndex2, TProperty>(
				this,
				m_ImplementableProperties.OfSignature(typeof(TProperty), typeof(TIndex1), typeof(TIndex2)).SelectIf(where));
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public EventSelectors.Typed<TBase, TDelegate> Event<TDelegate>(Expression<Func<TBase, TDelegate>> @event)
		//{
		//	//return new EventSelectors.Typed<TBase, TDelegate>(this, typeof(TBase).GetEvents().SelectIf(where));
		//	throw new NotImplementedException();
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventSelectors.Untyped<TBase> AllEvents(Func<EventInfo, bool> where = null)
		{
			return new EventSelectors.Untyped<TBase>(this, typeof(TBase).GetEvents().SelectIf(where));
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public EventSelectors.Typed<TBase, TDelegate> Events<TDelegate>(Func<EventInfo, bool> where = null)
		//{
		//	throw new NotImplementedException();
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> DecorateWith<TImplementor>() where TImplementor : IDecorationImplementor, new()
		{
			var implementor = new TImplementor();
			implementor.ImplementDecoration<TBase>(this);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> DecorateWith(IDecorationImplementor implementor)
		{
			implementor.ImplementDecoration<TBase>(this);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> DecorateWith(IEnumerable<IDecorationImplementor> implementors)
		{
			foreach ( var singleImplementor in implementors )
			{
				singleImplementor.ImplementDecoration<TBase>(this);
			}

			return this;
		}
		
		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MemberInfo[] GetImplementableMembers()
		{
			return m_ImplementableMembers;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilField DefineField(string name, Type fieldType, bool isStatic)
		{
			var field = new HappilField(m_HappilClass, name, fieldType, isStatic);
			m_HappilClass.AddUndeclaredMember(field);
			return field;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilField DefineField<T>(string name, bool isStatic)
		{
			return DefineField(name, typeof(T), isStatic);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void DefineMethodBodyInScope<T>(HappilMethod method, Action<T> userDefinition)
			where T : IHappilMethodBodyBase
		{
			using ( method.CreateBodyScope() )
			{
				userDefinition((T)(object)method);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private IHappilClassBody<TBase> DefineConstructor(
			IHappilAttributes attributes, 
			Action<HappilConstructor> invokeBodyDefinition, 
			params Type[] argumentTypes)
		{
			var resolvedArgumentTypes = argumentTypes.Select(TypeTemplate.Resolve).ToArray();
			var constructorMember = new HappilConstructor(m_HappilClass, resolvedArgumentTypes);

			constructorMember.SetAttributes(attributes as HappilAttributes);
			constructorMember.AddBodyDefinition(() => {
				invokeBodyDefinition(constructorMember);
			});

			m_HappilClass.AddUndeclaredMember(constructorMember);
			m_HappilClass.DefineFactoryMethod(constructorMember.ConstructorInfo, argumentTypes);
			
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//private MemberInfo[] GatherImplementableMembers()
		//{
		//	var allTypes = m_ReflectedType.GetTypeHierarchy();
		//	var members = new HashSet<MemberInfo>();

		//	foreach ( var type in allTypes )
		//	{
		//		GatherImplementableMembers(type, members);
		//	}
			
		//	return members.ToArray();
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//private static void GatherImplementableMembers(Type type, HashSet<MemberInfo> members)
		//{
		//	var implementableBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		//	var methods = type.GetMethods(implementableBindingFlags).Where(IsImplementableMethod).Cast<MemberInfo>();
		//	var properties = type.GetProperties(implementableBindingFlags).Where(IsImplementableProperty).Cast<MemberInfo>();
		//	var events = type.GetEvents(implementableBindingFlags).Where(IsImplementableEvent).Cast<MemberInfo>();

		//	foreach ( var singleMember in methods.Concat(properties).Concat(events) )
		//	{
		//		members.Add(singleMember);
		//	}
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//private static bool IsImplementableMethod(MethodInfo method)
		//{
		//	return (method.DeclaringType.IsInterface || method.IsAbstract || method.IsVirtual);
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//private static bool IsImplementableProperty(PropertyInfo property)
		//{
		//	if ( property.DeclaringType.IsInterface )
		//	{
		//		return true;
		//	}

		//	var getter = property.GetGetMethod();

		//	if ( getter != null && (getter.IsAbstract || getter.IsVirtual) )
		//	{
		//		return true;
		//	}

		//	var setter = property.GetSetMethod();

		//	if ( setter != null && (setter.IsAbstract || setter.IsVirtual) )
		//	{
		//		return true;
		//	}

		//	return false;
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//private static bool IsImplementableEvent(EventInfo @event)
		//{
		//	if ( @event.DeclaringType.IsInterface )
		//	{
		//		return true;
		//	}

		//	var add = @event.GetAddMethod();

		//	if ( add != null && (add.IsVirtual || add.IsAbstract) )
		//	{
		//		return true;
		//	}

		//	var remove = @event.GetRemoveMethod();

		//	if ( remove != null && (remove.IsVirtual || remove.IsAbstract) )
		//	{
		//		return true;
		//	}

		//	return false;
		//}
	}
}
