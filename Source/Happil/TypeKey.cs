using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil
{
	public class TypeKey
	{
		private readonly Type m_BaseType;
		private readonly Type m_PrimaryInterface;
		private readonly Type[] m_SecondaryInterfacesArray;
		private readonly HashSet<Type> m_SecondaryInterfaces;
		private readonly int m_HashCode;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeKey(Type baseType = null, Type primaryInterface = null, params Type[] secondaryInterfaces)
		{
			m_BaseType = (baseType ?? typeof(object));
			m_PrimaryInterface = primaryInterface;
			m_SecondaryInterfacesArray = secondaryInterfaces;
			m_SecondaryInterfaces = new HashSet<Type>(secondaryInterfaces ?? new Type[0]);
			m_HashCode = CalculateHashCode();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IEquatable<TypeKey> Members

		public bool Equals(TypeKey other)
		{
			if ( m_BaseType != other.m_BaseType )
			{
				return false;
			}

			if ( m_PrimaryInterface != other.m_PrimaryInterface )
			{
				return false;
			}

			if ( !m_SecondaryInterfaces.SetEquals(other.m_SecondaryInterfaces) )
			{
				return false;
			}

			return true;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
	
		public override bool Equals(object obj)
		{
			var other = (obj as TypeKey);

			if ( other != null )
			{
				return Equals(other);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override int GetHashCode()
		{
			return m_HashCode;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IDisposable CreateTypeTemplateScope()
		{
			var typePairCount = 
				1 +  // TBase
				(m_PrimaryInterface != null ? 1 : 0) +  // TPrimary
				(m_SecondaryInterfacesArray != null ? Math.Min(m_SecondaryInterfacesArray.Length, 2) : 0); // TSecondary1, TSecondary2

			var typePairs = new Type[typePairCount * 2];
			var index = 0;

			typePairs[index++] = typeof(TypeTemplate.TBase);
			typePairs[index++] = m_BaseType;

			if ( m_PrimaryInterface != null )
			{
				typePairs[index++] = typeof(TypeTemplate.TPrimary);
				typePairs[index++] = m_PrimaryInterface;
			}

			if ( m_SecondaryInterfacesArray != null )
			{
				if ( m_SecondaryInterfacesArray.Length >= 1 )
				{
					typePairs[index++] = typeof(TypeTemplate.TSecondary1);
					typePairs[index++] = m_SecondaryInterfacesArray[0];
				}

				if ( m_SecondaryInterfacesArray.Length >= 2 )
				{
					typePairs[index++] = typeof(TypeTemplate.TSecondary2);
					typePairs[index++] = m_SecondaryInterfacesArray[1];
				}
			}

			return TypeTemplate.CreateScope(typePairs);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string SuggestClassName(ObjectFactoryBase factory)
		{
			var suggestedName = new StringBuilder();

			suggestedName.Append(factory.Module.SimpleName + ".");
			suggestedName.Append(factory.GetType().Name.TrimSuffix("Factory") + ".");

			if ( m_PrimaryInterface != null )
			{
				suggestedName.Append(m_PrimaryInterface.Name.TrimPrefix("I"));
			}
			else if ( m_BaseType != null && m_BaseType != typeof(object) )
			{
				suggestedName.Append(m_BaseType.Name);
			}
			else
			{
				suggestedName.Append("EmittedClass");
			}

			return suggestedName.ToString();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type BaseType
		{
			get
			{
				return m_BaseType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type PrimaryInterface
		{
			get
			{
				return m_PrimaryInterface;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type[] SecondaryInterfaces
		{
			get
			{
				return m_SecondaryInterfaces.ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private int CalculateHashCode()
		{
			var hashCode = m_BaseType.GetHashCode();

			if ( m_PrimaryInterface != null )
			{
				hashCode ^= m_PrimaryInterface.GetHashCode();
			}
			else
			{
				hashCode ^= 0xABAC;
			}

			hashCode ^= m_SecondaryInterfaces.Count;
			
			return hashCode;
		}
	}
}
