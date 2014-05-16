using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Happil.UnitTests.Writers
{
	[TestFixture]
	public class AnonymousMethodWriterTests : ClosureTestFixtureBase
	{
		[Test, Ignore("test not finished")]
		public void OneAnonymousMethod_ExternalLocal_OneClosure()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, input) => {
					var prefix = w.Local("PFX");
					var output = w.Local(initialValue: input.Select(s => prefix + s));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();
			CollectionAssert.Contains(base.Class.GetNestedClasses(), anonymousMethod.OwnerClass);

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(""));
			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(""));
		}
	}
}
