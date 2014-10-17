using System;
using Hapil.Decorators;
using Hapil.Members;
using Hapil.Writers;

namespace Hapil
{
	public abstract class DecorationConvention : IObjectFactoryConvention
	{
		private readonly Will m_Will;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected DecorationConvention(Will will)
		{
			m_Will = will;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IObjectFactoryConvention Members

		bool IObjectFactoryConvention.ShouldApply(ObjectFactoryContext context)
		{
			return this.ShouldApply(context);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IObjectFactoryConvention.Apply(ObjectFactoryContext context)
		{
			context.CreateDecorationWriter(this);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual bool ShouldApply(ObjectFactoryContext context)
		{
			return true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnClass(ClassType classType, DecoratingClassWriter classWriter)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnStaticField(FieldMember member, Func<FieldDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnStaticConstructor(MethodMember member, Func<ConstructorDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnField(FieldMember member, Func<FieldDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnConstructor(MethodMember member, Func<ConstructorDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnEvent(EventMember member, Func<EventDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void VisitClass(ClassType classType, DecoratingClassWriter classWriter)
		{
			OnClass(classType, classWriter);
			classType.ForEachMember<MemberBase>(VisitMember);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitMember(MemberBase member)
		{
			switch ( member.Kind )
			{
				case MemberKind.StaticField:
				case MemberKind.InstanceField:
					VisitMemberAs<FieldMember>(member, Will.DecorateFields, VisitField);
					break;
				case MemberKind.StaticConstructor:
				case MemberKind.InstanceConstructor:
					VisitMemberAs<ConstructorMember>(member, Will.DecorateConstructors, VisitConstructor);
					break;
				case MemberKind.StaticAnonymousMethod:
				case MemberKind.InstanceAnonymousMethod:
				case MemberKind.VirtualMethod:
					VisitMemberAs<MethodMember>(member, Will.DecorateMethods, VisitMethod);
					break;
				case MemberKind.InstanceProperty:
					VisitMemberAs<PropertyMember>(member, Will.DecorateProperties, VisitProperty);
					break;
				case MemberKind.InstanceEvent:
					VisitMemberAs<EventMember>(member, Will.DecorateEvents, VisitEvent);
					break;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitMemberAs<TMember>(MemberBase member, Will willFlag, Action<TMember> visitAction) where TMember : MemberBase
		{
			if ( m_Will.HasFlag(willFlag) )
			{
				visitAction((TMember)member);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitField(FieldMember fieldMember)
		{
			var decoratorFactory = new LazyFactory<FieldDecorationBuilder>(() => new FieldDecorationBuilder(fieldMember));

			if ( fieldMember.IsStatic )
			{
				OnStaticField(fieldMember, decoratorFactory.GetObject);
			}
			else
			{
				OnField(fieldMember, decoratorFactory.GetObject);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitEvent(EventMember eventMember)
		{
			var decoratorFactory = new LazyFactory<EventDecorationBuilder>(() => new EventDecorationBuilder(eventMember));
			OnEvent(eventMember, decoratorFactory.GetObject);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitProperty(PropertyMember propertyMember)
		{
			var decoratorFactory = new LazyFactory<PropertyDecorationBuilder>(() => new PropertyDecorationBuilder(propertyMember));
			OnProperty(propertyMember, decoratorFactory.GetObject);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitMethod(MethodMember methodMember)
		{
			var decoratorFactory = new LazyFactory<MethodDecorationBuilder>(() => new DecoratingMethodWriter(methodMember).DecorationBuilder);
			OnMethod(methodMember, decoratorFactory.GetObject);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void VisitConstructor(ConstructorMember methodMember)
		{
			var decoratorFactory = new LazyFactory<ConstructorDecorationBuilder>(() =>
				(ConstructorDecorationBuilder)new DecoratingConstructorWriter(methodMember).DecorationBuilder);

			if ( methodMember.IsStatic )
			{
				OnStaticConstructor(methodMember, decoratorFactory.GetObject);
			}
			else
			{
				OnConstructor(methodMember, decoratorFactory.GetObject);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Flags]
		public enum Will
		{
			DecorateClass = 0,
			DecorateFields = 0x01,
			DecorateConstructors = 0x02,
			DecorateMethods = 0x04,
			DecorateProperties = 0x08,
			DecorateEvents = 0x10
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
