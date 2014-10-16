using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Happil.Applied.ApiContracts.Impl;
using Happil.Operands;
using Happil.Writers;

namespace Happil.Applied.ApiContracts
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class ItemsNotEmptyAttribute : ApiCheckAttribute
	{
		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			var parameterInfo = (ParameterInfo)info;
			var parameterType = parameterInfo.ParameterType.UnderlyingType();

			if ( typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterType) )
			{
				member.AddCheck(new CollectionItemsNotEmptyCheckWriter(parameterInfo));
			}
			else
			{
				throw new NotSupportedException(string.Format("ItemsNotEmpty is not supported on parameter of type [{0}].", parameterType.FullName));
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class CollectionItemsNotEmptyCheckWriter : ApiArgumentCheckWriter
		{
			public CollectionItemsNotEmptyCheckWriter(ParameterInfo parameterInfo)
				: base(parameterInfo)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
			{
				Static.Void(ApiContract.ItemsNotEmpty, argument.CastTo<System.Collections.IEnumerable>(), writer.Const(ParameterName), writer.Const(isOutput));
			}
		}
	}
}
