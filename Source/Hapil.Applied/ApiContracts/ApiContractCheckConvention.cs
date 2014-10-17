using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Applied.ApiContracts.Impl;
using Hapil.Decorators;
using Hapil.Members;
using Hapil.Writers;

namespace Hapil.Applied.ApiContracts
{
	public class ApiContractCheckConvention : DecorationConvention
	{
		private ApiContractDescription m_Contract;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ApiContractCheckConvention()
			: base(Will.DecorateClass | Will.DecorateConstructors | Will.DecorateMethods | Will.DecorateProperties)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of DecorationConvention

		protected override void OnClass(ClassType classType, DecoratingClassWriter classWriter)
		{
			m_Contract = new ApiContractDescription(classType.Key);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
		{
			if ( member.Kind == MemberKind.VirtualMethod )
			{
				var description = m_Contract.TryGetMember<ApiMethodMemberDescription>(member.MemberDeclaration);

				if ( description != null )
				{
					description.ApplyChecks(decorate());
				}
			}
		}

		#endregion
	}
}
