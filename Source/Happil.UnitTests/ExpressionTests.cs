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

		[Test]
		public void CanReadAndWriteImplementedProperties()
		{
			//-- Arrange

			DeriveClassFrom<IntPropertiesBase2>()
				.DefaultConstructor()
				.Method(cls => cls.SwapPropertyValues).Implement(m => {
					var temp = m.Local<int>();
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

		[Test]
		public void CanReadAndWriteInheritedProperties()
		{
			//-- Arrange

			DeriveClassFrom<IntPropertiesBase3>()
				.DefaultConstructor()
				.Method(cls => cls.SwapPropertyValues).Implement(m => {
					var temp = m.Local<int>();
					var @this = m.This<IntPropertiesBase3>();

					temp.Assign(@this.Property(x => x.PropertyOne));
					@this.Property(x => x.PropertyOne).Assign(@this.Property(x => x.PropertyTwo));
					@this.Property(x => x.PropertyTwo).Assign(temp);
				});

			//-- Act

			var obj = CreateClassInstanceAs<IntPropertiesBase3>().UsingDefaultConstructor();

			obj.PropertyOne = 11;
			obj.PropertyTwo = 22;

			obj.SwapPropertyValues();

			//-- Assert

			Assert.That(obj.PropertyOne, Is.EqualTo(22));
			Assert.That(obj.PropertyTwo, Is.EqualTo(11));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanTraverseAndReadProperties()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IPropertyContainersReader>()
				.Method<AncestorRepository.IPropertyContainerOne, AncestorRepository.IPropertyContainerTwo, int>(intf => intf.SumAll)
				.Implement(
					(m, container1, container2) => {
						var sum = m.Local<int>(initialValue: m.Const(0));

						sum.Assign(sum + container1.P(x => x.One).P(x => x.PropertyOne));
						sum.Assign(sum + container2.P(x => x.Two).P(x => x.PropertyOne));
						sum.Assign(sum + container2.P(x => x.Two).P(x => x.PropertyTwo));

						m.Return(sum);
					});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IPropertyContainersReader>().UsingDefaultConstructor();
			var c1 = new PropertyContainerOneImpl() { One = new OnePropertyImpl() };
			var c2 = new PropertyContainerTwoImpl() { Two = new TwoPropertiesImpl() };

			c1.One.PropertyOne = 100;
			c2.Two.PropertyOne = 11;
			c2.Two.PropertyTwo = 22;
			
			var sumA = obj.SumAll(c1, c2);

			//-- Assert

			Assert.That(sumA, Is.EqualTo(133));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanTraverseAndWriteProperties()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IPropertyContainersWriter>()
				.Method<AncestorRepository.IPropertyContainerOne, AncestorRepository.IPropertyContainerTwo, int>(intf => intf.SetAll).Implement(
					(m, container1, container2, value) => {
						container1.P(x => x.One).P(x => x.PropertyOne).Assign(value);
						container2.P(x => x.Two).P(x => x.PropertyOne).Assign(value);
						container2.P(x => x.Two).P(x => x.PropertyTwo).Assign(value);
					});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IPropertyContainersWriter>().UsingDefaultConstructor();
			var c1 = new PropertyContainerOneImpl() { One = new OnePropertyImpl() };
			var c2 = new PropertyContainerTwoImpl() { Two = new TwoPropertiesImpl() };

			obj.SetAll(c1, c2, 999);

			//-- Assert

			Assert.That(c1.One.PropertyOne, Is.EqualTo(999));
			Assert.That(c2.Two.PropertyOne, Is.EqualTo(999));
			Assert.That(c2.Two.PropertyTwo, Is.EqualTo(999));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanTraverseAndReadAndWriteProperties()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IPropertyContainersWriter>()
				.Method<AncestorRepository.IPropertyContainerOne, AncestorRepository.IPropertyContainerTwo, int>(intf => intf.SetAll).Implement(
					(m, container1, container2, value) => {
						var temp = m.Local<int>();											// int temp;
						
						temp.Assign(container1.P(x => x.One).P(x => x.PropertyOne));		// temp = container1.One.PropertyOne;
						container1.P(x => x.One).P(x => x.PropertyOne).Assign(				// container1.One.PropertyOne = 
							container2.P(x => x.Two).P(x => x.PropertyOne));				//     container2.Two.PropertyOne;
						container2.P(x => x.Two).P(x => x.PropertyOne).Assign(temp);		// container2.Two.PropertyOne = temp;
						
						container2.P(x => x.Two).P(x => x.PropertyTwo).Assign(				// container2.Two.PropertyTwo = 
							container1.P(x => x.One).P(x => x.PropertyOne) +				//     container1.One.PropertyOne +
							container2.P(x => x.Two).P(x => x.PropertyOne) +				//     container2.Two.PropertyOne +
							value);															//     value;
					});

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IPropertyContainersWriter>().UsingDefaultConstructor();
			var c1 = new PropertyContainerOneImpl() { One = new OnePropertyImpl() };
			var c2 = new PropertyContainerTwoImpl() { Two = new TwoPropertiesImpl() };

			c1.One.PropertyOne = 111;
			c2.Two.PropertyOne = 222;
			c2.Two.PropertyTwo = -1;

			obj.SetAll(c1, c2, 1000);

			//-- Assert

			Assert.That(c1.One.PropertyOne, Is.EqualTo(222));
			Assert.That(c2.Two.PropertyOne, Is.EqualTo(111));
			Assert.That(c2.Two.PropertyTwo, Is.EqualTo(1333));
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class IntPropertiesBase3
		{
			public abstract void SwapPropertyValues();
			public int PropertyOne { get; set; }
			public int PropertyTwo { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class OnePropertyImpl : AncestorRepository.IOneProperty
		{
			public int PropertyOne { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TwoPropertiesImpl : AncestorRepository.ITwoProperties
		{
			public int PropertyOne { get; set; }
			public int PropertyTwo { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class PropertyContainerOneImpl : AncestorRepository.IPropertyContainerOne
		{
			public AncestorRepository.IOneProperty One { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class PropertyContainerTwoImpl : AncestorRepository.IPropertyContainerTwo
		{
			public AncestorRepository.ITwoProperties Two { get; set; }
		}
	}
}
