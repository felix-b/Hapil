using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hapil
{
	/// <summary>
	/// Contains type parameters for implementation of multiple members by a template.
	/// </summary>
	public static class TypeTemplate
	{
		private static readonly Type[] s_ArgumentTemplateTypes;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static TypeTemplate()
		{
			s_ArgumentTemplateTypes = new[] {
				typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7), typeof(TArg8)
			};
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsTemplateType(Type type)
		{
            if ( type.IsGenericType )
            {
                if ( type.GetGenericTypeDefinition() != typeof(ITemplateType<>) )
                {
                    return type.GetGenericArguments().Any(IsTemplateType);
                }
                else
                {
                    return false;
                }
			}
            else if ( type.IsArray )
            {
                return IsTemplateType(type.GetElementType());
            }
            else if ( type != typeof(ITemplateType) )
            {
                return typeof(ITemplateType).IsAssignableFrom(type);
            }
            else
            {
                return false;
            }
        }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsDefined(Type templateType)
		{
			ValidateTemplateType(templateType);
			
			var scope = Scope.Current;
			return (scope != null && scope.TryResolve(templateType) != null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type Resolve(Type type)
		{
			if ( type != null && IsTemplateType(type) )
			{
				return Scope.ValidateCurrent().Resolve(type);
			}
			else
			{
				return type;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type TryResolve(Type type)
		{
			if ( IsTemplateType(type) )
			{
				return Scope.ValidateCurrent().TryResolve(type);
			}
			else
			{
				return type;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type Resolve<T>()
		{
			return Resolve(typeof(T));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type TryResolve<T>()
		{
			return TryResolve(typeof(T));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static IDisposable CreateScope<TTemplate>(Type actualType)
			where TTemplate : ITemplateType<TTemplate>
		{
			return new Scope(typeof(TTemplate), actualType);
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static IDisposable CreateScope<TTemplate1, TTemplate2>(Type actualType1, Type actualType2)
            where TTemplate1 : ITemplateType<TTemplate1>
            where TTemplate2 : ITemplateType<TTemplate2>
        {
            return new Scope(typeof(TTemplate1), actualType1, typeof(TTemplate2), actualType2);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static IDisposable CreateScope<TTemplate1, TTemplate2, TTemplate3>(Type actualType1, Type actualType2, Type actualType3)
            where TTemplate1 : ITemplateType<TTemplate1>
            where TTemplate2 : ITemplateType<TTemplate2>
            where TTemplate3 : ITemplateType<TTemplate3>
        {
            return new Scope(typeof(TTemplate1), actualType1, typeof(TTemplate2), actualType2, typeof(TTemplate3), actualType3);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static IDisposable CreateScope<TTemplate1, TTemplate2, TTemplate3, TTemplate4>(
            Type actualType1, 
            Type actualType2, 
            Type actualType3, 
            Type actualType4)
            where TTemplate1 : ITemplateType<TTemplate1>
            where TTemplate2 : ITemplateType<TTemplate2>
            where TTemplate3 : ITemplateType<TTemplate3>
            where TTemplate4 : ITemplateType<TTemplate4>
        {
            return new Scope(typeof(TTemplate1), actualType1, typeof(TTemplate2), actualType2, typeof(TTemplate3), actualType3, typeof(TTemplate4), actualType4);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static IDisposable CreateScope(params Type[] templateActualTypePairs)
		{
			return new Scope(templateActualTypePairs);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static object ResolveValue(object value)
		{
			if ( value != null && IsTemplateType(value.GetType()) )
			{
				return Scope.Current.Resolve(value.GetType()).GetDefaultValue();
			}
			else
			{
				return value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static void BuildArgumentsTypePairs(Type[] parameterTypes, Type[] pairArray, int arrayStartIndex)
		{
			for ( int i = 0 ; i < parameterTypes.Length && i < s_ArgumentTemplateTypes.Length ; i++ )
			{
				pairArray[arrayStartIndex + (i * 2)] = s_ArgumentTemplateTypes[i];
				pairArray[arrayStartIndex + (i * 2) + 1] = parameterTypes[i];
			}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        internal static IDisposable Save()
        {
            return Scope.Save();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        internal static IDisposable Restore(IDisposable saved)
	    {
            Scope.Restore((Scope)saved);
            return saved;
	    }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void ValidateTemplateType(Type templateType)
		{
            if ( !typeof(ITemplateType).IsAssignableFrom(templateType) )
			{
				throw new ArgumentException(string.Format("Type '{0}' is not a valid template type.", templateType.FullName));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public interface ITemplateType
	    {
	    }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface ITemplateType<T> : ITemplateType 
            where T : ITemplateType<T>
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        // ReSharper disable InconsistentNaming

        public interface TBase : ITemplateType<TBase> { }
        public interface TInterface : ITemplateType<TInterface> { }
        public interface TPrimary : ITemplateType<TPrimary> { }
        public interface TSecondary1 : ITemplateType<TSecondary1> { }
        public interface TSecondary2 : ITemplateType<TSecondary2> { }
        public interface TReturn : ITemplateType<TReturn> { }
        public interface TProperty : ITemplateType<TProperty> { }
        public interface TArgument : ITemplateType<TArgument> { }
        public interface TArg1 : ITemplateType<TArg1> { }
        public interface TArg2 : ITemplateType<TArg2> { }
        public interface TArg3 : ITemplateType<TArg3> { }
        public interface TArg4 : ITemplateType<TArg4> { }
        public interface TArg5 : ITemplateType<TArg5> { }
        public interface TArg6 : ITemplateType<TArg6> { }
        public interface TArg7 : ITemplateType<TArg7> { }
        public interface TArg8 : ITemplateType<TArg8> { }
        public interface TIndex1 : ITemplateType<TIndex1> { }
        public interface TIndex2 : ITemplateType<TIndex2> { }
        public interface TItem : ITemplateType<TItem> { }
        public interface TKey : ITemplateType<TKey> { }
        public interface TValue : ITemplateType<TValue> { }
        public interface TEventHandler : ITemplateType<TEventHandler> { }
        public interface TEventArgs : ITemplateType<TEventArgs> { }
        public interface TField : ITemplateType<TField> { }
        public interface TClosure : ITemplateType<TClosure> { }
        public interface TDependency : ITemplateType<TDependency> { }
        public interface TService : ITemplateType<TService> { }
        public interface TServiceImpl : TService, ITemplateType<TServiceImpl> { }
        public interface TContract : ITemplateType<TContract> { }
        public interface TImpl : TContract, ITemplateType<TImpl> { }
        public interface TContract2 : ITemplateType<TContract2> { }
        public interface TImpl2 : TContract2, ITemplateType<TImpl2> { }
        public interface TAbstract : ITemplateType<TAbstract> { }
        public interface TConcrete : TAbstract, ITemplateType<TConcrete> { }
        public interface TAbstract2 : ITemplateType<TAbstract2> { }
        public interface TConcrete2 : TAbstract2, ITemplateType<TConcrete2> { }
        public interface TRequest : ITemplateType<TRequest> { }
        public interface TRequestImpl : TRequest, ITemplateType<TRequestImpl> { }
        public interface TReply : ITemplateType<TReply> { }
        public interface TReplyImpl : TReply, ITemplateType<TReplyImpl> { }
        public interface TCollection<T> : ICollection<T>, ITemplateType<TCollection<T>> { }
        public interface TContractCollection<T> : ICollection<T>, ITemplateType<TContractCollection<T>> { }
        public interface TImplCollection<T> : ICollection<T>, ITemplateType<TImplCollection<T>> { }
        public interface TAbstractCollection<T> : ICollection<T>, ITemplateType<TAbstractCollection<T>> { }
        public interface TConcreteCollection<T> : ICollection<T>, ITemplateType<TConcreteCollection<T>> { }
        public struct TStruct : ITemplateType<TStruct> { }
        public struct TStruct2 : ITemplateType<TStruct2> { }

		// ReSharper restore InconsistentNaming

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class Scope : IDisposable
		{
            private readonly Dictionary<Type, Type> m_ActualTypesByTemplates;
            private Scope m_Outer;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Scope(params Type[] templateActualTypePairs)
			{
				m_Outer = s_Current;
				m_ActualTypesByTemplates = new Dictionary<Type, Type>();

				for ( int i = 0 ; i < templateActualTypePairs.Length - 1 ; i += 2 )
				{
					if ( templateActualTypePairs[i] != null && templateActualTypePairs[i + 1] != null )
					{
						m_ActualTypesByTemplates.Add(templateActualTypePairs[i], templateActualTypePairs[i + 1]);
					}
				}

				s_Current = this;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

            public Scope(Dictionary<Type, Type> actualTypesByTemplates)
            {
                m_ActualTypesByTemplates = actualTypesByTemplates;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IDisposable Members

			public void Dispose()
			{
				if ( ValidateCurrent() != this )
				{
					throw new InvalidOperationException("Template type scopes are not balanced!");
				}

				s_Current = m_Outer;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------
	
			public Type Resolve(Type templateType)
			{
				var actualType = TryResolve(templateType);

				if ( actualType != null )
				{
					return actualType;
				}

				throw new ArgumentException(string.Format("Template type '{0}' is not defined in the current scope.", templateType.Name));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Type TryResolve(Type templateType)
			{
                Type actualType;
                
                if ( m_ActualTypesByTemplates.TryGetValue(templateType, out actualType) )
				{
					return actualType;
				}

			    if ( templateType.IsGenericType )
			    {
			        return TryResolveGenericType(templateType);
			    }

			    if ( templateType.IsArray )
			    {
			        return TryResolveArrayType(templateType);
			    }

			    if ( m_Outer != null )
				{
					return m_Outer.TryResolve(templateType);
				}

                return null;
			}

            //-------------------------------------------------------------------------------------------------------------------------------------------------

			private Type TryResolveGenericType(Type type)
			{
                //var definition = type.GetGenericTypeDefinition();
                //var resolvedDefinition = (IsTemplateType(definition) ? TryResolve(definition) : definition);

                //if ( resolvedDefinition == null )
                //{
                //    return null;
                //}

				var typeArguments = type.GetGenericArguments();

				for ( int i = 0 ; i < typeArguments.Length ; i++ )
				{
					if ( IsTemplateType(typeArguments[i]) )
					{
						var resolvedTypeArgument = TryResolve(typeArguments[i]);

						if ( resolvedTypeArgument == null )
						{
							return null;
						}
					
						typeArguments[i] = resolvedTypeArgument;
					}

				}

                return type.GetGenericTypeDefinition().MakeGenericType(typeArguments);
			}

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private Type TryResolveArrayType(Type type)
            {
                var resolvedElementType = TryResolve(type.GetElementType());

                if ( resolvedElementType != null )
                {
                    return resolvedElementType.MakeArrayType();
                }
                else
                {
                    return null;
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

			[ThreadStatic]
			private static Scope s_Current;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Scope ValidateCurrent()
			{
				var currentScope = s_Current;

				if ( currentScope == null )
				{
					throw new InvalidOperationException("Type template is not defined in the current scope.");
				}

				return currentScope;
			}

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static Scope Save()
            {
                var effectiveTemplates = new Dictionary<Type, Type>();

                for ( var scope = s_Current ; scope != null ; scope = scope.m_Outer )
                {
                    foreach ( var template in scope.m_ActualTypesByTemplates )
                    {
                        if ( !effectiveTemplates.ContainsKey(template.Key) )
                        {
                            effectiveTemplates.Add(template.Key, template.Value);
                        }
                    }
                }

                return new Scope(effectiveTemplates);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public static void Restore(Scope saved)
            {
                if ( saved != null )
                {
                    saved.m_Outer = s_Current;
                    s_Current = saved;
                }
            }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static Scope Current
			{
				get
				{
					return s_Current;
				}
			}
		}
	}
}
