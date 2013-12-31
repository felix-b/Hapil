using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Happil.UnitTests.Selectors
{
	[TestFixture]
	public class MethodSelectorsTests : ClassPerTestCaseFixtureBase 
	{
		[Test]
		public void CanSelectAllMethodsAndLoopOver()
		{
			//-- Arrange

			var voidMethods = new List<string>();
			var valueTypeMethods = new List<string>();
			var allMethods = new List<string>();

			//-- Act

			base.DeriveClassFrom<object>()
				.ImplementInterface<IMethodSelectorsTestOne>()
				.AllMethods(where: m => m.IsVoid()).ForEach(m => voidMethods.Add(m.Name))
				.AllMethods(where: m => m.ReturnType.IsValueType && !m.IsVoid()).ForEach(m => valueTypeMethods.Add(m.Name))
				.AllMethods().ForEach(m => allMethods.Add(m.Name));

			//-- Assert

			Assert.That(voidMethods, Is.EquivalentTo(new[] {
				"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight"
			}));
			Assert.That(valueTypeMethods, Is.EquivalentTo(new[] {
				"Eleven", "Thirteen", "Fifteen"
			}));
			Assert.That(allMethods, Is.EquivalentTo(new[] {
				"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", 
				"Eleven", "Twelwe", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanSelectAllMethodsAndImplement()
		{
			//-- Arrange

			base.DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<IMethodSelectorsTestOne>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => {
					m.Return();
				})
				.AllMethods(where: m => m.ReturnType.IsValueType).Implement(m => {
					m.Return(m.Default(m.ReturnType));		
				})
				.AllMethods().Implement(m => {
					m.Return(m.Const<object>(null));
				});

			//-- Act

			var obj = CreateClassInstanceAs<IMethodSelectorsTestOne>().UsingDefaultConstructor();

			obj.One();
			obj.Five(123, "ABC");

			var fourteenValue = obj.Fourteen(999);
			var fifteenValue = obj.Fifteen(TimeSpan.MaxValue, "VVV");

			//-- Assert

			Assert.That(fourteenValue, Is.Null);
			Assert.That(fifteenValue, Is.EqualTo(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of ClassPerTestCaseFixtureBase

		protected override bool AllClassesAreCompleteTypes
		{
			get
			{
				return false;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMethodSelectorsTestOne
		{
			void One();
			void Two();
			void Three(int n);
			void Four(string s);
			void Five(int n, string s);
			void Six(string s, int n);
			void Seven(TimeSpan t, string s, int n);
			void Eight(string s, int n, TimeSpan t);
			int Eleven();
			string Twelwe();
			int Thirteen(string s);
			string Fourteen(int n);
			int Fifteen(TimeSpan t, string s);
			string Sixteen(int n, TimeSpan t);
			object Seventeen(TimeSpan t, string s, int n);
			IEnumerable<int> Eighteen(string s, int n, TimeSpan t);
		}
	}
}
