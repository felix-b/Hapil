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
	public abstract class InRangeAttributeBase : ApiCheckAttribute
	{
		protected InRangeAttributeBase()
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member)
		{
			var parameterInfo = (ParameterInfo)info;

			if ( AssignedMin.HasValue )
			{
				member.AddCheck(new MinValueCheckWriter(parameterInfo, AssignedMin.Value, AssignedMinExclusive.GetValueOrDefault()));
			}

			if ( AssignedMax.HasValue )
			{
				member.AddCheck(new MaxValueCheckWriter(parameterInfo, AssignedMax.Value, AssignedMaxExclusive.GetValueOrDefault()));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected double? AssignedMin { get; set; }
		protected bool? AssignedMinExclusive { get; set; }
		protected double? AssignedMax { get; set; }
		protected bool? AssignedMaxExclusive { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected abstract class BoundValueCheckWriter : ApiArgumentCheckWriter
		{
			//private readonly ParameterInfo m_ParameterInfo;
			private readonly double m_BoundValue;
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
				//m_ParameterInfo = parameterInfo;
				m_BoundValue = boundValue;
				//m_Exclusive = exclusive;
				m_CheckMethodTypeLong = (exclusive ? exclusiveCheckMethodTypeInt : inclusiveCheckMethodTypeInt);
				m_CheckMethodTypeDouble = (exclusive ? exclusiveCheckMethodTypeDouble : inclusiveCheckMethodTypeDouble);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnWriteArgumentCheck(MethodWriterBase writer, Operand<TypeTemplate.TArgument> argument, bool isOutput)
			{
				Type actualParameterType = TypeTemplate.Resolve<TypeTemplate.TArgument>().UnderlyingType();

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

		protected class MinValueCheckWriter : BoundValueCheckWriter
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

		protected class MaxValueCheckWriter : BoundValueCheckWriter
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

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class InRangeAttribute : InRangeAttributeBase
	{
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

		public double Min
		{
			get
			{
				return AssignedMin.GetValueOrDefault();
			}
			set
			{
				AssignedMin = value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public double Max
		{
			get
			{
				return AssignedMax.GetValueOrDefault();
			}
			set
			{
				AssignedMax = value;
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

		public bool MinExclusive
		{
			get
			{
				return AssignedMinExclusive.GetValueOrDefault();
			}
			set
			{
				AssignedMinExclusive = value;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool MaxExclusive
		{
			get
			{
				return AssignedMaxExclusive.GetValueOrDefault();
			}
			set
			{
				AssignedMaxExclusive = value;
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class PositiveAttribute : InRangeAttributeBase
	{
		public PositiveAttribute()
		{
			base.AssignedMin = 0;
			base.AssignedMinExclusive = true;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NegativeAttribute : InRangeAttributeBase
	{
		public NegativeAttribute()
		{
			base.AssignedMax = 0;
			base.AssignedMaxExclusive = true;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotNegativeAttribute : InRangeAttributeBase
	{
		public NotNegativeAttribute()
		{
			base.AssignedMin = 0;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class NotPositiveAttribute : InRangeAttributeBase
	{
		public NotPositiveAttribute()
		{
			base.AssignedMax = 0;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class OneBasedAttribute : InRangeAttributeBase
	{
		public OneBasedAttribute()
		{
			base.AssignedMin = 1;
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class ZeroBasedAttribute : InRangeAttributeBase
	{
		public ZeroBasedAttribute()
		{
			base.AssignedMin = 0;
		}
	}
}
