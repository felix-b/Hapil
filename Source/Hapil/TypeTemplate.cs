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
		private static readonly HashSet<Type> s_AllTemplateTypes;
		private static readonly Type[] s_ArgumentTemplateTypes;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		static TypeTemplate()
		{
			s_AllTemplateTypes = new HashSet<Type>(new[] {
				typeof(TBase), typeof(TInterface), typeof(TPrimary), typeof(TSecondary1), typeof(TSecondary2),
				typeof(TReturn), typeof(TProperty),
				typeof(TArgument), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7), typeof(TArg8), 
                typeof(TIndex1), typeof(TIndex2), typeof(TItem), typeof(TKey), typeof(TValue),
				typeof(TEventHandler), typeof(TEventArgs),
				typeof(TField),
				typeof(TClosure),
				typeof(TDependency), typeof(TService), typeof(TServiceImpl),
                typeof(TContract), typeof(TImpl), typeof(TContract2), typeof(TImpl2),
                typeof(TAbstract), typeof(TConcrete), typeof(TAbstract2), typeof(TConcrete2),
                typeof(TRequest), typeof(TRequestImpl), typeof(TReply), typeof(TReplyImpl),
                typeof(TStruct), typeof(TStruct2)
			});

			s_ArgumentTemplateTypes = new[] {
				typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7), typeof(TArg8)
			};
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsTemplateType(Type type)
		{
			if ( type.IsGenericType )
			{
				if ( type.GetGenericTypeDefinition() != typeof(TemplateTypeBase<>) )
				{
					return type.GetGenericArguments().Any(IsTemplateType);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return s_AllTemplateTypes.Contains(type);
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
			where TTemplate : TemplateTypeBase<TTemplate>
		{
			return new Scope(typeof(TTemplate), actualType);
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static IDisposable CreateScope<TTemplate1, TTemplate2>(Type actualType1, Type actualType2)
            where TTemplate1 : TemplateTypeBase<TTemplate1>
            where TTemplate2 : TemplateTypeBase<TTemplate2>
        {
            return new Scope(typeof(TTemplate1), actualType1, typeof(TTemplate2), actualType2);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static IDisposable CreateScope<TTemplate1, TTemplate2, TTemplate3>(Type actualType1, Type actualType2, Type actualType3)
            where TTemplate1 : TemplateTypeBase<TTemplate1>
            where TTemplate2 : TemplateTypeBase<TTemplate2>
            where TTemplate3 : TemplateTypeBase<TTemplate3>
        {
            return new Scope(typeof(TTemplate1), actualType1, typeof(TTemplate2), actualType2, typeof(TTemplate3), actualType3);
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
			if ( !s_AllTemplateTypes.Contains(templateType) )
			{
				throw new ArgumentException(string.Format("Type '{0}' is not a valid template type.", templateType.FullName));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        // ReSharper disable InconsistentNaming

		public interface TemplateTypeBase<T> where T : TemplateTypeBase<T>
		{
            //public static Type ActualType
            //{
            //    get
            //    {
            //        return Scope.ValidateCurrent().Resolve(typeof(T));
            //    }
            //}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface TBase : TemplateTypeBase<TBase> { }
        public interface TInterface : TemplateTypeBase<TInterface> { }
        public interface TPrimary : TemplateTypeBase<TPrimary> { }
        public interface TSecondary1 : TemplateTypeBase<TSecondary1> { }
        public interface TSecondary2 : TemplateTypeBase<TSecondary2> { }
        public interface TReturn : TemplateTypeBase<TReturn> { }
        public interface TProperty : TemplateTypeBase<TProperty> { }
        public interface TArgument : TemplateTypeBase<TArgument> { }
        public interface TArg1 : TemplateTypeBase<TArg1> { }
        public interface TArg2 : TemplateTypeBase<TArg2> { }
        public interface TArg3 : TemplateTypeBase<TArg3> { }
        public interface TArg4 : TemplateTypeBase<TArg4> { }
        public interface TArg5 : TemplateTypeBase<TArg5> { }
        public interface TArg6 : TemplateTypeBase<TArg6> { }
        public interface TArg7 : TemplateTypeBase<TArg7> { }
        public interface TArg8 : TemplateTypeBase<TArg8> { }
        public interface TIndex1 : TemplateTypeBase<TIndex1> { }
        public interface TIndex2 : TemplateTypeBase<TIndex2> { }
        public interface TItem : TemplateTypeBase<TItem> { }
        public interface TKey : TemplateTypeBase<TKey> { }
        public interface TValue : TemplateTypeBase<TValue> { }
        public interface TEventHandler : TemplateTypeBase<TEventHandler> { }
        public interface TEventArgs : TemplateTypeBase<TEventArgs> { }
        public interface TField : TemplateTypeBase<TField> { }
        public interface TClosure : TemplateTypeBase<TClosure> { }
        public interface TDependency : TemplateTypeBase<TDependency> { }
        public interface TService : TemplateTypeBase<TService> { }
        public interface TServiceImpl : TService, TemplateTypeBase<TServiceImpl> { }
        public interface TContract : TemplateTypeBase<TContract> { }
        public interface TImpl : TContract, TemplateTypeBase<TImpl> { }
        public interface TContract2 : TemplateTypeBase<TContract2> { }
        public interface TImpl2 : TContract2, TemplateTypeBase<TImpl2> { }
        public interface TAbstract : TemplateTypeBase<TAbstract> { }
        public interface TConcrete : TAbstract, TemplateTypeBase<TConcrete> { }
        public interface TAbstract2 : TemplateTypeBase<TAbstract2> { }
        public interface TConcrete2 : TAbstract2, TemplateTypeBase<TConcrete2> { }
        public interface TRequest : TemplateTypeBase<TRequest> { }
        public interface TRequestImpl : TRequest, TemplateTypeBase<TRequestImpl> { }
        public interface TReply : TemplateTypeBase<TReply> { }
        public interface TReplyImpl : TReply, TemplateTypeBase<TReplyImpl> { }
        public struct TStruct : TemplateTypeBase<TStruct> { }
        public struct TStruct2 : TemplateTypeBase<TStruct2> { }

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
				if ( templateType.IsGenericType )
				{
					return TryResolveGenericType(templateType);
				}

				Type actualType;

				if ( m_ActualTypesByTemplates.TryGetValue(templateType, out actualType) )
				{
					return actualType;
				}
				else if ( m_Outer != null )
				{
					return m_Outer.TryResolve(templateType);
				}
				else
				{
					return null;
				}
			}

            //-------------------------------------------------------------------------------------------------------------------------------------------------

			private Type TryResolveGenericType(Type type)
			{
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
                        effectiveTemplates[template.Key] = template.Value;
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
