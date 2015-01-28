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
    public class CircularDependencyTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanCreateMutuallyDependentTypes()
        {
            //-- Arrange

            var factory = new TestEntityFactory(base.Module);

            //-- Act

            var product1 = factory.New<IProduct>();
            var product2 = factory.New<IProduct>();
            var order1 = factory.New<IOrder>();
            var orderLine1 = factory.New<IOrderLine>();
            var orderLine2 = factory.New<IOrderLine>();

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
                from line in prod.OrderLines
                group line by prod
                into g
                select new {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    SoldValue = g.Sum(ol => ol.Product.Price * ol.Quantity)
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
            public TestEntityFactory(DynamicModule module)
                : base(module, new TestCircularDtoConvention())
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TEntity New<TEntity>()
            {
                return CreateInstanceOf<TEntity>().UsingDefaultConstructor();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class TestCircularDtoConvention : ImplementationConvention
        {
            public TestCircularDtoConvention()
                : base(Will.ImplementPrimaryInterface)
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            #region Overrides of ImplementationConvention

            protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
            {
                writer.AllProperties(IsScalarProperty).ImplementAutomatic();

                var initializers = new List<Action<ConstructorWriter>>();
                var explicitImpl = writer.ImplementInterfaceExplicitly<TypeTemplate.TInterface>();
                
                explicitImpl.AllProperties(IsSingleNavigationProperty).ForEach(p => ImplementSingleNavigationProperty(explicitImpl, p, initializers));
                explicitImpl.AllProperties(IsNavigationCollectionProperty).ForEach(p => ImplementNavigationCollectionProperty(explicitImpl, p, initializers));

                writer.Constructor(cw => initializers.ForEach(init => init(cw)));
            }

            #endregion

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private void ImplementSingleNavigationProperty(
                ImplementationClassWriter<TypeTemplate.TInterface> explicitImplementation, 
                PropertyInfo property,
                List<Action<ConstructorWriter>> initializers)
            {
                var entityTypeKey = new TypeKey(primaryInterface: property.PropertyType);
                var entityType = base.Context.Factory.FindDynamicType(entityTypeKey);

                using ( TT.CreateScope<TT.TValue>(entityType) )
                {
                    var backingField = explicitImplementation.Field<TT.TValue>("_" + property.Name.ToCamelCase());
                    explicitImplementation.NewVirtualWritableProperty<TT.TValue>(property.Name).ImplementAutomatic(backingField);

                    explicitImplementation.Property(property).Implement(
                        getter: p => p.Get(m => m.Return(backingField.CastTo<TT.TProperty>())),
                        setter: p => p.Set((m, value) => backingField.Assign(value.CastTo<TT.TValue>()))
                    );
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private void ImplementNavigationCollectionProperty(
                ImplementationClassWriter<TypeTemplate.TInterface> explicitImplementation, 
                PropertyInfo property,
                List<Action<ConstructorWriter>> initializers)
            {
                Type entityContractType;
                property.PropertyType.IsCollectionType(out entityContractType);

                var entityTypeKey = new TypeKey(primaryInterface: entityContractType);
                var entityType = base.Context.Factory.FindDynamicType(entityTypeKey);

                using ( TT.CreateScope<TT.TValue, TT.TItem>(entityType, entityContractType) )
                {
                    var backingField = explicitImplementation.Field<ICollection<TT.TValue>>("_" + property.Name.ToCamelCase());
                    var adapterField = explicitImplementation.Field<TestCollectionAdapter<TT.TValue, TT.TItem>>("_" + property.Name.ToCamelCase() + "Adapter");

                    initializers.Add(new Action<ConstructorWriter>(cw => {
                        using ( TT.CreateScope<TT.TValue, TT.TItem>(entityType, entityContractType) )
                        {
                            backingField.Assign(cw.New<List<TT.TValue>>());
                            adapterField.Assign(cw.New<TestCollectionAdapter<TT.TValue, TT.TItem>>(backingField));
                        }
                    }));

                    explicitImplementation.NewVirtualWritableProperty<ICollection<TT.TValue>>(property.Name).Implement(
                        getter: p => p.Get(m => m.Return(p.BackingField)),
                        setter: p => p.Set((m, value) => {
                            adapterField.Assign(m.New<TestCollectionAdapter<TT.TValue, TT.TItem>>(value));
                            backingField.Assign(value);
                        }));

                    explicitImplementation.Property(property).Implement(
                        getter: p => p.Get(m => m.Return(adapterField.CastTo<TT.TProperty>()))
                    );
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private bool IsScalarProperty(PropertyInfo property)
            {
                return (!property.PropertyType.IsCollectionType() && !TestEntityAttribute.IsEntity(property.PropertyType));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private bool IsSingleNavigationProperty(PropertyInfo property)
            {
                return (!property.PropertyType.IsCollectionType() && TestEntityAttribute.IsEntity(property.PropertyType));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private bool IsNavigationCollectionProperty(PropertyInfo property)
            {
                Type itemType;
                return (property.PropertyType.IsCollectionType(out itemType) && TestEntityAttribute.IsEntity(itemType));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
        public class TestEntityAttribute : Attribute
        {
            public static bool IsEntity(Type t)
            {
                return (t.GetCustomAttribute<TestEntityAttribute>() != null);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class TestCollectionAdapter<TFrom, TTo> : ICollection<TTo>
        {
            private readonly ICollection<TFrom> m_InnerCollection;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TestCollectionAdapter(ICollection<TFrom> innerCollection)
            {
                m_InnerCollection = innerCollection;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void Add(TTo item)
            {
                m_InnerCollection.Add((TFrom)(object)item);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void Clear()
            {
                m_InnerCollection.Clear();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public bool Contains(TTo item)
            {
                return m_InnerCollection.Contains((TFrom)(object)item);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void CopyTo(TTo[] array, int arrayIndex)
            {
                var items = new TFrom[m_InnerCollection.Count];
                m_InnerCollection.CopyTo(items, 0);

                for ( int i = 0 ; i < items.Length ; i++ )
                {
                    array[i + arrayIndex] = (TTo)(object)items[i];
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public int Count
            {
                get
                {
                    return m_InnerCollection.Count;
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public bool IsReadOnly
            {
                get
                {
                    return m_InnerCollection.IsReadOnly;
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public bool Remove(TTo item)
            {
                return m_InnerCollection.Remove((TFrom)(object)item);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public IEnumerator<TTo> GetEnumerator()
            {
                return (IEnumerator<TTo>)m_InnerCollection.GetEnumerator();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return m_InnerCollection.GetEnumerator();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestEntity]
        public interface IProduct
        {
            int Id { get; set; }
            string Name { get; set; }
            decimal Price { get; set; }
            ICollection<IOrderLine> OrderLines { get; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestEntity]
        public interface IOrderLine
        {
            int Id { get; set; }
            IProduct Product { get; set; }
            IOrder Order { get; set; }
            int Quantity { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestEntity]
        public interface IOrder
        {
            int Id { get; set; }
            DateTime PlacedOn { get; set; }
            ICollection<IOrderLine> OrderLines { get; }
        }
    }
}
