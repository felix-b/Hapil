﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Hapil.Testing.NUnit;
using Hapil.Expressions;
using Hapil.Operands;
using NUnit.Framework;

namespace Hapil.UnitTests.Members
{
	[TestFixture]
	public class ConstructorTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void CanCreateObjectUsingNonDefaultConstructor()
		{
			//-- Arrange

			Field<int> intField;
			Field<string> stringField;

			DeriveClassFrom<AncestorRepository.BaseOne>()
				.Field<int>("m_IntField", out intField)
				.Field<string>("m_StringField", out stringField)
				.Constructor<int, string>((ctor, intValue, stringValue) => {
					ctor.Base();
					intField.Assign(intValue);
					stringField.Assign(stringValue);
				})
				.ImplementInterface<IMyFieldValues>()
				.Method<int>(intf => intf.GetIntFieldValue).Implement(f => {
					f.Return(intField);
				})
				.Method<string>(intf => intf.GetStringFieldValue).Implement(f => {
					f.Return(stringField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<IMyFieldValues>().UsingConstructor<int, string>(123, "ABC");

			//-- Assert

			Assert.That(obj.GetIntFieldValue(), Is.EqualTo(123));
			Assert.That(obj.GetStringFieldValue(), Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateObjectUsingPrimaryConstructor()
		{
			//-- Arrange

			Field<int> intField;
			Field<string> stringField;

			DeriveClassFrom<AncestorRepository.BaseOne>()
				.PrimaryConstructor("IntField", out intField, "StringField", out stringField)
				.ImplementInterface<IMyFieldValues>()
				.Method<int>(intf => intf.GetIntFieldValue).Implement(f => {
					f.Return(intField);
				})
				.Method<string>(intf => intf.GetStringFieldValue).Implement(f => {
					f.Return(stringField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<IMyFieldValues>().UsingConstructor<int, string>(123, "ABC");

			//-- Assert

			Assert.That(obj.GetIntFieldValue(), Is.EqualTo(123));
			Assert.That(obj.GetStringFieldValue(), Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateStaticConstructor()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.StaticConstructor(m => {
					Static.Prop(() => OutputList).Add(m.Const(".CCTOR"));
				})
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IOneProperty>()
				.AllProperties().ImplementAutomatic();

			OutputList = new List<string>();

			//-- Act

			var obj1 = CreateClassInstanceAs<AncestorRepository.IOneProperty>().UsingDefaultConstructor();
			var output1 = OutputList.ToArray();

			var obj2 = CreateClassInstanceAs<AncestorRepository.IOneProperty>().UsingDefaultConstructor();
			var output2 = OutputList.ToArray();

			//-- Assert

			Assert.That(output1, Is.EqualTo(new[] { ".CCTOR" }));
			Assert.That(output2, Is.EqualTo(new[] { ".CCTOR" }));
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanCallBaseConstructorGeneralizedWay()
        {
            //-- Arrange

            DeriveClassFrom<AncestorRepository.BaseWithConstructorParameters>()
                .Constructor<MemoryStream>((cw, strm) => {
                    var baseConstructor = typeof(AncestorRepository.BaseWithConstructorParameters)
                        .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(c => !c.IsPrivate)
                        .OrderByDescending(c => c.GetParameters().Length)
                        .First();
                    var baseArguments = new IOperand[3];
                    baseArguments[0] = strm;
                    baseArguments[1] = cw.Const<long>(12345);
                    baseArguments[2] = cw.Const<string>("ABCD");
                    cw.Base(baseConstructor, baseArguments);
                });

            var inputStream = new MemoryStream();

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.BaseWithConstructorParameters>().UsingConstructor(inputStream);

            //-- Assert

            Assert.That(obj.Stream, Is.SameAs(inputStream));
            Assert.That(obj.Index, Is.EqualTo(12345));
            Assert.That(obj.Name, Is.EqualTo("ABCD"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeStaticFields()
		{
			//-- Arrange

			Field<int> intField;
			Field<string> stringField;

			DeriveClassFrom<object>()
				.StaticField<int>("s_IntField", out intField)
				.StaticField<string>("s_StringField", out stringField)
				.StaticConstructor(m => {
					intField.Assign(123);
					stringField.Assign("ABC");
				})
				.DefaultConstructor()
				.ImplementInterface<IMyFieldValues>()
				.Method<int>(intf => intf.GetIntFieldValue).Implement(f => {
					f.Return(intField);
				})
				.Method<string>(intf => intf.GetStringFieldValue).Implement(f => {
					f.Return(stringField);
				});

			//-- Act

			var obj = CreateClassInstanceAs<IMyFieldValues>().UsingDefaultConstructor();

			//-- Assert

			Assert.That(obj.GetIntFieldValue(), Is.EqualTo(123));
			Assert.That(obj.GetStringFieldValue(), Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static List<string> OutputList { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMyFieldValues
		{
			int GetIntFieldValue();
			string GetStringFieldValue();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class MyFieldValuesExample : AncestorRepository.BaseOne, IMyFieldValues
		{
			private int m_IntField;
			private string m_StringField;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MyFieldValuesExample(int intValue, string stringValue)
			{
				m_IntField = intValue;
				m_StringField = stringValue;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int GetIntFieldValue()
			{
				return m_IntField;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string GetStringFieldValue()
			{
				return m_StringField;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static IMyFieldValues FactoryMethod1(int arg1, string arg2)
			{
				return new MyFieldValuesExample(arg1, arg2);
			}
		}
	}
}
