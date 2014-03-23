using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;

namespace Happil.Writers
{
	public class ClassImplementationWriter<TBase> : ClassWriterBase
	{
		public ClassImplementationWriter(ClassType classType)
			: base(classType)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassImplementationWriter<TBase> DefaultConstructor()
		{
			//TODO: define default constructor
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override void Flush()
		{
			throw new NotImplementedException();
		}

	}
}
