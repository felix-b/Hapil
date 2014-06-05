using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Applied.ApiContracts.Impl
{
	public abstract class ApiCheckAttribute : Attribute
	{
		public abstract void ContributeChecks(ICustomAttributeProvider info, ApiMemberDescription member);

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public virtual void ContributeChecks(ParameterInfo info, ApiMethodMemberDescription methodMember)
		//{
		//	throw new NotSupportedException(string.Format("{0} is not supported on method parameters.", this.GetType().Name));
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public virtual void ContributeChecks(PropertyInfo info, ApiMemberDescription propertyMember)
		//{
		//	throw new NotSupportedException(string.Format("{0} is not supported on properties.", this.GetType().Name));
		//}
	}
}
