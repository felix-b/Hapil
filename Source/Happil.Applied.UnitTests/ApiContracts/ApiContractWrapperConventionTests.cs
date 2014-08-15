using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Happil.Applied.ApiContracts;
using Happil.Applied.Conventions;
using Happil.Applied.XTuple;
using NUnit.Framework;

namespace Happil.Applied.UnitTests.ApiContracts
{
	[TestFixture]
	public class ApiContractWrapperConventionTests : NUnitEmittedTypesTestBase
	{
		private TestComponent m_Component;
		private ITestComponent m_ApiContractWrapper;
		private string m_ExpectedExceptionParamName;
		private ApiContractCheckType m_ExpectedExceptionFailedCheck;
		private ApiContractCheckDirection m_ExpectedExceptionFailedCheckDirection;
		private string[] m_ExpectedExceptionLog;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			var factory = new ConventionObjectFactory(
				module: base.Module,
				transientConventionsFactory: context => new IObjectFactoryConvention[] {
					new ApiContractWrapperConvention()
				});

			m_Component = new TestComponent();
			m_ApiContractWrapper = factory.CreateInstanceOf<ITestComponent>().UsingConstructor<ITestComponent>(m_Component);

			m_ExpectedExceptionParamName = null;
			m_ExpectedExceptionLog = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void ArgumentNotNull_PassNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "str";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullString(str: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ArgumentNotNull_PassNonNullValue_DoNotThrow()
		{
			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullString(str: "ABC");

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotNullString(ABC)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void RefArgumentNotNull_PassNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "data";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			Stream data = null;
			m_ApiContractWrapper.AMethodWithNotNullRefParam(size: 0, data: ref data);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void RefArgumentNotNull_SetToNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "data";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Output;
			m_ExpectedExceptionLog = new[] { "AMethodWithNotNullRefParam(-1,Stream[123])" };

			//-- Act

			Stream data = new MemoryStream(new byte[123]);
			m_ApiContractWrapper.AMethodWithNotNullRefParam(size: -1, data: ref data);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void RefArgumentNotNull_PassNonNullValue_DoNotThrow()
		{
			//-- Arrange

			Stream originalData = new MemoryStream(new byte[123]);
			var data = originalData;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullRefParam(size: 0, data: ref data);

			//-- Assert

			Assert.That(data, Is.SameAs(originalData));
			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotNullRefParam(0,Stream[123])" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void RefArgumentNotNull_SetToNonNullValue_DoNotThrow()
		{
			//-- Arrange

			Stream originalData = new MemoryStream(new byte[123]);
			var data = originalData;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullRefParam(size: 456, data: ref data);

			//-- Assert

			Assert.That(data, Is.Not.SameAs(originalData));
			Assert.That(data.Length, Is.EqualTo(456));
			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotNullRefParam(456,Stream[123])" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void OutArgumentNotNull_SetToNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "data";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Output;
			m_ExpectedExceptionLog = new[] { "AMethodWithNotNullOutParam(0)" };

			//-- Act

			Stream data;
			m_ApiContractWrapper.AMethodWithNotNullOutParam(size: 0, data: out data);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void OutArgumentNotNull_SetToNonNullValue_DoNotThrow()
		{
			//-- Act

			var result = m_ApiContractWrapper.ANotNullFunction(x: 123);

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "ANotNullFunction(123)" }));
			Assert.That(((MemoryStream)result).ToArray(), Is.EqualTo(Encoding.Unicode.GetBytes("123")));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void ReturnValueNotNull_ReturnNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "(Return Value)";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Output;
			m_ExpectedExceptionLog = new[] { "ANotNullFunction(0)" };

			//-- Act

			m_ApiContractWrapper.ANotNullFunction(x: 0);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ReturnValueNotNull_ReturnNonNullValue_DoNotThrow()
		{
			//-- Act

			var result = m_ApiContractWrapper.ANotNullFunction(x: 123);

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "ANotNullFunction(123)" }));
			Assert.That(((MemoryStream)result).ToArray(), Is.EqualTo(Encoding.Unicode.GetBytes("123")));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void StringArgumentNotEmpty_PassNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "str";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotEmpty;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotEmptyString(str: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void StringArgumentNotEmpty_PassEmptyString_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "str";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotEmpty;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotEmptyString(str: "");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void StringArgumentNotEmpty_PassNonEmptyString_DoNotThrow()
		{
			//-- Act

			m_ApiContractWrapper.AMethodWithNotEmptyString(str: "ABC");

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotEmptyString(ABC)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void StringReturnValueNotEmpty_ReturnNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "(Return Value)";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotEmpty;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Output;
			m_ExpectedExceptionLog = new[] { "ANotEmptyStringFunction(0)" };

			//-- Act

			m_ApiContractWrapper.ANotEmptyStringFunction(x: 0);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void StringReturnValueNotEmpty_ReturnEmptyString_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "(Return Value)";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotEmpty;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Output;
			m_ExpectedExceptionLog = new[] { "ANotEmptyStringFunction(-1)" };

			//-- Act

			m_ApiContractWrapper.ANotEmptyStringFunction(x: -1);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void StringReturnValueNotEmpty_ReturnNonEmptyString_DoNotThrow()
		{
			//-- Act

			var result = m_ApiContractWrapper.ANotEmptyStringFunction(x: 123);

			//-- Assert

			Assert.That(result, Is.EqualTo("123"));
			Assert.That(m_Component.Log, Is.EqualTo(new[] { "ANotEmptyStringFunction(123)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void CollectionArgumentNotEmpty_PassNull_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "items";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotEmpty;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotEmptyCollection(items: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void CollectionArgumentNotEmpty_PassEmptyCollection_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "items";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.NotEmpty;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotEmptyCollection(items: new string[0]);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CollectionArgumentNotEmpty_PassNonEmptyCollection_DoNotThrow()
		{
			//-- Act

			m_ApiContractWrapper.AMethodWithNotEmptyCollection(items: new[] { "A", "B", "C" });

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotEmptyCollection(A,B,C)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void ItemsNotNull_ArrayWithNullItem_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "items";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.ItemsNotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullListItems(items: new[] {  "A", null, "C" });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ItemsNotNull_ArrayOfNonNullItems_DoNotThrow()
		{
			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullListItems(items: new[] { "A", "B", "C" });

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotNullListItems(A,B,C)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void ItemsNotNull_CustomCollectionWithNullItem_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "items";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.ItemsNotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullCollectionItems(items: new CustomCollection(new object[] { "A", null, "C" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ItemsNotNull_CustomCollectionWithNonNullItems_DoNotThrow()
		{
			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullCollectionItems(items: new CustomCollection(new object[] { "A", "B", "C" }));

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotNullCollectionItems(A,B,C)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(ApiContractException), Handler = "HandleApiContractException")]
		public void ItemsNotNull_GenericCustomCollectionWithNullItem_Throw()
		{
			//-- Arrange

			m_ExpectedExceptionParamName = "items";
			m_ExpectedExceptionFailedCheck = ApiContractCheckType.ItemsNotNull;
			m_ExpectedExceptionFailedCheckDirection = ApiContractCheckDirection.Input;

			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullCollectionItems(items: new CustomCollection<string> { "A", null, "C" });
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ItemsNotNull_GenericCustomCollectionWithNonNullItems_DoNotThrow()
		{
			//-- Act

			m_ApiContractWrapper.AMethodWithNotNullCollectionItems(items: new CustomCollection<string> { "A", "B", "C" });

			//-- Assert

			Assert.That(m_Component.Log, Is.EqualTo(new[] { "AMethodWithNotNullCollectionItems(A,B,C)" }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void HandleApiContractException(Exception e)
		{
			var typedException = (ApiContractException)e;

			if ( m_ExpectedExceptionParamName != null )
			{
				Assert.That(typedException.ParamName, Is.EqualTo(m_ExpectedExceptionParamName), "ExpectedException.ParamName");
				Assert.That(typedException.FailedCheck, Is.EqualTo(m_ExpectedExceptionFailedCheck), "ExpectedException.FailedCheck");
				Assert.That(typedException.FailedCheckDirection, Is.EqualTo(m_ExpectedExceptionFailedCheckDirection), "ExpectedException.FailedCheckDirection");
			}

			if ( m_ExpectedExceptionLog != null )
			{
				Assert.That(m_Component.Log, Is.EqualTo(m_ExpectedExceptionLog));
			}
			else
			{
				Assert.That(m_Component.Log, Is.Empty);
			}
		}
	}
}
