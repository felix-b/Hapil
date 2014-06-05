using System;
using System.Reflection;
using Happil.Applied.ApiContracts.Impl;
using Happil.Decorators;

namespace Happil.Applied.ApiContracts
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotEmptyAttribute : ApiCheckAttribute
	{
		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			member.AddCheck(new StringNotEmptyCheckWriter((ParameterInfo)info));
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class StringNotEmptyCheckWriter : ApiMethodCheckWriter
		{
			private readonly ParameterInfo m_Parameter;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public StringNotEmptyCheckWriter(ParameterInfo parameter)
			{
				m_Parameter = parameter;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void WriteMethodCheck(MethodDecorationBuilder decoration)
			{
				if ( m_Parameter.Position < 0 )
				{
					decoration.OnReturnValue((w, retVal) =>
						Static.Void(ApiContract.NotEmpty, retVal.CastTo<string>(), w.Const("(Return Value)"), w.Const(true))
					);
				}
				else
				{
					decoration.OnBefore(w => {
						var argument = w.Argument<string>(m_Parameter.Position + 1);
						Static.Void(ApiContract.NotEmpty, argument, w.Const(m_Parameter.Name), w.Const(false));
					});
				}
			}
		}
	}
}