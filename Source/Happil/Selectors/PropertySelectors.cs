using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;

namespace Happil.Selectors
{
	public static class PropertySelectors
	{
		public abstract class Base<TBase>
		{
			internal Base(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
			{
				this.OwnerBody = ownerBody;
				this.SelectedProperties = selectedProperties.ToArray();
			}
			//internal IHappilClassBody<TBase> DefineMethod(Action<HappilMethod> invokeBodyDefinition, MethodInfo declaration)
			//{
			//	HappilMethod methodMember = (
			//		declaration.ReturnType == typeof(void) || declaration.ReturnType == null
			//		? new VoidHappilMethod(OwnerBody.HappilClass, declaration)
			//		: new HappilMethod(OwnerBody.HappilClass, declaration));

			//	OwnerBody.HappilClass.RegisterMember(methodMember, bodyDefinition: () => {
			//		using ( methodMember.CreateBodyScope() )
			//		{
			//			invokeBodyDefinition(methodMember);
			//		}
			//	});

			//	return OwnerBody;
			//}
			public IHappilClassBody<TBase> ImplementAutomatic()
			{
				throw new NotImplementedException();
				//return OwnerBody;
			}

			internal HappilClassBody<TBase> OwnerBody { get; private set; }
			internal PropertyInfo[] SelectedProperties { get; private set; }
		}
		public class Untyped<TBase> : Base<TBase>
		{
			internal Untyped(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}
			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<object>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<object>, IHappilPropertySetter> setter)
			{
				throw new NotImplementedException();
			}
		}
		public class Typed<TBase, TProperty> : Base<TBase>
		{
			internal Typed(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}
			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TProperty>, IHappilPropertySetter> setter)
			{
				throw new NotImplementedException();
			}
		}
		public class Indexer1Arg<TBase, TProperty, TIndex> : Base<TBase>
		{
			internal Indexer1Arg(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}
			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<TProperty, TIndex>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TProperty, TIndex>, IHappilPropertySetter> setter)
			{
				throw new NotImplementedException();
			}
		}
		public class Indexer2Args<TBase, TProperty, TIndex1, TIndex2> : Base<TBase>
		{
			internal Indexer2Args(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}
			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<TProperty, TIndex1, TIndex2>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TProperty, TIndex1, TIndex2>, IHappilPropertySetter> setter)
			{
				throw new NotImplementedException();
			}
		}
	}
}
