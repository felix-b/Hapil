using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Writers;

namespace Hapil
{
    public class DynamicMethodCompiler
    {
        private readonly DynamicModule m_TargetModule;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public DynamicMethodCompiler(DynamicModule targetModule)
        {
            m_TargetModule = targetModule;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public NonTemplatedSignatures<TDelegate> ForDelegate<TDelegate>()
        {
            return new NonTemplatedSignatures<TDelegate>(this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TemplatedSignatures<TDelegate> ForTemplatedDelegate<TDelegate>()
        {
            return new TemplatedSignatures<TDelegate>(this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action CompileStaticVoidMethod<TA1>(string name, Action<VoidMethodWriter> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action>(
                name,
                body);

            return (Action)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1> CompileStaticVoidMethod<TA1>(string name, Action<VoidMethodWriter, Argument<TA1>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1>>(
                name,
                w => body(w, w.Arg1<TA1>()),
                typeof(TA1));

            return (Action<TA1>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2> CompileStaticVoidMethod<TA1, TA2>(string name, Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1, TA2>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()),
                typeof(TA1), typeof(TA2));

            return (Action<TA1, TA2>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3> CompileStaticVoidMethod<TA1, TA2, TA3>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()),
                typeof(TA1), typeof(TA2), typeof(TA3));

            return (Action<TA1, TA2, TA3>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4> CompileStaticVoidMethod<TA1, TA2, TA3, TA4>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4));

            return (Action<TA1, TA2, TA3, TA4>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5));

            return (Action<TA1, TA2, TA3, TA4, TA5>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5, TA6> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5, TA6>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6));

            return (Action<TA1, TA2, TA3, TA4, TA5, TA6>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7));

            return (Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8));

            return (Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TReturn> CompileStaticFunction<TReturn>(string name, Action<FunctionMethodWriter<TReturn>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TReturn>, TReturn>(
                name,
                body);

            return (Func<TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TReturn> CompileStaticFunction<TA1, TReturn>(string name, Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>()),
                typeof(TA1));

            return (Func<TA1, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TReturn> CompileStaticFunction<TA1, TA2, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TA2, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()),
                typeof(TA1), typeof(TA2));

            return (Func<TA1, TA2, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TReturn> CompileStaticFunction<TA1, TA2, TA3, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TA2, TA3, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()),
                typeof(TA1), typeof(TA2), typeof(TA3));

            return (Func<TA1, TA2, TA3, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4));

            return (Func<TA1, TA2, TA3, TA4, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5));

            return (Func<TA1, TA2, TA3, TA4, TA5, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TA6, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6));

            return (Func<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7));

            return (Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
        {
            var dynamicMethodDelegate = InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8));

            return (Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>)dynamicMethodDelegate;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private Delegate InternalCompileStaticVoidMethod<TDelegate>(string name, Action<VoidMethodWriter> script, params Type[] argumentTypes)
        {
            var methodFactory = new DynamicMethodFactory(
                m_TargetModule,
                name,
                typeof(TDelegate));
                //argumentTypes: argumentTypes,
                //returnType: typeof(void));

            var methodMember = new MethodMember(
                ownerClass: null,
                methodFactory: methodFactory);

            var writer = new VoidMethodWriter(methodMember, script);
            methodMember.Write();
            methodMember.Compile();

            return methodFactory.DynamicMethod.CreateDelegate(TypeTemplate.Resolve<TDelegate>());
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private Delegate InternalCompileStaticFunction<TDelegate, TReturn>(string name, Action<FunctionMethodWriter<TReturn>> script, params Type[] argumentTypes)
        {
            var methodFactory = new DynamicMethodFactory(
                m_TargetModule,
                name,
                typeof(TDelegate));
                //argumentTypes: argumentTypes,
                //returnType: typeof(TReturn));

            var methodMember = new MethodMember(
                ownerClass: null,
                methodFactory: methodFactory);

            var writer = new FunctionMethodWriter<TReturn>(methodMember, script);
            methodMember.Write();
            methodMember.Compile();

            return methodFactory.DynamicMethod.CreateDelegate(TypeTemplate.Resolve<TDelegate>());
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class NonTemplatedSignatures<TDelegate>
        {
            private readonly DynamicMethodCompiler _owner;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public NonTemplatedSignatures(DynamicMethodCompiler owner)
            {
                _owner = owner;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod(string name, Action<VoidMethodWriter> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    body);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1>(string name, Action<VoidMethodWriter, Argument<TA1>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1, TA2>(string name, Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1, TA2, TA3>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TReturn>(string name, Action<FunctionMethodWriter<TReturn>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    body);
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TReturn>(string name, Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TA2, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TA2, TA3, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TA2, TA3, TA4, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public TDelegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
            {
                return (TDelegate)(object)_owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public class TemplatedSignatures<TDelegate>
        {
            private readonly DynamicMethodCompiler _owner;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TemplatedSignatures(DynamicMethodCompiler owner)
            {
                _owner = owner;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod(string name, Action<VoidMethodWriter> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    body);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1>(string name, Action<VoidMethodWriter, Argument<TA1>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1, TA2>(string name, Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1, TA2, TA3>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(
                string name,
                Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
            {
                return _owner.InternalCompileStaticVoidMethod<TDelegate>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TReturn>(string name, Action<FunctionMethodWriter<TReturn>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    body);
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TReturn>(string name, Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TA2, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TA2, TA3, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TA2, TA3, TA4, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
            }

            //-----------------------------------------------------------------------------------------------------------------------------------------------------

            public Delegate CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>(
                string name,
                Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
            {
                return _owner.InternalCompileStaticFunction<TDelegate, TReturn>(
                    name,
                    w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
            }
        }
    }
}
