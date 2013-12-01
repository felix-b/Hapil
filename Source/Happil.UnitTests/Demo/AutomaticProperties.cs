using System;
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

		private Type DefineClassMemberByMember()
		{
			var @class = _factoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl").Implements<IDemoInterface>(
				c => c.DefaultConstructor(),
				c => c.Property(i => i.Number,
					getter: prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					setter: prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument("value"))
					)
				),
				c => c.Property(i => i.Text,
					getter: prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					setter: prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument("value"))
					)
				),
				c => c.Property(i => i.OptionalInterval,
					getter: prop => prop.Get(
						x => x.Return(prop.BackingField)
					),
					setter: prop => prop.Set(
						x => prop.BackingField.Assign(x.Argument("value"))
					)
				)
			);

			return @class.CreateType();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private Type DefineClassAsWildcard()
		{
			var @class = _factoryUnderTest.DefineClass("Happil.Demo.AutomaticProperties.Impl").Implements<IDemoInterface>(
				type => type.DefaultConstructor(),
				type => type.AutomaticProperties()
			);

			return @class.CreateType();
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
