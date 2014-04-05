using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class TemplateEventWriter : EventWriterBase
	{
		public TemplateEventWriter(
			EventMember ownerEvent,
			Func<TemplateEventWriter, EventWriterBase.IEventWriterAddOn> addScript,
			Func<TemplateEventWriter, EventWriterBase.IEventWriterRemoveOn> removeScript)
			: base(ownerEvent)
		{
			if ( addScript != null )
			{
				addScript(this);
			}

			if ( removeScript != null )
			{
				removeScript(this);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IEventWriterAddOn Add(Action<VoidMethodWriter, Argument<TypeTemplate.TEventHandler>> script)
		{
			var writer = new VoidMethodWriter(
				OwnerEvent.AddMethod, 
				w => script(w, w.Arg1<TypeTemplate.TEventHandler>()));
			
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IEventWriterRemoveOn Remove(Action<VoidMethodWriter, Argument<TypeTemplate.TEventHandler>> script)
		{
			var writer = new VoidMethodWriter(
				OwnerEvent.RemoveMethod,
				w => script(w, w.Arg1<TypeTemplate.TEventHandler>()));
			
			return null;
		}
	}
}
