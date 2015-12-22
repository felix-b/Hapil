using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using Hapil;
using Hapil.Operands;
using Hapil.Expressions;
using Hapil.Statements;
using Hapil.Members;
using Hapil.Writers;
using NUnit.Framework;

namespace Hapil.UnitTests.Operands
{
	[TestFixture]
	public class AssignmentTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void AssignFieldToField()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();

			var field1 = classWriter.Field<int>("f1");
			var field2 = classWriter.Field<int>("f2");

			//-- Act

			classWriter.Method(cls => cls.VirtualVoidMethod).Implement(w => {
				field1.Assign(field2);
			});

			CreateClassInstanceAs<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			var method = classWriter.OwnerClass.GetMemberByName<MethodMember>("VirtualVoidMethod");

			Assert.That(
				method.GetMethodText(),
				Is.EqualTo("VirtualVoidMethod():void{[this.Field[f1] = this.Field[f2]];}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AssignConstToField()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();

			var field1 = classWriter.Field<int>("f1");
			IOperand<int> const1 = new Constant<int>(123);

			//-- Act

			classWriter.Method(cls => cls.VirtualVoidMethod).Implement(m => {
				field1.Assign(const1);
			});

			CreateClassInstanceAs<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			var method = classWriter.OwnerClass.GetMemberByName<MethodMember>("VirtualVoidMethod");

			Assert.That(
				method.GetMethodText(),
				Is.EqualTo("VirtualVoidMethod():void{[this.Field[f1] = Const[123]];}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AssignExpressionToField()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();

			var field1 = classWriter.Field<int>("f1");
			var field2 = classWriter.Field<int>("f2");

			//-- Act

			classWriter.Method(cls => cls.VirtualVoidMethod).Implement(m => {
				field1.Assign(field2 + 123);
			});

			CreateClassInstanceAs<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			var method = classWriter.OwnerClass.GetMemberByName<MethodMember>("VirtualVoidMethod");

			Assert.That(
				method.GetMethodText(),
				Is.EqualTo("VirtualVoidMethod():void{[this.Field[f1] = [this.Field[f2] + Const[123]]];}"));
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanAssignPublicFieldOfStruct()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.ITester>()
                .Method<string>(intf => intf.TestFunc).Implement(m => {
                    var structLocal = m.Local<TestStruct>(initialValue: m.New<TestStruct>());
                    structLocal.Field(x => x.Value).Assign(123);
                    m.Return(structLocal.FuncToString());
                })
                .AllMethods().Throw<NotImplementedException>();

            //-- Act

            var tester = CreateClassInstanceAs<AncestorRepository.ITester>().UsingDefaultConstructor();
            var result = tester.TestFunc();

            //-- Assert

            Assert.That(result, Is.EqualTo("123"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanReadPublicPropertyOfStruct()
        {
            var dt = DateTime.Now;
            var st = dt.ToString("yyyy-MM-dd");

            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.ITester>()
                .Method<string>(intf => intf.TestFunc).Implement(m => {
                    var sourceLocal = m.Local<DateTime>();
                    sourceLocal.Assign(m.New<DateTime>(
                        m.Const(2015), m.Const(10), m.Const(24), m.Const(10), m.Const(40), m.Const(50), m.Const(999), m.Const(DateTimeKind.Utc)
                    ));

                    var transformedLocal = m.Local<DateTime>();
                    transformedLocal.Assign(
                        m.New<DateTime>(
                            sourceLocal.Prop(x => x.Year),
                            sourceLocal.Prop(x => x.Month),
                            sourceLocal.Prop(x => x.Day),
                            sourceLocal.Prop(x => x.Hour),
                            sourceLocal.Prop(x => x.Minute),
                            sourceLocal.Prop(x => x.Second),
                            sourceLocal.Prop(x => x.Millisecond),
                            m.Const(DateTimeKind.Utc)
                        )
                    );
                    m.Return(transformedLocal.Func<string, string>(x => x.ToString, m.Const("yyyy-MM-dd HH:mm:ss.fff")));
                })
                .AllMethods().Throw<NotImplementedException>();

            //-- Act

            var tester = CreateClassInstanceAs<AncestorRepository.ITester>().UsingDefaultConstructor();
            var result = tester.TestFunc();

            //-- Assert

            Assert.That(result, Is.EqualTo("2015-10-24 10:40:50.999"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public struct TestStruct
        {
            public int Value;

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class StructFieldAssignmentCompiledExample : AncestorRepository.ITester
        {
            #region ITester Members

            public void TestAction()
            {
                throw new NotImplementedException();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public string TestFunc()
            {
                var s = new TestStruct();
                s.Value = 123;
                return s.ToString();
            }

            #endregion
        }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestClassOne1 : AncestorRepository.BaseOne
		{
			private int f1;

			#region Overrides of TestBaseOne

			public override void VirtualVoidMethod()
			{
				f1 = 123;
			}

			#endregion

			public int F1
			{
				get
				{
					return f1;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestClassOne2 : AncestorRepository.BaseOne
		{
			private int f1 = 123;
			private int f2 = 456;

			#region Overrides of TestBaseOne

			public override void VirtualVoidMethod()
			{
				f1 = f2 + 123;
			}

			#endregion

			public int F1
			{
				get
				{
					return f1;
				}
			}
			public int F2
			{
				get
				{
					return f2;
				}
			}
		}
	}
}
