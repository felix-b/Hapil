using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hapil.Fluent
{
	public interface IHappilLoopBody
	{
		void Continue();
		void Break();
	}
}
