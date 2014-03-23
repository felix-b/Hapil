using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class ObjectFactoryBaseTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanCreateObjectWithTestFactory()
		{
			DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();
		}
	}
}
