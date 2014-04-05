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
		public ImplementationClassWriter<TBase> DefaultConstructor(Func<MethodMember, AttributeWriter> attributes = null)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.DefaultConstructor(OwnerClass));
			var writer = new DefaultConstructorWriter(constructor);
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> StaticConstructor(Action<ConstructorWriter> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, body);
			return this;
		}
		public ImplementationClassWriter<TBase> StaticConstructor(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.StaticConstructor(OwnerClass));
			var writer = new ConstructorWriter(constructor, body);
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor(Action<ConstructorWriter> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, Type.EmptyTypes));
			var writer = new ConstructorWriter(constructor, body);
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, Type.EmptyTypes));
			var writer = new ConstructorWriter(constructor, body);
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1>(Action<ConstructorWriter, Argument<TA1>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>()));
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1, TA2>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>, Argument<TA2>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
			writer.AddAttributes(attributes);
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
			return this;
		}
		public ImplementationClassWriter<TBase> Constructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(Func<MethodMember, AttributeWriter> attributes, Action<ConstructorWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
		{
			var constructor = DefineConstructor(ConstructorMethodFactory.InstanceConstructor(OwnerClass, new[] { typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8) }));
			var writer = new ConstructorWriter(constructor, w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
			writer.AddAttributes(attributes);
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
