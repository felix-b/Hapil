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
	public class DecorationImplementorTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void SingleActionLogAspect()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => {})
				.Method<int>(x => x.Three).Implement(m => m.ReturnConst(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.ReturnConst(456))
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("EXCEPTION-5")
				.DecorateWith<ActionLogAspect>()
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
		public void TwoActionLogAspects()
		{
			//-- Arrange

			OnDefineNewClass(key => DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => { })
				.Method<int>(x => x.Three).Implement(m => m.ReturnConst(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.ReturnConst(456))
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("EXCEPTION-5")
				.DecorateWith(new ActionLogAspect(logPrefix: "INNER"))
				.DecorateWith(new ActionLogAspect(logPrefix: "OUTER"))
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

		public static List<string> ActionLog { get; set; }
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ActionLogAspect : IDecorationImplementor
		{
			private readonly string m_LogPrefix;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ActionLogAspect() : this("1")
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public ActionLogAspect(string logPrefix)
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
							var decoratedReturnValue = m.Proceed();

							if ( !object.ReferenceEquals(decoratedReturnValue, null) )
							{
								var returnValue = m.Local<TypeTemplate.TReturn>(initialValue: decoratedReturnValue);
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
	}
}

// ReSharper restore ConvertToLambdaExpression
