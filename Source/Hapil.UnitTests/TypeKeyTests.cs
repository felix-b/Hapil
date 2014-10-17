using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;

namespace Hapil.UnitTests
{
	[TestFixture]
	public class TypeKeyTests
	{
		[Test]
		public void EqualByBaseClass()
		{
			//-- Arrange

			var key1 = new TypeKey(baseType: typeof(Stream));
			var key2 = new TypeKey(baseType: typeof(Stream));

			//-- Assert

			AssertKeysEqual(key1, key2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NotEqualByBaseClass()
		{
			//-- Arrange

			var key1 = new TypeKey(baseType: typeof(Stream));
			var key2 = new TypeKey(baseType: typeof(Random));

			//-- Assert

			AssertKeysNotEqual(key1, key2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void EqualByPrimaryInterface()
		{
			//-- Arrange

			var key1 = new TypeKey(primaryInterface: typeof(IDisposable));
			var key2 = new TypeKey(primaryInterface: typeof(IDisposable));

			//-- Assert

			AssertKeysEqual(key1, key2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NotEqualByPrimaryInterface()
		{
			//-- Arrange

			var key1 = new TypeKey(primaryInterface: typeof(IDisposable));
			var key2 = new TypeKey(primaryInterface: typeof(ISerializable));

			//-- Assert

			AssertKeysNotEqual(key1, key2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void EqualBySecondaryInterfaces()
		{
			//-- Arrange

			var key1 = new TypeKey(secondaryInterfaces: new[] { typeof(IDisposable), typeof(ISerializable) });
			var key2 = new TypeKey(secondaryInterfaces: new[] { typeof(IDisposable), typeof(ISerializable) });

			//-- Assert

			AssertKeysEqual(key1, key2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void EqualBySecondaryInterfacesInDifferentOrder()
		{
			//-- Arrange

			var key1 = new TypeKey(secondaryInterfaces: new[] { typeof(IDisposable), typeof(ISerializable) });
			var key2 = new TypeKey(secondaryInterfaces: new[] { typeof(ISerializable), typeof(IDisposable) });

			//-- Assert

			AssertKeysEqual(key1, key2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NotEqualBySecondaryInterfaces()
		{
			//-- Arrange

			var key1 = new TypeKey(secondaryInterfaces: new[] { typeof(IDisposable), typeof(ISerializable) });
			var key2 = new TypeKey(secondaryInterfaces: new[] { typeof(IDisposable), typeof(IComparable) });

			//-- Assert

			AssertKeysNotEqual(key1, key2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void AssertKeysEqual(TypeKey key1, TypeKey key2)
		{
			var hashCode1 = key1.GetHashCode();
			var hashCode2 = key2.GetHashCode();
			Assert.That(hashCode1, Is.EqualTo(hashCode2));

			Assert.That(key1.Equals(key2), Is.True);
			Assert.That(key2.Equals(key1), Is.True);
			Assert.That(key1.Equals((object)key2), Is.True);
			Assert.That(key2.Equals((object)key1), Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void AssertKeysNotEqual(TypeKey key1, TypeKey key2)
		{
			Assert.That(key1.Equals(key2), Is.False);
			Assert.That(key2.Equals(key1), Is.False);
			Assert.That(key1.Equals((object)key2), Is.False);
			Assert.That(key2.Equals((object)key1), Is.False);
		}
	}
}
