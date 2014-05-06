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

		internal FieldMember(ClassType ownerClass, string name, Type fieldType, bool isStatic = false, bool isPublic = false)
			: base(ownerClass, name)
		{
			m_IsStatic = isStatic;

			var actualType = TypeTemplate.Resolve(fieldType);
			var attributes = GetFieldAttributes(isStatic, isPublic);
			var uniqueName = ownerClass.TakeMemberName(name);

			m_FieldBuilder = ownerClass.TypeBuilder.DefineField(uniqueName, actualType, attributes);
			m_Writers = new List<FieldWriter>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Field<T> AsOperand<T>()
		{
			var targetOperand = (m_IsStatic ? null : new ThisOperand<object>(OwnerClass));
			return new Field<T>(targetOperand, this);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Field<T> AsOperand<T>(IOperand target)
		{
			return new Field<T>(target, this);
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

		public Type FieldType
		{
			get
			{
				return m_FieldBuilder.FieldType;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberKind Kind
		{
			get
			{
				return (m_FieldBuilder.IsStatic ? MemberKind.StaticField : MemberKind.InstanceField);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool IsStatic
		{
			get
			{
				return m_FieldBuilder.IsStatic;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddWriter(FieldWriter writer)
		{
			m_Writers.Add(writer);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddAttributes(Func<FieldMember, AttributeWriter> attributeWriterFactory)
		{
			if ( attributeWriterFactory != null )
			{
				var attributeWriter = attributeWriterFactory(this);

				if ( attributeWriter != null )
				{
					foreach ( var attribute in attributeWriter.GetAttributes() )
					{
						m_FieldBuilder.SetCustomAttribute(attribute);
					}
				}
			}
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static FieldAttributes GetFieldAttributes(bool isStatic, bool isPublic)
		{
			return (
				(isStatic ? FieldAttributes.Static : 0) |
				(isPublic ? FieldAttributes.Public : FieldAttributes.Private));
		}
	}
}
