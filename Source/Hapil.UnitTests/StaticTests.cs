using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using NUnit.Framework;
using TT = Hapil.TypeTemplate;

namespace Hapil.UnitTests
{
    [TestFixture]
    public class StaticTests : NUnitEmittedTypesTestBase
    {
        [Test]
        public void CanBuildLinqExpressionTree()
        {
            //-- Arrange

            using ( TT.CreateScope<TT.TImpl, TT.TValue>(typeof(AncestorRepository.OperatorInputOutput), typeof(int)) )
            {
                DeriveClassFrom<object>()
                    .DefaultConstructor()
                    .NewVirtualFunction<string, Expression<Func<TT.TImpl, TT.TValue>>>("BuildPropertyLambda").Implement((m, propertyName) => {
                        var parameterLocal = m.Local<ParameterExpression>();
                        parameterLocal.Assign(Static.Func(Expression.Parameter, m.Const(TT.Resolve<TT.TImpl>()), m.Const("p")));

                        var propertyGetter = m.Local<MethodInfo>();
                        propertyGetter.Assign(
                            m.Const(TT.Resolve<TT.TImpl>())
                            .Func<string, PropertyInfo>(t => t.GetProperty, propertyName)
                            .Func<MethodInfo>(pi => pi.GetGetMethod));

                        m.Return(Static.Func<Expression, ParameterExpression[], Expression<Func<TT.TImpl, TT.TValue>>>(
                            Expression.Lambda<Func<TT.TImpl, TT.TValue>>, 
                            Static.Func<Expression, MethodInfo, MemberExpression>(
                                Expression.Property, 
                                parameterLocal, 
                                propertyGetter
                            ),
                            m.NewArray<ParameterExpression>(values: parameterLocal)
                        ));
                    });
            }

            //-- Act

            dynamic obj = CreateClassInstanceAs<object>().UsingDefaultConstructor();
            var lambda = (Expression<Func<AncestorRepository.OperatorInputOutput, int>>)obj.BuildPropertyLambda("IntValue");

            //-- Assert

            Assert.That(lambda.ToString(), Is.EqualTo("p => p.IntValue"));
        }
    }
}
