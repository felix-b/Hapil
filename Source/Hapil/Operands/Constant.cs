using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Hapil.Operands
{
    public class Constant<T> : Operand<T>
    {
        private readonly T m_Value;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Constant(T value)
        {
            m_Value = value;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            bool isNull = object.ReferenceEquals(null, m_Value);

            if ( m_Value is Type )
            {
                return string.Format("Type[{0}]", isNull ? "null" : (m_Value as Type).FriendlyName());
            }
            else
            {
                return string.Format("Const[{0}]", isNull ? "null" : m_Value.ToString());
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public T Value
        {
            get
            {
                return m_Value;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override OperandKind Kind
        {
            get
            {
                return OperandKind.Constant;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void OnEmitTarget(ILGenerator il)
        {
            // constants have no target
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void OnEmitLoad(ILGenerator il)
        {
            var actualValue = ResolveActualValue();

            if ( !TryEmitConvertibleValue(il, actualValue as IConvertible) )
            {
                if ( !TryEmitStaticDelegateValue(il, actualValue as Delegate) )
                {
                    if ( !TryEmitMetadataTokenValue(il, actualValue as Type, actualValue as MethodInfo, actualValue as FieldInfo) )
                    {
                        if ( !TryEmitNullValue(il, actualValue) )
                        {
                            throw Helpers.CreateConstantNotSupportedException(OperandType);
                        }
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void OnEmitStore(ILGenerator il)
        {
            throw new NotSupportedException("Constants are not assignable.");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void OnEmitAddress(ILGenerator il)
        {
            throw new NotSupportedException("Constants are not assignable.");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private object ResolveActualValue()
        {
            return TypeTemplate.ResolveValue(m_Value);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private bool TryEmitConvertibleValue(ILGenerator il, IConvertible convertible)
        {
            if ( convertible != null )
            {
                Helpers.EmitConvertible(il, convertible);

                var valueType = convertible.GetType();

                if ( valueType.IsValueType && OperandType == typeof(object) )
                {
                    il.Emit(OpCodes.Box, valueType);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private bool TryEmitMetadataTokenValue(ILGenerator il, Type type, MethodInfo method, FieldInfo field)
        {
            if ( type != null )
            {
                il.Emit(OpCodes.Ldtoken, type);
                il.Emit(OpCodes.Call, s_TypeGetTypeFromHandleMethod);
            }
            else if ( method != null )
            {
                il.Emit(OpCodes.Ldtoken, method);
                il.Emit(OpCodes.Call, s_MethodBaseGetMethodFromHandleMethod);
                il.Emit(OpCodes.Castclass, typeof(MethodInfo));
            }
            else if ( field != null )
            {
                il.Emit(OpCodes.Ldtoken, field);
                il.Emit(OpCodes.Call, s_FieldInfoGetFieldFromHandleMethod);
            }
            else
            {
                return false;
            }

            return true;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private bool TryEmitStaticDelegateValue(ILGenerator il, Delegate @delegate)
        {
            if ( @delegate != null )
            {
                if ( !@delegate.Method.IsStatic )
                {
                    throw new NotSupportedException("Constants of delegate types can only point to static methods.");
                }

                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ldftn, @delegate.Method);
                il.Emit(OpCodes.Newobj, DelegateShortcuts.GetDelegateConstructor(@delegate.GetType()));

                return true;
            }
            else
            {
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private bool TryEmitNullValue(ILGenerator il, object value)
        {
            if ( object.ReferenceEquals(null, value) )
            {
                il.Emit(OpCodes.Ldnull);
                return true;
            }
            else
            {
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static readonly MethodInfo s_TypeGetTypeFromHandleMethod = typeof(Type).GetMethod(
            "GetTypeFromHandle",
            BindingFlags.Public | BindingFlags.Static);

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static readonly MethodInfo s_MethodBaseGetMethodFromHandleMethod = typeof(MethodBase).GetMethod(
            "GetMethodFromHandle",
            BindingFlags.Public | BindingFlags.Static,
            null,
            new Type[] { typeof(RuntimeMethodHandle) }, 
            null);

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static readonly MethodInfo s_FieldInfoGetFieldFromHandleMethod = typeof(FieldInfo).GetMethod(
            "GetFieldFromHandle",
            BindingFlags.Public | BindingFlags.Static,
            null,
            new Type[] { typeof(RuntimeFieldHandle) },
            null);

        ////-----------------------------------------------------------------------------------------------------------------------------------------------------

        //public static implicit operator T(HapilConstant<T> operand)
        //{
        //	return operand.m_Value;
        //}
    }
}
