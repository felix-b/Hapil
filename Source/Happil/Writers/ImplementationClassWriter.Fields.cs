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
		public ImplementationClassWriter<TBase> Field<T>(string name, out FieldAccessOperand<T> field)
		{
			field = this.Field<T>(name);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Field<T>(string name, Func<FieldMember, AttributeWriter> attributes, out FieldAccessOperand<T> field)
		{
			var fieldMember = DefineField<T>(name, isStatic: false);
			fieldMember.AddAttributes(attributes);
			field = fieldMember.AsOperand<T>();
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticField<T>(string name, out FieldAccessOperand<T> field)
		{
			field = this.StaticField<T>(name);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticField<T>(string name, Func<FieldMember, AttributeWriter> attributes, out FieldAccessOperand<T> field)
		{
			var fieldMember = DefineField<T>(name, isStatic: true);
			fieldMember.AddAttributes(attributes);
			field = fieldMember.AsOperand<T>();
			return this;
		}
	}
}
