using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Hapil.Samples.Logging.UnitTests
{
	[TestFixture]
    public class DummyTests
    {
		[Test]
		public void TestOne()
		{
			Assert.That(10, Is.EqualTo(10), "10 doesn't equal to 10?");
		}
		[Test]
		public void TestTwo()
		{
			Assert.That(20, Is.EqualTo(20), "20 doesn't equal to 20?");
		}
		[Test]
		public void TestThree()
		{
			Assert.That(30, Is.EqualTo(30), "30 doesn't equal to 30?");
		}
	}
}
