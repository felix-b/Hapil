using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class CompositeConventionTests
	{
		[Test]
		public void ShouldApply_AllFalse_False()
		{
			//-- Arrange

			var composite = new CompositeConvention(
				new TestConvention(shouldApply: false),
				new TestConvention(shouldApply: false)	
			);

			//-- Act

			var shouldApply = composite.ShouldApply(context: null);

			//-- Assert

			Assert.That(shouldApply, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ShouldApply_AtLeastOneTrue_True()
		{
			//-- Arrange

			var composite = new CompositeConvention(
				new TestConvention(shouldApply: false),
				new TestConvention(shouldApply: true)
			);

			//-- Act

			var shouldApply = composite.ShouldApply(context: null);

			//-- Assert

			Assert.That(shouldApply, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Apply_AllShouldApply_AllApplied()
		{
			//-- Arrange

			var containedConventions = new[] {
				new TestConvention(shouldApply: true),
				new TestConvention(shouldApply: true)
			};

			//-- Act

			var composite = new CompositeConvention(containedConventions);
			composite.Apply(context: null);

			//-- Assert

			Assert.That(containedConventions[0].WasApplied, Is.True);
			Assert.That(containedConventions[1].WasApplied, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Apply_SomeShouldApply_OnlyThoseApplied()
		{
			//-- Arrange

			var containedConventions = new[] {
				new TestConvention(shouldApply: false),
				new TestConvention(shouldApply: true),
				new TestConvention(shouldApply: false)
			};

			//-- Act

			var composite = new CompositeConvention(containedConventions);
			composite.Apply(context: null);

			//-- Assert

			Assert.That(containedConventions[0].WasApplied, Is.False);
			Assert.That(containedConventions[1].WasApplied, Is.True);
			Assert.That(containedConventions[2].WasApplied, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ShouldApply_SameContextPropagated()
		{
			//-- Arrange

			var containedConventions = new[] {
				new TestConvention(shouldApply: false),
				new TestConvention(shouldApply: false)
			};

			var context = new ObjectFactoryContext(factory: null, typeKey: new TypeKey());

			//-- Act

			var composite = new CompositeConvention(containedConventions);
			composite.ShouldApply(context);

			//-- Assert

			Assert.That(containedConventions[0].ShouldApplyContext, Is.SameAs(context));
			Assert.That(containedConventions[1].ShouldApplyContext, Is.SameAs(context));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Apply_SameContextPropagated()
		{
			//-- Arrange

			var containedConventions = new[] {
				new TestConvention(shouldApply: true),
				new TestConvention(shouldApply: true)
			};

			var context = new ObjectFactoryContext(factory: null, typeKey: new TypeKey());

			//-- Act

			var composite = new CompositeConvention(containedConventions);
			composite.Apply(context);

			//-- Assert

			Assert.That(containedConventions[0].ApplyContext, Is.SameAs(context));
			Assert.That(containedConventions[1].ApplyContext, Is.SameAs(context));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestConvention : IObjectFactoryConvention
		{
			private readonly bool m_ShouldApply;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestConvention(bool shouldApply)
			{
				m_ShouldApply = shouldApply;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IObjectFactoryConvention Members

			public bool ShouldApply(ObjectFactoryContext context)
			{
				ShouldApplyContext = context;
				return m_ShouldApply;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Apply(ObjectFactoryContext context)
			{
				ApplyContext = context;
				WasApplied = true;
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public bool WasApplied { get; private set; }
			public ObjectFactoryContext ShouldApplyContext { get; private set; }
			public ObjectFactoryContext ApplyContext { get; private set; }
		}
	}
}
