using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class TypeTemplateTests : ClassPerTestCaseFixtureBase
	{
      	[Test]
		public void TestTBase()
		{
			//-- Arrange

			OnDefineNewClass(key => Module.DeriveClassFrom<TypeTemplate.TBase>(TestCaseClassName)
				.DefaultConstructor()
				.AllProperties().ImplementAutomatic()
			);

			//-- Act

			DefineClassByKey(new HappilTypeKey(baseType: typeof(AncestorRepository.BaseTwo)));

			var obj = CreateClassInstanceAs<AncestorRepository.BaseTwo>().UsingDefaultConstructor();

			obj.FirstValue = 123;
			obj.SecondValue = "ABC";

			//-- Assert

			Assert.That(obj.FirstValue, Is.EqualTo(123));
			Assert.That(obj.SecondValue, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Fails/nit implemented")]
		public void TestTPrimary()
		{
			//-- Arrange

			OnDefineNewClass(key => Module.DeriveClassFrom<object>(TestCaseClassName)
				.DefaultConstructor()
				.ImplementInterface<TypeTemplate.TPrimary>()
				.AllProperties().ImplementAutomatic()
				.ImplementInterface<IEquatable<TypeTemplate.TPrimary>>()
				.Method<TypeTemplate.TPrimary, bool>(intf => intf.Equals).Implement((m, other) => {
					m.ReturnConst(false);
				})
			);

			//-- Act

			DefineClassByKey(new HappilTypeKey(primaryInterface: typeof(AncestorRepository.IFewReadWriteProperties)));

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();

			obj.AnInt = 123;
			obj.AString = "ABC";

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(123));
			Assert.That(obj.AString, Is.EqualTo("ABC"));
		}
	}
}
