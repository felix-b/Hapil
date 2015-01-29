using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Hapil.Writers;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class NestedFactoryTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanPreserveOuterContextWhenUsingNestedFactory()
        {
            //-- Arrange

            var containerFactory = new TestContainerFactory(base.Module);

            //-- Act + Assert

            var container = containerFactory.CreateContainerInstance<AncestorRepository.IReadWritePropertiesContainer>();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class TestContainerFactory : ConventionObjectFactory
        {
            public TestContainerFactory(DynamicModule module)
                : base(module, new ContainerConvention(module))
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public T CreateContainerInstance<T>() where T : class
            {
                return CreateInstanceOf<T>().UsingDefaultConstructor();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class ContainerConvention : ImplementationConvention
        {
            private readonly ConventionObjectFactory _containedFactory;
            private TypeKey _initialKey;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public ContainerConvention(DynamicModule module)
                : base(Will.InspectDeclaration | Will.ImplementPrimaryInterface)
            {
                _containedFactory = new ConventionObjectFactory(module, new ContainedConvention());
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnInspectDeclaration(ObjectFactoryContext context)
            {
                _initialKey = context.TypeKey;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
            {
                writer.DefaultConstructor();
                writer.AllProperties().ForEach(propertyInfo => {
                    var containedType = _containedFactory.FindDynamicType(new TypeKey(primaryInterface: propertyInfo.PropertyType));

                    using ( TT.CreateScope<TT.TValue>(containedType) )
                    {
                        var backingField = writer.Field<TT.TValue>("m_" + propertyInfo.Name);
                        writer.Property(propertyInfo).Implement(
                            p => p.Get(pw => pw.Return(backingField.CastTo<TT.TProperty>())),
                            p => p.Set((pw, value) => backingField.Assign(value.CastTo<TT.TValue>()))
                        );
                    }

                    if ( base.Context == null || !object.ReferenceEquals(base.Context.TypeKey, _initialKey) )
                    {
                        throw new Exception("The initial key is gone!!");
                    }
                });
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class ContainedConvention : ImplementationConvention
        {
            public ContainedConvention()
                : base(Will.ImplementPrimaryInterface)
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
            {
                writer.DefaultConstructor();
                writer.AllProperties().ImplementAutomatic();
            }
        }
    }
}
