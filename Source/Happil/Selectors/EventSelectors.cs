using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;

namespace Happil.Selectors
{
	public static class EventSelectors
	{
		public abstract class Base<TBase>
		{
			internal Base(HappilClassBody<TBase> ownerBody, IEnumerable<EventInfo> selectedEvents)
			{
				this.OwnerBody = ownerBody;
				this.SelectedEvents = selectedEvents.ToArray();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ImplementAutomatic()
			{
				var eventsToImplement = OwnerBody.HappilClass.TakeNotImplementedMembers(SelectedEvents);

				foreach ( var declaration in eventsToImplement )
				{
					var declarationCopy = declaration;
					OwnerBody.HappilClass.GetOrAddDeclaredMember(
						declaration, 
						memberFactory: () => new HappilEvent(OwnerBody.HappilClass, declarationCopy));
				}

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal HappilClassBody<TBase> OwnerBody { get; private set; }
			internal EventInfo[] SelectedEvents { get; private set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		public class Untyped<TBase> : Base<TBase>
		{
			internal Untyped(HappilClassBody<TBase> ownerBody, IEnumerable<EventInfo> selectedEvents)
				: base(ownerBody, selectedEvents)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			//public IHappilClassBody<TBase> Implement(
			//	Action<IVoidHappilMethodBody, HappilArgument<Delegate>> add,
			//	Action<IVoidHappilMethodBody, HappilArgument<Delegate>> remove)
			//{
			//	throw new NotImplementedException();
			//}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public class Typed<TBase, THandler> : Base<TBase>
		//{
		//	internal Typed(HappilClassBody<TBase> ownerBody, IEnumerable<EventInfo> selectedEvents)
		//		: base(ownerBody, selectedEvents)
		//	{
		//	}

		//	IHappilClassBody<TBase> Implement(
		//		Action<IVoidHappilMethodBody, HappilArgument<THandler>> add,
		//		Action<IVoidHappilMethodBody, HappilArgument<THandler>> remove)
		//	{
		//		throw new NotImplementedException();
		//	}
		//}
	}
}
