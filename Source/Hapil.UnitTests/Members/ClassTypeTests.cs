using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Operands;
using Hapil.Testing.NUnit;
using Hapil.Writers;
using Moq;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests.Members
{
    [TestFixture]
    public class ClassTypeTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanUseOneBackingFieldWithDualPropertyImplementations()
        {
            //-- Arrange

            var factory = new ConventionObjectFactory(
                base.Module,
                new DualReadOnlyPropertyConvention());

            //-- Act

            var asInterface = factory.CreateInstanceOf<AncestorRepository.IReadOnlyPropertiesWithDefaults>().UsingDefaultConstructor();
            dynamic asDynamic = asInterface;

            //-- Assert

            Assert.That(asInterface, Is.Not.Null);
          
            Assert.That(asInterface.AString, Is.EqualTo("ABC"));
            Assert.That(asInterface.AnInt, Is.EqualTo(123));
            Assert.That(asInterface.AnObject, Is.EqualTo(DayOfWeek.Monday));

            Assert.That(asDynamic.AString, Is.EqualTo("ABC"));
            Assert.That(asDynamic.AnInt, Is.EqualTo(123));
            Assert.That(asDynamic.AnObject, Is.EqualTo(DayOfWeek.Monday));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class DualReadOnlyPropertyConvention : ImplementationConvention
        {
            public DualReadOnlyPropertyConvention()
                : base(Will.ImplementPrimaryInterface)
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TT.TInterface> implicitImpl)
            {
                var explicitImpl = implicitImpl.ImplementInterfaceExplicitly<TT.TInterface>();
                var propertiesToInitialize = new List<PropertyInfo>();

                implicitImpl.AllProperties().ForEach(property => {
                    using ( TT.CreateScope<TT.TProperty>(property.PropertyType) )
                    {
                        ImplementProperty(implicitImpl, explicitImpl, property, propertiesToInitialize);
                    }
                });
                
                implicitImpl.Constructor(cw => {
                    ImplementConstructor(implicitImpl, propertiesToInitialize);
                });
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private void ImplementConstructor(
                ImplementationClassWriter<TypeTemplate.TInterface> implicitImpl, 
                List<PropertyInfo> propertiesToInitialize)
            {
                foreach ( var property in propertiesToInitialize )
                {
                    using ( TT.CreateScope<TT.TProperty>(property.PropertyType) )
                    {
                        var backingField = implicitImpl.OwnerClass.GetPropertyBackingField(property).AsOperand<TT.TProperty>();
                        var defaultAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
                        var defaultValue = (defaultAttribute != null ? defaultAttribute.Value : null);

                        if ( defaultValue != null )
                        {
                            var valueOperandType = typeof(Constant<>).MakeGenericType(defaultValue.GetType());
                            var valueOperand = ((IOperand)Activator.CreateInstance(valueOperandType, defaultValue)).CastTo<TT.TProperty>();
                            backingField.Assign(valueOperand);
                        }
                    }
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private void ImplementProperty(
                ImplementationClassWriter<TypeTemplate.TInterface> implicitImpl, 
                ImplementationClassWriter<TypeTemplate.TInterface> explicitImpl, 
                PropertyInfo property, 
                List<PropertyInfo> propertiesToInitialize)
            {
                var backingField = implicitImpl.Field<TT.TProperty>("m_" + property.Name);
                implicitImpl.NewVirtualWritableProperty<TT.TProperty>(property.Name).ImplementAutomatic(backingField);
                explicitImpl.Property(property).Implement(p => p.Get(pw => pw.Return(backingField)));
                implicitImpl.OwnerClass.SetPropertyBackingField(property, backingField);
                propertiesToInitialize.Add(property);
            }
        }
    }
}
