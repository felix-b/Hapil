using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public partial class ImplementationClassWriter<TBase> : ClassWriterBase
	{
		public ImplementationClassWriter<TBase> DefaultConstructor()
		{
			DefineConstructor(ConstructorMethodFactory.DefaultConstructor(OwnerClass));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticConstructor(Action<ConstructorWriter> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			body(new ConstructorWriter(constructor));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1>(Action<ConstructorWriter, Argument<TA1>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor);
			body(writer, writer.Arg1<TA1>());
			
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor);
			body(writer, writer.Arg1<TA1>(), writer.Arg2<TA2>());

			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3>(
			Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor);
			body(writer, writer.Arg1<TA1>(), writer.Arg2<TA2>(), writer.Arg3<TA3>());

			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4>(
			Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor);
			body(writer, writer.Arg1<TA1>(), writer.Arg2<TA2>(), writer.Arg3<TA3>(), writer.Arg4<TA4>());

			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5>(
			Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor);
			body(writer, writer.Arg1<TA1>(), writer.Arg2<TA2>(), writer.Arg3<TA3>(), writer.Arg4<TA4>(), writer.Arg5<TA5>());

			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private MethodMember DefineConstructor(ConstructorMethodFactory factory)
		{
			var constructorMember = new MethodMember(OwnerClass, factory);
			OwnerClass.AddMember(constructorMember);
			return constructorMember;
		}
	}
}
