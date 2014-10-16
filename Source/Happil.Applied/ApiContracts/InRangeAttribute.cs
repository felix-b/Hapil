//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Happil.Applied.ApiContracts.Impl;

//namespace Happil.Applied.ApiContracts
//{
//	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
//	public class InRangeAttribute : ApiCheckAttribute
//	{
//		public InRangeAttribute()
//		{
//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		public InRangeAttribute(double min, double max)
//		{
//			this.Min = min;
//			this.Max = max;
//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
//		{
//			var parameterInfo = (ParameterInfo)info;
//			var parameterType = parameterInfo.ParameterType.UnderlyingType();

//			if ( parameterType == typeof(string) )
//			{
//				member.AddCheck(new StringNotEmptyCheckWriter(parameterInfo));
//			}
//			else if ( typeof(System.Collections.ICollection).IsAssignableFrom(parameterType) )
//			{
//				member.AddCheck(new CollectionNotEmptyCheckWriter(parameterInfo));
//			}
//			else
//			{
//				throw new NotSupportedException(string.Format("InRange is not supported on parameter of type [{0}].", parameterType.FullName));
//			}
//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		public double? Min { get; set; }
//		public double? Max { get; set; }
//		public bool MinExclusive { get; set; }
//		public bool MaxExclusive { get; set; }
	
//		//-----------------------------------------------------------------------------------------------------------------------------------------------------



//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		private class StringNotEmptyCheckWriter : ApiArgumentCheckWriter
//		{
//			public StringNotEmptyCheckWriter(ParameterInfo parameterInfo)
//				: base(parameterInfo)
//			{
//			}

//			//-------------------------------------------------------------------------------------------------------------------------------------------------

//			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
//			{
//				Static.Void(ApiContract.NotEmpty, argument.CastTo<string>(), writer.Const(ParameterName), writer.Const(isOutput));
//			}
//		}

//	}
//}
