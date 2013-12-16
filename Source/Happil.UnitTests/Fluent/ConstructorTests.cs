//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Happil.Fluent;
//using NUnit.Framework;

//namespace Happil.UnitTests.Fluent
//{
//	[TestFixture]
//	public class ConstructorTests : ClassPerTestFixtureBase<TestBaseOne>
//	{
//		[Test]
//		public void Test1()
//		{
//			HappilField<int> intField;
//			HappilField<string> stringField;

//			ClassBody
//				.Field<int>("m_IntField", out intField)
//				.Field<string>("m_StringField", out stringField)
//				.Constructor<int, string>((ctor, intValue, stringValue) => {
//					intField.Assign(intValue);
//					stringField.Assign(stringValue);
//				})
//				.Function<int>(x => x.VirtualFuncWithNoArgs, m => {
//					m.Return(intField + stringField.Get(s => s.Length));	
//				});

//			Class.CreateType();
//			Class.GetFactoryMethods();
//		}



//	}
//}
