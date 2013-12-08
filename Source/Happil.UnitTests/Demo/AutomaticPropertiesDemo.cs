using System;
using System.Reflection;
using NUnit.Framework;

namespace Happil.UnitTests.Demo
{
	[TestFixture, Ignore("This is only demo")]
	public class AutomaticPropertiesDemo
	{
		private HappilFactory m_FactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
            m_FactoryUnderTest = new HappilFactory("Happil.Demo.Impl.dll");
		}

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
		[Test]
		public void SuperDuperHappyPath()
		{
			m_FactoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl")
				.Implement<IDemoInterface>()
				.AutomaticProperties();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
		public void MemberByMember()
		{
			m_FactoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl")
				.Implement<IDemoInterface>()
				.Property(intf => intf.Number,
					prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument<int>("value"))
					)
				)
				.Property(intf => intf.Text,
					prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument<string>("value"))
					)
				)
				.Property(intf => intf.OptionalInterval,
					prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument<TimeSpan?>("value"))
					)
				);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
		public void MultipleMembersByTemplate()
		{
			m_FactoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl")
				.Implement<IDemoInterface>()
				.AutomaticProperties(where: prop => !IsNullableProperty(prop))
				.Properties<object>(where: IsNullableProperty,
					getter: prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					setter: prop => prop.Set(
						x => x.Throw<InvalidOperationException>("Nullable values cannot be set on this object")
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
