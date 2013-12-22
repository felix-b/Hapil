using System;
using System.Reflection;
using NUnit.Framework;

namespace Happil.UnitTests.Demo
{
	[TestFixture, Ignore("This is only demo")]
	public class AutomaticPropertiesDemo
	{
		private HappilModule m_FactoryUnderTest;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
            m_FactoryUnderTest = new HappilModule("Happil.Demo.Impl");
		}

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
		[Test]
		public void SuperDuperHappyPath()
		{
			m_FactoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl")
				.ImplementInterface<IDemoInterface>()
				.AllProperties().ImplementAutomatic();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
		public void MemberByMember()
		{
			m_FactoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl")
				.ImplementInterface<IDemoInterface>()
				.Property(intf => intf.Number).Implement(
					prop => prop.Get(m => {
						m.Return(prop.BackingField);
					}),
					prop => prop.Set((m, value) => {
						prop.BackingField.Assign(value);
					})
				)
				.Property(intf => intf.Text).Implement(
					prop => prop.Get(
						(m) => m.Return(prop.BackingField)
					),
					prop => prop.Set(
						(m, value) => prop.BackingField.Assign(value)
					)
				)
				.Property(intf => intf.OptionalInterval).Implement(
					prop => prop.Get(
						(m) => m.Return(prop.BackingField)
					),
					prop => prop.Set(
						(m, value) => prop.BackingField.Assign(value)
					)
				);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
		public void MultipleMembersByTemplate()
		{
			m_FactoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl")
				.ImplementInterface<IDemoInterface>()
				.AllProperties(where: prop => !IsNullableProperty(prop)).ImplementAutomatic()
				.AllProperties(where: IsNullableProperty).Implement(
					getter: prop => prop.Get(
						(m) => m.Return(prop.BackingField)
					),
					setter: prop => prop.Set(
						(m, value) => m.Throw<InvalidOperationException>("Nullable values cannot be set on this object")
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
