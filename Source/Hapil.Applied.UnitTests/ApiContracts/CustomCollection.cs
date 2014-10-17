using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Applied.UnitTests.ApiContracts
{
	internal class CustomCollection : System.Collections.ICollection
	{
		private readonly List<object> m_InnerList = new List<object>();
		private readonly object m_SyncRoot = new object();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CustomCollection(params object[] items)
		{
			m_InnerList.AddRange(items);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			m_InnerList.ToArray().CopyTo(array, index);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int Count
		{
			get
			{
				return m_InnerList.Count;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public object SyncRoot
		{
			get
			{
				return m_SyncRoot;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return m_InnerList.GetEnumerator();
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	internal class CustomCollection<T> : ICollection<T>
	{
		private readonly List<T> m_InnerList = new List<T>();

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region ICollection<T> Members

		public void Add(T item)
		{
			m_InnerList.Add(item);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Clear()
		{
			m_InnerList.Clear();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool Contains(T item)
		{
			return m_InnerList.Contains(item);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void CopyTo(T[] array, int arrayIndex)
		{
			m_InnerList.CopyTo(array, arrayIndex);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public int Count
		{
			get
			{
				return m_InnerList.Count;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool Remove(T item)
		{
			return m_InnerList.Remove(item);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return m_InnerList.GetEnumerator();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return m_InnerList.GetEnumerator();
		}

		#endregion
	}
}
