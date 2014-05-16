using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class TypeExtensionTests
	{
		[Test]
		public void FriendlyName_SimpleType_ReturnAsIs()
		{
			//-- Arrange

			var type = typeof(string);

			//-- Act

			var friendlyName = type.FriendlyName();

			//-- Assert

			Assert.That(friendlyName, Is.EqualTo("String"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FriendlyName_ArrayType_ReturnInCSharpSyntax()
		{
			//-- Arrange

			var type = typeof(string[]);

			//-- Act

			var friendlyName = type.FriendlyName();

			//-- Assert

			Assert.That(friendlyName, Is.EqualTo("String[]"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FriendlyName_GenericTypeWithSingleArgument_ReturnInCSharpSyntax()
		{
			//-- Arrange

			var type = typeof(List<string>);

			//-- Act

			var friendlyName = type.FriendlyName();

			//-- Assert

			Assert.That(friendlyName, Is.EqualTo("List<String>"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FriendlyName_GenericTypeWithMultipleArguments_ReturnInCSharpSyntax()
		{
			//-- Arrange

			var type = typeof(Dictionary<int, string>);

			//-- Act

			var friendlyName = type.FriendlyName();

			//-- Assert

			Assert.That(friendlyName, Is.EqualTo("Dictionary<Int32,String>"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FriendlyName_GenericTypeNestedGenericTypes_ReturnInCSharpSyntax()
		{
			//-- Arrange

			var type = typeof(Dictionary<List<DayOfWeek>, List<Dictionary<int, string>>>);

			//-- Act

			var friendlyName = type.FriendlyName();

			//-- Assert

			Assert.That(friendlyName, Is.EqualTo("Dictionary<List<DayOfWeek>,List<Dictionary<Int32,String>>>"));
		}
	}
}
