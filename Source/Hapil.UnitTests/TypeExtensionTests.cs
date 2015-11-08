using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hapil.UnitTests
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
		public void FriendlyName_NestedGenericTypeArguments_ReturnInCSharpSyntax()
		{
			//-- Arrange

			var type = typeof(Dictionary<List<DayOfWeek>, List<Dictionary<int, string>>>);

			//-- Act

			var friendlyName = type.FriendlyName();

			//-- Assert

			Assert.That(friendlyName, Is.EqualTo("Dictionary<List<DayOfWeek>,List<Dictionary<Int32,String>>>"));
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void FriendlyName_NonGenericTypeNestedInGenericType_ReturnInCSharpSyntax()
        {
            //-- Arrange

            var type = typeof(AnOuterGenericType<string>.AnInnerType);

            //-- Act

            var friendlyName = type.FriendlyName();

            //-- Assert

            Assert.That(friendlyName, Is.EqualTo("TypeExtensionTests.AnOuterGenericType<String>.AnInnerType"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void FriendlyName_GenericTypeNestedInGenericType_ReturnInCSharpSyntax()
        {
            //-- Arrange

            var type = typeof(AnOuterGenericType<string>.AGenericInnerType<int>);

            //-- Act

            var friendlyName = type.FriendlyName();

            //-- Assert

            Assert.That(friendlyName, Is.EqualTo("TypeExtensionTests.AnOuterGenericType<String>.AGenericInnerType<Int32>"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void UnderlyingType_RegularType_ReturnTypeAsIs()
		{
			//-- Act

			var underlyingType = typeof(int).UnderlyingType();

			//-- Assert

			Assert.That(underlyingType, Is.EqualTo(typeof(int)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void UnderlyingType_ByRefType_ReturnUnderlyingType()
		{
			//-- Arrange

			var byRefType = typeof(int).MakeByRefType();

			//-- Act

			var underlyingType = byRefType.UnderlyingType();

			//-- Assert

			Assert.That(byRefType, Is.Not.EqualTo(typeof(int)));
			Assert.That(underlyingType, Is.EqualTo(typeof(int)));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestCase(typeof(System.Collections.ICollection))]
		[TestCase(typeof(ICollection<int>))]
		[TestCase(typeof(ICollection<>))]
		[TestCase(typeof(IList<int>))]
		[TestCase(typeof(IList<>))]
		[TestCase(typeof(List<int>))]
		[TestCase(typeof(List<>))]
		public void IsCollectionType_True(Type typeUnderTest)
		{
			//-- Act

			var isCollectionType = typeUnderTest.IsCollectionType();

			//-- Assert

			Assert.That(isCollectionType, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestCase(typeof(System.Collections.ICollection), "Object")]
		[TestCase(typeof(Array), "Object")]
		[TestCase(typeof(int[]), "Int32")]
		[TestCase(typeof(ICollection<int>), "Int32")]
		[TestCase(typeof(ICollection<>), "T")]
		[TestCase(typeof(IList<int>), "Int32")]
		[TestCase(typeof(IList<>), "T")]
		[TestCase(typeof(List<int>), "Int32")]
		[TestCase(typeof(List<>), "T")]
		public void IsCollectionType_True_ElementTypeExtracted(Type typeUnderTest, string expectedElementTypeName)
		{
			//-- Arrange

			Type elementType;

			//-- Act

			var isCollectionType = typeUnderTest.IsCollectionType(out elementType);

			//-- Assert

			Assert.That(isCollectionType, Is.True);
			Assert.That(elementType.Name, Is.EqualTo(expectedElementTypeName));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestCase(typeof(int))]
		[TestCase(typeof(string))]
		[TestCase(typeof(System.Collections.IEnumerable))]
		[TestCase(typeof(IEnumerable<int>))]
		public void IsCollectionTypethElementType_False(Type typeUnderTest)
		{
			//-- Arrange

			Type elementType;

			//-- Act

			var isCollectionType = typeUnderTest.IsCollectionType(out elementType);

			//-- Assert

			Assert.That(isCollectionType, Is.False);
			Assert.That(elementType, Is.Null);
		}
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public class AnOuterGenericType<T>
	    {
	        public class AnInnerType
	        {
	        }
            public class AGenericInnerType<S>
            {
            }
        }
    }
}
