using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression

namespace Happil.UnitTests
{
	[TestFixture]
	public class AspectPipelineTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanImplementSingleAspectWithCallback()
		{
			//-- Arrange

			OnDefineNewClass(key => HappilFactoryBase.ImplementAspects(
				Module.DeriveClassFrom<object>(TestCaseClassName), 
				key,
				ImplementActionLogAspect1)
			);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static List<string> ActionLog { get; set; }
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void ImplementActionLogAspect1(IHappilClassBody<object> classDefinition, HappilTypeKey key)
		{
			ImplementPrefixedActionLogAspect("1-", classDefinition, key);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void ImplementActionLogAspect2(IHappilClassBody<object> classDefinition, HappilTypeKey key)
		{
			ImplementPrefixedActionLogAspect("2-", classDefinition, key);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void ImplementPrefixedActionLogAspect(string logPrefix, IHappilClassBody<object> classDefinition, HappilTypeKey key)
		{
			classDefinition.AsBase<TypeTemplate.TPrimary>().AllMethods().Extend(
				attributes: null,
				body: m => {
					Static.Prop(() => ActionLog).Add(m.Const(logPrefix + "ON-BEFORE:" + m.MethodInfo.Name));
					
					m.Try(() => {
						if ( m.MethodInfo.IsVoid() )
						{
							m.Proceed();
							Static.Prop(() => ActionLog).Add(m.Const(logPrefix + "ON-RETURN:" + m.MethodInfo.Name));
							Static.Prop(() => ActionLog).Add(m.Const(logPrefix + "ON-SUCCESS:" + m.MethodInfo.Name));
						}
						else
						{
							var returnValue = m.Local<TypeTemplate.TReturn>();
							returnValue.Assign(m.Proceed());
							Static.Prop(() => ActionLog).Add(m.Const(logPrefix + "ON-RETURN-VALUE:" + m.MethodInfo.Name + "=") + returnValue.Func<string>(x => x.ToString));
							Static.Prop(() => ActionLog).Add(m.Const(logPrefix + "ON-SUCCESS:" + m.MethodInfo.Name));
							m.Return(returnValue);
						}
					})
					.Catch<Exception>(e => {
						Static.Prop(() => ActionLog).Add(m.Const(logPrefix + "ON-ERROR:" + m.MethodInfo.Name + "=") + e.Prop(x => x.Message));
					})
					.Finally(() => {
						Static.Prop(() => ActionLog).Add(m.Const(logPrefix + "ON-AFTER:" + m.MethodInfo.Name));
					});
				}
			);
		}
	}
}

// ReSharper restore ConvertToLambdaExpression
