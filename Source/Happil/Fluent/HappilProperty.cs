using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	public class HappilProperty<T> : HappilOperand<T>
	{
		internal HappilProperty()
			: base(ownerMethod: null)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

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
