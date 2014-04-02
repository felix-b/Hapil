using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class PropertyWriter<T> : PropertyWriterBase
	{
		private readonly Func<PropertyWriter<T>, IPropertyWriterGetter> m_GetterScript;
		private readonly Func<PropertyWriter<T>, IPropertyWriterSetter> m_SetterScript;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyWriter(
			PropertyMember ownerProperty,
			Func<PropertyWriter<T>, PropertyWriterBase.IPropertyWriterGetter> getterScript,
			Func<PropertyWriter<T>, PropertyWriterBase.IPropertyWriterSetter> setterScript)
			: base(ownerProperty)
		{
			m_GetterScript = getterScript;
			m_SetterScript = setterScript;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterGetter Get(Action<FunctionMethodWriter<T>> body)
		{
			var writer = new FunctionMethodWriter<T>(OwnerProperty.GetterMethod, body);
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterSetter Set(Action<VoidMethodWriter, Argument<T>> body)
		{
			var writer = new VoidMethodWriter(OwnerProperty.SetterMethod, w => body(w, w.Arg1<T>()));
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( m_GetterScript != null )
			{
				m_GetterScript(this);
			}

			if ( m_SetterScript != null )
			{
				m_SetterScript(this);
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------
	
	public class PropertyWriter<TIndex1, T> : PropertyWriterBase
	{
		private readonly Func<PropertyWriter<TIndex1, T>, IPropertyWriterGetter> m_GetterScript;
		private readonly Func<PropertyWriter<TIndex1, T>, IPropertyWriterSetter> m_SetterScript;

		public PropertyWriter(
			PropertyMember ownerProperty,
			Func<PropertyWriter<TIndex1, T>, PropertyWriterBase.IPropertyWriterGetter> getterScript,
			Func<PropertyWriter<TIndex1, T>, PropertyWriterBase.IPropertyWriterSetter> setterScript)
			: base(ownerProperty)
		{
			m_GetterScript = getterScript;
			m_SetterScript = setterScript;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterGetter Get(Action<FunctionMethodWriter<T>, Argument<TIndex1>> body)
		{
			var writer = new FunctionMethodWriter<T>(
				OwnerProperty.GetterMethod, 
				w => body(w, w.Arg1<TIndex1>()));
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterSetter Set(Action<VoidMethodWriter, Argument<TIndex1>, Argument<T>> body)
		{
			var writer = new VoidMethodWriter(
				OwnerProperty.SetterMethod, 
				w => body(w, w.Arg1<TIndex1>(), w.Arg2<T>()));
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( m_GetterScript != null )
			{
				m_GetterScript(this);
			}

			if ( m_SetterScript != null )
			{
				m_SetterScript(this);
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------
	
	public class PropertyWriter<TIndex1, TIndex2, T> : PropertyWriterBase
	{
		private readonly Func<PropertyWriter<TIndex1, TIndex2, T>, IPropertyWriterGetter> m_GetterScript;
		private readonly Func<PropertyWriter<TIndex1, TIndex2, T>, IPropertyWriterSetter> m_SetterScript;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyWriter(
			PropertyMember ownerProperty,
			Func<PropertyWriter<TIndex1, TIndex2, T>, PropertyWriterBase.IPropertyWriterGetter> getterScript,
			Func<PropertyWriter<TIndex1, TIndex2, T>, PropertyWriterBase.IPropertyWriterSetter> setterScript)
			: base(ownerProperty)
		{
			m_GetterScript = getterScript;
			m_SetterScript = setterScript;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterGetter Get(Action<FunctionMethodWriter<T>, Argument<TIndex1>, Argument<TIndex2>> body)
		{
			var writer = new FunctionMethodWriter<T>(
				OwnerProperty.GetterMethod, 
				w => body(w, w.Arg1<TIndex1>(), w.Arg2<TIndex2>()));
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IPropertyWriterSetter Set(Action<VoidMethodWriter, Argument<TIndex1>, Argument<TIndex2>, Argument<T>> body)
		{
			var writer = new VoidMethodWriter(
				OwnerProperty.SetterMethod,
				w => body(w, w.Arg1<TIndex1>(), w.Arg2<TIndex2>(), w.Arg3<T>()));
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( m_GetterScript != null )
			{
				m_GetterScript(this);
			}

			if ( m_SetterScript != null )
			{
				m_SetterScript(this);
			}
		}
	}
}
