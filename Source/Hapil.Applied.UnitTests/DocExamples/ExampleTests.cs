using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Hapil.Decorators;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Writers;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.Applied.UnitTests.DocExamples
{
    [TestFixture]
    public class ExampleTests
    {
        private DynamicModule m_Module;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            m_Module = new DynamicModule(
                "Hapil.UnitTests.EmittedBy" + this.GetType().Name,
                allowSave: true,
                saveDirectory: TestContext.CurrentContext.TestDirectory);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            m_Module.SaveAssembly();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void DataTransferObjectExample()
        {
            //-- Arrange

            var factory = new ConventionObjectFactory(
                m_Module,
                new AutomaticPropertiesConvention(),
                new DataContractConvention(dataContractNamespace: "urn:felix-b.hapil.examples"),
                new NotifyPropertyChangedImplementorConvention(),
                new NotifyPropertyChangedDecoratorConvention());

            //-- Act

            var customer = factory.CreateInstanceOf<ICustomer>().UsingDefaultConstructor();

            //-- Assert

        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface ICustomer
        {
            int Id { get; set; }
            string FullName { get; set; }
            string EmailAddress { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class AutomaticPropertiesConvention : ImplementationConvention
        {
            public AutomaticPropertiesConvention()
                : base(Will.ImplementPrimaryInterface)
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
            {
                writer
                    .DefaultConstructor()
                    .AllProperties().ImplementAutomatic();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class DataContractConvention : DecorationConvention
        {
            private readonly string m_DataContractNamespace;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public DataContractConvention(string dataContractNamespace)
                : base(Will.DecorateProperties)
            {
                m_DataContractNamespace = dataContractNamespace;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnClass(ClassType classType, DecoratingClassWriter classWriter)
            {
                classWriter.Attribute<DataContractAttribute>(value => value
                    .Named(attr => attr.Namespace, m_DataContractNamespace)
                    .Named(attr => attr.Name, classType.Key.PrimaryInterface.Name.TrimPrefix("I")));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
            {
                decorate().Attribute<DataMemberAttribute>();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class NotifyPropertyChangedImplementorConvention : ImplementationConvention
        {
            public NotifyPropertyChangedImplementorConvention()
                : base(Will.ImplementBaseClass)
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnImplementBaseClass(ImplementationClassWriter<TypeTemplate.TBase> writer)
            {
                writer.ImplementInterface<INotifyPropertyChanged>()
                    .AllEvents().ImplementAutomatic();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class NotifyPropertyChangedDecoratorConvention : DecorationConvention
        {
            public NotifyPropertyChangedDecoratorConvention()
                : base(Will.DecorateProperties)
            {
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override void OnProperty(PropertyMember property, Func<PropertyDecorationBuilder> decorate)
            {
                Local<TT.TProperty> oldValue = null;

                decorate()
                    .Setter()
                        .OnBefore(w => 
                            oldValue = w.Local(initialValue: property.BackingField.AsOperand<TT.TProperty>())
                        )
                        .OnSuccess(w =>
                            w.If(w.Arg1<TT.TProperty>() != oldValue).Then(() =>
                                w.RaiseEvent("PropertyChanged", w.New<PropertyChangedEventArgs>(w.Const(property.Name)))
                            )
                        );
            }
        }
    }
}
