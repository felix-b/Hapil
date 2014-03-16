using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression

namespace Happil.UnitTests
{
	[TestFixture]
	public class DecorationImplementorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanImplementDecoratorFromScratch()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => {})
				.Method<int>(x => x.Three).Implement(m => m.ReturnConst(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.ReturnConst(456))
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("EXCEPTION-5")
				.DecorateWith<FromScratchActionLogAspect>()
			);
			DefineClassByKey(new HappilTypeKey(primaryInterface: typeof(AncestorRepository.IFewMethods)));

			ActionLog = new List<string>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			obj.One();
			var result4 = obj.Four("ABC");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "EXCEPTION-5");

			//-- Assert

			Assert.That(result4, Is.EqualTo(456));
			Assert.That(ActionLog, Is.EqualTo(new[] { 
				"1-ON-BEFORE:One", "1-ON-RETURN-VOID:One", "1-ON-SUCCESS:One", "1-ON-AFTER:One",
				"1-ON-BEFORE:Four", "1-ON-RETURN-VALUE:Four=456", "1-ON-SUCCESS:Four", "1-ON-AFTER:Four",
				"1-ON-BEFORE:Five", "1-ON-ERROR:Five=EXCEPTION-5", "1-ON-AFTER:Five"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanCombineTwoDecoratorsInPipeline()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => { })
				.Method<int>(x => x.Three).Implement(m => m.ReturnConst(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.ReturnConst(456))
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("EXCEPTION-5")
				.DecorateWith(new FromScratchActionLogAspect(logPrefix: "INNER"))
				.DecorateWith(new FromScratchActionLogAspect(logPrefix: "OUTER"))
			);
			DefineClassByKey(new HappilTypeKey(primaryInterface: typeof(AncestorRepository.IFewMethods)));

			ActionLog = new List<string>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			obj.One();
			var result4 = obj.Four("ABC");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "EXCEPTION-5");

			//-- Assert

			Assert.That(result4, Is.EqualTo(456));
			Assert.That(ActionLog, Is.EqualTo(new[] { 
				"OUTER-ON-BEFORE:One", 
					"INNER-ON-BEFORE:One", "INNER-ON-RETURN-VOID:One", "INNER-ON-SUCCESS:One", "INNER-ON-AFTER:One",
				"OUTER-ON-RETURN-VOID:One", "OUTER-ON-SUCCESS:One", "OUTER-ON-AFTER:One",
				"OUTER-ON-BEFORE:Four", 
					"INNER-ON-BEFORE:Four", "INNER-ON-RETURN-VALUE:Four=456", "INNER-ON-SUCCESS:Four", "INNER-ON-AFTER:Four",
				"OUTER-ON-RETURN-VALUE:Four=456", "OUTER-ON-SUCCESS:Four", "OUTER-ON-AFTER:Four",
				"OUTER-ON-BEFORE:Five", 
					"INNER-ON-BEFORE:Five", "INNER-ON-ERROR:Five=EXCEPTION-5", "INNER-ON-AFTER:Five",
				"OUTER-ON-ERROR:Five=EXCEPTION-5", "OUTER-ON-AFTER:Five"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanDeriveImplementorFromBase()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => { })
				.Method<int>(x => x.Three).Implement(m => m.ReturnConst(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.ReturnConst(456))
				.Method<int, string>(x => x.Five).Implement((m, n) => 
					m.Switch(n)
						.Case(0).Do(() => m.Throw<InvalidOperationException>("E0"))
						.Case(1).Do(() => m.Throw<ExceptionRepository.TestExceptionOne>("E1"))
						.Default(() => m.Return(n.Func<string>(x => x.ToString)))
				)
				.DecorateWith<BasedActionLogAspect>()
			);
			DefineClassByKey(new HappilTypeKey(primaryInterface: typeof(AncestorRepository.IFewMethods)));

			ActionLog = new List<string>();

			//-- Act

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingDefaultConstructor();

			obj.One();
			var resultFour = obj.Four("ABC");
			ExpectException<InvalidOperationException>(() => obj.Five(0), "E0");
			var resultFiveOf1 = obj.Five(1);
			var resultFiveOf123 = obj.Five(123);

			//-- Assert

			Assert.That(resultFour, Is.EqualTo(456));
			Assert.That(resultFiveOf1, Is.Null);
			Assert.That(resultFiveOf123, Is.EqualTo("123"));
			
			Assert.That(ActionLog, Is.EqualTo(new[] { 
				"ON-BEFORE:One", "ON-RETURN-VOID:One", "ON-SUCCESS:One", "ON-AFTER:One",
				"ON-BEFORE:Four", "ON-RETURN-VALUE:Four=456", "ON-SUCCESS:Four", "ON-AFTER:Four",
				"ON-BEFORE:Five", "ON-EXCEPTION-UNKNOWN:Five=InvalidOperationException:E0", "ON-FAILURE:Five", "ON-AFTER:Five",
				"ON-BEFORE:Five", "ON-EXCEPTION-ONE:Five=E1", "ON-FAILURE:Five", "ON-AFTER:Five",
				"ON-BEFORE:Five", "ON-RETURN-VALUE:Five=123", "ON-SUCCESS:Five", "ON-AFTER:Five"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static List<string> ActionLog { get; set; }
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class FromScratchActionLogAspect : IDecorationImplementor
		{
			private readonly string m_LogPrefix;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public FromScratchActionLogAspect() : this("1")
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public FromScratchActionLogAspect(string logPrefix)
			{
				m_LogPrefix = logPrefix;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IAspectImplementor Members

			public void ImplementDecoration<TBase>(IHappilClassBody<TBase> classDefinition)
			{
				classDefinition.AsBase<TypeTemplate.TPrimary>().AllMethods().Decorate(
					attributes: null,
					body: m => {
						Static.Prop(() => ActionLog).Add(m.Const(m_LogPrefix + "-ON-BEFORE:" + m.MethodInfo.Name));

						m.Try(() => {
							var returnValueExpression = m.Proceed();

							if ( !object.ReferenceEquals(returnValueExpression, null) )
							{
								var returnValue = m.Local<TypeTemplate.TReturn>(initialValue: returnValueExpression);
								Static.Prop(() => ActionLog).Add(m.Const(m_LogPrefix + "-ON-RETURN-VALUE:" + m.MethodInfo.Name + "=") + returnValue.Func<string>(x => x.ToString));
								Static.Prop(() => ActionLog).Add(m.Const(m_LogPrefix + "-ON-SUCCESS:" + m.MethodInfo.Name));
								m.Return(returnValue);
							}
							else
							{
								Static.Prop(() => ActionLog).Add(m.Const(m_LogPrefix + "-ON-RETURN-VOID:" + m.MethodInfo.Name));
								Static.Prop(() => ActionLog).Add(m.Const(m_LogPrefix + "-ON-SUCCESS:" + m.MethodInfo.Name));
							}
						})
						.Catch<Exception>(e => {
							Static.Prop(() => ActionLog).Add(m.Const(m_LogPrefix + "-ON-ERROR:" + m.MethodInfo.Name + "=") + e.Prop(x => x.Message));
							m.Throw();
						})
						.Finally(() => {
							Static.Prop(() => ActionLog).Add(m.Const(m_LogPrefix + "-ON-AFTER:" + m.MethodInfo.Name));
						});
					}
				);
			}

			#endregion
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BasedActionLogAspect : DecorationImplementorBase
		{
			#region Overrides of DecorationImplementorBase

			public override void OnMethod(MethodInfo info, MethodDecorationBuilder decoration)
			{
				decoration
					.OnBefore(m => 
						Static.Prop(() => ActionLog).Add(m.Const("ON-BEFORE:" + info.Name))
					)
					.OnReturnValue((m, retVal) =>
						Static.Prop(() => ActionLog).Add(m.Const("ON-RETURN-VALUE:" + info.Name + "=") + retVal.Func<string>(x => x.ToString))
					)
					.OnReturnVoid(m =>
						Static.Prop(() => ActionLog).Add(m.Const("ON-RETURN-VOID:" + info.Name))
					)
					.OnSuccess(m =>
						Static.Prop(() => ActionLog).Add(m.Const("ON-SUCCESS:" + info.Name))
					)
					.OnException<ExceptionRepository.TestExceptionOne>((m, e) => {
						Static.Prop(() => ActionLog).Add(m.Const("ON-EXCEPTION-ONE:" + m.MethodInfo.Name + "=") + e.Prop(x => x.Message));
						if ( !m.MethodInfo.IsVoid() )
						{
							m.Return(m.Default<TypeTemplate.TReturn>());
						}
					})
					.OnException<Exception>((m, e) => {
						Static.Prop(() => ActionLog).Add(
							m.Const("ON-EXCEPTION-UNKNOWN:" + m.MethodInfo.Name + "=") + 
							e.Func<Type>(x => x.GetType).Prop<string>(x => x.Name) + 
							m.Const(":") + 
							e.Prop(x => x.Message));
						m.Throw();
					})
					.OnFailure(m => 
						Static.Prop(() => ActionLog).Add(m.Const("ON-FAILURE:" + info.Name))
					)
					.OnAfter(m => 
						Static.Prop(() => ActionLog).Add(m.Const("ON-AFTER:" + info.Name))
					);
			}

			#endregion
		}
	}
}

// ReSharper restore ConvertToLambdaExpression
