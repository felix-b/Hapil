using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public partial class ImplementationClassWriter<TBase> : ClassWriterBase
	{
		public ITemplatePropertySelector AllProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<NA, NA, TypeTemplate.TProperty>(this, m_Members.ImplementableProperties.SelectIf(where));
		}
		public ITemplatePropertySelector ReadOnlyProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<NA, NA, TypeTemplate.TProperty>(this, m_Members.ImplementableProperties.Where(p => p.CanRead && !p.CanWrite).SelectIf(where));
		}
		public ITemplatePropertySelector ReadWriteProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<NA, NA, TypeTemplate.TProperty>(this, m_Members.ImplementableProperties.Where(p => p.CanRead && p.CanWrite).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertySelector<TProperty> Property<TProperty>(Expression<Func<TBase, TProperty>> property)
		{
			return new PropertySelector<NA, NA, TProperty>(this, Helpers.ResolvePropertyFromLambda(property));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertySelector<TProperty> Properties<TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<NA, NA, TProperty>(
				this,
				m_Members.ImplementableProperties.OfSignature(typeof(TProperty)).SelectIf(where));
		}
		public IPropertySelector<TIndex, TProperty> This<TIndex, TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<TIndex, NA, TProperty>(
				this,
				m_Members.ImplementableProperties.OfSignature(typeof(TProperty), typeof(TIndex)).SelectIf(where));
		}
		public IPropertySelector<TIndex1, TIndex2, TProperty> This<TIndex1, TIndex2, TProperty>(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<TIndex1, TIndex2, TProperty>(
				this,
				m_Members.ImplementableProperties.OfSignature(typeof(TProperty), typeof(TIndex1), typeof(TIndex2)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IPropertySelectorBase : IEnumerable<PropertyInfo>
		{
			ImplementationClassWriter<TBase> ImplementAutomatic();
			ImplementationClassWriter<TBase> ImplementAutomatic(Func<PropertyMember, AttributeWriter> attributes);
			ImplementationClassWriter<TBase> ImplementPropagate(IOperand<TBase> target);
			ImplementationClassWriter<TBase> ImplementPropagate(Func<PropertyMember, AttributeWriter> attributes, IOperand<TBase> target);
			ImplementationClassWriter<TBase> Throw<TException>(string message = null);
			ImplementationClassWriter<TBase> ForEach(Action<PropertyInfo> action);
		}
		public interface ITemplatePropertySelector : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> Implement(
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterSetter> setter = null);
			ImplementationClassWriter<TBase> Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterSetter> setter = null);
		}
		public interface IPropertySelector<T> : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> ImplementAutomatic(FieldAccessOperand<T> backingField);
			ImplementationClassWriter<TBase> ImplementAutomatic(Func<PropertyMember, AttributeWriter> attributes, FieldAccessOperand<T> backingField);
			ImplementationClassWriter<TBase> Implement(
				Func<PropertyWriter<T>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<T>, PropertyWriterBase.IPropertyWriterSetter> setter = null);
			ImplementationClassWriter<TBase> Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<PropertyWriter<T>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<T>, PropertyWriterBase.IPropertyWriterSetter> setter = null);
		}
		public interface IPropertySelector<TIndex1, T> : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> Implement(
				Func<PropertyWriter<TIndex1, T>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, T>, PropertyWriterBase.IPropertyWriterSetter> setter = null);
			ImplementationClassWriter<TBase> Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<PropertyWriter<TIndex1, T>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, T>, PropertyWriterBase.IPropertyWriterSetter> setter = null);
		}
		public interface IPropertySelector<TIndex1, TIndex2, T> : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> Implement(
				Func<PropertyWriter<TIndex1, TIndex2, T>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, TIndex2, T>, PropertyWriterBase.IPropertyWriterSetter> setter = null);
			ImplementationClassWriter<TBase> Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<PropertyWriter<TIndex1, TIndex2, T>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, TIndex2, T>, PropertyWriterBase.IPropertyWriterSetter> setter = null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class PropertySelector<TIndex1, TIndex2, TProperty> : 
			IPropertySelectorBase,
			ITemplatePropertySelector,
			IPropertySelector<TProperty>,
			IPropertySelector<TIndex1, TProperty>,
			IPropertySelector<TIndex1, TIndex2, TProperty>
		{
			private readonly ClassType m_OwnerClass;
			private readonly ImplementationClassWriter<TBase> m_ClassWriter;
			private readonly PropertyInfo[] m_SelectedProperties;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public PropertySelector(ImplementationClassWriter<TBase> classWriter, IEnumerable<PropertyInfo> selectedProperties)
				: this(classWriter, selectedProperties.ToArray())
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public PropertySelector(ImplementationClassWriter<TBase> classWriter, params PropertyInfo[] selectedProperties)
			{
				m_OwnerClass = classWriter.OwnerClass;
				m_ClassWriter = classWriter;
				m_SelectedProperties = selectedProperties;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IEnumerable Members

			IEnumerator<PropertyInfo> IEnumerable<PropertyInfo>.GetEnumerator()
			{
				return ((IEnumerable<PropertyInfo>)m_SelectedProperties).GetEnumerator();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return m_SelectedProperties.GetEnumerator();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IPropertySelectorBase Members

			ImplementationClassWriter<TBase> IPropertySelectorBase.ImplementAutomatic()
			{
				return DefinePropertyImplementations(property => new AutomaticPropertyWriter(property));
			}
			ImplementationClassWriter<TBase> IPropertySelectorBase.ImplementAutomatic(Func<PropertyMember, AttributeWriter> attributes)
			{
				return DefinePropertyImplementations(attributes, property => new AutomaticPropertyWriter(property));
			}
			ImplementationClassWriter<TBase> IPropertySelectorBase.ImplementPropagate(IOperand<TBase> target)
			{
				return DefinePropertyImplementations(property => new PropagatingPropertyWriter(property, target));
			}
			ImplementationClassWriter<TBase> IPropertySelectorBase.ImplementPropagate(Func<PropertyMember, AttributeWriter> attributes, IOperand<TBase> target)
			{
				return DefinePropertyImplementations(attributes, property => new PropagatingPropertyWriter(property, target));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			ImplementationClassWriter<TBase> IPropertySelectorBase.Throw<TException>(string message)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			ImplementationClassWriter<TBase> IPropertySelectorBase.ForEach(Action<PropertyInfo> action)
			{
				foreach ( var method in m_SelectedProperties )
				{
					action(method);
				}

				return m_ClassWriter;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ITemplatePropertySelector Members

			ImplementationClassWriter<TBase> ITemplatePropertySelector.Implement(
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(p => new TemplatePropertyWriter(p, getter, setter));
			}
			ImplementationClassWriter<TBase> ITemplatePropertySelector.Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<TemplatePropertyWriter, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(attributes, p => new TemplatePropertyWriter(p, getter, setter));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IPropertySelector<TProperty> Members

			ImplementationClassWriter<TBase> IPropertySelector<TProperty>.ImplementAutomatic(
				FieldAccessOperand<TProperty> backingField)
			{
				return DefinePropertyImplementations(p => new AutomaticPropertyWriter(p), backingField);
			}
			ImplementationClassWriter<TBase> IPropertySelector<TProperty>.ImplementAutomatic(
				Func<PropertyMember, AttributeWriter> attributes,
				FieldAccessOperand<TProperty> backingField)
			{
				return DefinePropertyImplementations(attributes, p => new AutomaticPropertyWriter(p), backingField);
			}
			ImplementationClassWriter<TBase> IPropertySelector<TProperty>.Implement(
				Func<PropertyWriter<TProperty>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TProperty>, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(p => new PropertyWriter<TProperty>(p, getter, setter));
			}
			ImplementationClassWriter<TBase> IPropertySelector<TProperty>.Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<PropertyWriter<TProperty>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TProperty>, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(attributes, p => new PropertyWriter<TProperty>(p, getter, setter));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IPropertySelector<TIndex1,TProperty> Members

			ImplementationClassWriter<TBase> IPropertySelector<TIndex1, TProperty>.Implement(
				Func<PropertyWriter<TIndex1, TProperty>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, TProperty>, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(p => new PropertyWriter<TIndex1, TProperty>(p, getter, setter));
			}
			ImplementationClassWriter<TBase> IPropertySelector<TIndex1, TProperty>.Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<PropertyWriter<TIndex1, TProperty>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, TProperty>, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(attributes, p => new PropertyWriter<TIndex1, TProperty>(p, getter, setter));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IPropertySelector<TIndex1,TIndex2,TProperty> Members

			ImplementationClassWriter<TBase> IPropertySelector<TIndex1, TIndex2, TProperty>.Implement(
				Func<PropertyWriter<TIndex1, TIndex2, TProperty>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, TIndex2, TProperty>, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(p => new PropertyWriter<TIndex1, TIndex2, TProperty>(p, getter, setter));
			}
			ImplementationClassWriter<TBase> IPropertySelector<TIndex1, TIndex2, TProperty>.Implement(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<PropertyWriter<TIndex1, TIndex2, TProperty>, PropertyWriterBase.IPropertyWriterGetter> getter,
				Func<PropertyWriter<TIndex1, TIndex2, TProperty>, PropertyWriterBase.IPropertyWriterSetter> setter)
			{
				return DefinePropertyImplementations(attributes, p => new PropertyWriter<TIndex1, TIndex2, TProperty>(p, getter, setter));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private ImplementationClassWriter<TBase> DefinePropertyImplementations<TWriter>(
				Func<PropertyMember, TWriter> writerFactory, 
				FieldMember backingField = null)
				where TWriter : PropertyWriterBase
			{
				return DefinePropertyImplementations<TWriter>(attributes: null, writerFactory: writerFactory, backingField: backingField);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private ImplementationClassWriter<TBase> DefinePropertyImplementations<TWriter>(
				Func<PropertyMember, AttributeWriter> attributes,
				Func<PropertyMember, TWriter> writerFactory,
				FieldMember backingField = null)
				where TWriter : PropertyWriterBase
			{
				var propertiesToImplement = m_OwnerClass.TakeNotImplementedMembers(m_SelectedProperties);

				foreach ( var property in propertiesToImplement )
				{
					var propertyMember = new PropertyMember(m_OwnerClass, property, backingField);
					m_OwnerClass.AddMember(propertyMember);
					
					var writer = writerFactory(propertyMember);
					writer.AddAttributes(attributes);
				}

				return m_ClassWriter;
			}
		}
	}
}
