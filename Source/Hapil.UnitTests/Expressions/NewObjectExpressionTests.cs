using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests.Expressions
{
	[TestFixture]
	public class NewObjectExpressionTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void CanCreateNewObjectWithDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Method<object>(cls => cls.DoTest).Implement(m => {
					m.Return(m.New<TargetRepository.TargetWithDefaultConstructor>());
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = tester.DoTest();

			//-- Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<TargetRepository.TargetWithDefaultConstructor>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateNewObjectWithNonDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Method<object>(cls => cls.DoTest).Implement(m => {
					m.Return(m.New<TargetRepository.TargetWithNonDefaultConstructor>(m.Const(111), m.Const("AAA")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (tester.DoTest() as TargetRepository.TargetWithNonDefaultConstructor);

			//-- Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result.IntValue, Is.EqualTo(111));
			Assert.That(result.StringValue, Is.EqualTo("AAA"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCreateNewObjectWithSelectedNonDefaultConstructor()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.ObjectCreationTester>()
				.DefaultConstructor()
				.Method<object>(cls => cls.DoTest).Implement(m => {
					m.Return(m.New<TargetRepository.TargetWithMultipleConstructors>(m.Const("AAA")));
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.ObjectCreationTester>().UsingDefaultConstructor();
			var result = (tester.DoTest() as TargetRepository.TargetWithMultipleConstructors);

			//-- Assert

			Assert.That(result, Is.Not.Null);
			Assert.That(result.IntValue, Is.EqualTo(999));
			Assert.That(result.StringValue, Is.EqualTo("AAA"));
		}
	}
}
