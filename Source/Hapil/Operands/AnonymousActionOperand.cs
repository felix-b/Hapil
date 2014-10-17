using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Writers;

namespace Hapil.Operands
{
	internal class AnonymousActionOperand : AnonymousDelegateOperand<Action>
	{
		public AnonymousActionOperand(ClassType ownerClass, Action<VoidMethodWriter> body)
			: base(ownerClass, new Type[0], null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: body);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1> : AnonymousDelegateOperand<Action<TA1>>
	{
		public AnonymousActionOperand(ClassType ownerClass, Action<VoidMethodWriter, Argument<TA1>> body)
			: base(ownerClass, new[] { typeof(TA1) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(w, new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1, TA2> : AnonymousDelegateOperand<Action<TA1, TA2>>
	{
		public AnonymousActionOperand(ClassType ownerClass, Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(
					w, 
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1, TA2, TA3> : AnonymousDelegateOperand<Action<TA1, TA2, TA3>>
	{
		public AnonymousActionOperand(
			ClassType ownerClass, 
			Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1, TA2, TA3, TA4> : AnonymousDelegateOperand<Action<TA1, TA2, TA3, TA4>>
	{
		public AnonymousActionOperand(
			ClassType ownerClass,
			Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1, TA2, TA3, TA4, TA5> : AnonymousDelegateOperand<Action<TA1, TA2, TA3, TA4, TA5>>
	{
		public AnonymousActionOperand(
			ClassType ownerClass,
			Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false),
					new Argument<TA5>(base.Statements, index: 5, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1, TA2, TA3, TA4, TA5, TA6> : AnonymousDelegateOperand<Action<TA1, TA2, TA3, TA4, TA5, TA6>>
	{
		public AnonymousActionOperand(
			ClassType ownerClass,
			Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false),
					new Argument<TA5>(base.Statements, index: 5, isByRef: false, isOut: false),
					new Argument<TA6>(base.Statements, index: 6, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1, TA2, TA3, TA4, TA5, TA6, TA7> : AnonymousDelegateOperand<Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7>>
	{
		public AnonymousActionOperand(
			ClassType ownerClass,
			Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false),
					new Argument<TA5>(base.Statements, index: 5, isByRef: false, isOut: false),
					new Argument<TA6>(base.Statements, index: 6, isByRef: false, isOut: false),
					new Argument<TA7>(base.Statements, index: 7, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousActionOperand<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> : AnonymousDelegateOperand<Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>>
	{
		public AnonymousActionOperand(
			ClassType ownerClass,
			Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8) }, null)
		{
			var writer = new VoidMethodWriter(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false),
					new Argument<TA5>(base.Statements, index: 5, isByRef: false, isOut: false),
					new Argument<TA6>(base.Statements, index: 6, isByRef: false, isOut: false),
					new Argument<TA7>(base.Statements, index: 7, isByRef: false, isOut: false),
					new Argument<TA8>(base.Statements, index: 8, isByRef: false, isOut: false)));

			base.WriteMethodBody(writer);
		}
	}
}
