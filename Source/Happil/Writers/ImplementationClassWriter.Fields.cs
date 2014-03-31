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
		public FieldAccessOperand<T> Field<T>(string name)
		{
			var field = DefineField<T>(name, isStatic: false);
			return field.AsOperand<T>(); //TODO: check that T is compatible with field type
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Field<T>(string name, out FieldAccessOperand<T> field)
		{
			field = this.Field<T>(name);
			return this;
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------
		//TODO:redesign
		//public ImplementationClassWriter<TBase> Field<T>(string name, IHappilAttributes attributes, out FieldAccessOperand<T> field)
		//{
		//	var fieldMember = DefineField<T>(name, isStatic: false);
		//	fieldMember.SetAttributes(attributes as HappilAttributes);
		//	field = fieldMember.AsOperand<T>();
		//	return this;
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldAccessOperand<T> StaticField<T>(string name)
		{
			var fieldMember = DefineField<T>(name, isStatic: true);
			return fieldMember.AsOperand<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticField<T>(string name, out FieldAccessOperand<T> field)
		{
			field = this.StaticField<T>(name);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO:redesign
		//public ImplementationClassWriter<TBase> StaticField<T>(string name, IHappilAttributes attributes, out FieldAccessOperand<T> field)
		//{
		//	var fieldMember = DefineField<T>(name, isStatic: true);
		//	fieldMember.SetAttributes(attributes as HappilAttributes);
		//	field = fieldMember.AsOperand<T>();
		//	return this;
		//}
	}
}
