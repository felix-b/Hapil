using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Applied.ApiContracts.Impl;
using Hapil.Operands;
using Hapil.Writers;

namespace Hapil.Applied.ApiContracts
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public abstract class StringLengthAttributeBase : ApiCheckAttribute
	{
		protected StringLengthAttributeBase()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			var parameterInfo = (ParameterInfo)info;
			var actualParameterType = parameterInfo.ParameterType.UnderlyingType();

			if ( actualParameterType != typeof(string) )
			{
				throw new NotSupportedException(string.Format("StringLength is not supported on parameter of type [{0}].", actualParameterType));
			}

			member.AddCheck(new StringLengthCheckWriter(parameterInfo, this));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected int? MinLength { get; set; }
		protected int? MaxLength { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class StringLengthCheckWriter : ApiArgumentCheckWriter
		{
			private readonly ParameterInfo m_ParameterInfo;
			private readonly StringLengthAttributeBase m_Attribute;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public StringLengthCheckWriter(ParameterInfo parameterInfo, StringLengthAttributeBase attribute)
				: base(parameterInfo)
			{
				m_ParameterInfo = parameterInfo;
				m_Attribute = attribute;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
			{
				if ( m_Attribute.MinLength.HasValue )
				{
					Static.Void(
						ApiContract.MinStringLength, 
						argument.CastTo<string>(), writer.Const(m_Attribute.MinLength.Value), writer.Const(m_ParameterInfo.Name), writer.Const(isOutput));
				}

				if ( m_Attribute.MaxLength.HasValue )
				{
					Static.Void(
						ApiContract.MaxStringLength, 
						argument.CastTo<string>(), writer.Const(m_Attribute.MaxLength.Value), writer.Const(m_ParameterInfo.Name), writer.Const(isOutput));
				}
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class LengthAttribute : StringLengthAttributeBase
	{
		public LengthAttribute(int min, int max)
		{
			base.MinLength = min;
			base.MaxLength = max;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class MinLengthAttribute : StringLengthAttributeBase
	{
		public MinLengthAttribute(int length)
		{
			base.MinLength = length;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class MaxLengthAttribute : StringLengthAttributeBase
	{
		public MaxLengthAttribute(int length)
		{
			base.MaxLength = length;
		}
	}
}
