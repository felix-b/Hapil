using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Decorators;
using Happil.Members;

namespace Happil.Writers
{
	public class DecoratingClassWriter : ClassWriterBase
	{
		private readonly ClassDecoratorBase m_Decorator;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DecoratingClassWriter(ClassType ownerClass, ClassDecoratorBase decorator)
			: base(ownerClass)
		{
			m_Decorator = decorator;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			//m_Decorator.OnClassType(OwnerClass);

			//var allMembersOrderedByKind = OwnerClass.GetAllMembers().OrderBy(m => m.Kind).ToArray();

			//foreach ( var member in allMembersOrderedByKind )
			//{
			//	var currentMethodMember = member;

			//	switch ( member.Kind )
			//	{
			//		case MemberKind.StaticConstructor:
			//			m_Decorator.OnStaticConstructor((MethodMember)member, );
			//	}

			//	//OnMethod(currentMethodMember, () => new DecoratingMethodWriter(currentMethodMember).DecorationBuilder);
			//}
		}
	}
}
