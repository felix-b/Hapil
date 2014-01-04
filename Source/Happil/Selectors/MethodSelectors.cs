using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;

namespace Happil.Selectors
{
	public static class MethodSelectors
	{
		public abstract class Base<TBase> : IEnumerable<MethodInfo>
		{
			internal Base(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
			{
				this.OwnerBody = ownerBody;
				this.SelectedMethods = selectedMethods.ToArray();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IEnumerable<MethodInfo> Members

			public IEnumerator<MethodInfo> GetEnumerator()
			{
				return SelectedMethods.AsEnumerable().GetEnumerator();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return SelectedMethods.GetEnumerator();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Throw<TException>(string message = null) where TException : Exception
			{
				return DefineMembers<object>(m => m.Throw<TException>(message));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> ForEach(Action<MethodInfo> action)
			{
				foreach ( var method in SelectedMethods )
				{
					action(method);
				}

				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal IHappilClassBody<TBase> DefineMembers<TReturn>(Action<HappilMethod> invokeBodyDefinition)
			{
				var methodsToImplement = OwnerBody.HappilClass.TakeNotImplementedMembers(SelectedMethods);

				foreach ( var declaration in methodsToImplement )
				{
					HappilMethod methodMember = (
						declaration.IsVoid()
						? (HappilMethod)new VoidHappilMethod(OwnerBody.HappilClass, declaration)
						: (HappilMethod)new HappilMethod<TReturn>(OwnerBody.HappilClass, declaration));

					OwnerBody.HappilClass.RegisterMember(
						methodMember,
						bodyDefinition: () => {
							using ( methodMember.CreateBodyScope() )
							{
								invokeBodyDefinition(methodMember);
							}
						});
				}
				
				return OwnerBody;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal HappilClassBody<TBase> OwnerBody { get; private set; }
			internal MethodInfo[] SelectedMethods { get; private set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Untyped<TBase> : Base<TBase>
		{
			internal Untyped(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(Action<IHappilMethodBodyTemplate> body)
			{
				return DefineMembers<object>(body);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		public class Void<TBase> : Base<TBase>
		{
			internal Void(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(Action<IVoidHappilMethodBody> body)
			{
				return DefineMembers<object>(methodMember => body((IVoidHappilMethodBody)methodMember));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		public class Void1Arg<TBase, TArg1> : Base<TBase>
		{
			internal Void1Arg(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(Action<IVoidHappilMethodBody, HappilArgument<TArg1>> body)
			{
				return DefineMembers<object>(methodMember => body(
					(IVoidHappilMethodBody)methodMember,
					new HappilArgument<TArg1>(methodMember, 1)));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Void2Args<TBase, TArg1, TArg2> : Base<TBase>
		{
			internal Void2Args(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(Action<IVoidHappilMethodBody, HappilArgument<TArg1>, HappilArgument<TArg2>> body)
			{
				return DefineMembers<object>(methodMember => body(
					(IVoidHappilMethodBody)methodMember,
					new HappilArgument<TArg1>(methodMember, 1),
					new HappilArgument<TArg2>(methodMember, 2)));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Void3Args<TBase, TArg1, TArg2, TArg3> : Base<TBase>
		{
			internal Void3Args(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(Action<IVoidHappilMethodBody, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body)
			{
				return DefineMembers<object>(methodMember => body(
					(IVoidHappilMethodBody)methodMember,
					new HappilArgument<TArg1>(methodMember, 1),
					new HappilArgument<TArg2>(methodMember, 2),
					new HappilArgument<TArg3>(methodMember, 3)));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Functions<TBase, TReturn> : Base<TBase>
		{
			internal Functions(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(Action<IHappilMethodBody<TReturn>> body)
			{
				return DefineMembers<TReturn>(methodMember => body((IHappilMethodBody<TReturn>)methodMember));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Functions1Arg<TBase, TArg1, TReturn> : Base<TBase>
		{
			internal Functions1Arg(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilClassBody<TBase> Implement(Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>> body)
			{
				return DefineMembers<TReturn>(methodMember => body(
					(IHappilMethodBody<TReturn>)methodMember,
					new HappilArgument<TArg1>(methodMember, 1)));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Functions2Args<TBase, TArg1, TArg2, TReturn> : Base<TBase>
		{
			internal Functions2Args(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
			
			public IHappilClassBody<TBase> Implement(Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>, HappilArgument<TArg2>> body)
			{
				return DefineMembers<TReturn>(methodMember => body(
					(IHappilMethodBody<TReturn>)methodMember,
					new HappilArgument<TArg1>(methodMember, 1),
					new HappilArgument<TArg2>(methodMember, 2)));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Functions3Args<TBase, TArg1, TArg2, TArg3, TReturn> : Base<TBase>
		{
			internal Functions3Args(HappilClassBody<TBase> ownerBody, IEnumerable<MethodInfo> selectedMethods)
				: base(ownerBody, selectedMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------
			
			public IHappilClassBody<TBase> Implement(Action<IHappilMethodBody<TReturn>, HappilArgument<TArg1>, HappilArgument<TArg2>, HappilArgument<TArg3>> body)
			{
				return DefineMembers<TReturn>(methodMember => body(
					(IHappilMethodBody<TReturn>)methodMember,
					new HappilArgument<TArg1>(methodMember, 1),
					new HappilArgument<TArg2>(methodMember, 2),
					new HappilArgument<TArg3>(methodMember, 3)));
			}
		}
	}
}
