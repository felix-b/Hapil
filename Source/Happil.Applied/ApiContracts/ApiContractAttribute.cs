using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Applied.ApiContracts
{
	public class ApiContractAttribute : Attribute
	{
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNullAttribute : ApiContractAttribute
	{
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotEmptyAttribute : ApiContractAttribute
	{
	}

	////-----------------------------------------------------------------------------------------------------------------------------------------------------

	//[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	//public class PositiveAttribute : ApiContractAttribute
	//{
	//}

	////-----------------------------------------------------------------------------------------------------------------------------------------------------

	//[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	//public class NotNegativeAttribute : ApiContractAttribute
	//{
	//}

	////-----------------------------------------------------------------------------------------------------------------------------------------------------

	//[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	//public class NegativeAttribute : ApiContractAttribute
	//{
	//}

	////-----------------------------------------------------------------------------------------------------------------------------------------------------

	//[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	//public class NotPositiveAttribute : ApiContractAttribute
	//{
	//}
}
