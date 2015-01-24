using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;

namespace Hapil.Writers
{
	public partial class ImplementationClassWriter<TBase> : ClassWriterBase
	{
	    public IVoidMethodSelector NewVirtualVoidMethod(string name)
	    {
            return new NewMethodSelector<NA, NA, NA, NA, NA, NA, NA, NA, NA>(this, name, new MethodSignature(
                isStatic: false, 
                isPublic: true, 
                argumentTypes: Type.EmptyTypes, 
                argumentNames: new string[0]));
        }
        public IVoidMethodSelector<TA1> NewVirtualVoidMethod<TA1>(string name, string arg1 = "arg1")
        {
            return new NewMethodSelector<NA, TA1, NA, NA, NA, NA, NA, NA, NA>(this, name, new MethodSignature(
                isStatic: false,
                isPublic: true, 
                argumentTypes: new[] { typeof(TA1) }, 
                argumentNames: new[] { arg1 }));
        }
        public IVoidMethodSelector<TA1, TA2> NewVirtualVoidMethod<TA1, TA2>(string name, string arg1 = "arg1", string arg2 = "arg2")
        {
            return new NewMethodSelector<NA, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, name, new MethodSignature(
                isStatic: false,
                isPublic: true, 
                argumentTypes: new[] { typeof(TA1), typeof(TA2) }, 
                argumentNames: new[] { arg1, arg2 }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public IFunctionMethodSelector<TReturn> NewVirtualFunction<TReturn>(string name)
        {
            return new NewMethodSelector<TReturn, NA, NA, NA, NA, NA, NA, NA, NA>(this, name, new MethodSignature(
                isStatic: false,
                isPublic: true, 
                argumentTypes: Type.EmptyTypes, 
                argumentNames: new string[0],
                returnType: typeof(TReturn)));
        }
        public IFunctionMethodSelector<TA1, TReturn> NewVirtualFunction<TA1, TReturn>(string name, string arg1 = "arg1")
        {
            return new NewMethodSelector<TReturn, TA1, NA, NA, NA, NA, NA, NA, NA>(this, name, new MethodSignature(
                isStatic: false,
                isPublic: true,
                argumentTypes: new[] { typeof(TA1) },
                argumentNames: new[] { arg1 },
                returnType: typeof(TReturn)));
        }
        public IFunctionMethodSelector<TA1, TA2, TReturn> NewVirtualFunction<TA1, TA2, TReturn>(string name, string arg1 = "arg1", string arg2 = "arg2")
        {
            return new NewMethodSelector<TReturn, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, name, new MethodSignature(
                isStatic: false,
                isPublic: true, 
                argumentTypes: new[] { typeof(TA1), typeof(TA2) }, 
                argumentNames: new[] { arg1, arg2 },
                returnType: typeof(TReturn)));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public IPropertySelector<TProperty> NewVirtualWritableProperty<TProperty>(string propertyName)
        {
            return new NewPropertySelector<NA, NA, TProperty>(this, propertyName, typeof(TProperty));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    private class NewMethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> : 
            MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>
	    {
            private readonly string m_MethodName;
            private readonly MethodSignature m_Signature;

	        //-------------------------------------------------------------------------------------------------------------------------------------------------

	        public NewMethodSelector(ImplementationClassWriter<TBase> classWriter, string methodName, MethodSignature signature)
	            : base(classWriter, new MethodInfo[0])
	        {
	            m_MethodName = methodName;
	            m_Signature = signature;
	        }

	        //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override ImplementationClassWriter<TBase> DefineMethodImplementations<TWriter>(
                Func<MethodMember, AttributeWriter> attributeWriterFactory, 
                Func<MethodMember, TWriter> writerFactory)
            {
                var methodFactory = new NewMethodFactory(base.OwnerClass, m_MethodName, m_Signature);
                var methodMember = new MethodMember(base.OwnerClass, methodFactory);

                base.OwnerClass.AddMember(methodMember);

                var writer = writerFactory(methodMember);
                writer.AddAttributes(attributeWriterFactory);

                return base.ClassWriter;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class NewPropertySelector<TIndex1, TIndex2, TProperty> : PropertySelector<TIndex1, TIndex2, TProperty>
        {
            private readonly string m_PropertyName;
            private readonly Type m_PropertyType;
            private readonly Type[] m_IndexerTypes;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public NewPropertySelector(ImplementationClassWriter<TBase> classWriter, string propertyName, Type propertyType, params Type[] indexerTypes)
                : base(classWriter, new PropertyInfo[0])
            {
                m_IndexerTypes = indexerTypes;
                m_PropertyType = propertyType;
                m_PropertyName = propertyName;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override ImplementationClassWriter<TBase> DefinePropertyImplementations<TWriter>(
                Func<PropertyMember, AttributeWriter> attributes,
                Func<PropertyMember, TWriter> writerFactory,
                FieldMember backingField = null)
            {
                var propertyMember = new PropertyMember(
                    base.OwnerClass, 
                    m_PropertyName, 
                    m_PropertyType, 
                    indexerTypes: m_IndexerTypes, 
                    backingField: backingField);

                base.OwnerClass.AddMember(propertyMember);

                var writer = writerFactory(propertyMember);
                writer.AddAttributes(attributes);

                return base.ClassWriter;
            }
        }
    }
}
