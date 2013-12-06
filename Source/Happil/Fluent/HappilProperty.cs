using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilProperty<T> : HappilOperand<T>
	{
		public HappilPropertyGetter Get(Action<HappilMethodBody> body)
		{
			throw new NotImplementedException();
		}

		public HappilPropertySetter Set(Action<HappilMethodBody> body)
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
	}
}
