using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Expressions
{
	[TestFixture]
	public class UnaryOperatorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void TestNewArray()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var arr = m.Local<int[]>();
					arr.Assign(m.NewArray<int>(input));
					Static.Prop(() => OutputArray).Assign(arr);
					m.Return(arr.Length());
				});

			OutputArray = null;

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var result = tester.DoTest(10);

			//-- Assert

			Assert.That(result, Is.EqualTo(10));
			Assert.That(OutputArray, Is.Not.Null);
			Assert.That(OutputArray.Length, Is.EqualTo(10));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Initializers are not yet implemented")]
		public void TestNewArrayWithInitializer()
		{
			////-- Arrange

			//DeriveClassFrom<AncestorRepository.StatementTester>()
			//	.DefaultConstructor()
			//	.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
			//		var arr = m.Local<int[]>();
			//		arr.Assign(m.NewArray<int>().Init(1, 2, 3, 4, 5));
			//		Static.Prop(() => OutputArray).Assign(arr);
			//		m.Return(arr.Length());
			//	});

			//OutputArray = null;

			////-- Act

			//var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			//var result = tester.DoTest(10);

			////-- Assert

			//Assert.That(result, Is.EqualTo(10));
			//Assert.That(OutputArray, Is.Not.Null);
			//Assert.That(OutputArray.Length, Is.EqualTo(10));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static int[] OutputArray { get; set; }
	}
}
