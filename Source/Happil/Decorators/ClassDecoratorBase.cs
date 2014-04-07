using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil.Decorators
{
	public abstract class ClassDecoratorBase
	{
		public virtual void OnClassType(ClassType classType)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnStaticField(FieldMember member, Func<FieldDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnStaticConstructor(MethodMember member, Func<ConstructorDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnField(FieldMember member, Func<FieldDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnConstructor(MethodMember member, Func<ConstructorDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual void OnEvent(EventMember member, Func<EventDecorationBuilder> decorate)
		{
		}
	}
}
