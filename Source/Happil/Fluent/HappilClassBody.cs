using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
			return new HappilClassBody<T>(m_HappilClass, typeof(T));
		}

		public IHappilClassBody<TBase> Implement(Type interfaceType, params Func<IHappilClassBody<object>, IHappilClassBody<TBase>>[] members)
		{
			m_HappilClass.ImplementInterface(interfaceType);
			return new HappilClassBody<TBase>(m_HappilClass, interfaceType);
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

		public HappilField<T> Field<T>(string name)
		{
			var field = new HappilField<T>(m_HappilClass, name);
			m_HappilClass.RegisterMember(field);
			return field;
		}

		public IHappilClassBody<TBase> Field<T>(string name, out HappilField<T> field)
		{
			field = this.Field<T>(name);
			return this;
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

		public IHappilClassBody<TBase> DefaultConstructor()
		{
			return DefineConstructor(ctor => ctor.Base());
		}

		public IHappilClassBody<TBase> Constructor(
			Action<IHappilConstructorBody> body)
		{
			return DefineConstructor(body);
		}

		public IHappilClassBody<TBase> Constructor<TArg1>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>> body)
		{
			return DefineConstructor(
				(ctor) => body(ctor, new HappilArgument<TArg1>(ctor, 1)),
				typeof(TArg1));
		}

		public IHappilClassBody<TBase> Constructor<TArg1, TArg2>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>> body)
		{
			return DefineConstructor(
				(ctor) => body(ctor, new HappilArgument<TArg1>(ctor, 1), new HappilArgument<TArg2>(ctor, 2)),
				typeof(TArg1), typeof(TArg2));

			//var argumentTypes = new[] { typeof(TArg1), typeof(TArg2) };
			//var constructorMember = new HappilConstructor(m_HappilClass, argumentTypes);

			//m_HappilClass.RegisterMember(constructorMember, bodyDefinition: () => {
			//	using ( constructorMember.CreateBodyScope() )
			//	{
			//		body(constructorMember, new HappilArgument<TArg1>(constructorMember, 1), new HappilArgument<TArg2>(constructorMember, 2));
			//	}
			//});

			//m_HappilClass.DefineFactoryMethod(constructorMember.ConstructorInfo, argumentTypes);
			//return this;
		}

		private IHappilClassBody<TBase> DefineConstructor(Action<HappilConstructor> invokeBodyDefinition, params Type[] argumentTypes)
		{
			var constructorMember = new HappilConstructor(m_HappilClass, argumentTypes);

			m_HappilClass.RegisterMember(constructorMember, bodyDefinition: () => {
				using ( constructorMember.CreateBodyScope() )
				{
					invokeBodyDefinition(constructorMember);
				}
			});

			m_HappilClass.DefineFactoryMethod(constructorMember.ConstructorInfo, argumentTypes);
			return this;
		}

		public IHappilClassBody<TBase> Constructor<TArg1, TArg2, TArg3>(
			Action<IHappilConstructorBody, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> VoidMethod(Expression<Func<TBase, Action>> method, Action<IVoidHappilMethodBody> body)
		{
			var methodMember = new VoidHappilMethod(m_HappilClass, GetMethodInfoFromLambda(method));
			
			m_HappilClass.RegisterMember(
				methodMember,
				bodyDefinition: () => DefineMethodBodyInScope(methodMember, body));
			
			return this;
		}

		public IHappilClassBody<TBase> VoidMethod<TArg1>(Expression<Func<TBase, Action<TArg1>>> method, Action<IVoidHappilMethodBody, HappilArgument<TArg1>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> VoidMethod<TArg1, TArg2>(Expression<Func<TBase, Action<TArg1, TArg2>>> method, Action<IVoidHappilMethodBody, HappilArgument<TArg1>, HappilArgument<TArg2>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> VoidMethod<TArg1, TArg2, TArg3>(Expression<Func<TBase, Action<TArg1, TArg2, TArg3>>> method, Action<IVoidHappilMethodBody, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Function<TReturn>(Expression<Func<TBase, Func<TReturn>>> method, Action<IHappilMethodBody<TReturn>> body)
		{
			var methodMember = new HappilMethod<TReturn>(m_HappilClass, GetMethodInfoFromLambda(method));

			m_HappilClass.RegisterMember(methodMember, bodyDefinition: () => {
				using ( methodMember.CreateBodyScope() )
				{
					body(methodMember);
				}
			});					

			return this;
		}

		public IHappilClassBody<TBase> Function<TArg1, TReturn>(Expression<Func<TBase, Func<TArg1, TReturn>>> method, Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Function<TArg1, TArg2, TReturn>(Expression<Func<TBase, Func<TArg1, TArg2, TReturn>>> method, Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>, HappilArgument<TArg1>> body)
		{
			throw new NotImplementedException();
		}

		public IHappilClassBody<TBase> Methods(Action<IVoidHappilMethodBody> body)
		{
			foreach ( var methodInfo in m_ReflectedType.GetMethods().Where(m => m.ReturnType == typeof(void)) )
			{
				var method = new VoidHappilMethod(m_HappilClass, methodInfo);
				
				m_HappilClass.RegisterMember(method, bodyDefinition: () => {
					using ( method.CreateBodyScope() )
					{
						body(method);
					}
				});
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
		
		private static MethodInfo GetMethodInfoFromLambda(LambdaExpression lambda)
		{
			var createDelegateCall = (MethodCallExpression)(((UnaryExpression)lambda.Body).Operand);
			var methodDeclaration = (MethodInfo)((ConstantExpression)createDelegateCall.Arguments[2]).Value;
			return methodDeclaration;
		}
	}
}
