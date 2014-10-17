using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Fluent;

namespace Hapil.Selectors
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

			internal IHappilClassBody<TBase> DefineMembers<TProperty>(Action<HappilProperty.BodyBase> invokeAccessorBodyDefinitions)
			{
				var propertiesToImplement = OwnerBody.HappilClass.TakeNotImplementedMembers(SelectedProperties);

				foreach ( var declaration in propertiesToImplement )
				{
					using ( TypeTemplate.CreateScope(typeof(TypeTemplate.TProperty), declaration.PropertyType) )
					{
						var copyOfDeclaration = declaration;
						var propertyMember = OwnerBody.HappilClass.GetOrAddDeclaredMember(
							copyOfDeclaration,
							memberFactory: () => new HappilProperty(OwnerBody.HappilClass, copyOfDeclaration));

						invokeAccessorBodyDefinitions(propertyMember.GetPropertyBody<TypeTemplate.TProperty>());
					}
				}

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal IHappilClassBody<TBase> DefineAutomaticImplementation<TProperty>(Func<IHappilPropertyBody<TProperty>, IHappilAttributes> attributes)
			{
				DefineMembers<TProperty>(body => {
					ValidateAutomaticImplementation(body.Declaration);

					if ( attributes != null )
					{
						body.SetAttributes(attributes((IHappilPropertyBody<TProperty>)body) as HappilAttributes);
					}

					((IHappilPropertyBody<TProperty>)body).Get(m => m.Return(body.BackingField.AsOperand<TProperty>()));

					if ( body.Declaration.CanWrite )
					{
						((IHappilPropertyBody<TProperty>)body).Set((m, value) => body.BackingField.AsOperand<TProperty>().Assign(value));
					}
				});

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal HappilClassBody<TBase> OwnerBody { get; private set; }
			internal PropertyInfo[] SelectedProperties { get; private set; }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ValidateAutomaticImplementation(PropertyInfo declaration)
			{
				if ( !declaration.CanRead )
				{
					throw new NotSupportedException(string.Format(
						"Property '{0}' cannot have automatic implementation because it has no getter.", declaration.Name));
				}

				if ( declaration.IsIndexer() )
				{
					throw new NotSupportedException(string.Format(
						"Property '{0}' cannot have automatic implementation because it is an indexer.", declaration.Name));
				}
			}
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
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilPropertySetter> setter = null)
			{
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilAttributes> nullAttributes = null;
				return Implement(nullAttributes, getter, setter);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				IHappilAttributes attributes,
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilPropertySetter> setter = null)
			{
				return Implement(prop => attributes, getter, setter);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilAttributes> attributes,
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<TypeTemplate.TProperty>(body => {
					if ( attributes != null )
					{
						body.SetAttributes(attributes((IHappilPropertyBody<TypeTemplate.TProperty>)body) as HappilAttributes);
					}
					if ( getter != null )
					{
						getter((IHappilPropertyBody<TypeTemplate.TProperty>)body);
					}
					if ( setter != null )
					{
						setter((IHappilPropertyBody<TypeTemplate.TProperty>)body);
					}
				});

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic()
			{
				return DefineAutomaticImplementation<TypeTemplate.TProperty>(attributes: null);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic(IHappilAttributes attributes)
			{
				return DefineAutomaticImplementation<TypeTemplate.TProperty>(prop => attributes);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic(Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilAttributes> attributes)
			{
				return DefineAutomaticImplementation<TypeTemplate.TProperty>(attributes);
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
				return Implement(null, getter, setter);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				IHappilAttributes attributes,
				Func<IHappilPropertyBody<TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TProperty>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<TProperty>(body => {
					body.SetAttributes(attributes as HappilAttributes);
					
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

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic()
			{
				return DefineAutomaticImplementation<TypeTemplate.TProperty>(attributes: null);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic(IHappilAttributes attributes)
			{
				return DefineAutomaticImplementation<TypeTemplate.TProperty>(prop => attributes);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic(Func<IHappilPropertyBody<TypeTemplate.TProperty>, IHappilAttributes> attributes)
			{
				return DefineAutomaticImplementation<TypeTemplate.TProperty>(attributes);
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
				return Implement(null, getter, setter);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				IHappilAttributes attributes,
				Func<IHappilPropertyBody<TIndex, TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TIndex, TProperty>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<TProperty>(body => {
					body.SetAttributes(attributes as HappilAttributes);
		
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
				return Implement(null, getter, setter);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(
				IHappilAttributes attributes,
				Func<IHappilPropertyBody<TIndex1, TIndex2, TProperty>, IHappilPropertyGetter> getter,
				Func<IHappilPropertyBody<TIndex1, TIndex2, TProperty>, IHappilPropertySetter> setter = null)
			{
				DefineMembers<TProperty>(body => {
					body.SetAttributes(attributes as HappilAttributes);

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
