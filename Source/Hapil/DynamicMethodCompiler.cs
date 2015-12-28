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

        public Action<TA1, TA2> CompileStaticVoidMethod<TA1, TA2>(string name, Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
        {
            var methodFactory = new DynamicMethodFactory(
                m_TargetModule, 
                name,
                argumentTypes: new[] { typeof(TA1), typeof(TA2) },
                returnType: typeof(void));

            var methodMember = new MethodMember(
                ownerClass: null,
                methodFactory: methodFactory);

            methodMember.AddWriter(new VoidMethodWriter(methodMember,
                script: w => {
                    body(w, w.Arg1<TA1>(), w.Arg2<TA2>());
                }));

            methodMember.Write();
            methodMember.Compile();

            return (Action<TA1, TA2>)methodFactory.DynamicMethod.CreateDelegate(typeof(Action<TA1, TA2>));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Func<TA1, TReturn> CompileStaticFunction<TA1, TReturn>(string name, Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
        {
            var methodFactory = new DynamicMethodFactory(
                m_TargetModule,
                name,
                argumentTypes: new[] { typeof(TA1) },
                returnType: typeof(TReturn));

            var methodMember = new MethodMember(
                ownerClass: null,
                methodFactory: methodFactory);

            methodMember.AddWriter(new FunctionMethodWriter<TReturn>(methodMember,
                script: w => {
                    body(w, w.Arg1<TA1>());
                }));

            methodMember.Write();
            methodMember.Compile();

            return (Func<TA1, TReturn>)methodFactory.DynamicMethod.CreateDelegate(typeof(Func<TA1, TReturn>));
        }
    }
}
