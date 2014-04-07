using System;
using System.Collections.Generic;
using Happil.Expressions;
using Happil.Members;
using Happil.Writers;
using NUnit.Framework;
// ReSharper disable ConvertToLambdaExpression

namespace Happil.UnitTests.Writers
{
	[TestFixture]
	public class DecorationClassWriterTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void CanManuallyCreateLoggingDecorator()
		{
			//-- Arrange

			FieldAccessOperand<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.Field<List<string>>("m_Log", out logField)
				.Constructor<List<string>>((w, log) => logField.Assign(log))
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => { })
				.Method<int>(x => x.Three).Implement(m => m.Return(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.Return(456))
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("TEST-EXCEPTION");

			ManuallyImplementLoggingDecorator(implementor, logField);

			//-- Act

			var actionLog = new List<string>();
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(actionLog);

			obj.One();
			var result4 = obj.Four("ABC");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "TEST-EXCEPTION");

			//-- Assert

			Assert.That(result4, Is.EqualTo(456));
			Assert.That(actionLog, Is.EqualTo(new[] { 
				"One:BEFORE", "One:AFTER",
				"Four:BEFORE", "Four:RETVAL=456", "Four:AFTER",
				"Five:BEFORE", "Five:ERROR=TEST-EXCEPTION", "Five:AFTER"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanManuallyCreateTwoNestedLoggingDecorators()
		{
			//-- Arrange

			FieldAccessOperand<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.Field<List<string>>("m_Log", out logField)
				.Constructor<List<string>>((w, log) => logField.Assign(log))
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => { })
				.Method<int>(x => x.Three).Implement(m => m.Return(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.Return(456))
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("TEST-EXCEPTION");

			ManuallyImplementLoggingDecorator(implementor, logField, logSuffix: "-1");
			ManuallyImplementLoggingDecorator(implementor, logField, logSuffix: "-2");

			//-- Act

			var actionLog = new List<string>();
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(actionLog);

			obj.One();
			var result4 = obj.Four("ABC");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "TEST-EXCEPTION");

			//-- Assert

			Assert.That(result4, Is.EqualTo(456));
			Assert.That(actionLog, Is.EqualTo(new[] { 
				#region One
				"One:BEFORE-2", 
					"One:BEFORE-1", 
					"One:AFTER-1", 
				"One:AFTER-2", 
				#endregion
				#region Four
				"Four:BEFORE-2", 
					"Four:BEFORE-1", 
					"Four:RETVAL-1=456", 
					"Four:AFTER-1",
				"Four:RETVAL-2=456", 
				"Four:AFTER-2",
				#endregion
				#region Five
				"Five:BEFORE-2", 
					"Five:BEFORE-1", 
					"Five:ERROR-1=TEST-EXCEPTION", 
					"Five:AFTER-1",
				"Five:ERROR-2=TEST-EXCEPTION", 
				"Five:AFTER-2"
				#endregion
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CanManuallyCreateThreeNestedLoggingDecorators()
		{
			//-- Arrange

			FieldAccessOperand<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.Field<List<string>>("m_Log", out logField)
				.Constructor<List<string>>((w, log) => logField.Assign(log))
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods(where: m => m.IsVoid()).Implement(m => { })
				.Method<int>(x => x.Three).Implement(m => m.Return(123))
				.Method<string, int>(x => x.Four).Implement((m, s) => m.Return(456))
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("TEST-EXCEPTION");

			ManuallyImplementLoggingDecorator(implementor, logField, logSuffix: "-1");
			ManuallyImplementLoggingDecorator(implementor, logField, logSuffix: "-2");
			ManuallyImplementLoggingDecorator(implementor, logField, logSuffix: "-3");

			//-- Act

			var actionLog = new List<string>();
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(actionLog);

			obj.One();
			var result4 = obj.Four("ABC");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "TEST-EXCEPTION");

			//-- Assert

			Assert.That(result4, Is.EqualTo(456));
			Assert.That(actionLog, Is.EqualTo(new[] { 
				#region One
				"One:BEFORE-3", 
					"One:BEFORE-2", 
						"One:BEFORE-1", 
						"One:AFTER-1", 
					"One:AFTER-2", 
				"One:AFTER-3",
				#endregion
				#region Four
				"Four:BEFORE-3", 
					"Four:BEFORE-2", 
						"Four:BEFORE-1", 
						"Four:RETVAL-1=456", 
						"Four:AFTER-1",
					"Four:RETVAL-2=456", 
					"Four:AFTER-2",
				"Four:RETVAL-3=456", 
				"Four:AFTER-3",
				#endregion
				#region Five
				"Five:BEFORE-3", 
					"Five:BEFORE-2", 
						"Five:BEFORE-1", 
						"Five:ERROR-1=TEST-EXCEPTION", 
						"Five:AFTER-1",
					"Five:ERROR-2=TEST-EXCEPTION", 
					"Five:AFTER-2",
				"Five:ERROR-3=TEST-EXCEPTION", 
				"Five:AFTER-3"
				#endregion
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void ManuallyImplementLoggingDecorator(
			ImplementationClassWriter<AncestorRepository.IFewMethods> implementor, 
			FieldAccessOperand<List<string>> logField,
			string logSuffix = "")
		{
			var decorator = new ImplementationClassWriter<AncestorRepository.IFewMethods>(implementor.OwnerClass);
			decorator.AllMethods().ForEachMember(method => new TemplateMethodWriter(method, MethodWriterModes.Decorator, w => {
				logField.Add(w.Const(w.OwnerMethod.Name + ":BEFORE" + logSuffix));
				w.Try(() => {
					var retVal = w.Proceed<TypeTemplate.TReturn>();
					if ( !method.Signature.IsVoid )
					{
						logField.Add(w.Const(w.OwnerMethod.Name + ":RETVAL" + logSuffix + "=") + retVal.Func<string>(x => x.ToString));
						w.Return(retVal);
					}
				}).Catch<Exception>(e => {
					logField.Add(w.Const(w.OwnerMethod.Name + ":ERROR" + logSuffix + "=") + e.Prop(x => x.Message));
					w.Throw();
				}).Finally(() => {
					logField.Add(w.Const(w.OwnerMethod.Name + ":AFTER" + logSuffix));
				});
			}));
		}
	}
}
