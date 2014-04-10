using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class AutomaticEventWriter : EventWriterBase
	{
		public AutomaticEventWriter(EventMember ownerEvent)
			: base(ownerEvent)
		{
			var addOn = new VoidMethodWriter(
				OwnerEvent.AddMethod,
				w => WriteAddRemoveMethod(w, w.Arg1<TypeTemplate.TEventHandler>(), manipulation: Delegate.Combine));

			var removeOn = new VoidMethodWriter(
				OwnerEvent.RemoveMethod,
				w => WriteAddRemoveMethod(w, w.Arg1<TypeTemplate.TEventHandler>(), manipulation: Delegate.Remove));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void WriteAddRemoveMethod(
			VoidMethodWriter w, 
			Argument<TypeTemplate.TEventHandler> value, 
			Func<Delegate, Delegate, Delegate> manipulation)
		{
			var oldHandler = w.Local<TypeTemplate.TEventHandler>();
			var newHandler = w.Local<TypeTemplate.TEventHandler>();
			var lastHandler = w.Local<TypeTemplate.TEventHandler>(initialValue: OwnerEvent.BackingField.AsOperand<TypeTemplate.TEventHandler>());

			w.Do(loop => {
				oldHandler.Assign(lastHandler);

				newHandler.Assign(Static.Func(manipulation,
					oldHandler.CastTo<Delegate>(),
					value.CastTo<Delegate>()).CastTo<TypeTemplate.TEventHandler>());

				lastHandler.Assign(Static.GenericFunc((x, y, z) => Interlocked.CompareExchange(ref x, y, z),
					OwnerEvent.BackingField.AsOperand<TypeTemplate.TEventHandler>(),
					newHandler,
					oldHandler));
			}).While(lastHandler != oldHandler);
		}
	}
}
