using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using Hapil.Expressions;
using Hapil.Operands;
using NUnit.Framework;
using Repo = Hapil.UnitTests.AncestorRepository;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests
{
	[TestFixture]
	public class TypeTemplateTests : NUnitEmittedTypesTestBase
	{
      	[Test]
		public void CanDeriveFromTBase()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<TT.TBase>(key)
				.DefaultConstructor()
				.AllProperties().ImplementAutomatic()
			);

			//-- Act

			DefineClassByKey(new TypeKey(baseType: typeof(Repo.BaseTwo)));

			var obj = CreateClassInstanceAs<Repo.BaseTwo>().UsingDefaultConstructor();

			obj.FirstValue = 123;
			obj.SecondValue = "ABC";

			//-- Assert

			Assert.That(obj.FirstValue, Is.EqualTo(123));
			Assert.That(obj.SecondValue, Is.EqualTo("ABC"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanImplementTPrimary()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<TT.TPrimary>()
				.AllProperties().ImplementAutomatic()
				.ImplementInterface<IEquatable<TT.TPrimary>>()
				.Method<TT.TPrimary, bool>(intf => intf.Equals).Implement((m, other) => {
					m.Return(true);
				})
			);

			//-- Act

			DefineClassByKey(new TypeKey(primaryInterface: typeof(Repo.IFewReadWriteProperties)));

			var obj = CreateClassInstanceAs<Repo.IFewReadWriteProperties>().UsingDefaultConstructor();
			obj.AnInt = 123;
			obj.AString = "ABC";

			var equatable = (IEquatable<Repo.IFewReadWriteProperties>)obj;
			var equalsResult = equatable.Equals(obj);

			//-- Assert

			Assert.That(obj.AnInt, Is.EqualTo(123));
			Assert.That(obj.AString, Is.EqualTo("ABC"));
			Assert.That(equalsResult, Is.True);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanHaveMembersOfTypeTBase()
		{
			//-- Arrange

			Field<TT.TBase> remoteField, localField;

			OnDefineNewClass(key => DeriveClassFrom<Repo.BaseTwo>()
				.Field<TT.TBase>("m_Remote", out remoteField)
				.Field<TT.TBase>("m_Local", out localField)
				.DefaultConstructor()
				.Constructor<TT.TBase, TT.TBase>((m, remote, local) => {
					remoteField.Assign(remote);
					localField.Assign(local);
				})
				.AllProperties().ImplementAutomatic()
				.ImplementInterface<Repo.IVersionControlled<TT.TBase>>()
				.Property(intf => intf.RemoteVersion).Implement(
					prop => prop.Get(m => m.Return(remoteField))
				)
				.Property(intf => intf.LocalVersion).Implement(
					prop => prop.Get(m => m.Return(localField))
				)
				.AllProperties().Implement(
					prop => prop.Get(m => m.Return(m.This<TT.TProperty>()))
				)
			);

			//-- Act

			DefineClassByKey(new TypeKey(
				baseType: typeof(Repo.BaseTwo),
				primaryInterface: typeof(Repo.IVersionControlled<Repo.BaseTwo>)));

			var remoteObj = CreateClassInstanceAs<Repo.BaseTwo>().UsingDefaultConstructor();
			var localObj = CreateClassInstanceAs<Repo.BaseTwo>().UsingDefaultConstructor();
			var workingObj = CreateClassInstanceAs<Repo.BaseTwo>()
				.UsingConstructor<Repo.BaseTwo, Repo.BaseTwo>(remoteObj, localObj, constructorIndex: 1);

			workingObj.FirstValue = 123;
			workingObj.SecondValue = "ABC";

			var versionedObj = (Repo.IVersionControlled<Repo.BaseTwo>)workingObj;

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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanHaveMembersOfTypeTPrimary()
		{
			//-- Arrange

			Field<TT.TPrimary> remoteField, localField;

			OnDefineNewClass(key => DeriveClassFrom<object>()
				.Field<TT.TPrimary>("m_Remote", out remoteField)
				.Field<TT.TPrimary>("m_Local", out localField)
				.DefaultConstructor()
				.Constructor<TT.TPrimary, TT.TPrimary>((m, remote, local) => {
					remoteField.Assign(remote);
					localField.Assign(local);
				})
				.ImplementInterface<TT.TPrimary>()
				.AllProperties().ImplementAutomatic()
				.ImplementInterface<Repo.IVersionControlled<TT.TPrimary>>()
				.Property(intf => intf.RemoteVersion).Implement(
					prop => prop.Get(m => m.Return(remoteField))
				)
				.Property(intf => intf.LocalVersion).Implement(
					prop => prop.Get(m => m.Return(localField))
				)
				.AllProperties().Implement(
					prop => prop.Get(m => m.Return(m.This<TT.TProperty>()))
				)
			);

			//-- Act

			DefineClassByKey(new TypeKey(
				primaryInterface: typeof(Repo.IFewReadWriteProperties),
				secondaryInterfaces: typeof(Repo.IVersionControlled<Repo.IFewPropertiesWithIndexers>)));

			var remoteObj = CreateClassInstanceAs<Repo.IFewReadWriteProperties>().UsingDefaultConstructor();
			var localObj = CreateClassInstanceAs<Repo.IFewReadWriteProperties>().UsingDefaultConstructor();
			var workingObj = CreateClassInstanceAs<Repo.IFewReadWriteProperties>()
				.UsingConstructor<Repo.IFewReadWriteProperties, Repo.IFewReadWriteProperties>(remoteObj, localObj, constructorIndex: 1);

			workingObj.AnInt = 123;
			workingObj.AString = "ABC";

			var versionedObj = (Repo.IVersionControlled<Repo.IFewReadWriteProperties>)workingObj;

			//-- Assert

			Assert.That(remoteObj, Is.Not.SameAs(localObj));
			Assert.That(localObj, Is.Not.SameAs(workingObj));
			Assert.That(remoteObj, Is.Not.SameAs(workingObj));

			Assert.That(workingObj.AnInt, Is.EqualTo(123));
			Assert.That(workingObj.AString, Is.EqualTo("ABC"));
			Assert.That(versionedObj.RemoteVersion, Is.SameAs(remoteObj));
			Assert.That(versionedObj.LocalVersion, Is.SameAs(localObj));
			Assert.That(versionedObj.WorkingVersion, Is.SameAs(versionedObj));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanUseTemplateTypesInConstructorArguments()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<TT.TBase>()
				.Constructor<TT.TBase, TT.TPrimary>((m, @base, primary) => {
				})
				.ImplementInterface<TT.TPrimary>()
				.AllProperties().ImplementAutomatic()
			);

			//-- Act

			DefineClassByKey(new TypeKey(
				baseType: typeof(Repo.BaseOne),
				primaryInterface: typeof(Repo.IOneProperty)));

			var obj = CreateClassInstanceAs<Repo.IOneProperty>().UsingConstructor<Repo.BaseOne, Repo.IOneProperty>(null, null);
			var constructorInfo = obj.GetType().GetConstructor(new[] { typeof(Repo.BaseOne), typeof(Repo.IOneProperty) });

			//-- Assert

			Assert.That(obj, Is.Not.Null);
			Assert.That(constructorInfo, Is.Not.Null);
			Assert.That(constructorInfo.GetParameters().Length, Is.EqualTo(2));
			Assert.That(constructorInfo.GetParameters()[0].ParameterType, Is.EqualTo(typeof(Repo.BaseOne)));
			Assert.That(constructorInfo.GetParameters()[1].ParameterType, Is.EqualTo(typeof(Repo.IOneProperty)));

		}
	}
}
