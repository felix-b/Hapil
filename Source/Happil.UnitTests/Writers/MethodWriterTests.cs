using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Writers
{
	[TestFixture]
	public class MethodWriterTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void ImplementInterfaceMethods_AllEmpty()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementBase<AncestorRepository.IFewMethods>()
				.AllMethods().ImplementEmpty();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			obj.One();
			var result3 = obj.Three();
			var result5 = obj.Five(123);

			//-- Assert

			Assert.That(result3, Is.EqualTo(0));
			Assert.That(result5, Is.Null);
		}
	}
}
