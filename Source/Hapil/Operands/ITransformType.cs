using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hapil.Operands
{
	internal interface ITransformType
	{
		Operand<T> TransformToType<T>();
	}
}
