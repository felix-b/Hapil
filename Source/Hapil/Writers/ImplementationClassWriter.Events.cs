using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Writers
{
	public partial class ImplementationClassWriter<TBase> : ClassWriterBase
	{
		public ITemplateEventSelector AllEvents(Func<EventInfo, bool> where = null)
		{
			return new EventSelector(this, m_Members.ImplementableEvents.SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IEventSelectorBase : IEnumerable<EventInfo>
		{
			ImplementationClassWriter<TBase> ImplementAutomatic();
			ImplementationClassWriter<TBase> ImplementAutomatic(Func<EventMember, AttributeWriter> attributes);
			ImplementationClassWriter<TBase> ImplementPropagate(IOperand<TBase> target);
			ImplementationClassWriter<TBase> ImplementPropagate(Func<EventMember, AttributeWriter> attributes, IOperand<TBase> target);
			ImplementationClassWriter<TBase> ForEach(Action<EventInfo> action);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITemplateEventSelector : IEventSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(
				Func<TemplateEventWriter, EventWriterBase.IEventWriterAddOn> add,
				Func<TemplateEventWriter, EventWriterBase.IEventWriterRemoveOn> remove);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class EventSelector : IEventSelectorBase, ITemplateEventSelector
		{
			private readonly ClassType m_OwnerClass;
			private readonly ImplementationClassWriter<TBase> m_ClassWriter;
			private readonly EventInfo[] m_SelectedEvents;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public EventSelector(ImplementationClassWriter<TBase> classWriter, IEnumerable<EventInfo> selectedEvents)
				: this(classWriter, selectedEvents.ToArray())
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public EventSelector(ImplementationClassWriter<TBase> classWriter, params EventInfo[] selectedEvents)
			{
				m_OwnerClass = classWriter.OwnerClass;
				m_ClassWriter = classWriter;
				m_SelectedEvents = selectedEvents;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IEnumerable Members

			IEnumerator<EventInfo> IEnumerable<EventInfo>.GetEnumerator()
			{
				return ((IEnumerable<EventInfo>)m_SelectedEvents).GetEnumerator();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return m_SelectedEvents.GetEnumerator();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IEventSelectorBase Members

			public ImplementationClassWriter<TBase> ImplementAutomatic()
			{
				DefineEventImplementations(@event => new AutomaticEventWriter(@event));
				return m_ClassWriter;
			}
			public ImplementationClassWriter<TBase> ImplementAutomatic(Func<EventMember, AttributeWriter> attributes)
			{
				return DefineEventImplementations(attributes, @event => new AutomaticEventWriter(@event));
			}
			public ImplementationClassWriter<TBase> ImplementPropagate(IOperand<TBase> target)
			{
				return DefineEventImplementations(@event => new PropagatingEventWriter(@event, target));
			}
			public ImplementationClassWriter<TBase> ImplementPropagate(Func<EventMember, AttributeWriter> attributes, IOperand<TBase> target)
			{
				return DefineEventImplementations(attributes, @event => new PropagatingEventWriter(@event, target));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ImplementationClassWriter<TBase> ForEach(Action<EventInfo> action)
			{
				foreach ( var singleEvent in m_SelectedEvents )
				{
					action(singleEvent);
				}

				return m_ClassWriter;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ITemplateEventSelector Members

			ImplementationClassWriter<TBase> ITemplateEventSelector.Implement(
				Func<TemplateEventWriter, EventWriterBase.IEventWriterAddOn> add,
				Func<TemplateEventWriter, EventWriterBase.IEventWriterRemoveOn> remove)
			{
				return DefineEventImplementations(@event => new TemplateEventWriter(@event, add, remove));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private ImplementationClassWriter<TBase> DefineEventImplementations<TWriter>(Func<EventMember, TWriter> writerFactory) 
				where TWriter : EventWriterBase
			{
				return DefineEventImplementations(attributes: null, writerFactory: writerFactory);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private ImplementationClassWriter<TBase> DefineEventImplementations<TWriter>(
				Func<EventMember, AttributeWriter> attributes,
				Func<EventMember, TWriter> writerFactory)
				where TWriter : EventWriterBase
			{
				var eventsToImplement = m_OwnerClass.TakeNotImplementedMembers(m_SelectedEvents);

				foreach ( var singleEvent in eventsToImplement )
				{
					var eventMember = new EventMember(m_OwnerClass, singleEvent);
					m_OwnerClass.AddMember(eventMember);
					
					var writer = writerFactory(eventMember);
					writer.AddAttributes(attributes);
				}

				return m_ClassWriter;
			}
		}
	}
}
