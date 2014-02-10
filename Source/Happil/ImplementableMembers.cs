using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil
{
	public sealed class TypeMembers
	{
		private readonly Type m_ReflectedType;
		private readonly Type[] m_TypeHierarchy;
		private readonly FieldInfo[] m_Fields;
		private readonly MethodInfo[] m_Methods;
		private readonly MethodInfo[] m_ImplementableMethods;
		private readonly PropertyInfo[] m_Properties;
		private readonly PropertyInfo[] m_ImplementableProperties;
		private readonly EventInfo[] m_Events;
		private readonly EventInfo[] m_ImplementableEvents;
		private readonly MemberInfo[] m_Members;
		private readonly MemberInfo[] m_ImplementableMembers;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TypeMembers(Type reflectedType)
		{
			m_ReflectedType = reflectedType;
			m_TypeHierarchy = reflectedType.GetTypeHierarchy();

			var implementableBindingFlags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			m_Fields = m_TypeHierarchy.SelectMany(t => t.GetFields(implementableBindingFlags)).ToArray();
			m_Methods = m_TypeHierarchy.SelectMany(t => t.GetMethods(implementableBindingFlags)).ToArray();
			m_Properties = m_TypeHierarchy.SelectMany(t => t.GetProperties(implementableBindingFlags)).ToArray();
			m_Events = m_TypeHierarchy.SelectMany(t => t.GetEvents(implementableBindingFlags)).ToArray();
			m_Members = m_Methods.Cast<MemberInfo>().Concat(m_Properties).Concat(m_Events).ToArray();

			m_ImplementableMethods = m_Methods.Where(IsImplementableMethod).ToArray();
			m_ImplementableProperties = m_Properties.Where(IsImplementableProperty).ToArray();
			m_ImplementableEvents = m_Events.Where(IsImplementableEvent).ToArray();
			m_ImplementableMembers = m_ImplementableMethods.Cast<MemberInfo>().Concat(m_ImplementableProperties).Concat(m_ImplementableEvents).ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type ReflectedType
		{
			get { return m_ReflectedType; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type[] TypeHierarchy
		{
			get { return m_TypeHierarchy; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldInfo[] Fields
		{
			get { return m_Fields; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo[] Methods
		{
			get { return m_Methods; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodInfo[] ImplementableMethods
		{
			get { return m_ImplementableMethods; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyInfo[] Properties
		{
			get { return m_Properties; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyInfo[] ImplementableProperties
		{
			get { return m_ImplementableProperties; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventInfo[] Events
		{
			get { return m_Events; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventInfo[] ImplementableEvents
		{
			get { return m_ImplementableEvents; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MemberInfo[] ImplementableMembers
		{
			get { return m_ImplementableMembers; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, TypeMembers> s_TypeMembersByReflectedType = 
			new ConcurrentDictionary<Type, TypeMembers>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static TypeMembers Of<T>()
		{
			return Of(typeof(T));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static TypeMembers Of(Type reflectedType)
		{
			return s_TypeMembersByReflectedType.GetOrAdd(
				reflectedType, 
				key => new TypeMembers(key));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static bool IsImplementableMethod(MethodInfo method)
		{
			return (method.DeclaringType.IsInterface || method.IsAbstract || method.IsVirtual);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static bool IsImplementableProperty(PropertyInfo property)
		{
			if ( property.DeclaringType.IsInterface )
			{
				return true;
			}

			var getter = property.GetGetMethod();

			if ( getter != null && (getter.IsAbstract || getter.IsVirtual) )
			{
				return true;
			}

			var setter = property.GetSetMethod();

			if ( setter != null && (setter.IsAbstract || setter.IsVirtual) )
			{
				return true;
			}

			return false;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static bool IsImplementableEvent(EventInfo @event)
		{
			if ( @event.DeclaringType.IsInterface )
			{
				return true;
			}

			var add = @event.GetAddMethod();

			if ( add != null && (add.IsVirtual || add.IsAbstract) )
			{
				return true;
			}

			var remove = @event.GetRemoveMethod();

			if ( remove != null && (remove.IsVirtual || remove.IsAbstract) )
			{
				return true;
			}

			return false;
		}
	}
}
