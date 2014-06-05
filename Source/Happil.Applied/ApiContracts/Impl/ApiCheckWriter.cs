using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Happil.Decorators;

namespace Happil.Applied.ApiContracts.Impl
{
	public abstract class ApiCheckWriter
	{
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public abstract class ApiMethodCheckWriter : ApiCheckWriter
	{
		public abstract void WriteMethodCheck(MethodDecorationBuilder decoration);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public abstract class ApiPropertyCheckWriter : ApiCheckWriter
	{
		public abstract void WritePropertyCheck(PropertyDecorationBuilder decoration);
	}

	////---------------------------------------------------------------------------------------------------------------------------------------------------------

	//public class NotNullCheck : ApiCheck<object>
	//{
	//	public override void Check(string parameterName, object parameterValue)
	//	{
	//		if ( parameterValue == null )
	//		{
	//			throw new ArgumentNullException(paramName: parameterName);
	//		}
	//	}
	//}

	////---------------------------------------------------------------------------------------------------------------------------------------------------------

	//public class NotEmptyStringCheck : ApiCheck<string>
	//{
	//	public override void Check(string parameterName, string parameterValue)
	//	{
	//		if ( string.IsNullOrEmpty(parameterValue) )
	//		{
	//			throw new ArgumentException(
	//				message: "String cannot be null or empty.",
	//				paramName: parameterName);
	//		}
	//	}
	//}

	////---------------------------------------------------------------------------------------------------------------------------------------------------------

	//public class NotEmptyCollectionCheck<T> : ApiCheck<ICollection<T>>
	//{
	//	public override void Check(string parameterName, ICollection<T> parameterValue)
	//	{
	//		if ( parameterValue == null || parameterValue.Count == 0 )
	//		{
	//			throw new ArgumentException(
	//				message: "Collection cannot be null or empty.",
	//				paramName: parameterName);
	//		}
	//	}
	//}

	////---------------------------------------------------------------------------------------------------------------------------------------------------------

	//public class NotEmptyEnumerableCheck<T> : ApiCheck<IEnumerable<T>>
	//{
	//	public override void Check(string parameterName, IEnumerable<T> parameterValue)
	//	{
	//		if ( parameterValue == null || !parameterValue.Any() )
	//		{
	//			throw new ArgumentException(
	//				message: "Sequence cannot be null or empty.",
	//				paramName: parameterName);
	//		}
	//	}
	//}
}
