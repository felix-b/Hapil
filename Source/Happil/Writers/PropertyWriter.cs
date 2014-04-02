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
		public PropertyWriter(PropertyMember ownerProperty, Action<PropertyWriter<T>> script)
			: base(ownerProperty)
		{
			script(this);
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
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------
	
	public class PropertyWriter<TIndex1, T> : PropertyWriterBase
	{
		public PropertyWriter(PropertyMember ownerProperty, Action<PropertyWriter<TIndex1, T>> script)
			: base(ownerProperty)
		{
			script(this);
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
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------
	
	public class PropertyWriter<TIndex1, TIndex2, T> : PropertyWriterBase
	{
		public PropertyWriter(PropertyMember ownerProperty, Action<PropertyWriter<TIndex1, TIndex2, T>> script)
			: base(ownerProperty)
		{
			script(this);
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
	}
}
