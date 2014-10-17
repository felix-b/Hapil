using System;
using System.Reflection;
using Hapil.Applied.ApiContracts.Impl;
using Hapil.Decorators;
using Hapil.Operands;
using Hapil.Writers;

namespace Hapil.Applied.ApiContracts
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotEmptyAttribute : ApiCheckAttribute
	{
		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			var parameterInfo = (ParameterInfo)info;
			var parameterType = parameterInfo.ParameterType.UnderlyingType();

			if ( parameterType == typeof(string) )
			{
				member.AddCheck(new StringNotEmptyCheckWriter(parameterInfo));
			}
			else if ( typeof(System.Collections.ICollection).IsAssignableFrom(parameterType) )
			{
				member.AddCheck(new CollectionNotEmptyCheckWriter(parameterInfo));
			}
			else
			{
				throw new NotSupportedException(string.Format("NotEmpty is not supported on parameter of type [{0}].", parameterType.FullName));
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class StringNotEmptyCheckWriter : ApiArgumentCheckWriter
		{
			public StringNotEmptyCheckWriter(ParameterInfo parameterInfo)
				: base(parameterInfo)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
			{
				Static.Void(ApiContract.NotEmpty, argument.CastTo<string>(), writer.Const(ParameterName), writer.Const(isOutput));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class CollectionNotEmptyCheckWriter : ApiArgumentCheckWriter, ICheckCollectionTypes
		{
			public CollectionNotEmptyCheckWriter(ParameterInfo parameterInfo)
				: base(parameterInfo)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
			{
				Static.Void(ApiContract.NotEmpty, argument.CastTo<System.Collections.ICollection>(), writer.Const(ParameterName), writer.Const(isOutput));
			}
		}
	}
}