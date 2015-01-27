using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Hapil.Writers;
using NUnit.Framework;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class CircularDependencyTests : NUnitEmittedTypesTestBase
    {
        [Test, Ignore("WIP")]
        public void CanCreateMutuallyDependentTypes()
        {
            
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class CircularDtoConvention : ImplementationConvention
        {
            public CircularDtoConvention()
                : base(Will.InspectDeclaration | Will.ImplementPrimaryInterface)
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            #region Overrides of ImplementationConvention

            protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
            {
                
            }

            #endregion

            //-------------------------------------------------------------------------------------------------------------------------------------------------


        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
        public class EntityAttribute : Attribute
        {
            public static bool IsEntity(Type t)
            {
                return (t.GetCustomAttribute<EntityAttribute>() != null);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class CollectionAdapter<TFrom, TTo> : ICollection<TTo> where TFrom : TTo
        {
            private readonly ICollection<TFrom> m_InnerCollection;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public CollectionAdapter(ICollection<TFrom> innerCollection)
            {
                m_InnerCollection = innerCollection;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void Add(TTo item)
            {
                throw new NotImplementedException();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void Clear()
            {
                throw new NotImplementedException();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public bool Contains(TTo item)
            {
                throw new NotImplementedException();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public void CopyTo(TTo[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public bool Remove(TTo item)
            {
                throw new NotImplementedException();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public IEnumerator<TTo> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Entity]
        public interface IProduct
        {
            int Id { get; set; }
            string Name { get; set; }
            decimal Price { get; set; }
            ICollection<IOrderLine> OrderLines { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Entity]
        public interface IOrderLine
        {
            int Id { get; set; }
            IProduct Product { get; set; }
            IOrder Order { get; set; }
            int Quantity { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Entity]
        public interface IOrder
        {
            int Id { get; set; }
            DateTime PlacedOn { get; set; }
            ICollection<IOrderLine> OrderLines { get; set; }
        }
    }
}
