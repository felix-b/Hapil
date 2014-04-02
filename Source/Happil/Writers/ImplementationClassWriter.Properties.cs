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
			return new PropertySelector<NA, NA, TypeTemplate.TProperty>(this, m_Members.ImplementableProperties);
		}
		public ITemplatePropertySelector ReadOnlyProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<NA, NA, TypeTemplate.TProperty>(this, m_Members.ImplementableProperties.Where(p => p.CanRead && !p.CanWrite));
		}
		public ITemplatePropertySelector ReadWriteProperties(Func<PropertyInfo, bool> where = null)
		{
			return new PropertySelector<NA, NA, TypeTemplate.TProperty>(this, m_Members.ImplementableProperties.Where(p => p.CanRead && p.CanWrite));
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
			ImplementationClassWriter<TBase> Throw<TException>(string message);
			ImplementationClassWriter<TBase> ForEach(Action<PropertyInfo> action);
		}
		public interface ITemplatePropertySelector : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<TemplatePropertyWriter> body);
		}
		public interface IPropertySelector<T> : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<PropertyWriter<T>> body);
		}
		public interface IPropertySelector<TIndex1, T> : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<PropertyWriter<TIndex1, T>> body);
		}
		public interface IPropertySelector<TIndex1, TIndex2, T> : IPropertySelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<PropertyWriter<TIndex1, TIndex2, T>> body);
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
				return DefinePropertyImplementations(p => new AutomaticPropertyWriter(p));
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

			ImplementationClassWriter<TBase> ITemplatePropertySelector.Implement(Action<TemplatePropertyWriter> body)
			{
				return DefinePropertyImplementations(p => new TemplatePropertyWriter(p, body));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IPropertySelector<TProperty> Members

			ImplementationClassWriter<TBase> IPropertySelector<TProperty>.Implement(Action<PropertyWriter<TProperty>> body)
			{
				return DefinePropertyImplementations(p => new PropertyWriter<TProperty>(p, body));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IPropertySelector<TIndex1,TProperty> Members

			ImplementationClassWriter<TBase> IPropertySelector<TIndex1, TProperty>.Implement(Action<PropertyWriter<TIndex1, TProperty>> body)
			{
				return DefinePropertyImplementations(p => new PropertyWriter<TIndex1, TProperty>(p, body));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IPropertySelector<TIndex1,TIndex2,TProperty> Members

			ImplementationClassWriter<TBase> IPropertySelector<TIndex1, TIndex2, TProperty>.Implement(Action<PropertyWriter<TIndex1, TIndex2, TProperty>> body)
			{
				return DefinePropertyImplementations(p => new PropertyWriter<TIndex1, TIndex2, TProperty>(p, body));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private ImplementationClassWriter<TBase> DefinePropertyImplementations<TWriter>(Func<PropertyMember, TWriter> writerFactory)
				where TWriter : PropertyWriterBase
			{
				var propertiesToImplement = m_OwnerClass.TakeNotImplementedMembers(m_SelectedProperties);

				foreach ( var property in propertiesToImplement )
				{
					var propertyMember = new PropertyMember(m_OwnerClass, property);
					m_OwnerClass.AddMember(propertyMember);
					writerFactory(propertyMember);
				}

				return m_ClassWriter;
			}
		}
	}
}
