using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.Fluent
{
	public class HappilProperty
	{
		public HappilPropertyGetter Get(Action<HappilMethodBody> body)
		{
			throw new NotImplementedException();
		}

		public HappilPropertySetter Set(Action<HappilMethodBody> body)
		{
			throw new NotImplementedException();
		}

		public HappilField<object> BackingField
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
