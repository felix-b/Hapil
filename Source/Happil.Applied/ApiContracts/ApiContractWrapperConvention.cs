using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Applied.Conventions;

namespace Happil.Applied.ApiContracts
{
	public class ApiContractWrapperConvention : CompositeConvention
	{
		public ApiContractWrapperConvention()
			: base(
				new NamingConvention(), 
				new CallTargetConvention(), 
				new ApiContractCheckConvention())
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class NamingConvention : ImplementationConvention
		{
			public NamingConvention()
				: base(Will.InspectDeclaration)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region Overrides of ImplementationConvention

			protected override void OnInspectDeclaration(ObjectFactoryContext context)
			{
				context.ClassFullName = (
					context.TypeKey.PrimaryInterface.Namespace + "." +
					"ApiContractOf_" +
					context.TypeKey.PrimaryInterface.Name);
			}

			#endregion
		}
	}
}
