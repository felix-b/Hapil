using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace Happil
{
	public class HappilTypeKey : IEquatable<HappilTypeKey>
	{
		private readonly Type m_BaseType;
		private readonly Type m_PrimaryInterface;
		private readonly HashSet<Type> m_SecondaryInterfaces;
		private readonly int m_HashCode;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilTypeKey(Type baseType, Type primaryInterface, params Type[] secondaryInterfaces)
		{
			m_BaseType = (baseType ?? typeof(object));
			m_PrimaryInterface = primaryInterface;
			m_SecondaryInterfaces = new HashSet<Type>(secondaryInterfaces ?? new Type[0]);
			m_HashCode = CalculateHashCode();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IEquatable<HappilTypeKey> Members

		public bool Equals(HappilTypeKey other)
		{
			if ( m_BaseType != other.m_BaseType )
			{
				return false;
			}

			if ( m_PrimaryInterface != other.m_PrimaryInterface )
			{
				return false;
			}

			if ( m_SecondaryInterfaces.SetEquals(other.m_SecondaryInterfaces) )
			{
				return false;
			}

			return true;
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
	
		public override bool Equals(object obj)
		{
			var other = (obj as HappilTypeKey);

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
