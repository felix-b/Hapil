﻿using System;
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
    }
}
