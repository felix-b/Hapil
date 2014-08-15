using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Applied.ApiContracts
{
	public static class ApiContract
	{
		public static void NotNull(object value, string parameterName, bool isOutput = false)
		{
			if ( value == null )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.NotNull, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void NotEmpty(string value, string parameterName, bool isOutput = false)
		{
			if ( string.IsNullOrEmpty(value) )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.NotEmpty, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void NotEmpty(ICollection collection, string parameterName, bool isOutput = false)
		{
			if ( collection == null || collection.Count == 0 )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.NotEmpty, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void ItemsNotNull(IEnumerable collection, string parameterName, bool isOutput = false)
		{
			if ( collection != null && collection.Cast<object>().Any(item => item == null) )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.ItemsNotNull, isOutput);
			}
		}
	}
}
