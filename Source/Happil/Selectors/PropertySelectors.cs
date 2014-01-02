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
		public abstract class Base<TBase> : IEnumerable<PropertyInfo>
		{
			internal Base(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
			{
				this.OwnerBody = ownerBody;
				this.SelectedProperties = selectedProperties.ToArray();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IEnumerable<PropertyInfo> Members

			public IEnumerator<PropertyInfo> GetEnumerator()
			{
				return SelectedProperties.AsEnumerable().GetEnumerator();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return SelectedProperties.GetEnumerator();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ForEach(Action<PropertyInfo> action)
			{
				foreach ( var property in SelectedProperties )
				{
					action(property);
				}

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic()
			{
				throw new NotImplementedException();
				//return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal IHappilClassBody<TBase> DefineMembers<TReturn>(
				Action<HappilMethod> invokeGetterBodyDefinition, 
				Action<HappilMethod> invokeSetterBodyDefinition)
			{
				//var propertiesToImplement = OwnerBody.HappilClass.TakeNotImplementedMembers(SelectedProperties);

				//foreach ( var declaration in propertiesToImplement )
				//{
				//	HappilMethod methodMember = (
				//		declaration.IsVoid()
				//		? (HappilMethod)new VoidHappilMethod(OwnerBody.HappilClass, declaration)
				//		: (HappilMethod)new HappilMethod<TReturn>(OwnerBody.HappilClass, declaration));

				//	OwnerBody.HappilClass.RegisterMember(
				//		methodMember,
				//		bodyDefinition: () => {
				//			using ( methodMember.CreateBodyScope() )
				//			{
				//				invokeBodyDefinition(methodMember);
				//			}
				//		});
				//}

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

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
				Func<IHappilPropertyBody<object>, IHappilPropertySetter> setter = null)
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
				Func<IHappilPropertyBody<TProperty>, IHappilPropertySetter> setter = null)
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
				Func<IHappilPropertyBody<TProperty, TIndex>, IHappilPropertySetter> setter = null)
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
				Func<IHappilPropertyBody<TProperty, TIndex1, TIndex2>, IHappilPropertySetter> setter = null)
			{
				throw new NotImplementedException();
			}
		}
	}
}
