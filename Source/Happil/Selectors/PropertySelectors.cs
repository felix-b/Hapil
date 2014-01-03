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

			internal IHappilClassBody<TBase> DefineMembers<TProperty>(Action<HappilProperty<TProperty>.BodyBase> invokeAccessorBodyDefinitions)
			{
				var propertiesToImplement = OwnerBody.HappilClass.TakeNotImplementedMembers(SelectedProperties);

				foreach ( var declaration in propertiesToImplement )
				{
					var propertyMember = new HappilProperty<TProperty>(OwnerBody.HappilClass, declaration);
					OwnerBody.HappilClass.RegisterMember(propertyMember, propertyMember.BodyDefinitionAction);
					
					invokeAccessorBodyDefinitions(propertyMember.Body);
				}

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal HappilClassBody<TBase> OwnerBody { get; private set; }
			internal PropertyInfo[] SelectedProperties { get; private set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Untyped<TBase> : Base<TBase>
		{
			internal Untyped(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<object>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<object>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<object>(body => {
					if ( getter != null )
					{
						getter((IHappilPropertyBody<object>)body);
					}
					if ( setter != null )
					{
						setter((IHappilPropertyBody<object>)body);
					}
				});

				return OwnerBody;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Typed<TBase, TProperty> : Base<TBase>
		{
			internal Typed(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TProperty>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<TProperty>(body => {
					if ( getter != null )
					{
						getter((IHappilPropertyBody<TProperty>)body);
					}
					if ( setter != null )
					{
						setter((IHappilPropertyBody<TProperty>)body);
					}
				});

				return OwnerBody;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Indexer1Arg<TBase, TIndex, TProperty> : Base<TBase>
		{
			internal Indexer1Arg(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<TIndex, TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TIndex, TProperty>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<TProperty>(body => {
					if ( getter != null )
					{
						getter((IHappilPropertyBody<TIndex, TProperty>)body);
					}
					if ( setter != null )
					{
						setter((IHappilPropertyBody<TIndex, TProperty>)body);
					}
				});

				return OwnerBody;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Indexer2Args<TBase, TIndex1, TIndex2, TProperty> : Base<TBase>
		{
			internal Indexer2Args(HappilClassBody<TBase> ownerBody, IEnumerable<PropertyInfo> selectedProperties)
				: base(ownerBody, selectedProperties)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<TIndex1, TIndex2, TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TIndex1, TIndex2, TProperty>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<TProperty>(body => {
					if ( getter != null )
					{
						getter((IHappilPropertyBody<TIndex1, TIndex2, TProperty>)body);
					}
					if ( setter != null )
					{
						setter((IHappilPropertyBody<TIndex1, TIndex2, TProperty>)body);
					}
				});

				return OwnerBody;
			}
		}
	}
}
