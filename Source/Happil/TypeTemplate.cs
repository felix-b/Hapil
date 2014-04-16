using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Happil
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
				typeof(TBase), typeof(TPrimary), typeof(TSecondary1), typeof(TSecondary2),
				typeof(TReturn), typeof(TProperty),
				typeof(TArgument), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6), typeof(TArg7), typeof(TArg8), 
				typeof(TIndex1), typeof(TIndex2),
				typeof(TEventHandler), typeof(TEventArgs),
				typeof(TField),
				typeof(TClosure)
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
			if ( IsTemplateType(type) )
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

		private static void ValidateTemplateType(Type templateType)
		{
			if ( !s_AllTemplateTypes.Contains(templateType) )
			{
				throw new ArgumentException(string.Format("Type '{0}' is not a valid template type.", templateType.FullName));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class TemplateTypeBase<T> where T : TemplateTypeBase<T>
		{
			public static Type ActualType
			{
				get
				{
					return Scope.ValidateCurrent().Resolve(typeof(T));
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		// ReSharper disable InconsistentNaming

		public class TBase : TemplateTypeBase<TBase> { }
		public class TPrimary : TemplateTypeBase<TPrimary> { }
		public class TSecondary1 : TemplateTypeBase<TSecondary1> { }
		public class TSecondary2 : TemplateTypeBase<TSecondary2> { }
		public class TReturn : TemplateTypeBase<TReturn> { }
		public class TProperty : TemplateTypeBase<TProperty> { }
		public class TArgument : TemplateTypeBase<TArgument> { }
		public class TArg1 : TemplateTypeBase<TArg1> { }
		public class TArg2 : TemplateTypeBase<TArg2> { }
		public class TArg3 : TemplateTypeBase<TArg3> { }
		public class TArg4 : TemplateTypeBase<TArg4> { }
		public class TArg5 : TemplateTypeBase<TArg5> { }
		public class TArg6 : TemplateTypeBase<TArg6> { }
		public class TArg7 : TemplateTypeBase<TArg7> { }
		public class TArg8 : TemplateTypeBase<TArg8> { }
		public class TIndex1 : TemplateTypeBase<TIndex1> { }
		public class TIndex2 : TemplateTypeBase<TIndex2> { }
		public class TEventHandler : TemplateTypeBase<TEventHandler> { }
		public class TEventArgs : TemplateTypeBase<TEventArgs> { }
		public class TField : TemplateTypeBase<TField> { }
		public class TClosure : TemplateTypeBase<TClosure> { }

		// ReSharper restore InconsistentNaming

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Scope : IDisposable
		{
			private readonly Scope m_Outer;
			private readonly Dictionary<Type, Type> m_ActualTypesByTemplates;

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
