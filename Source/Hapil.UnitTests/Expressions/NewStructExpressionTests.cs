using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using Hapil.Expressions;
using Hapil.Operands;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests.Expressions
{
	[TestFixture]
	public class NewStructExpressionTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void CanCreateValueTypeWithDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Method<object>(cls => cls.DoTest).Implement(m => {
					m.Return(m.New<TimeSpan>().CastTo<object>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (TimeSpan)tester.DoTest();

			//-- Assert

			Assert.That(result, Is.EqualTo(TimeSpan.Zero));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateValueTypeWithNonDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Method<object>(cls => cls.DoTest).Implement(m => {
					m.Return(m.New<TimeSpan>(m.Const(3), m.Const(0), m.Const(0)).CastTo<object>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (TimeSpan)tester.DoTest();

			//-- Assert

			Assert.That(result, Is.EqualTo(TimeSpan.FromHours(3)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypeFieldWithDefaultConstructor()
		{
			//-- Arrange

			Field<TimeSpan> field;

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Field<TimeSpan>("m_Time", out field)
				.Method<object>(cls => cls.DoTest).Implement(m => {
					field.Assign(m.New<TimeSpan>());
					m.Return(field.CastTo<object>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (TimeSpan)tester.DoTest();

			//-- Assert

			Assert.That(result, Is.EqualTo(TimeSpan.Zero));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypeFieldWithNonDefaultConstructor()
		{
			//-- Arrange

			Field<TimeSpan> field;

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Field<TimeSpan>("m_Time", out field)
				.Method<object>(cls => cls.DoTest).Implement(m => {
					field.Assign(m.New<TimeSpan>(m.Const(3), m.Const(0), m.Const(0)));
					m.Return(field.CastTo<object>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (TimeSpan)tester.DoTest();

			//-- Assert

			Assert.That(result, Is.EqualTo(TimeSpan.FromHours(3)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypeArgumentWithDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StructCreationTester>()
				.DefaultConstructor()
				.Method<TimeSpan>(cls => cls.DoTest).Implement((m, value) => {
					value.Assign(m.New<TimeSpan>());
					m.This<AncestorRepository.StructCreationTester>().Prop(x => x.TimeValue).Assign(value);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StructCreationTester>().UsingDefaultConstructor();
			tester.TimeValue = TimeSpan.MinValue;
			tester.DoTest(TimeSpan.MaxValue);

			//-- Assert

			Assert.That(tester.TimeValue, Is.EqualTo(TimeSpan.Zero));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypeArgumentWithNonDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StructCreationTester>()
				.DefaultConstructor()
				.Method<TimeSpan>(cls => cls.DoTest).Implement((m, value) => {
					value.Assign(m.New<TimeSpan>(m.Const(3), m.Const(0), m.Const(0)));
					m.This<AncestorRepository.StructCreationTester>().Prop(x => x.TimeValue).Assign(value);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StructCreationTester>().UsingDefaultConstructor();
			tester.DoTest(TimeSpan.MaxValue);

			//-- Assert

			Assert.That(tester.TimeValue, Is.EqualTo(TimeSpan.FromHours(3)));
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanInitializeValueTypeByRefArgumentWithDefaultConstructor()
        {
            //-- Arrange

            DeriveClassFrom<AncestorRepository.StructCreationTester2>()
                .DefaultConstructor()
                .Method<TimeSpan>(cls => t => cls.DoTest(out t)).Implement((m, value) => {
                    value.Assign(m.Default<TimeSpan>());
                    m.This<AncestorRepository.StructCreationTester2>().Prop(x => x.TimeValue).Assign(value);
                });

            //-- Act

            var tester = CreateClassInstanceAs<AncestorRepository.StructCreationTester2>().UsingDefaultConstructor();
            TimeSpan outputValue;
            
            tester.TimeValue = TimeSpan.MinValue;
            tester.DoTest(out outputValue);

            //-- Assert

            Assert.That(tester.TimeValue, Is.EqualTo(TimeSpan.Zero));
            Assert.That(outputValue, Is.EqualTo(TimeSpan.Zero));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanInitializeValueTypeByRefArgumentWithNonDefaultConstructor()
        {
            //-- Arrange

            DeriveClassFrom<AncestorRepository.StructCreationTester2>()
                .DefaultConstructor()
                .Method<TimeSpan>(cls => t => cls.DoTest(out t)).Implement((m, value) => {
                    value.Assign(m.New<TimeSpan>(m.Const(3), m.Const(0), m.Const(0)));
                    m.This<AncestorRepository.StructCreationTester2>().Prop(x => x.TimeValue).Assign(value);
                });

            //-- Act

            var tester = CreateClassInstanceAs<AncestorRepository.StructCreationTester2>().UsingDefaultConstructor();
            TimeSpan outputValue;

            tester.TimeValue = TimeSpan.MinValue;
            tester.DoTest(out outputValue);

            //-- Assert

            Assert.That(tester.TimeValue, Is.EqualTo(TimeSpan.FromHours(3)));
            Assert.That(outputValue, Is.EqualTo(TimeSpan.FromHours(3)));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypeLocalWithDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Method<object>(cls => cls.DoTest).Implement(m => {
					var temp = m.Local<TimeSpan>();
					temp.Assign(m.New<TimeSpan>());
					m.Return(temp.CastTo<object>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (TimeSpan)tester.DoTest();

			//-- Assert

			Assert.That(result, Is.EqualTo(TimeSpan.Zero));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypeLocalWithNonDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Method<object>(cls => cls.DoTest).Implement(m => {
					var temp = m.Local<TimeSpan>();
					temp.Assign(m.New<TimeSpan>(m.Const(3), m.Const(0), m.Const(0)));
					m.Return(temp.CastTo<object>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (TimeSpan)tester.DoTest();

			//-- Assert

			Assert.That(result, Is.EqualTo(TimeSpan.FromHours(3)));
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanInitializeTemplatedValueTypePropertyWithDefaultConstructor()
        {
            //-- Arrange

            using ( TT.CreateScope<TT.TValue>(typeof(TimeSpan)) )
            {
                DeriveClassFrom<AncestorRepository.StructCreationTester>()
                    .DefaultConstructor()
                    .AllMethods(where: m => m.DeclaringType != typeof(object)).Implement(m => {
                        m.This<AncestorRepository.StructCreationTester>().Prop(x => x.TimeValue).Assign(m.New<TT.TValue>().CastTo<TimeSpan>());
                    });
            }

            //-- Act

            var tester = CreateClassInstanceAs<AncestorRepository.StructCreationTester>().UsingDefaultConstructor();
            tester.TimeValue = TimeSpan.MinValue;
            tester.DoTest(TimeSpan.MaxValue);

            //-- Assert

            Assert.That(tester.TimeValue, Is.EqualTo(TimeSpan.Zero));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypePropertyWithDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StructCreationTester>()
				.DefaultConstructor()
				.Method<TimeSpan>(cls => cls.DoTest).Implement((m, value) => {
					m.This<AncestorRepository.StructCreationTester>().Prop(x => x.TimeValue).Assign(m.New<TimeSpan>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StructCreationTester>().UsingDefaultConstructor();
			tester.TimeValue = TimeSpan.MinValue;
			tester.DoTest(TimeSpan.MaxValue);

			//-- Assert

			Assert.That(tester.TimeValue, Is.EqualTo(TimeSpan.Zero));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanInitializeValueTypePropertyWithNonDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StructCreationTester>()
				.DefaultConstructor()
				.Method<TimeSpan>(cls => cls.DoTest).Implement((m, value) => {
					m.This<AncestorRepository.StructCreationTester>().Prop(x => x.TimeValue).Assign(
						m.New<TimeSpan>(m.Const(3), m.Const(0), m.Const(0)));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StructCreationTester>().UsingDefaultConstructor();
			tester.DoTest(TimeSpan.MaxValue);

			//-- Assert

			Assert.That(tester.TimeValue, Is.EqualTo(TimeSpan.FromHours(3)));
		}
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public object CompiledExample1()
	    {
            TimeSpan timeSpan = new TimeSpan(3, 0, 0);
            return timeSpan;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TimeSpan CompiledExample2(ref TimeSpan t1, out TimeSpan t2)
        {
            t2 = t1 + t1;
            t1 = default(TimeSpan);
            return t2;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public object CompiledExample3()
        {
            return new TimeSpan(3, 0, 0);
        }
    }
}
