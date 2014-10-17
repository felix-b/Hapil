using System;
using System.Reflection;
using Hapil.Applied.ApiContracts.Impl;
using Hapil.Decorators;
using Hapil.Operands;
using Hapil.Writers;

namespace Hapil.Applied.ApiContracts
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNullAttribute : ApiCheckAttribute
	{
		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			member.AddCheck(new NotNullCheckWriter((ParameterInfo)info));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class NotNullCheckWriter : ApiArgumentCheckWriter, ICheckCollectionTypes
		{
			public NotNullCheckWriter(ParameterInfo parameterInfo)
				: base(parameterInfo)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
			{
				Static.Void(ApiContract.NotNull, argument.CastTo<object>(), writer.Const(ParameterName), writer.Const(isOutput));
			}
		}
	}
}
