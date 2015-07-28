using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Operands;
using Hapil.Testing.NUnit;
using NUnit.Framework;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class EnumShortcutsTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanTestEnumFlags()
        {
            //-- arrange

            Field<TestFlags> enumField;

            DeriveClassFrom<object>()
                .PrimaryConstructor("Flags", out enumField)
                .NewVirtualFunction<bool>("HasFirst").Implement(f => {
                    f.Return(enumField.EnumHasFlag(TestFlags.First));
                })
                .NewVirtualFunction<bool>("HasSecond").Implement(f => {
                    f.Return(enumField.EnumHasFlag(TestFlags.Second));
                });

            //-- act

            dynamic obj = CreateClassInstanceAs<object>().UsingConstructor(TestFlags.First);

            var hasFirst = obj.HasFirst();
            var hasSecond = obj.HasSecond();


            //-- assert

            Assert.That(hasFirst, Is.True);
            Assert.That(hasSecond, Is.False);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanCompareEnumValuesForEquality()
        {
            //-- arrange

            Field<TestFlags> enumField;

            DeriveClassFrom<object>()
                .PrimaryConstructor("Flags", out enumField)
                .NewVirtualFunction<bool>("IsFirst").Implement(f => {
                    f.Return(enumField == TestFlags.First);
                })
                .NewVirtualFunction<bool>("IsSecond").Implement(f => {
                    f.Return(enumField == TestFlags.Second);
                })
                .NewVirtualFunction<bool>("IsNotFirst").Implement(f => {
                    f.Return(enumField != TestFlags.First);
                })
                .NewVirtualFunction<bool>("IsNotSecond").Implement(f => {
                    f.Return(enumField != TestFlags.Second);
                });

            //-- act

            dynamic obj1 = CreateClassInstanceAs<object>().UsingConstructor(TestFlags.First);
            dynamic obj2 = CreateClassInstanceAs<object>().UsingConstructor(TestFlags.Second);

            bool[] results1 = new bool[] { obj1.IsFirst(), obj1.IsNotFirst(), obj1.IsSecond(), obj1.IsNotSecond() };
            bool[] results2 = new bool[] { obj2.IsFirst(), obj2.IsNotFirst(), obj2.IsSecond(), obj2.IsNotSecond() };

            //-- assert

            Assert.That(results1, Is.EqualTo(new bool[] { true, false, false, true }));
            Assert.That(results2, Is.EqualTo(new bool[] { false, true, true, false }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test, Category("Benchmark")]
        public void BenchmarkCompiledEnumHasFlag()
        {
            TestFlags flags = TestFlags.First;

            var clock1 = Stopwatch.StartNew();
            for ( int i = 0 ; i < 1000000 ; i++ )
            {
                if ( ((int)flags & 1) != 0 )
                {
                    flags = (TestFlags)2;
                }
                else if ( ((int)flags & 2) != 0 )
                {
                    flags = (TestFlags)1;
                }
            }
            Console.WriteLine(clock1.Elapsed);
            
            flags = TestFlags.First;
            
            var clock2 = Stopwatch.StartNew();
            for ( int i = 0; i < 1000000; i++ )
            {
                if ( flags.HasFlag(TestFlags.First) )
                {
                    flags = (TestFlags)2;
                }
                else if ( flags.HasFlag(TestFlags.Second) )
                {
                    flags = (TestFlags)1;
                }
            }
            Console.WriteLine(clock2.Elapsed);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test, Category("Benchmark")]
        public void BenchmarkDynamicEnumHasFlag()
        {
            DeriveClassFrom<object>()
                .DefaultConstructor()
                .NewVirtualVoidMethod("DoTest").Implement(m => {
                    var flags = m.Local<TestFlags>(initialValueConst: TestFlags.First);
                    var clock = m.Local<Stopwatch>(initialValue: Static.Func(Stopwatch.StartNew));
                    m.For(from: m.Const(0), to: m.Const(1000000)).Do((loop, i) => {
                        m.If(flags.EnumHasFlag(TestFlags.First)).Then(() => {
                            flags.Assign(TestFlags.Second);
                        })                
                        .ElseIf(flags.EnumHasFlag(TestFlags.Second)).Then(() => {
                            flags.Assign(TestFlags.First);
                        });   
                    });
                    Static.Void<object>(Console.WriteLine, clock.Prop<TimeSpan>(x => x.Elapsed).CastTo<object>());
                });

            dynamic obj = CreateClassInstanceAs<object>().UsingDefaultConstructor();
            obj.DoTest();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void CompiledExample()
        {
            var x = TestFlags.First;

            if ( x.HasFlag(TestFlags.Second) )
            {
                x &= ~TestFlags.First;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public enum TestFlags
        {
            First = 0x01,
            Second = 0x02
        }
    }
}
