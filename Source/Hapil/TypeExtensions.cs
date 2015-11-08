using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Hapil
{
	public static class TypeExtensions
	{
		public static object GetDefaultValue(this Type type)
		{
			if ( type.IsValueType )
			{
				return Activator.CreateInstance(type);
			}
			else
			{
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static int GetIntegralTypeSize(this Type type)
		{
			if ( type.IsIntegralType() )
			{
				var underlyingType = (type.IsEnum ? type.GetEnumUnderlyingType() : type);

				switch ( Type.GetTypeCode(underlyingType) )
				{
					case TypeCode.Boolean:
						return sizeof(bool);
					case TypeCode.Char:
						return sizeof(char);
					case TypeCode.SByte:
						return sizeof(sbyte);
					case TypeCode.Byte:
						return sizeof(byte);
					case TypeCode.Int16:
						return sizeof(short);
					case TypeCode.UInt16:
						return sizeof(ushort);
					case TypeCode.Int32:
						return sizeof(int);
					case TypeCode.UInt32:
						return sizeof(uint);
					case TypeCode.Int64:
						return sizeof(long);
					case TypeCode.UInt64:
						return sizeof(ulong);
				}
			}

			throw new ArgumentException("Not an integral type.", paramName: "type");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsIntegralType(this Type type)
		{
			if ( type.IsEnum )
			{
				return true;
			}

			if ( !type.IsPrimitive )
			{
				return false;
			}

			switch ( Type.GetTypeCode(type) )
			{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return true;
				default:
					return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsNumericType(this Type type)
		{
			if ( !type.IsPrimitive )
			{
				return (type == typeof(decimal));
			}

			switch ( Type.GetTypeCode(type) )
			{
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
				case TypeCode.Single:
				case TypeCode.Double: 
					return true;
				default:
					return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsNullableValueType(this Type type)
		{
			return (
				type.IsValueType &&
				type.IsGenericType && 
				!type.IsGenericTypeDefinition && 
				type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public static bool IsStructType(this Type type)
	    {
	        return (type.IsValueType && !type.IsPrimitive && !type.IsEnum);
	    }

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsCollectionType(this Type type)
		{
			if ( type.IsGenericType )
			{
				return (type.IsGenericCollectionType() || type.GetInterfaces().Any(IsGenericCollectionType));
			}
			else
			{
				return typeof(System.Collections.ICollection).IsAssignableFrom(type);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsCollectionType(this Type type, out Type elementType)
		{
			if ( type.IsGenericType )
			{
				Type collectionType;

				if ( type.IsGenericCollectionType() )
				{
					collectionType = type;
				}
				else
				{
					collectionType = type.GetInterfaces().FirstOrDefault(IsGenericCollectionType);
				}

				if ( collectionType != null )
				{
					elementType = collectionType.GetGenericArguments()[0]; 
					return true;
				}
			}
			else if ( type.IsArray )
			{
				elementType = type.GetElementType();
				return true;
			}
			else if ( typeof(System.Collections.ICollection).IsAssignableFrom(type) )
			{
				elementType = typeof(object);
				return true;
			}

			elementType = null;
			return false;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static bool IsGenericCollectionType(this Type type)
		{
			if ( !type.IsGenericType )
			{
				return false;
			}
			else if ( type.IsGenericTypeDefinition )
			{
				return (type == typeof(ICollection<>) || type.GetInterfaces().Contains(typeof(ICollection<>)));
			}
			else
			{
				var genericArguments = type.GetGenericArguments();

				if ( genericArguments.Length == 1 )
				{
					return typeof(ICollection<>).MakeGenericType(genericArguments).IsAssignableFrom(type);
				}
				else
				{
					return false;
				}
			}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public static bool IsEquivalentType(this Type type, Type other)
	    {
	        if ( type == other )
	        {
	            return true;
	        }

	        if ( type.IsByRef && !other.IsByRef && type.GetElementType() == other )
	        {
	            return true;
	        }
        
            if ( other.IsByRef && !type.IsByRef && other.GetElementType() == type )
            {
                return true;
            }

	        return false;
	    }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type[] GetTypeHierarchy(this Type type)
		{
			var baseTypes = new HashSet<Type>();
			GetAllBaseTypes(type, baseTypes);
			return baseTypes.ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static string FriendlyName(this Type type)
		{
			if ( type.IsGenericType )
			{
				var nameBuilder = new StringBuilder();
				AppendFriendlyName(type, nameBuilder);

				return nameBuilder.ToString();
			}
			else
			{
				return type.Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Type UnderlyingType(this Type type)
		{
			if ( type.IsByRef )
			{
				return type.GetElementType();
			}
			else
			{
				return type;
			}
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static ConstructorInfo GetRuntimeOrBuilderConstructor(this Type type, Type[] argumentTypes)
        {
            if ( type.GetType().Name.Contains("Builder") )
            {
                var genericType = type.GetGenericTypeDefinition();
                var genericConstructor = genericType.GetConstructors().First(ci => MatchMethodParameters(ci.GetParameters(), argumentTypes));
                return TypeBuilder.GetConstructor(type, genericConstructor);
            }
            else
            {
                return type.GetConstructor(argumentTypes);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static Type GetClosedDeclaringType(this Type nestedType, out Type[] nestedTypeArguments)
        {
            if ( nestedType.DeclaringType != null && 
                nestedType.DeclaringType.IsGenericType && 
                nestedType.DeclaringType.IsGenericTypeDefinition && 
                !nestedType.IsGenericTypeDefinition )
            {
                var declaringTypeArgumentCount = nestedType.DeclaringType.GetGenericArguments().Length;
                var allTypeArguments = nestedType.GetGenericArguments();
                
                var declaringTypeArguments = allTypeArguments.Take(declaringTypeArgumentCount).ToArray();
                nestedTypeArguments = allTypeArguments.Skip(declaringTypeArgumentCount).ToArray();
                
                return nestedType.DeclaringType.MakeGenericType(declaringTypeArguments);
            }

            nestedTypeArguments = null;
            return nestedType.DeclaringType;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    private static bool MatchMethodParameters(ParameterInfo[] parameters, Type[] argumentTypes)
	    {
	        if ( parameters.Length != argumentTypes.Length )
	        {
	            return false;
	        }

	        for ( int i = 0 ; i < parameters.Length ; i++ )
	        {
	            if ( !MatchParameterType(parameters[i].ParameterType, argumentTypes[i]) )
	            {
	                return false;
	            }
	        }

	        return true;
	    }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static bool MatchParameterType(Type parameterType, Type argumentType)
        {
            if ( parameterType.IsAssignableFrom(argumentType) )
            {
                return true;
            }
            else if ( parameterType.IsGenericType != argumentType.IsGenericType )
            {
                return false;
            }
            else if ( parameterType.Name != argumentType.Name )
            {
                return false;
            }
            else if ( parameterType.IsGenericType && parameterType.GetGenericArguments().Length != argumentType.GetGenericArguments().Length )
            {
                return false;
            }

            return true;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void AppendFriendlyName(Type type, StringBuilder output)
		{
		    Type[] nestedTypeArguments = null;

		    if ( type.IsNested && type.DeclaringType != null )
		    {
                AppendFriendlyName(type.GetClosedDeclaringType(out nestedTypeArguments), output);
		        output.Append(".");
		    }

			if ( type.IsGenericType && (nestedTypeArguments == null || nestedTypeArguments.Length > 0) )
			{
				var backquoteIndex = type.Name.IndexOf('`');

				output.Append(backquoteIndex > 0 ? type.Name.Substring(0, backquoteIndex) : type.Name);
				output.Append('<');

                var typeArguments = nestedTypeArguments ?? type.GetGenericArguments();

				for ( int i = 0 ; i < typeArguments.Length ; i++ )
				{
					AppendFriendlyName(typeArguments[i], output);

					if ( i < typeArguments.Length - 1 )
					{
						output.Append(',');
					}
				}

				output.Append('>');
			}
			else
			{
				output.Append(type.Name);
			}
		}

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void GetAllBaseTypes(Type currentType, HashSet<Type> baseTypes)
		{
			if ( baseTypes.Add(currentType) )
			{
				if ( currentType.IsClass && currentType.BaseType != null )
				{
					GetAllBaseTypes(currentType.BaseType, baseTypes);
				}
				
				foreach ( var baseInterface in currentType.GetInterfaces() )
				{
					GetAllBaseTypes(baseInterface, baseTypes);
				}
			}
		}
	}
}
