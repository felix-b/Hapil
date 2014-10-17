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
	public class InRangeAttribute : ApiCheckAttribute
	{
		private double? m_AssignedMin;
		private double? m_AssignedMax;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public InRangeAttribute()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public InRangeAttribute(double min, double max)
		{
			this.Min = min;
			this.Max = max;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			var parameterInfo = (ParameterInfo)info;

			if ( m_AssignedMin.HasValue )
			{
				member.AddCheck(new MinValueCheckWriter(parameterInfo, m_AssignedMin.Value, MinExclusive));
			}

			if ( m_AssignedMax.HasValue )
			{
				member.AddCheck(new MaxValueCheckWriter(parameterInfo, m_AssignedMax.Value, MaxExclusive));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public double Min
		{
			get
			{
				return m_AssignedMin.GetValueOrDefault();
			}
			set
			{
				m_AssignedMin = value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public double Max
		{
			get
			{
				return m_AssignedMax.GetValueOrDefault();
			}
			set
			{
				m_AssignedMax = value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool Exclusive
		{
			get
			{
				return (MinExclusive && MaxExclusive);
			}
			set
			{
				MinExclusive = value;
				MaxExclusive = value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool MinExclusive { get; set; }
		public bool MaxExclusive { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public double? AssignedMin
		{
			get
			{
				return m_AssignedMin;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public double? AssignedMax
		{
			get
			{
				return m_AssignedMax;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private abstract class BoundValueCheckWriter : ApiArgumentCheckWriter
		{
			private readonly ParameterInfo m_ParameterInfo;
			private readonly double m_BoundValue;
			private readonly bool m_Exclusive;
			private readonly Action<long, long, string, bool> m_CheckMethodTypeLong;
			private readonly Action<double, double, string, bool> m_CheckMethodTypeDouble;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected BoundValueCheckWriter(
				ParameterInfo parameterInfo, 
				double boundValue, 
				bool exclusive,
				Action<long, long, string, bool> exclusiveCheckMethodTypeInt,
				Action<long, long, string, bool> inclusiveCheckMethodTypeInt,
				Action<double, double, string, bool> exclusiveCheckMethodTypeDouble,
				Action<double, double, string, bool> inclusiveCheckMethodTypeDouble)
				: base(parameterInfo)
			{
				m_ParameterInfo = parameterInfo;
				m_BoundValue = boundValue;
				m_Exclusive = exclusive;
				m_CheckMethodTypeLong = (exclusive ? exclusiveCheckMethodTypeInt : inclusiveCheckMethodTypeInt);
				m_CheckMethodTypeDouble = (exclusive ? exclusiveCheckMethodTypeDouble : inclusiveCheckMethodTypeDouble);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
			{
				Type actualParameterType = TypeTemplate.Resolve(m_ParameterInfo.ParameterType).UnderlyingType();

				if ( actualParameterType.IsIntegralType() )
				{
					Static.Void(
						m_CheckMethodTypeLong,
						argument.CastTo<long>(), writer.Const((long)m_BoundValue), writer.Const(ParameterName), writer.Const(isOutput));
				}
				else if ( actualParameterType.IsNumericType() )
				{
					Static.Void(
						m_CheckMethodTypeDouble,
						argument.CastTo<double>(), writer.Const(m_BoundValue), writer.Const(ParameterName), writer.Const(isOutput));
				}
				else
				{
					throw new NotSupportedException(string.Format("InRange is not supported on parameter of type [{0}].", actualParameterType));
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class MinValueCheckWriter : BoundValueCheckWriter
		{
			public MinValueCheckWriter(ParameterInfo parameterInfo, double minValue, bool exclusive)
				: base(
					parameterInfo,
					minValue,
					exclusive,	
					ApiContract.GreaterThan,
					ApiContract.GreaterThanOrEqual,
					ApiContract.GreaterThan,
					ApiContract.GreaterThanOrEqual)
			{
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class MaxValueCheckWriter : BoundValueCheckWriter
		{
			public MaxValueCheckWriter(ParameterInfo parameterInfo, double maxValue, bool exclusive)
				: base(
					parameterInfo,
					maxValue,
					exclusive,
					ApiContract.LessThan,
					ApiContract.LessThanOrEqual,
					ApiContract.LessThan,
					ApiContract.LessThanOrEqual)
			{
			}
		}
	}
}
