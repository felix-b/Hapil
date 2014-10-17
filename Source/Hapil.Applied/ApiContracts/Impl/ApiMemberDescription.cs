using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Decorators;

namespace Hapil.Applied.ApiContracts.Impl
{
	public abstract class ApiMemberDescription
	{
		private readonly MemberInfo m_Declaration;
		private readonly List<ApiCheckWriter> m_ApiChecks;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ApiMemberDescription(MemberInfo declaration)
		{
			m_Declaration = declaration;
			m_ApiChecks = new List<ApiCheckWriter>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AddCheck(ApiCheckWriter check)
		{
			m_ApiChecks.Add(check);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MemberInfo Declaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected void AddChecksFrom(ICustomAttributeProvider attributable)
		{
			var attributes = attributable.GetCustomAttributes(typeof(ApiCheckAttribute), inherit: true).OfType<ApiCheckAttribute>().ToArray();

			foreach ( var singleAttribute in attributes )
			{
				singleAttribute.ContributeChecks(attributable, this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected T[] GetChecks<T>() where T : ApiCheckWriter
		{
			return m_ApiChecks.Cast<T>().ToArray();
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public class ApiMethodMemberDescription : ApiMemberDescription
	{
		public ApiMethodMemberDescription(MethodInfo declaration)
			: base(declaration)
		{
			foreach ( var parameter in declaration.GetParameters() )
			{
				base.AddChecksFrom(parameter);
			}

			if ( declaration.ReturnParameter != null )
			{
				base.AddChecksFrom(declaration.ReturnParameter);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ApplyChecks(MethodDecorationBuilder decoration)
		{
			foreach ( var check in base.GetChecks<ApiMethodCheckWriter>() )
			{
				check.WriteMethodCheck(decoration);
			}
		}
	}
}
