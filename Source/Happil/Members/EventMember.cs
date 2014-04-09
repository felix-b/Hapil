using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Writers;

namespace Happil.Members
{
	public class EventMember : MemberBase
	{
		private const MethodAttributes ContainedMethodAttributes =
			MethodAttributes.Public |
			MethodAttributes.NewSlot |
			MethodAttributes.Final |
			MethodAttributes.HideBySig |
			MethodAttributes.SpecialName |
			MethodAttributes.Virtual;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private readonly EventInfo m_Declaration;
		private readonly EventBuilder m_EventBuilder;
		private readonly List<EventWriterBase> m_Writers;
		private readonly MethodMember m_AddMethod;
		private readonly MethodMember m_RemoveMethod;
		private readonly string m_UniqueName;
		private FieldMember m_BackingField;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventMember(ClassType ownerClass, EventInfo declaration)
			: base(ownerClass, declaration.Name)
		{
			m_Writers = new List<EventWriterBase>();
			m_Declaration = declaration;
			m_UniqueName = ownerClass.TakeMemberName(declaration.Name);
			
			m_EventBuilder = ownerClass.TypeBuilder.DefineEvent(
				m_UniqueName,
				declaration.Attributes,
				declaration.EventHandlerType);

			m_AddMethod = new MethodMember(ownerClass, new VirtualMethodFactory(ownerClass, declaration.GetAddMethod()));
			m_EventBuilder.SetAddOnMethod((MethodBuilder)m_AddMethod.MethodFactory.Builder);

			m_RemoveMethod = new MethodMember(ownerClass, new VirtualMethodFactory(ownerClass, declaration.GetRemoveMethod()));
			m_EventBuilder.SetRemoveOnMethod((MethodBuilder)m_RemoveMethod.MethodFactory.Builder);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO: refactor to remove code duplication with same method in writer class
		internal void AddAttributes(Func<EventMember, AttributeWriter> attributeWriterFactory)
		{
			if ( attributeWriterFactory != null )
			{
				var attributeWriter = attributeWriterFactory(this);

				if ( attributeWriter != null )
				{
					foreach ( var attribute in attributeWriter.GetAttributes() )
					{
						m_EventBuilder.SetCustomAttribute(attribute);
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberInfo MemberDeclaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventInfo EventDeclaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public EventBuilder EventBuilder
		{
			get
			{
				return m_EventBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldMember BackingField
		{
			get
			{
				if ( m_BackingField == null )
				{
					m_BackingField = new FieldMember(OwnerClass, "m_" + m_Declaration.Name + "EventHandler", m_Declaration.EventHandlerType);
					OwnerClass.AddMember(m_BackingField);
				}

				return m_BackingField;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override string Name
		{
			get
			{
				return m_UniqueName;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override MemberKind Kind
		{
			get
			{
				return MemberKind.InstanceEvent;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember AddMethod
		{
			get
			{
				return m_AddMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodMember RemoveMethod
		{
			get
			{
				return m_RemoveMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override IDisposable CreateTypeTemplateScope()
		{
			return TypeTemplate.CreateScope<TypeTemplate.TEventHandler>(m_Declaration.EventHandlerType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddWriter(EventWriterBase writer)
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

			if ( m_AddMethod != null )
			{
				using ( m_AddMethod.CreateTypeTemplateScope() )
				{
					m_AddMethod.Write();
				}
			}

			if ( m_RemoveMethod != null )
			{
				using ( m_RemoveMethod.CreateTypeTemplateScope() )
				{
					m_RemoveMethod.Write();
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override void Compile()
		{
			using ( m_AddMethod.CreateTypeTemplateScope() )
			{
				m_AddMethod.Compile();
			}

			using ( m_RemoveMethod.CreateTypeTemplateScope() )
			{
				m_RemoveMethod.Compile();
			}
		}
	}
}
