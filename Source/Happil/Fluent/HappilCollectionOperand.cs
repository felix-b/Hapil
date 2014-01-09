using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	public class HappilCollectionOperand<T, TItem> : HappilOperand<T> 
		where T : IList<TItem>
	{
		private readonly HappilOperand<T> m_Collection;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilCollectionOperand(HappilOperand<T> collection)
			: base(collection.OwnerMethod)
		{
			m_Collection = collection;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilOperand<int> IndexOf(IHappilOperand<TItem> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Insert(IHappilOperand<int> index, IHappilOperand<TItem> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RemoveAt(IHappilOperand<int> index)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<TItem> this[IHappilOperand<int> index]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Add(IHappilOperand<TItem> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Clear()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<bool> Contains(IHappilOperand<TItem> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void CopyTo(IHappilOperand<TItem[]> array, IHappilOperand<int> arrayIndex)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<int> Count
		{
			get { throw new NotImplementedException(); }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<bool> IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<bool> Remove(IHappilOperand<TItem> item)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilOperand<IEnumerator<TItem>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotImplementedException();
		}
	}
}
