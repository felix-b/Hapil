using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Operands;
using Happil.Writers;

namespace Happil.Members
{
	public class FieldMember : MemberBase
	{
		private readonly FieldBuilder m_FieldBuilder;
		private readonly bool m_IsStatic;
		private readonly List<FieldWriter> m_Writers;
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal FieldMember(ClassType ownerClass, string name, Type fieldType, bool isStatic = false)
			: base(ownerClass, name)
		{
			m_IsStatic = isStatic;

			var actualType = TypeTemplate.Resolve(fieldType);
			var attributes = (isStatic ? FieldAttributes.Private | FieldAttributes.Static : FieldAttributes.Private);
			var uniqueName = ownerClass.TakeMemberName(name);

			m_FieldBuilder = ownerClass.TypeBuilder.DefineField(uniqueName, actualType, attributes);
			m_Writers = new List<FieldWriter>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldAccessOperand<T> AsOperand<T>()
		{
			var targetOperand = (m_IsStatic ? null : new ThisOperand<object>());
			return new FieldAccessOperand<T>(targetOperand, m_FieldBuilder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberInfo MemberDeclaration
		{
			get
			{
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string Name
		{
			get
			{
				return m_FieldBuilder.Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddWriter(FieldWriter writer)
		{
			m_Writers.Add(writer);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Write()
		{
			foreach ( var writer in m_Writers )
			{
				writer.Flush();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Compile()
		{
			// nothing
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal FieldBuilder FieldBuilder
		{
			get
			{
				return m_FieldBuilder;
			}
		}
	}
}
