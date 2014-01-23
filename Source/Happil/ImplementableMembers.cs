using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil
{
	public sealed class ImplementableMembers
	{
		private readonly Type m_ReflectedType;
		private readonly Type[] m_TypeHierarchy;
		private readonly MethodInfo[] m_Methods;
		private readonly PropertyInfo[] m_Properties;
		private readonly EventInfo[] m_Events;
		private readonly MemberInfo[] m_Members;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ImplementableMembers(Type reflectedType)
		{
			m_ReflectedType = reflectedType;
			m_TypeHierarchy = reflectedType.GetTypeHierarchy();

			var implementableBindingFlags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			m_Methods = m_TypeHierarchy.SelectMany(t => t.GetMethods(implementableBindingFlags).Where(IsImplementableMethod)).ToArray();
			m_Properties = m_TypeHierarchy.SelectMany(t => t.GetProperties(implementableBindingFlags).Where(IsImplementableProperty)).ToArray();
			m_Events = m_TypeHierarchy.SelectMany(t => t.GetEvents(implementableBindingFlags).Where(IsImplementableEvent)).ToArray();
			m_Members = m_Methods.Cast<MemberInfo>().Concat(m_Properties).Concat(m_Events).ToArray();
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

		public MethodInfo[] Methods
		{
			get { return m_Methods; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyInfo[] Properties
		{
			get { return m_Properties; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventInfo[] Events
		{
			get { return m_Events; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MemberInfo[] Members
		{
			get { return m_Members; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly ConcurrentDictionary<Type, ImplementableMembers> s_ImplementableMembersByReflectedType = 
			new ConcurrentDictionary<Type, ImplementableMembers>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static ImplementableMembers Of<T>()
		{
			return Of(typeof(T));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static ImplementableMembers Of(Type reflectedType)
		{
			return s_ImplementableMembersByReflectedType.GetOrAdd(
				reflectedType, 
				key => new ImplementableMembers(key));
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
