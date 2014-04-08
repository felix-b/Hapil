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
			m_Decorator.OnClassType(OwnerClass, this);
			OwnerClass.ForEachMember<MemberBase>(VisitMember);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitMember(MemberBase member)
		{
			var methodMember = member as MethodMember;
			var propertyMember = member as PropertyMember;
			var eventMember = member as EventMember;
			var fieldMember = member as FieldMember;

			if ( methodMember != null )
			{
				VisitMethod(methodMember);
			}
			else if ( propertyMember != null )
			{
				VisitProperty(propertyMember);
			}
			else if ( eventMember != null )
			{
				VisitEvent(eventMember);
			}
			else if ( fieldMember != null )
			{
				VisitField(fieldMember);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitField(FieldMember fieldMember)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitEvent(EventMember eventMember)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitProperty(PropertyMember propertyMember)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitMethod(MethodMember methodMember)
		{
			switch ( methodMember.Kind )
			{
				case MemberKind.VirtualMethod:
				case MemberKind.InstanceAnonymousMethod:
				case MemberKind.StaticAnonymousMethod:
					m_Decorator.OnMethod(methodMember, () => new DecoratingMethodWriter(methodMember).DecorationBuilder);
					break;
			}
		}
	}
}
