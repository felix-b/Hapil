using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Hapil.Writers;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class MetadataTypeKeyTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanUseMetadataBasedTypeKey()
        {
            //-- Arrange

            var factory = new TestEntityFactory(base.Module, CreateTestMetaRepository());

            //-- Act

            var product1 = factory.New("Product");
            var product2 = factory.New("Product");
            var order1 = factory.New("Order");
            var orderLine1 = factory.New("OrderLine");
            var orderLine2 = factory.New("OrderLine");

            product1.Id = 111;
            product1.Name = "AAA";
            product1.Price = 100;
            product2.Id = 222;
            product2.Name = "BBB";
            product2.Price = 200;

            orderLine1.Order = order1;
            orderLine1.Product = product1;
            orderLine1.Quantity = 1;

            orderLine2.Order = order1;
            orderLine2.Product = product2;
            orderLine2.Quantity = 2;

            product1.OrderLines.Add(orderLine1);
            product2.OrderLines.Add(orderLine2);

            //-- Assert

            var query =
                from prod in new[] { product1, product2 }
                from line in ((System.Collections.IList)prod.OrderLines).Cast<dynamic>()
                group line by prod
                    into g
                    select new
                    {
                        Id = g.Key.Id,
                        Name = g.Key.Name,
                        SoldValue = g.Sum(ol => (decimal)ol.Product.Price * (int)ol.Quantity)
                    };

            var results = query.Select(r => string.Format("{0}:{1}={2}", r.Id, r.Name, r.SoldValue)).ToArray();

            Assert.That(results, Is.EqualTo(new[] {
                "111:AAA=100",
                "222:BBB=400"
            }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        private class TestEntityFactory : ConventionObjectFactory
        {
            public TestEntityFactory(DynamicModule module, MetaRepository metadata)
                : base(module, new TestEntityConvention(metadata))
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public dynamic New(string entityName)
            {
                var key = new TestEntityTypeKey(entityName);
                return base.GetOrBuildType(key).CreateInstance<object>();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class TestEntityTypeKey : TypeKey
        {
            private readonly string m_EntityName;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TestEntityTypeKey(string entityName)
            {
                m_EntityName = entityName;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public override bool Equals(TypeKey other)
            {
                var otherTestEntityKey = (other as TestEntityTypeKey);

                if ( otherTestEntityKey != null )
                {
                    return (otherTestEntityKey.m_EntityName == this.m_EntityName);
                }
                else
                {
                    return base.Equals(other);
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public override int GetHashCode()
            {
                return m_EntityName.GetHashCode();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------
            
            public string EntityName
            {
                get { return m_EntityName; }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class TestEntityConvention : ImplementationConvention
        {
            private readonly MetaRepository m_Metadata;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TestEntityConvention(MetaRepository metadata)
                : base(Will.InspectDeclaration | Will.ImplementBaseClass)
            {
                m_Metadata = metadata;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnInspectDeclaration(ObjectFactoryContext context)
            {
                var entityKey = (TestEntityTypeKey)context.TypeKey;
                context.ClassFullName = context.Module.SimpleName + ".EntityObjects." + entityKey.EntityName;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnImplementBaseClass(ImplementationClassWriter<TypeTemplate.TBase> writer)
            {
                var entityKey = (TestEntityTypeKey)base.Context.TypeKey;
                var metaEntity = m_Metadata.Entities.Single(e => e.Name == entityKey.EntityName);
                var initializers = new List<Action<ConstructorWriter>>();

                foreach ( var metaProperty in metaEntity.Properties )
                {
                    switch ( metaProperty.Kind )
                    {
                        case MetaPropertyKind.Scalar:
                            ImplementScalarProperty(writer, metaProperty, initializers);
                            break;
                        case MetaPropertyKind.Navigation:
                            ImplementNavigationProperty(writer, metaProperty, initializers);
                            break;
                        case MetaPropertyKind.NavigationCollection:
                            ImplementNavigationCollectionProperty(writer, metaProperty, initializers);
                            break;
                    }
                }

                writer.Constructor(cw => initializers.ForEach(f => f(cw)));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            private void ImplementScalarProperty(
                ImplementationClassWriter<TypeTemplate.TBase> classWriter, 
                MetaProperty metaProperty, 
                List<Action<ConstructorWriter>> initializers)
            {
                using ( TT.CreateScope<TT.TProperty>(metaProperty.ScalarType) )
                {
                    classWriter.NewVirtualWritableProperty<TT.TProperty>(metaProperty.Name).ImplementAutomatic();
                }
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            private void ImplementNavigationProperty(
                ImplementationClassWriter<TypeTemplate.TBase> classWriter, 
                MetaProperty metaProperty, 
                List<Action<ConstructorWriter>> initializers)
            {
                var entityKey = new TestEntityTypeKey(metaProperty.NavigationEntityName);
                var entityType = base.Context.Factory.FindDynamicType(entityKey);

                using ( TT.CreateScope<TT.TProperty>(entityType) )
                {
                    classWriter.NewVirtualWritableProperty<TT.TProperty>(metaProperty.Name).ImplementAutomatic();
                }
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            private void ImplementNavigationCollectionProperty(
                ImplementationClassWriter<TypeTemplate.TBase> classWriter,
                MetaProperty metaProperty, 
                List<Action<ConstructorWriter>> initializers)
            {
                var entityKey = new TestEntityTypeKey(metaProperty.NavigationEntityName);
                var entityType = base.Context.Factory.FindDynamicType(entityKey);
                var propertyType = typeof(IList<>).MakeGenericType(entityType);
                var concreteCollectionType = typeof(List<>).MakeGenericType(entityType);

                using ( TT.CreateScope<TT.TProperty>(propertyType) )
                {
                    var backingField = classWriter.Field<TT.TProperty>("_" + metaProperty.Name);
                    classWriter.NewVirtualWritableProperty<TT.TProperty>(metaProperty.Name).ImplementAutomatic(backingField);

                    initializers.Add(cw => {
                        using ( TT.CreateScope<TT.TProperty>(concreteCollectionType) )
                        {
                            backingField.Assign(cw.New<TT.TProperty>());
                        }
                    });
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private bool IsScalarProperty(PropertyInfo property)
            {
                return (!property.PropertyType.IsCollectionType() && !CircularDependencyTests.TestEntityAttribute.IsEntity(property.PropertyType));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private bool IsSingleNavigationProperty(PropertyInfo property)
            {
                return (!property.PropertyType.IsCollectionType() && CircularDependencyTests.TestEntityAttribute.IsEntity(property.PropertyType));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private bool IsNavigationCollectionProperty(PropertyInfo property)
            {
                Type itemType;
                return (property.PropertyType.IsCollectionType(out itemType) && CircularDependencyTests.TestEntityAttribute.IsEntity(itemType));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private MetaRepository CreateTestMetaRepository()
        {
            return new MetaRepository {
                Entities = new[] {
                    new MetaEntity {
                        Name = "Product",
                        Properties = new[] {
                            new MetaProperty {
                                Name = "Id",
                                Kind = MetaPropertyKind.Scalar,
                                ScalarType = typeof(int),
                            },
                            new MetaProperty {
                                Name = "Name",
                                Kind = MetaPropertyKind.Scalar,
                                ScalarType = typeof(string),
                            },
                            new MetaProperty {
                                Name = "Price",
                                Kind = MetaPropertyKind.Scalar,
                                ScalarType = typeof(decimal),
                            },
                            new MetaProperty {
                                Name = "OrderLines",
                                Kind = MetaPropertyKind.NavigationCollection,
                                NavigationEntityName = "OrderLine"
                            },
                        }
                    },
                    new MetaEntity {
                        Name = "Order",
                        Properties = new[] {
                            new MetaProperty {
                                Name = "Id",
                                Kind = MetaPropertyKind.Scalar,
                                ScalarType = typeof(int),
                            },
                            new MetaProperty {
                                Name = "PlacedAt",
                                Kind = MetaPropertyKind.Scalar,
                                ScalarType = typeof(DateTime),
                            },
                            new MetaProperty {
                                Name = "OrderLines",
                                Kind = MetaPropertyKind.NavigationCollection,
                                NavigationEntityName = "OrderLine"
                            },
                        }
                    },
                    new MetaEntity {
                        Name = "OrderLine",
                        Properties = new[] {
                            new MetaProperty {
                                Name = "Product",
                                Kind = MetaPropertyKind.Navigation,
                                NavigationEntityName = "Product"
                            },
                            new MetaProperty {
                                Name = "Order",
                                Kind = MetaPropertyKind.Navigation,
                                NavigationEntityName = "Order"
                            },
                            new MetaProperty {
                                Name = "Quantity",
                                Kind = MetaPropertyKind.Scalar,
                                ScalarType = typeof(int),
                            },
                        }
                    }
                }
            };
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class MetaRepository
        {
            public IList<MetaEntity> Entities { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class MetaEntity
        {
            public string Name { get; set; }
            public IList<MetaProperty> Properties { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        private class MetaProperty
        {
            public string Name { get; set; }
            public MetaPropertyKind Kind { get; set; }
            public Type ScalarType { get; set; }
            public string NavigationEntityName { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private enum MetaPropertyKind
        {
            Scalar,
            Navigation,
            NavigationCollection
        }
    }
}
