using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests.Operands
{
    [TestFixture]
    public class ConstantOperandTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanEmitTypeConstant()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.IHaveMetadataToken>()
                .Property(x => x.TypeFromToken).Implement(p => p.Get(m => m.Return(m.Const(typeof(FileStream)))))
                .AllProperties().Implement(p => p.Get(m => m.Throw<NotImplementedException>("Not implemented")));

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IHaveMetadataToken>().UsingDefaultConstructor();
            var returnedType = obj.TypeFromToken;

            //-- Assert

            Assert.That(returnedType, Is.SameAs(typeof(FileStream)));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanEmitTypeConstantOfOwnTypeBuilder()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.IHaveMetadataToken>()
                .Property(x => x.TypeFromToken).Implement(p => p.Get(m => m.Return(m.Const((Type)m.OwnerClass.TypeBuilder))))
                .AllProperties().Implement(p => p.Get(m => m.Throw<NotImplementedException>("Not implemented")));

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IHaveMetadataToken>().UsingDefaultConstructor();
            var returnedType = obj.TypeFromToken;

            //-- Assert

            Assert.That(returnedType, Is.SameAs(obj.GetType()));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanEmitMethodInfoConstant()
        {
            //-- Arrange

            var stringContainsMethod = TypeMemberCache.Of<string>().Methods.Single(m => m.Name == "Contains");

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.IHaveMetadataToken>()
                .Property(x => x.MethodFromToken).Implement(p => p.Get(m => m.Return(m.Const(stringContainsMethod))))
                .AllProperties().Implement(p => p.Get(m => m.Throw<NotImplementedException>("Not implemented")));

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IHaveMetadataToken>().UsingDefaultConstructor();
            var returnedMethodInfo = obj.MethodFromToken;

            //-- Assert

            Assert.That(returnedMethodInfo, Is.SameAs(stringContainsMethod));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanEmitMethodInfoOfOwnMethodBuilder()
        {
            //-- Arrange

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.IHaveMetadataToken>()
                .Property(x => x.MethodFromToken).Implement(p => p.Get(m => m.Return(m.Const((MethodInfo)m.OwnerMethod.MethodFactory.Builder))))
                .AllProperties().Implement(p => p.Get(m => m.Throw<NotImplementedException>("Not implemented")));

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IHaveMetadataToken>().UsingDefaultConstructor();
            var returnedMethodInfo = obj.MethodFromToken;
            var expectedMethodInfo = obj.GetType().GetProperty("MethodFromToken").GetGetMethod();

            //-- Assert

            Assert.That(expectedMethodInfo, Is.Not.Null);
            Assert.That(returnedMethodInfo, Is.SameAs(expectedMethodInfo));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanEmitFieldInfoConstant()
        {
            //-- Arrange

            var compiledFieldInfo = this.GetType().GetField("ACompiledField", BindingFlags.Public | BindingFlags.Static);

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .ImplementInterface<AncestorRepository.IHaveMetadataToken>()
                .Property(x => x.FieldFromToken).Implement(p => p.Get(m => m.Return(m.Const(compiledFieldInfo))))
                .AllProperties().Implement(p => p.Get(m => m.Throw<NotImplementedException>("Not implemented")));

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IHaveMetadataToken>().UsingDefaultConstructor();
            var returnedFieldInfo = obj.FieldFromToken;

            //-- Assert

            Assert.That(returnedFieldInfo, Is.SameAs(compiledFieldInfo));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanEmitFieldInfoConstantOfOwnField()
        {
            //-- Arrange

            Field<int> intField;

            DeriveClassFrom<object>()
                .DefaultConstructor()
                .StaticField<int>("IntField", out intField)
                .ImplementInterface<AncestorRepository.IHaveMetadataToken>()
                .Property(x => x.FieldFromToken).Implement(p => p.Get(m => m.Return(m.Const(((FieldMember)intField).FieldBuilder))))
                .AllProperties().Implement(p => p.Get(m => m.Throw<NotImplementedException>("Not implemented")));

            //-- Act

            var obj = CreateClassInstanceAs<AncestorRepository.IHaveMetadataToken>().UsingDefaultConstructor();
            var returnedFieldInfo = obj.FieldFromToken;
            var expectedFieldInfo = obj.GetType().GetField("IntField", BindingFlags.Static | BindingFlags.NonPublic);

            //-- Assert

            Assert.That(expectedFieldInfo, Is.Not.Null);
            Assert.That(returnedFieldInfo, Is.SameAs(expectedFieldInfo));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static int ACompiledField;
    }
}
