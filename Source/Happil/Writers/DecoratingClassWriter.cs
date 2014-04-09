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
			var constructorMember = member as ConstructorMember;
			var methodMember = member as MethodMember;
			var propertyMember = member as PropertyMember;
			var fieldMember = member as FieldMember;
			var eventMember = member as EventMember;

			if ( constructorMember != null )
			{
				VisitConstructor(constructorMember);
			}
			else if ( methodMember != null )
			{
				VisitMethod(methodMember);
			}
			else if ( propertyMember != null )
			{
				VisitProperty(propertyMember);
			}
			else if ( fieldMember != null )
			{
				VisitField(fieldMember);
			}
			else if ( eventMember != null )
			{
				VisitEvent(eventMember);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitField(FieldMember fieldMember)
		{
			var decoratorFactory = new LazyFactory<FieldDecorationBuilder>(() => new FieldDecorationBuilder(fieldMember));

			if ( fieldMember.IsStatic )
			{
				m_Decorator.OnStaticField(fieldMember, decoratorFactory.GetObject);
			}
			else
			{
				m_Decorator.OnField(fieldMember, decoratorFactory.GetObject);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitEvent(EventMember eventMember)
		{
			var decoratorFactory = new LazyFactory<EventDecorationBuilder>(() => new EventDecorationBuilder(eventMember));
			m_Decorator.OnEvent(eventMember, decoratorFactory.GetObject);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitProperty(PropertyMember propertyMember)
		{
			var decoratorFactory = new LazyFactory<PropertyDecorationBuilder>(() => new PropertyDecorationBuilder(propertyMember));
			m_Decorator.OnProperty(propertyMember, decoratorFactory.GetObject);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitMethod(MethodMember methodMember)
		{
			var decoratorFactory = new LazyFactory<MethodDecorationBuilder>(() => new DecoratingMethodWriter(methodMember).DecorationBuilder);

			switch ( methodMember.Kind )
			{
				case MemberKind.VirtualMethod:
				case MemberKind.InstanceAnonymousMethod:
				case MemberKind.StaticAnonymousMethod:
					m_Decorator.OnMethod(methodMember, decoratorFactory.GetObject);
					break;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitConstructor(MethodMember methodMember)
		{
			var decoratorFactory = new LazyFactory<ConstructorDecorationBuilder>(() => new ConstructorDecorationBuilder(new DecoratingMethodWriter(methodMember)));

			if ( methodMember.IsStatic )
			{
				m_Decorator.OnStaticConstructor(methodMember, decoratorFactory.GetObject);
			}
			else
			{
				m_Decorator.OnConstructor(methodMember, decoratorFactory.GetObject);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class LazyFactory<T> where T : class
		{
			private readonly Func<T> m_RealFactory;
			private T m_Object;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public LazyFactory(Func<T> realFactory)
			{
				m_RealFactory = realFactory;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T GetObject()
			{
				if ( m_Object == null )
				{
					m_Object = m_RealFactory();
				}

				return m_Object;
			}
		}
	}
}
