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
		public ImplementationClassWriter<TBase> Field<T>(string name, out Field<T> field, bool isPublic = false)
		{
			field = this.Field<T>(name, isPublic: isPublic);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Field<T>(string name, Func<FieldMember, AttributeWriter> attributes, out Field<T> field, bool isPublic = false)
		{
			var fieldMember = DefineField<T>(name, isStatic: false, isPublic: isPublic);
			fieldMember.AddAttributes(attributes);
			field = fieldMember.AsOperand<T>();
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticField<T>(string name, out Field<T> field, bool isPublic = false)
		{
			field = this.StaticField<T>(name, isPublic: isPublic);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticField<T>(string name, Func<FieldMember, AttributeWriter> attributes, out Field<T> field, bool isPublic = false)
		{
			var fieldMember = DefineField<T>(name, isStatic: true, isPublic: isPublic);
			fieldMember.AddAttributes(attributes);
			field = fieldMember.AsOperand<T>();
			return this;
		}
	}
}
