using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Applied.ApiContracts.Impl
{
	public class ApiContractDescription
	{
		private readonly TypeKey m_TypeKey;
		private readonly Dictionary<MemberInfo, ApiMemberDescription> m_Members;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ApiContractDescription(TypeKey typeKey)
		{
			m_TypeKey = typeKey;
			m_Members = new Dictionary<MemberInfo, ApiMemberDescription>();

			CreateMemberDescriptions();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public T TryGetMember<T>(MemberInfo declaration) where T : ApiMemberDescription
		{
			ApiMemberDescription member;

			if ( m_Members.TryGetValue(declaration, out member) )
			{
				return (T)member;
			}
			else
			{
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void CreateMemberDescriptions()
		{
			foreach ( var ancestorType in m_TypeKey.GetAllInterfaces() )
			{
				foreach ( var member in TypeMemberCache.Of(ancestorType).ImplementableMembers )
				{
					CreateMemberDescription(member);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void CreateMemberDescription(MemberInfo member)
		{
			var methodMember = member as MethodInfo;
			var propertyMember = member as PropertyInfo;
			var eventMember = member as EventInfo;

			ApiMemberDescription memberDescription = null;

			if ( methodMember != null )
			{
				memberDescription = new ApiMethodMemberDescription(methodMember);
			}
			else if ( propertyMember != null )
			{
				throw new NotImplementedException();
			}
			else if ( eventMember != null )
			{
				throw new NotImplementedException();
			}

			if ( memberDescription != null )
			{
				m_Members.Add(member, memberDescription);
			}
		}
	}
}
