using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilProperty<T> : HappilOperand<T>
	{
		public HappilPropertyGetter Get(Action<IHappilMethodBody<T>> body)
		{
			throw new NotImplementedException();
		}

		public HappilPropertySetter Set(Action<IHappilMethodBody<T>> body)
		{
			throw new NotImplementedException();
		}

		public HappilField<T> BackingField
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal HappilPropertyGetter Get(Action<IHappilMethodBody<int>> action)
		{
			throw new NotImplementedException();
		}
	}
}
