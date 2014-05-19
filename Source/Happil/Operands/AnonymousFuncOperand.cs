using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Writers;

namespace Happil.Operands
{
	internal class AnonymousFuncOperand<TReturn> : AnonymousDelegateOperand<Func<TReturn>>
	{
		public AnonymousFuncOperand(ClassType ownerClass, Action<FunctionMethodWriter<TReturn>> body)
			: base(ownerClass, new Type[0], typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: body,
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TReturn> : AnonymousDelegateOperand<Func<TA1, TReturn>>
	{
		public AnonymousFuncOperand(ClassType ownerClass, Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
			: base(ownerClass, new[] { typeof(TA1) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: w => body(w, new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TA2, TReturn> : AnonymousDelegateOperand<Func<TA1, TA2, TReturn>>
	{
		public AnonymousFuncOperand(ClassType ownerClass, Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: w => body(
					w, 
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TA2, TA3, TReturn> : AnonymousDelegateOperand<Func<TA1, TA2, TA3, TReturn>>
	{
		public AnonymousFuncOperand(
			ClassType ownerClass, 
			Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TA2, TA3, TA4, TReturn> : AnonymousDelegateOperand<Func<TA1, TA2, TA3, TA4, TReturn>>
	{
		public AnonymousFuncOperand(
			ClassType ownerClass,
			Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TA2, TA3, TA4, TA5, TReturn> : AnonymousDelegateOperand<Func<TA1, TA2, TA3, TA4, TA5, TReturn>>
	{
		public AnonymousFuncOperand(
			ClassType ownerClass,
			Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false),
					new Argument<TA5>(base.Statements, index: 5, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TA2, TA3, TA4, TA5, TA6, TReturn> : AnonymousDelegateOperand<Func<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>>
	{
		public AnonymousFuncOperand(
			ClassType ownerClass,
			Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false),
					new Argument<TA5>(base.Statements, index: 5, isByRef: false, isOut: false),
					new Argument<TA6>(base.Statements, index: 6, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn> : AnonymousDelegateOperand<Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>>
	{
		public AnonymousFuncOperand(
			ClassType ownerClass,
			Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
				ownerMethod: null,
				script: w => body(
					w,
					new Argument<TA1>(base.Statements, index: 1, isByRef: false, isOut: false),
					new Argument<TA2>(base.Statements, index: 2, isByRef: false, isOut: false),
					new Argument<TA3>(base.Statements, index: 3, isByRef: false, isOut: false),
					new Argument<TA4>(base.Statements, index: 4, isByRef: false, isOut: false),
					new Argument<TA5>(base.Statements, index: 5, isByRef: false, isOut: false),
					new Argument<TA6>(base.Statements, index: 6, isByRef: false, isOut: false),
					new Argument<TA7>(base.Statements, index: 7, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------------------------------------------

	internal class AnonymousFuncOperand<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn> : AnonymousDelegateOperand<Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>>
	{
		public AnonymousFuncOperand(
			ClassType ownerClass,
			Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
			: base(ownerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8) }, typeof(TReturn))
		{
			var writer = new FunctionMethodWriter<TReturn>(
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
					new Argument<TA8>(base.Statements, index: 8, isByRef: false, isOut: false)),
				mode: MethodWriterModes.Normal,
				attachToOwner: false);

			base.WriteMethodBody(writer);
		}
	}
}
