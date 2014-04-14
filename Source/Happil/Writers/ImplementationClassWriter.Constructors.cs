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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1>(
			string arg1Name, out Field<TA1> field1, 
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1));
			field1 = fields[0].AsOperand<TA1>();
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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1, TA2>(
			string arg1Name, out Field<TA1> field1,
			string arg2Name, out Field<TA2> field2,
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1), arg2Name, typeof(TA2));
			field1 = fields[0].AsOperand<TA1>();
			field2 = fields[1].AsOperand<TA2>();
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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1, TA2, TA3>(
			string arg1Name, out Field<TA1> field1,
			string arg2Name, out Field<TA2> field2,
			string arg3Name, out Field<TA3> field3,
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1), arg2Name, typeof(TA2), arg3Name, typeof(TA3));
			field1 = fields[0].AsOperand<TA1>();
			field2 = fields[1].AsOperand<TA2>();
			field3 = fields[2].AsOperand<TA3>();
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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1, TA2, TA3, TA4>(
			string arg1Name, out Field<TA1> field1,
			string arg2Name, out Field<TA2> field2,
			string arg3Name, out Field<TA3> field3,
			string arg4Name, out Field<TA4> field4,
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1), arg2Name, typeof(TA2), arg3Name, typeof(TA3), arg4Name, typeof(TA4));
			field1 = fields[0].AsOperand<TA1>();
			field2 = fields[1].AsOperand<TA2>();
			field3 = fields[2].AsOperand<TA3>();
			field4 = fields[3].AsOperand<TA4>();
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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1, TA2, TA3, TA4, TA5>(
			string arg1Name, out Field<TA1> field1,
			string arg2Name, out Field<TA2> field2,
			string arg3Name, out Field<TA3> field3,
			string arg4Name, out Field<TA4> field4,
			string arg5Name, out Field<TA5> field5,
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1), arg2Name, typeof(TA2), arg3Name, typeof(TA3), arg4Name, typeof(TA4), arg5Name, typeof(TA5));
			field1 = fields[0].AsOperand<TA1>();
			field2 = fields[1].AsOperand<TA2>();
			field3 = fields[2].AsOperand<TA3>();
			field4 = fields[3].AsOperand<TA4>();
			field5 = fields[4].AsOperand<TA5>();
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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1, TA2, TA3, TA4, TA5, TA6>(
			string arg1Name, out Field<TA1> field1,
			string arg2Name, out Field<TA2> field2,
			string arg3Name, out Field<TA3> field3,
			string arg4Name, out Field<TA4> field4,
			string arg5Name, out Field<TA5> field5,
			string arg6Name, out Field<TA6> field6,
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1), arg2Name, typeof(TA2), arg3Name, typeof(TA3), arg4Name, typeof(TA4), arg5Name, typeof(TA5), arg6Name, typeof(TA6));
			field1 = fields[0].AsOperand<TA1>();
			field2 = fields[1].AsOperand<TA2>();
			field3 = fields[2].AsOperand<TA3>();
			field4 = fields[3].AsOperand<TA4>();
			field5 = fields[4].AsOperand<TA5>();
			field6 = fields[5].AsOperand<TA6>();
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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(
			string arg1Name, out Field<TA1> field1,
			string arg2Name, out Field<TA2> field2,
			string arg3Name, out Field<TA3> field3,
			string arg4Name, out Field<TA4> field4,
			string arg5Name, out Field<TA5> field5,
			string arg6Name, out Field<TA6> field6,
			string arg7Name, out Field<TA7> field7,
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1), arg2Name, typeof(TA2), arg3Name, typeof(TA3), arg4Name, typeof(TA4), arg5Name, typeof(TA5), arg6Name, typeof(TA6), arg7Name, typeof(TA7));
			field1 = fields[0].AsOperand<TA1>();
			field2 = fields[1].AsOperand<TA2>();
			field3 = fields[2].AsOperand<TA3>();
			field4 = fields[3].AsOperand<TA4>();
			field5 = fields[4].AsOperand<TA5>();
			field6 = fields[5].AsOperand<TA6>();
			field7 = fields[6].AsOperand<TA7>();
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
		public ImplementationClassWriter<TBase> PrimaryConstructor<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(
			string arg1Name, out Field<TA1> field1,
			string arg2Name, out Field<TA2> field2,
			string arg3Name, out Field<TA3> field3,
			string arg4Name, out Field<TA4> field4,
			string arg5Name, out Field<TA5> field5,
			string arg6Name, out Field<TA6> field6,
			string arg7Name, out Field<TA7> field7,
			string arg8Name, out Field<TA8> field8,
			Func<MethodMember, AttributeWriter> attributes = null)
		{
			var fields = DefinePrimaryConstructor(attributes, arg1Name, typeof(TA1), arg2Name, typeof(TA2), arg3Name, typeof(TA3), arg4Name, typeof(TA4), arg5Name, typeof(TA5), arg6Name, typeof(TA6), arg7Name, typeof(TA7), arg8Name, typeof(TA8));
			field1 = fields[0].AsOperand<TA1>();
			field2 = fields[1].AsOperand<TA2>();
			field3 = fields[2].AsOperand<TA3>();
			field4 = fields[3].AsOperand<TA4>();
			field5 = fields[4].AsOperand<TA5>();
			field6 = fields[5].AsOperand<TA6>();
			field7 = fields[6].AsOperand<TA7>();
			field8 = fields[7].AsOperand<TA8>();
			return this;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ConstructorMember DefineConstructor(ConstructorMethodFactory factory)
		{
			var constructorMember = new ConstructorMember(OwnerClass, factory);
			OwnerClass.AddMember(constructorMember);
			return constructorMember;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private FieldMember[] DefinePrimaryConstructor(Func<MethodMember, AttributeWriter> attributes, params object[] argumentNameTypePairs)
		{
			if ( (argumentNameTypePairs.Length % 2) != 0 )
			{
				throw new ArgumentException("argumentNameTypePairs must be of even length");
			}

			var argumentNames = new string[argumentNameTypePairs.Length / 2];
			var fieldNames = new string[argumentNameTypePairs.Length / 2];
			var fieldTypes = new Type[argumentNameTypePairs.Length / 2];

			for ( int i = 0 ; i < fieldNames.Length ; i ++ )
			{
				var name = (string)argumentNameTypePairs[i * 2];

				argumentNames[i] = name.Substring(0, 1).ToLower() + name.Substring(1);
				fieldNames[i] = "m_" + name;
				fieldTypes[i] = (Type)argumentNameTypePairs[i * 2 + 1];
			}

			var constructorMember = new ConstructorMember(OwnerClass, ConstructorMethodFactory.InstanceConstructor(OwnerClass, fieldTypes, argumentNames));
			OwnerClass.AddMember(constructorMember);

			var fieldMembers = fieldTypes.Select((type, index) => DefineField(fieldNames[index], type, isStatic: false)).ToArray();
			var constructorWriter = new PrimaryConstructorWriter(constructorMember, fieldMembers);
			constructorWriter.AddAttributes(attributes);

			return fieldMembers;
		}
	}
}
