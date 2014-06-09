using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Happil.Decorators;
using Happil.Operands;
using Happil.Writers;

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

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public abstract class ApiArgumentCheckWriter : ApiMethodCheckWriter
	{
		private readonly ParameterInfo m_ParameterInfo;

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		protected ApiArgumentCheckWriter(ParameterInfo parameterInfo)
		{
			m_ParameterInfo = parameterInfo;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public override void WriteMethodCheck(MethodDecorationBuilder decoration)
		{
			if ( IsReturnParameter )
			{
				WriteReturnValueCheck(decoration);
			}
			else
			{
				if ( !m_ParameterInfo.IsOut )
				{
					WriteArgumentInputCheck(decoration);
				}

				if ( m_ParameterInfo.IsOut || m_ParameterInfo.ParameterType.IsByRef )
				{
					WriteArgumentOutputCheck(decoration);
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput);

		//-------------------------------------------------------------------------------------------------------------------------------------------------
		
		protected ParameterInfo ParameterInfo
		{
			get
			{
				return m_ParameterInfo;
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------
		
		protected bool IsReturnParameter
		{
			get
			{
				return (m_ParameterInfo.Position < 0);
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		protected string ParameterName
		{
			get
			{
				return (IsReturnParameter ? "(Return Value)" : m_ParameterInfo.Name);
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void WriteArgumentOutputCheck(MethodDecorationBuilder decoration)
		{
			decoration.OnSuccess(w => {
				using ( TypeTemplate.CreateScope<TypeTemplate.TArgument>(m_ParameterInfo.ParameterType) )
				{
					var argument = w.Argument<TypeTemplate.TArgument>(m_ParameterInfo.Position + 1);
					OnWriteArgumentCheck(w, argument, isOutput: true);
				}
			});
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void WriteArgumentInputCheck(MethodDecorationBuilder decoration)
		{
			decoration.OnBefore(w => {
				using ( TypeTemplate.CreateScope<TypeTemplate.TArgument>(m_ParameterInfo.ParameterType) )
				{
					var argument = w.Argument<TypeTemplate.TArgument>(m_ParameterInfo.Position + 1);
					OnWriteArgumentCheck(w, argument, isOutput: false);
				}
			});
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------
		
		private void WriteReturnValueCheck(MethodDecorationBuilder decoration)
		{
			decoration.OnReturnValue((w, retVal) => {
				using ( TypeTemplate.CreateScope<TypeTemplate.TArgument>(m_ParameterInfo.ParameterType) )
				{
					OnWriteArgumentCheck(w, retVal.CastTo<TypeTemplate.TArgument>(), isOutput: true);
				}
			});
		}
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
