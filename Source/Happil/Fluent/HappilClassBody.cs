using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	internal class HappilClassBody<TBase> : IHappilClassBody<TBase>, IHappilClassDefinition, IHappilClassDefinitionInternals
	{
		private readonly HappilClass m_HappilClass;
		private readonly Type m_ReflectedType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClassBody(HappilClass happilClass, Type reflectedType = null)
		{
			m_HappilClass = happilClass;
			m_ReflectedType = reflectedType ?? typeof(TBase);
		}

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

		public IHappilClassBody<TBase> Inherit(Type baseType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members)
		{
			return new HappilClassBody<TBase>(m_HappilClass, baseType);
		}

		public IHappilClassBody<TBase> Implement(Type interfaceType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members)
		{
			m_HappilClass.ImplementInterface(interfaceType);
			return new HappilClassBody<TBase>(m_HappilClass, interfaceType);
		}

		public IHappilClassBody<TClass> Inherit<TClass>(params Func<IHappilClassBody<TClass>, IHappilClassBody<TBase>>[] members)
		{
			return new HappilClassBody<TClass>(m_HappilClass);
		}

		public IHappilClassBody<TClass> Inherit<TClass>(Type baseType, params Func<IHappilClassBody<TClass>, IHappilClassBody<TBase>>[] members)
		{
			return new HappilClassBody<TClass>(m_HappilClass);
		}

		public IHappilClassBody<TInterface> Implement<TInterface>(params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members)
		{
			m_HappilClass.ImplementInterface(typeof(TInterface));
			return new HappilClassBody<TInterface>(m_HappilClass);
		}

		public IHappilClassBody<TInterface> Implement<TInterface>(Type interfaceType, params Func<IHappilClassBody<TInterface>, IHappilClassBody<TBase>>[] members)
		{
			m_HappilClass.ImplementInterface(interfaceType);
			return new HappilClassBody<TInterface>(m_HappilClass, interfaceType);
		}

		public IHappilClassBody<TBase> DefaultConstructor()
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Property<T>(System.Linq.Expressions.Expression<Func<TBase, T>> selector, Func<HappilProperty<T>, HappilPropertyGetter> getter, Func<HappilProperty<T>, HappilPropertySetter> setter = null)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Properties<T>(Func<HappilProperty<T>, HappilPropertyGetter> getter, Func<HappilProperty<T>, HappilPropertySetter> setter = null)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Properties<T>(Func<System.Reflection.PropertyInfo, bool> where, Func<HappilProperty<T>, HappilPropertyGetter> getter, Func<HappilProperty<T>, HappilPropertySetter> setter = null)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> AutomaticProperties()
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> AutomaticProperties(Func<System.Reflection.PropertyInfo, bool> where)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Method(Action<TBase> method, Action<IVoidHappilMethodBody> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Method<T1>(Action<TBase, T1> method, Action<IVoidHappilMethodBody, HappilArgument<T1>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Method<T1, T2>(Action<TBase, T1, T2> method, Action<IVoidHappilMethodBody, HappilArgument<T1>, HappilArgument<T2>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Method<T1, T2, T3>(Action<TBase, T1, T2, T3> method, Action<IVoidHappilMethodBody, HappilArgument<T1>, HappilArgument<T2>, HappilArgument<T3>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Method<TReturn>(Func<TBase, TReturn> method, Action<IHappilMethodBody<TReturn>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Methods(Action<IVoidHappilMethodBody> body)
		{
			foreach ( var methodInfo in m_ReflectedType.GetMethods() )//.Where(m => m.ReturnType == typeof(void)) )
			{
				m_HappilClass.DefineMethod(methodInfo);
			}

			return this;
		}

		public IHappilClassBody<TBase> Methods<TReturn>(Action<IHappilMethodBody<TReturn>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Methods<TReturn>(Func<System.Reflection.MethodInfo, bool> where, Action<IHappilMethodBody<TReturn>> body)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
