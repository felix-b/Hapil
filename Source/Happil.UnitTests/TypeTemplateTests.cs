using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class TypeTemplateTests : ClassPerTestCaseFixtureBase
	{
      	[Test]
		public void TestTBase()
		{
			//-- Arrange

			OnDefineNewClass(key => Module.DeriveClassFrom<TypeTemplate.TBase>(TestCaseClassName)
				.DefaultConstructor()
				.AllProperties().ImplementAutomatic()
			);

			//-- Act

			DefineClassByKey(new HappilTypeKey(baseType: typeof(AncestorRepository.BaseTwo)));

			var obj = CreateClassInstanceAs<AncestorRepository.BaseTwo>().UsingDefaultConstructor();

			obj.FirstValue = 123;
			obj.SecondValue = "ABC";

			//-- Assert

			Assert.That(obj.FirstValue, Is.EqualTo(123));
			Assert.That(obj.SecondValue, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTPrimaryWithMethods()
		{
			//-- Arrange

			OnDefineNewClass(key => Module.DeriveClassFrom<object>(TestCaseClassName)
				.DefaultConstructor()
				.ImplementInterface<TypeTemplate.TPrimary>()
				.AllProperties().ImplementAutomatic()
				.ImplementInterface<IEquatable<TypeTemplate.TPrimary>>()
				.Method<TypeTemplate.TPrimary, bool>(intf => intf.Equals).Implement((m, other) => {
					m.ReturnConst(true);
				})
			);

			//-- Act

			DefineClassByKey(new HappilTypeKey(primaryInterface: typeof(AncestorRepository.IFewReadWriteProperties)));

			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingDefaultConstructor();
			obj.AnInt = 123;
			obj.AString = "ABC";

			var equatable = (IEquatable<AncestorRepository.IFewReadWriteProperties>)obj;
			var equalsResult = equatable.Equals(obj);

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(123));
			Assert.That(obj.AString, Is.EqualTo("ABC"));
			Assert.That(equalsResult, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestTBaseWithProperties()
		{
			//-- Arrange

			HappilField<TypeTemplate.TBase> remoteField, localField;

			OnDefineNewClass(key => Module.DeriveClassFrom<AncestorRepository.BaseTwo>(TestCaseClassName)
				.Field<TypeTemplate.TBase>("m_Remote", out remoteField)
				.Field<TypeTemplate.TBase>("m_Local", out localField)
				.DefaultConstructor()
				.Constructor<TypeTemplate.TBase, TypeTemplate.TBase>((m, remote, local) => {
					remoteField.Assign(remote);
					localField.Assign(local);
				})
				.AllProperties().ImplementAutomatic()
				.ImplementInterface<AncestorRepository.IVersionControlled<TypeTemplate.TBase>>()
				.Property(intf => intf.RemoteVersion).Implement(
					prop => prop.Get(m => m.Return(remoteField))
				)
				.Property(intf => intf.LocalVersion).Implement(
					prop => prop.Get(m => m.Return(localField))
				)
				.AllProperties().Implement(
					prop => prop.Get(m => m.Return(m.This<TypeTemplate.TProperty>()))
				)
			);

			//-- Act

			DefineClassByKey(new HappilTypeKey(
				baseType: typeof(AncestorRepository.BaseTwo),
				primaryInterface: typeof(AncestorRepository.IVersionControlled<AncestorRepository.BaseTwo>)));

			var remoteObj = CreateClassInstanceAs<AncestorRepository.BaseTwo>().UsingDefaultConstructor();
			var localObj = CreateClassInstanceAs<AncestorRepository.BaseTwo>().UsingDefaultConstructor();
			var workingObj = CreateClassInstanceAs<AncestorRepository.BaseTwo>()
				.UsingConstructor<AncestorRepository.BaseTwo, AncestorRepository.BaseTwo>(remoteObj, localObj, constructorIndex: 1);

			workingObj.FirstValue = 123;
			workingObj.SecondValue = "ABC";

			var versionedObj = (AncestorRepository.IVersionControlled<AncestorRepository.BaseTwo>)workingObj;

			//-- Assert

			Assert.That(remoteObj, Is.Not.SameAs(localObj));
			Assert.That(localObj, Is.Not.SameAs(workingObj));
			Assert.That(remoteObj, Is.Not.SameAs(workingObj));

			Assert.That(workingObj.FirstValue, Is.EqualTo(123));
			Assert.That(workingObj.SecondValue, Is.EqualTo("ABC"));
			Assert.That(versionedObj.RemoteVersion, Is.SameAs(remoteObj));
			Assert.That(versionedObj.LocalVersion, Is.SameAs(localObj));
			Assert.That(versionedObj.WorkingVersion, Is.SameAs(versionedObj));
		}
	}
}
