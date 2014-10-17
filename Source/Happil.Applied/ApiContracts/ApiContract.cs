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
				throw new ApiContractException(parameterName, ApiContractCheckType.NotNull, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void ItemsNotEmpty(IEnumerable collection, string parameterName, bool isOutput = false)
		{
			if ( collection != null && collection.Cast<object>().Any(item => string.IsNullOrEmpty(item as string)) )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.NotEmpty, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void GreaterThan(long value, long minimum, string parameterName, bool isOutput = false)
		{
			if ( value <= minimum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.GreaterThan, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void GreaterThanOrEqual(long value, long minimum, string parameterName, bool isOutput = false)
		{
			if ( value < minimum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.GreaterThanOrEqual, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void GreaterThan(double value, double minimum, string parameterName, bool isOutput = false)
		{
			if ( value <= minimum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.GreaterThan, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void GreaterThanOrEqual(double value, double minimum, string parameterName, bool isOutput = false)
		{
			if ( value < minimum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.GreaterThanOrEqual, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void LessThan(long value, long maximum, string parameterName, bool isOutput = false)
		{
			if ( value >= maximum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.LessThan, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void LessThanOrEqual(long value, long maximum, string parameterName, bool isOutput = false)
		{
			if ( value > maximum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.LessThanOrEqual, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void LessThan(double value, double maximum, string parameterName, bool isOutput = false)
		{
			if ( value >= maximum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.LessThan, isOutput);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static void LessThanOrEqual(double value, double maximum, string parameterName, bool isOutput = false)
		{
			if ( value > maximum )
			{
				throw new ApiContractException(parameterName, ApiContractCheckType.LessThanOrEqual, isOutput);
			}
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public static void InRange(long value, long? min, long? max, bool minExclusive, bool maxExclusive, string parameterName, bool isOutput = false)
		//{
		//	if ( min.HasValue )
		//	{
		//		if ( (minExclusive && value <= min.Value) || (!minExclusive && value < min.Value) )
		//		{
		//			throw new ApiContractException(parameterName, ApiContractCheckType.RangeMin, isOutput);
		//		}
		//	}

		//	if ( max.HasValue )
		//	{
		//		if ( (maxExclusive && value >= max.Value) || (!maxExclusive && value > max.Value) )
		//		{
		//			throw new ApiContractException(parameterName, ApiContractCheckType.RangeMin, isOutput);
		//		}
		//	}
		//}
	}
}
