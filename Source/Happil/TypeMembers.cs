using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;

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

		public MethodSelector SelectAllMethods(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods, where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectVoids(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(void)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectVoids<TArg1>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(void), typeof(TArg1)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectVoids<TArg1, TArg2>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(void), typeof(TArg1), typeof(TArg2)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectVoids<TArg1, TArg2, TArg3>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(void), typeof(TArg1), typeof(TArg2), typeof(TArg3)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectFuncs<TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(TReturn)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectFuncs<TArg1, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(TReturn), typeof(TArg1)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectFuncs<TArg1, TArg2, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(TReturn), typeof(TArg1), typeof(TArg2)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSelector SelectFuncs<TArg1, TArg2, TArg3, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector(m_Methods.OfSignature(typeof(TReturn), typeof(TArg1), typeof(TArg2), typeof(TArg3)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelector SelectAllProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector(m_Properties, where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertySelector SelectProps<TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector(m_Properties.OfSignature(typeof(TProperty)), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventSelector SelectAllEvents(Func<EventInfo, bool> where = null)
		{
			return new EventSelector(m_Events, where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventSelector SelectEvents<TEventHandler>(Func<EventInfo, bool> where = null)
		{
			return new EventSelector(m_Events.Where(ev => ev.EventHandlerType == TypeTemplate.Resolve<TEventHandler>()), where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldSelector SelectAllFields(Func<FieldInfo, bool> where = null)
		{
			return new FieldSelector(m_Fields, where);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldSelector SelectFields<TField>(Func<FieldInfo, bool> where = null)
		{
			return new FieldSelector(m_Fields.Where(f => f.FieldType == TypeTemplate.Resolve<TField>()), where);
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class BaseSelector<TMemberInfo> where TMemberInfo : MemberInfo
		{
			private readonly TMemberInfo[] m_SelectedMembers;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected BaseSelector(IEnumerable<TMemberInfo> preSelectedMembers, Func<TMemberInfo, bool> optionalPredicate)
			{
				m_SelectedMembers = preSelectedMembers.SelectIf(optionalPredicate).ToArray();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void ForEach(Action<TMemberInfo> action)
			{
				foreach ( var member in m_SelectedMembers )
				{
					using ( CreateTemplateScope(member) )
					{
						action(member);
					}
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void ForEach(MemberAction action)
			{
				for ( int index = 0 ; index < m_SelectedMembers.Length ; index++ )
				{
					var member = m_SelectedMembers[index];

					using ( CreateTemplateScope(member) )
					{
						action(index, m_SelectedMembers.Length, member);
					}
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TMemberInfo Single()
			{
				return m_SelectedMembers.Single();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TMemberInfo[] ToArray()
			{
				return m_SelectedMembers.ToArray();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected virtual IDisposable CreateTemplateScope(TMemberInfo member)
			{
				return null;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public delegate void MemberAction(int index, int count, TMemberInfo member);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class MethodSelector : BaseSelector<MethodInfo>
		{
			public MethodSelector(IEnumerable<MethodInfo> preSelectedMembers, Func<MethodInfo, bool> optionalPredicate)
				: base(preSelectedMembers, optionalPredicate)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IDisposable CreateTemplateScope(MethodInfo method)
			{
				var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
				var templateTypePairs = new Type[2 * (1 + parameterTypes.Length)];

				templateTypePairs[0] = typeof(TypeTemplate.TReturn);
				templateTypePairs[1] = method.ReturnType;

				TypeTemplate.BuildArgumentsTypePairs(parameterTypes, templateTypePairs, arrayStartIndex: 2);
				return TypeTemplate.CreateScope(templateTypePairs);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class PropertySelector : BaseSelector<PropertyInfo>
		{
			public PropertySelector(IEnumerable<PropertyInfo> preSelectedMembers, Func<PropertyInfo, bool> optionalPredicate)
				: base(preSelectedMembers, optionalPredicate)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IDisposable CreateTemplateScope(PropertyInfo property)
			{
				var parameterTypes = property.GetIndexParameters().Select(p => p.ParameterType).ToArray();
				var templateTypePairs = new Type[2 * (1 + parameterTypes.Length)];

				templateTypePairs[0] = typeof(TypeTemplate.TProperty);
				templateTypePairs[1] = property.PropertyType;

				if ( parameterTypes.Length > 0 )
				{
					TypeTemplate.BuildArgumentsTypePairs(parameterTypes, templateTypePairs, arrayStartIndex: 2);
				}

				return TypeTemplate.CreateScope(templateTypePairs);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class EventSelector : BaseSelector<EventInfo>
		{
			public EventSelector(IEnumerable<EventInfo> preSelectedMembers, Func<EventInfo, bool> optionalPredicate)
				: base(preSelectedMembers, optionalPredicate)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IDisposable CreateTemplateScope(EventInfo @event)
			{
				return TypeTemplate.CreateScope<TypeTemplate.TEventHandler>(@event.EventHandlerType);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class FieldSelector : BaseSelector<FieldInfo>
		{
			public FieldSelector(IEnumerable<FieldInfo> preSelectedMembers, Func<FieldInfo, bool> optionalPredicate)
				: base(preSelectedMembers, optionalPredicate)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IDisposable CreateTemplateScope(FieldInfo field)
			{
				return TypeTemplate.CreateScope<TypeTemplate.TField>(field.FieldType);
			}
		}
	}
}
