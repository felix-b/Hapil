using System;
using Happil.Fluent;
using Happil.Statements;

namespace Happil
{
	/// <summary>
	/// Serves as type parameter for non-typed member selectors.
	/// </summary>
	public class TypeTemplate
	{
		private readonly Type m_CastType;
		private readonly object m_CastValue;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private TypeTemplate(Type castType, object castValue)
		{
			m_CastType = castType;
			m_CastValue = castValue;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type CastType
		{
			get
			{
				return m_CastType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public object CastValue
		{
			get
			{
				return m_CastValue;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[ThreadStatic]
		private static Scope s_CurrentScope;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static TypeTemplate Cast<T>(T constantValue)
		{
			return new TypeTemplate(typeof(T), constantValue);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsDefined
		{
			get
			{
				return (s_CurrentScope != null);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type Type
		{
			get
			{
				var scope = ValidateCurrentScope();
				return scope.TemplateType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilConstant<TypeTemplate> DefaultValue
		{
			get
			{
				var scope = ValidateCurrentScope();
				return new HappilConstant<TypeTemplate>(new TypeTemplate(scope.TemplateType, scope.TemplateType.GetDefaultValue()));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static IDisposable CreateScope(Type templateType)
		{
			if ( s_CurrentScope != null )
			{
				throw new InvalidOperationException("Type template scope is already defined.");
			}

			var scope = new Scope(templateType);
			s_CurrentScope = scope;

			return scope;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static Type Resolve<T>()
		{
			return Resolve(typeof(T));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static Type Resolve(Type type)
		{
			if ( type == typeof(TypeTemplate) )
			{
				return TypeTemplate.Type;
			}
			else
			{
				return type;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static Type TryResolve<T>()
		{
			return TryResolve(typeof(T));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal static Type TryResolve(Type type)
		{
			if ( type == typeof(TypeTemplate) )
			{
				return (TypeTemplate.IsDefined ? TypeTemplate.Type : null);
			}
			else
			{
				return type;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static Scope ValidateCurrentScope()
		{
			var currentScope = s_CurrentScope;

			if ( currentScope == null )
			{
				throw new InvalidOperationException("Type template is not defined in the current scope.");
			}

			return currentScope;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Scope : IDisposable
		{
			private readonly Type m_TemplateType;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Scope(Type templateType)
			{
				m_TemplateType = templateType;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IDisposable Members

			public void Dispose()
			{
				if ( ValidateCurrentScope() != this )
				{
					throw new InvalidOperationException("Template type scopes are not balanced!");
				}

				s_CurrentScope = null;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------
	
			public Type TemplateType
			{
				get
				{
					return m_TemplateType;
				}
			}
		}
	}
}
