﻿using System;
using System.Reflection;
using NUnit.Framework;

namespace Happil.UnitTests.Demo
{
	[TestFixture, Ignore("This is only demo")]
	public class AutomaticProperties
	{
		private HappilFactory _factoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			_factoryUnderTest = new HappilFactory();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void SuperDuperHappyPath()
		{
			_factoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl").Implement<IDemoInterface>(
				impl => impl.AutomaticProperties()
			);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void MemberByMember()
		{
			_factoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl").Implement<IDemoInterface>(

				impl => impl.Property(intf => intf.Number,
					prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument("value"))
					)
				),

				impl => impl.Property(intf => intf.Text,
					prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument("value"))
					)
				),

				impl => impl.Property(intf => intf.OptionalInterval,
					prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument("value"))
					)
				)
			);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void MultipleMembersByTemplate()
		{
			_factoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl").Implement<IDemoInterface>(
				
				impl => impl.AutomaticProperties(where: prop => !IsNullableProperty(prop)),
				
				impl => impl.Properties(where: IsNullableProperty,
					getter: prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					setter: prop => prop.Set(
						x => x.Throw<InvalidOperationException>("Nullable values cannot be set on this object")
					)
				)
			);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static bool IsNullableProperty(PropertyInfo property)
		{
			return property.PropertyType.Name.StartsWith("Nullable`1");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IDemoInterface
		{
			int Number { get; set; }
			string Text { get; set; }
			TimeSpan? OptionalInterval { get; set; }
		}
	}
}
