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
			var constructor = DefineConstructor(ConstructorMethodFactory.DefaultConstructor(OwnerClass));
			var writer = new DefaultConstructorWriter(constructor);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticConstructor(Action<ConstructorWriter> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, body);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1>(Action<ConstructorWriter, Argument<TA1>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>()));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
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
