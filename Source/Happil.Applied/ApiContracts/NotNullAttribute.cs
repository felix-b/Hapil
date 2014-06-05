using System;
using System.Reflection;
using Happil.Applied.ApiContracts.Impl;
using Happil.Decorators;

namespace Happil.Applied.ApiContracts
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNullAttribute : ApiCheckAttribute
	{
		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			var parameter = (ParameterInfo)info;

			if ( parameter.Position < 0 )
			{
				member.AddCheck(new ReturnValueNotNullCheckWriter());
			}
			else
			{
				member.AddCheck(new ArgumentNotNullCheckWriter((ParameterInfo)info));
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ArgumentNotNullCheckWriter : ApiMethodCheckWriter
		{
			private readonly ParameterInfo m_Parameter;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ArgumentNotNullCheckWriter(ParameterInfo parameter)
			{
				m_Parameter = parameter;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void WriteMethodCheck(MethodDecorationBuilder decoration)
			{
				if ( !m_Parameter.IsOut )
				{
					decoration.OnBefore(w => {
						using ( TypeTemplate.CreateScope<TypeTemplate.TArgument>(m_Parameter.ParameterType) )
						{
							var argument = w.Argument<TypeTemplate.TArgument>(m_Parameter.Position + 1);
							Static.Void(ApiContract.NotNull, argument.CastTo<object>(), w.Const(m_Parameter.Name), w.Const(false));
						}
					});
				}

				if ( m_Parameter.IsOut || m_Parameter.ParameterType.IsByRef )
				{
					decoration.OnAfter(w => {
						using ( TypeTemplate.CreateScope<TypeTemplate.TArgument>(m_Parameter.ParameterType) )
						{
							var argument = w.Argument<TypeTemplate.TArgument>(m_Parameter.Position + 1);
							Static.Void(ApiContract.NotNull, argument.CastTo<object>(), w.Const(m_Parameter.Name), w.Const(true));
						}
					});
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ReturnValueNotNullCheckWriter : ApiMethodCheckWriter
		{
			public override void WriteMethodCheck(MethodDecorationBuilder decoration)
			{
				decoration.OnReturnValue((w, retVal) =>
					Static.Void(ApiContract.NotNull, retVal.CastTo<object>(), w.Const("(Return Value)"), w.Const(true))
				);
			}
		}
	}
}
