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
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			var addOn = new VoidMethodWriter(
				OwnerEvent.AddMethod,
				w => WriteAddOnMethod(w, w.Arg1<TypeTemplate.TEventHandler>()));

			var setter = new VoidMethodWriter(
				OwnerEvent.RemoveMethod,
				w => WriteRemoveOnMethod(w, w.Arg1<TypeTemplate.TEventHandler>()));

			base.Flush();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO: refactor into template method
		private void WriteAddOnMethod(VoidMethodWriter m, Argument<TypeTemplate.TEventHandler> value)
		{
			var oldHandler = m.Local<TypeTemplate.TEventHandler>();
			var newHandler = m.Local<TypeTemplate.TEventHandler>();
			var lastHandler = m.Local<TypeTemplate.TEventHandler>(initialValue: OwnerEvent.BackingField.AsOperand<TypeTemplate.TEventHandler>());

			m.Do(loop => {
				oldHandler.Assign(lastHandler);

				newHandler.Assign(Static.Func(Delegate.Combine,
					oldHandler.CastTo<Delegate>(),
					value.CastTo<Delegate>()).CastTo<TypeTemplate.TEventHandler>());

				lastHandler.Assign(Static.GenericFunc((x, y, z) => Interlocked.CompareExchange(ref x, y, z),
					OwnerEvent.BackingField.AsOperand<TypeTemplate.TEventHandler>(),
					newHandler,
					oldHandler));
			}).While(lastHandler != oldHandler);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//TODO: refactor into template method
		private void WriteRemoveOnMethod(VoidMethodWriter m, Argument<TypeTemplate.TEventHandler> value)
		{
			var oldHandler = m.Local<TypeTemplate.TEventHandler>();
			var newHandler = m.Local<TypeTemplate.TEventHandler>();
			var lastHandler = m.Local<TypeTemplate.TEventHandler>(initialValue: OwnerEvent.BackingField.AsOperand<TypeTemplate.TEventHandler>());

			m.Do(loop => {
				oldHandler.Assign(lastHandler);

				newHandler.Assign(Static.Func(Delegate.Remove,
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
