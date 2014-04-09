using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Happil.Decorators;
using Happil.Expressions;
using Happil.Members;
using Happil.Writers;
using NUnit.Framework;

namespace Happil.UnitTests
{
	[TestFixture]
	public class DecorationTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void ManualDecorator()
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
		public void ManualDecorator_TwoLevelNesting()
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
		public void ManualDecorator_ThreeLevelNesting()
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

		[Test]
		public void DecoratorBuilder_Methods()
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
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("EOne");
			
			implementor.DecorateWith(new LoggingDecorator(logPrefix: ""));

			//-- Act

			var actionLog = new List<string>();
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(actionLog);

			obj.One();
			var result4 = obj.Four("ABC");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "EOne");

			//-- Assert

			Assert.That(result4, Is.EqualTo(456));
			Assert.That(
				actionLog,
				Is.EqualTo(new[] {
					"BEFORE:One", "RETVOID:One", "SUCCESS:One", "AFTER:One", 
					"BEFORE:Four", "RETVAL:Four=456", "SUCCESS:Four", "AFTER:Four", 
					"BEFORE:Five", "EXCEPTION-ONE:Five=EOne", "FAILURE:Five", "AFTER:Five"
				}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DecoratorBuilder_Methods_TwoLevelNesting()
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
				.Method<int, string>(x => x.Five).Throw<ExceptionRepository.TestExceptionOne>("EOne");

			implementor.DecorateWith(new LoggingDecorator(logPrefix: "INNER-"));
			implementor.DecorateWith(new LoggingDecorator(logPrefix: "OUTER-"));

			//-- Act

			var actionLog = new List<string>();
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(actionLog);

			obj.One();
			var result4 = obj.Four("ABC");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "EOne");

			//-- Assert

			Assert.That(result4, Is.EqualTo(456));
			Assert.That(
				actionLog,
				Is.EqualTo(new[] {
					"OUTER-BEFORE:One", 
						"INNER-BEFORE:One", "INNER-RETVOID:One", "INNER-SUCCESS:One", "INNER-AFTER:One", 
					"OUTER-RETVOID:One", "OUTER-SUCCESS:One", "OUTER-AFTER:One", 

					"OUTER-BEFORE:Four", 
						"INNER-BEFORE:Four", "INNER-RETVAL:Four=456", "INNER-SUCCESS:Four", "INNER-AFTER:Four", 
					"OUTER-RETVAL:Four=456", "OUTER-SUCCESS:Four", "OUTER-AFTER:Four", 
		
					"OUTER-BEFORE:Five", 
						"INNER-BEFORE:Five", "INNER-EXCEPTION-ONE:Five=EOne", "INNER-FAILURE:Five", "INNER-AFTER:Five",
					"OUTER-EXCEPTION-ONE:Five=EOne", "OUTER-FAILURE:Five", "OUTER-AFTER:Five"
				}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DecoratorBuilder_Properties()
		{
			//-- Arrange

			FieldAccessOperand<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.Field<List<string>>("m_Log", out logField)
				.Constructor<List<string>>((w, log) => logField.Assign(log))
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementAutomatic();

			implementor.DecorateWith(new LoggingDecorator(logPrefix: ""));

			//-- Act

			var actionLog = new List<string>();
			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingConstructor(actionLog);

			obj.AnInt = 123;
			var intResult = obj.AnInt;

			obj.AString = "ABC";
			var stringResult = obj.AString;

			//-- Assert

			Assert.That(intResult, Is.EqualTo(123));
			Assert.That(stringResult, Is.EqualTo("ABC"));

			Assert.That(
				actionLog,
				Is.EqualTo(new[] {
					"BEFORE-SET:AnInt=123", "AFTER-SET:AnInt", 
					"BEFORE-GET:AnInt", "AFTER-GET:AnInt=123", 
					"BEFORE-SET:AString=ABC", "AFTER-SET:AString", 
					"BEFORE-GET:AString", "AFTER-GET:AString=ABC", 
				}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DecoratorBuilder_Events()
		{
			//-- Arrange

			FieldAccessOperand<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.PrimaryConstructor("Log", out logField)
				.ImplementInterface<AncestorRepository.IFewEvents>()
				.AllMethods().ImplementEmpty()
				.AllEvents().ImplementAutomatic();

			implementor.DecorateWith(new LoggingDecorator(logPrefix: ""));

			//-- Act

			var actionLog = new List<string>();
			var obj = CreateClassInstanceAs<AncestorRepository.IFewEvents>().UsingConstructor(actionLog);

			obj.EventOne += (s, e) => {};
			obj.EventTwo -= (s, e) => { };

			//-- Assert

			Assert.That(
				actionLog,
				Is.EqualTo(new[] {
					"BEFORE-ADD:EventOne", "AFTER-ADD:EventOne", 
					"BEFORE-REMOVE:EventTwo", "AFTER-REMOVE:EventTwo" 
				}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DecoratorBuilder_Fields()
		{
			//-- Arrange

			FieldAccessOperand<int> number;
			FieldAccessOperand<string> text;
			FieldAccessOperand<List<string>> log;

			var implementor = DeriveClassFrom<object>()
				.PrimaryConstructor("Number", out number, "Text", out text, "Log", out log);

			implementor.DecorateWith(new LoggingDecorator(logPrefix: ""));

			//-- Act

			var obj = CreateClassInstanceAs<object>().UsingConstructor(123, "ABC", new List<string>());
			var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			var attributes = fields.Select(f => f.GetCustomAttributes(typeof(AttributeTests.TestAttributeOne))
				.OfType<AttributeTests.TestAttributeOne>()
				.Single())
				.ToArray();

			//-- Assert

			Assert.That(
				fields.Select(f => f.Name).ToArray(), 
				Is.EquivalentTo(new[] { "m_Number", "m_Text", "m_Log" }));
			Assert.That(
				attributes.Select(a => a.StringValue).ToArray(), 
				Is.EquivalentTo(new[] { "Number", "Text", "Log" }));
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class LoggingDecorator : ClassDecoratorBase
		{
			private readonly string m_LogPrefix;
			private FieldAccessOperand<List<string>> m_Log;

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public LoggingDecorator(string logPrefix)
			{
				m_LogPrefix = logPrefix;
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public override void OnClassType(ClassType classType, ClassWriterBase writer)
			{
				var actionLogField = classType.GetAllMembers().OfType<FieldMember>().Single(f => !f.IsStatic && f.FieldType == typeof(List<string>));
				m_Log = actionLogField.AsOperand<List<string>>();
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public override void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
			{
				decorate()
					.OnBefore(w => 
						m_Log.Add(w.Const(m_LogPrefix + "BEFORE:" + member.Name))
					)
					.OnReturnValue((w, retVal) => 
						m_Log.Add(w.Const(m_LogPrefix + "RETVAL:" + member.Name + "=") + retVal.Func<string>(x => x.ToString))
					)
					.OnReturnVoid(w => 
						m_Log.Add(w.Const(m_LogPrefix + "RETVOID:" + member.Name))
					)
					.OnException<ExceptionRepository.TestExceptionOne>((w, e) => {
						m_Log.Add(w.Const(m_LogPrefix + "EXCEPTION-ONE:" + member.Name + "=") + e.Prop(x => x.Message));
						w.Throw();
					})
					.OnFailure(w =>
						m_Log.Add(w.Const(m_LogPrefix + "FAILURE:" + member.Name))
					)
					.OnSuccess(w =>
						m_Log.Add(w.Const(m_LogPrefix + "SUCCESS:" + member.Name))
					)
					.OnAfter(w =>
						m_Log.Add(w.Const(m_LogPrefix + "AFTER:" + member.Name))
					);
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public override void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
			{
				if ( member.HasGetter )
				{
					decorate().Getter()
						.OnBefore(w => 
							m_Log.Add(w.Const(m_LogPrefix + "BEFORE-GET:" + member.Name))
						)
						.OnReturnValue((w, retVal) => 
							m_Log.Add(w.Const(m_LogPrefix + "AFTER-GET:" + member.Name + "=") + retVal.Func<string>(x => x.ToString))
						);
				}
				
				if ( member.HasSetter )
				{
					decorate().Setter()
						.OnBefore(w =>
							m_Log.Add(w.Const(m_LogPrefix + "BEFORE-SET:" + member.Name + "=") + w.Arg1<TypeTemplate.TProperty>().Func<string>(x => x.ToString))
						)
						.OnAfter(w =>
							m_Log.Add(w.Const(m_LogPrefix + "AFTER-SET:" + member.Name))
						);
				}
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public override void OnEvent(EventMember member, Func<EventDecorationBuilder> decorate)
			{
				decorate().AddOn()
					.OnBefore(w =>  
						m_Log.Add(w.Const(m_LogPrefix + "BEFORE-ADD:" + member.Name))
					)
					.OnAfter(w =>  
						m_Log.Add(w.Const(m_LogPrefix + "AFTER-ADD:" + member.Name))
					);

				decorate().RemoveOn()
					.OnBefore(w =>
						m_Log.Add(w.Const(m_LogPrefix + "BEFORE-REMOVE:" + member.Name))
					)
					.OnAfter(w =>
						m_Log.Add(w.Const(m_LogPrefix + "AFTER-REMOVE:" + member.Name))
					);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void OnField(FieldMember member, Func<FieldDecorationBuilder> decorate)
			{
				decorate().Attribute<AttributeTests.TestAttributeOne>(a => a.Named(x => x.StringValue, member.Name.TrimPrefix("m_")));
			}
		}
	}
}
