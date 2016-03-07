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

        public Action CompileStaticVoidMethod<TA1>(string name, Action<VoidMethodWriter> body)
        {
            return InternalCompileStaticVoidMethod<Action>(
                name,
                body);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1> CompileStaticVoidMethod<TA1>(string name, Action<VoidMethodWriter, Argument<TA1>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1>>(
                name,
                w => body(w, w.Arg1<TA1>()),
                typeof(TA1));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2> CompileStaticVoidMethod<TA1, TA2>(string name, Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1, TA2>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()),
                typeof(TA1), typeof(TA2));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3> CompileStaticVoidMethod<TA1, TA2, TA3>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()),
                typeof(TA1), typeof(TA2), typeof(TA3));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4> CompileStaticVoidMethod<TA1, TA2, TA3, TA4>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5, TA6> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5, TA6>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> CompileStaticVoidMethod<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(
            string name,
            Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
        {
            return InternalCompileStaticVoidMethod<Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TReturn> CompileStaticFunction<TReturn>(string name, Action<FunctionMethodWriter<TReturn>> body)
        {
            return InternalCompileStaticFunction<Func<TReturn>, TReturn>(
                name,
                body);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TReturn> CompileStaticFunction<TA1, TReturn>(string name, Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>()),
                typeof(TA1));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TReturn> CompileStaticFunction<TA1, TA2, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TA2, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()),
                typeof(TA1), typeof(TA2));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TReturn> CompileStaticFunction<TA1, TA2, TA3, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TA2, TA3, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()),
                typeof(TA1), typeof(TA2), typeof(TA3));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TA6, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn> CompileStaticFunction<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>(
            string name,
            Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
        {
            return InternalCompileStaticFunction<Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>, TReturn>(
                name,
                w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()),
                typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        //public Func<TA1, TReturn> CompileStaticFunction<TA1, TReturn>(string name, Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
        //{
        //    var methodFactory = new DynamicMethodFactory(
        //        m_TargetModule,
        //        name,
        //        argumentTypes: new[] { typeof(TA1) },
        //        returnType: typeof(TReturn));

        //    var methodMember = new MethodMember(
        //        ownerClass: null,
        //        methodFactory: methodFactory);

        //    methodMember.AddWriter(new FunctionMethodWriter<TReturn>(methodMember,
        //        script: w =>
        //        {
        //            body(w, w.Arg1<TA1>());
        //        }));

        //    methodMember.Write();
        //    methodMember.Compile();

        //    return (Func<TA1, TReturn>)methodFactory.DynamicMethod.CreateDelegate(typeof(Func<TA1, TReturn>));
        //}


        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private TDelegate InternalCompileStaticVoidMethod<TDelegate>(string name, Action<VoidMethodWriter> script, params Type[] argumentTypes)
        {
            var methodFactory = new DynamicMethodFactory(
                m_TargetModule,
                name,
                argumentTypes: argumentTypes,
                returnType: typeof(void));

            var methodMember = new MethodMember(
                ownerClass: null,
                methodFactory: methodFactory);

            methodMember.AddWriter(new VoidMethodWriter(methodMember, script));
            methodMember.Write();
            methodMember.Compile();

            return (TDelegate)(object)methodFactory.DynamicMethod.CreateDelegate(typeof(TDelegate));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private TDelegate InternalCompileStaticFunction<TDelegate, TReturn>(string name, Action<FunctionMethodWriter<TReturn>> script, params Type[] argumentTypes)
        {
            var methodFactory = new DynamicMethodFactory(
                m_TargetModule,
                name,
                argumentTypes: argumentTypes,
                returnType: typeof(TReturn));

            var methodMember = new MethodMember(
                ownerClass: null,
                methodFactory: methodFactory);

            methodMember.AddWriter(new FunctionMethodWriter<TReturn>(methodMember, script));
            methodMember.Write();
            methodMember.Compile();

            return (TDelegate)(object)methodFactory.DynamicMethod.CreateDelegate(typeof(TDelegate));
        }
    }
}
