using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class ExpressionTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanReadImplementedProperties()
		{
			//-- Arrange

			DeriveClassFrom<IntPropertiesBase1>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.SumPropertiesAndNumber).Implement((m, number) => {
					m.Return(
						m.This<AncestorRepository.ITwoProperties>().Property(x => x.PropertyOne) +
						m.This<AncestorRepository.ITwoProperties>().Property(x => x.PropertyTwo) + 
						number);
				})
				.ImplementInterface<AncestorRepository.ITwoProperties>()
				.Property(intf => intf.PropertyOne).ImplementAutomatic()
				.Property(intf => intf.PropertyTwo).ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<IntPropertiesBase1>().UsingDefaultConstructor();

			((AncestorRepository.ITwoProperties)obj).PropertyOne = 11;
			((AncestorRepository.ITwoProperties)obj).PropertyTwo = 22;

			var result = obj.SumPropertiesAndNumber(100);

			//-- Assert

			Assert.That(result, Is.EqualTo(133));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("Not yet implemented")]
		public void CanWriteImplementedProperties()
		{
			//-- Arrange

			DeriveClassFrom<IntPropertiesBase2>()
				.DefaultConstructor()
				.Method(cls => cls.SwapPropertyValues).Implement(m => {
					var temp = m.Local<int>("temp");
					var @this = m.This<AncestorRepository.ITwoProperties>();
					
					temp.Assign(@this.Property(x => x.PropertyOne));
					@this.Property(x => x.PropertyOne).Assign(@this.Property(x => x.PropertyTwo));
					@this.Property(x => x.PropertyTwo).Assign(temp);
				})
				.ImplementInterface<AncestorRepository.ITwoProperties>()
				.Properties<int>().ImplementAutomatic();

			//-- Act

			var obj = CreateClassInstanceAs<IntPropertiesBase2>().UsingDefaultConstructor();

			((AncestorRepository.ITwoProperties)obj).PropertyOne = 11;
			((AncestorRepository.ITwoProperties)obj).PropertyTwo = 22;

			obj.SwapPropertyValues();

			//-- Assert

			Assert.That(((AncestorRepository.ITwoProperties)obj).PropertyOne, Is.EqualTo(22));
			Assert.That(((AncestorRepository.ITwoProperties)obj).PropertyTwo, Is.EqualTo(11));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class IntPropertiesBase1
		{
			public abstract int SumPropertiesAndNumber(int number);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class IntPropertiesBase2
		{
			public abstract void SwapPropertyValues();
		}
	}
}
