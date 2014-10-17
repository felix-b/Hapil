using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using Hapil.Decorators;
using Hapil.Expressions;
using Hapil.Members;
using Hapil.Operands;
using Hapil.Writers;
using NUnit.Framework;

namespace Hapil.UnitTests
{
	[TestFixture]
	public class DecorationTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void ManualDecorator()
		{
			//-- Arrange

			Field<List<string>> logField;

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

			Field<List<string>> logField;

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

			Field<List<string>> logField;

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

			Field<List<string>> logField;

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

			Field<List<string>> logField;

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

			Field<List<string>> logField;

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

			Field<List<string>> logField;

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

			Field<int> number;
			Field<string> text;
			Field<List<string>> log;

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

		[Test]
		public void DecoratorBuilder_PropagateCalls_Methods()
		{
			//-- Arrange

			Field<AncestorRepository.IFewMethods> targetField;
			Field<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.PrimaryConstructor("Target", out targetField, "Log", out logField)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods().ImplementPropagate(targetField);
				
			implementor.DecorateWith(new LoggingDecorator(logPrefix: "OUTER-"));

			//-- Act

			var log = new List<string>();
			var target = new TestTarget(log);
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(target, log);

			obj.One();
			var result4 = obj.Four("ZZZ");
			ExpectException<ExceptionRepository.TestExceptionOne>(() => obj.Five(999), "EOne");

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] {
				"OUTER-BEFORE:One", 
					"TEST-One",
				"OUTER-RETVOID:One", "OUTER-SUCCESS:One", "OUTER-AFTER:One", 
				"OUTER-BEFORE:Four", 
					"TEST-Four=ZZZ", 
				"OUTER-RETVAL:Four=123", "OUTER-SUCCESS:Four", "OUTER-AFTER:Four",
				"OUTER-BEFORE:Five", 
					"TEST-Five=999", 
				"OUTER-EXCEPTION-ONE:Five=EOne", "OUTER-FAILURE:Five", "OUTER-AFTER:Five"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DecoratorBuilder_PropagateCalls_Properties()
		{
			//-- Arrange

			Field<AncestorRepository.IFewReadWriteProperties> targetField;
			Field<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.PrimaryConstructor("Target", out targetField, "Log", out logField)
				.ImplementInterface<AncestorRepository.IFewReadWriteProperties>()
				.AllProperties().ImplementPropagate(targetField);

			implementor.DecorateWith(new LoggingDecorator(logPrefix: ""));

			//-- Act

			var log = new List<string>();
			var target = new TestTarget(log);
			var obj = CreateClassInstanceAs<AncestorRepository.IFewReadWriteProperties>().UsingConstructor(target, log);

			obj.AnInt = 123;
			obj.AString = "ABC";

			var anIntResult = obj.AnInt;
			var aStringResult = obj.AString;

			//-- Assert

			Assert.That(anIntResult, Is.EqualTo(123));
			Assert.That(aStringResult, Is.EqualTo("ABC"));

			Assert.That(log, Is.EqualTo(new[] {
				"BEFORE-SET:AnInt=123", "AFTER-SET:AnInt", 
				"BEFORE-SET:AString=ABC", "AFTER-SET:AString", 
				"BEFORE-GET:AnInt", "AFTER-GET:AnInt=123", 
				"BEFORE-GET:AString", "AFTER-GET:AString=ABC" 
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void DecoratorBuilder_PropagateCalls_Events()
		{
			//-- Arrange

			Field<AncestorRepository.IFewEvents> targetField;
			Field<List<string>> logField;

			var implementor = DeriveClassFrom<object>()
				.PrimaryConstructor("Target", out targetField, "Log", out logField)
				.ImplementInterface<AncestorRepository.IFewEvents>()
				.AllMethods().ImplementPropagate(targetField)
				.AllEvents().ImplementPropagate(targetField);

			implementor.DecorateWith(new LoggingDecorator(logPrefix: ""));
			implementor.DecorateWith<EventInterceptingDecorator>();

			//-- Act

			var log = new List<string>();
			var target = new TestTarget(log);
			var obj = CreateClassInstanceAs<AncestorRepository.IFewEvents>().UsingConstructor(target, log);

			var handlerOneA = new EventHandler((sender, args) => log.Add("EventOne.A:" + args.ToString()));
			var handlerOneB = new EventHandler((sender, args) => log.Add("EventOne.B:" + args.ToString()));
			var handlerTwoA = new EventHandler<AncestorRepository.InOutEventArgs>((sender, args) => {
				log.Add("EventTwo.A:" + args.InputValue);
				args.OutputValue = "AAA";
			});
			var handlerTwoB = new EventHandler<AncestorRepository.InOutEventArgs>((sender, args) => {
				log.Add("EventTwo.B:" + args.InputValue);
				args.OutputValue = "BBB";
			});

			obj.EventOne += handlerOneA;
			obj.EventOne += handlerOneB;
			obj.EventTwo += handlerTwoA;
			obj.EventTwo += handlerTwoB;

			obj.RaiseOne();
			var output1 = obj.RaiseTwo("INPUT1");

			obj.EventTwo -= handlerTwoB;
			var output2 = obj.RaiseTwo("INPUT2");

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] {
				"BEFORE-ADD:EventOne", "AFTER-ADD:EventOne",
				"BEFORE-ADD:EventOne", "AFTER-ADD:EventOne",
				"BEFORE-ADD:EventTwo", "AFTER-ADD:EventTwo",
				"BEFORE-ADD:EventTwo", "AFTER-ADD:EventTwo",
				"BEFORE:RaiseOne", 
					"BEFORE-RAISE:EventOne=System.EventArgs", "EventOne.A:System.EventArgs", "AFTER-RAISE:EventOne=System.EventArgs",
					"BEFORE-RAISE:EventOne=System.EventArgs", "EventOne.B:System.EventArgs", "AFTER-RAISE:EventOne=System.EventArgs",
				"RETVOID:RaiseOne", "SUCCESS:RaiseOne", "AFTER:RaiseOne",
				"BEFORE:RaiseTwo", 
					"BEFORE-RAISE:EventTwo=IN{INPUT1}/OUT{}", "EventTwo.A:INPUT1", "AFTER-RAISE:EventTwo=IN{INPUT1}/OUT{AAA}",
					"BEFORE-RAISE:EventTwo=IN{INPUT1}/OUT{AAA}", "EventTwo.B:INPUT1", "AFTER-RAISE:EventTwo=IN{INPUT1}/OUT{BBB}",
				"RETVAL:RaiseTwo=BBB", "SUCCESS:RaiseTwo", "AFTER:RaiseTwo",
				"BEFORE-REMOVE:EventTwo", "AFTER-REMOVE:EventTwo", 
				"BEFORE:RaiseTwo", 
					"BEFORE-RAISE:EventTwo=IN{INPUT2}/OUT{}", "EventTwo.A:INPUT2", "AFTER-RAISE:EventTwo=IN{INPUT2}/OUT{AAA}",
				"RETVAL:RaiseTwo=AAA", "SUCCESS:RaiseTwo", "AFTER:RaiseTwo"
			}));

			Assert.That(output1, Is.EqualTo("BBB"));
			Assert.That(output2, Is.EqualTo("AAA"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ConstructorInjection_SingleDependency()
		{
			//-- Arrange

			var implementor = DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods().ImplementEmpty();

			implementor.DecorateWith(new PropagatingDecorator());

			//-- Act

			var log = new List<string>();
			var target = new TestTarget(log);
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(target);

			obj.One();
			var result4 = obj.Four("ZZZ");

			//-- Assert

			Assert.That(log, Is.EqualTo(new[] {
				"TEST-One",
				"TEST-Four=ZZZ",
			}));
			Assert.That(result4, Is.EqualTo(123));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ConstructorInjection_MultipleDependencies()
		{
			//-- Arrange

			Field<AncestorRepository.IFewMethods> targetField;

			var implementor = DeriveClassFrom<object>()
				.PrimaryConstructor("Target", out targetField)
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods().ImplementPropagate(targetField);

			implementor.DecorateWith(new DayOfWeekDecorator());

			//-- Act

			var log = new List<string>();
			var target = new TestTarget(log);
			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(target, log, DayOfWeek.Friday);

			obj.One();
			var result4 = obj.Four("ZZZ");

			//-- Assert

			Assert.That(result4, Is.EqualTo(123));
			Assert.That(log, Is.EqualTo(new[] {
				"FRIDAY-BEFORE:One", "TEST-One", "FRIDAY-AFTER:One", 
				"FRIDAY-BEFORE:Four", "TEST-Four=ZZZ", "FRIDAY-AFTER:Four"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ConstructorInjection_MultipleOverlappingDependencies()
		{
			//-- Arrange

			var implementor = DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.AllMethods().ImplementEmpty();

			implementor.DecorateWith(new PropagatingDecorator());
			implementor.DecorateWith(new DayOfWeekDecorator());
			implementor.DecorateWith(new NumberDecorator());

			//-- Act

			List<string> log = new List<string>();
			AncestorRepository.IFewMethods target = new TestTarget(log);

			var obj = CreateClassInstanceAs<AncestorRepository.IFewMethods>().UsingConstructor(
				target, log, DayOfWeek.Monday, 12345, 
				constructorIndex: 0);
			
			obj.One();
			var result4 = obj.Four("ZZZ");

			//-- Assert

			Assert.That(result4, Is.EqualTo(123));
			Assert.That(log, Is.EqualTo(new[] {
				"12345-BEFORE:One", 
					"MONDAY-BEFORE:One", 
						"TEST-One", 
					"MONDAY-AFTER:One", 
				"12345-AFTER:One",
				"12345-BEFORE:Four", 
					"MONDAY-BEFORE:Four", 
						"TEST-Four=ZZZ", 
					"MONDAY-AFTER:Four", 
				"12345-AFTER:Four"
			}));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static void ManuallyImplementLoggingDecorator(
			ImplementationClassWriter<AncestorRepository.IFewMethods> implementor,
			Field<List<string>> logField,
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

		private class LoggingDecorator : DecorationConvention
		{
			private readonly string m_LogPrefix;
			private Field<List<string>> m_Log;

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public LoggingDecorator(string logPrefix) 
				: base(Will.DecorateMethods | Will.DecorateProperties | Will.DecorateEvents | Will.DecorateFields)
			{
				m_LogPrefix = logPrefix;
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnClass(ClassType classType, DecoratingClassWriter writer)
			{
				m_Log = BindToLogField(classType, writer);
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
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

			protected override void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
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

			protected override void OnEvent(EventMember member, Func<EventDecorationBuilder> decorate)
			{
				decorate().OnAdd()
					.OnBefore(w =>  
						m_Log.Add(w.Const(m_LogPrefix + "BEFORE-ADD:" + member.Name))
					)
					.OnAfter(w =>  
						m_Log.Add(w.Const(m_LogPrefix + "AFTER-ADD:" + member.Name))
					);

				decorate().OnRemove()
					.OnBefore(w =>
						m_Log.Add(w.Const(m_LogPrefix + "BEFORE-REMOVE:" + member.Name))
					)
					.OnAfter(w =>
						m_Log.Add(w.Const(m_LogPrefix + "AFTER-REMOVE:" + member.Name))
					);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnField(FieldMember member, Func<FieldDecorationBuilder> decorate)
			{
				decorate().Attribute<AttributeTests.TestAttributeOne>(a => a.Named(x => x.StringValue, member.Name.TrimPrefix("m_")));
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private Field<List<string>> BindToLogField(ClassType classType, ClassWriterBase writer)
			{
				var actionLogField = classType.GetAllMembers().OfType<FieldMember>().Single(f => !f.IsStatic && f.FieldType == typeof(List<string>));
				return actionLogField.AsOperand<List<string>>();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class DayOfWeekDecorator : DecorationConvention
		{
			private Field<List<string>> m_Log;
			private Field<DayOfWeek> m_DayOfWeek;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public DayOfWeekDecorator()
				: base(Will.DecorateMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnClass(ClassType classType, DecoratingClassWriter writer)
			{
				m_Log = writer.DependencyField<List<string>>("m_Log");
				m_DayOfWeek = writer.DependencyField<DayOfWeek>("m_DayOfWeek");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
			{
				decorate()
					.OnBefore(w => 
						m_Log.Add(m_DayOfWeek.Func<string>(x => x.ToString).ToUpper() + w.Const("-BEFORE:" + member.Name))
					)
					.OnAfter(w =>
						m_Log.Add(m_DayOfWeek.Func<string>(x => x.ToString).ToUpper() + w.Const("-AFTER:" + member.Name))
					);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class NumberDecorator : DecorationConvention
		{
			private Field<List<string>> m_Log;
			private Field<int> m_Number;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public NumberDecorator()
				: base(Will.DecorateMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnClass(ClassType classType, DecoratingClassWriter writer)
			{
				m_Number = writer.DependencyField<int>("m_Number");
				m_Log = writer.DependencyField<List<string>>("m_Log");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
			{
				decorate()
					.OnBefore(w =>
						m_Log.Add(m_Number.Func<string>(x => x.ToString).ToUpper() + w.Const("-BEFORE:" + member.Name))
					)
					.OnAfter(w =>
						m_Log.Add(m_Number.Func<string>(x => x.ToString).ToUpper() + w.Const("-AFTER:" + member.Name))
					);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class EventInterceptingDecorator : DecorationConvention
		{
			private Field<List<string>> m_LogField;
			private Field<IDictionary<Delegate, Delegate>> m_EventHandlerMapField;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public EventInterceptingDecorator()
				: base(Will.DecorateConstructors | Will.DecorateEvents)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnClass(ClassType classType, DecoratingClassWriter writer)
			{
				m_LogField = writer.DependencyField<List<string>>("m_Log");
				m_EventHandlerMapField = writer.Field<IDictionary<Delegate, Delegate>>("m_EventHandlerMap");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnConstructor(MethodMember member, Func<ConstructorDecorationBuilder> decorate)
			{
				decorate()
					.OnSuccess(w =>
						m_EventHandlerMapField.Assign(w.New<ConcurrentDictionary<Delegate, Delegate>>())
					);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnEvent(EventMember member, Func<EventDecorationBuilder> decorate)
			{
				Local<EventInterceptorClosure<TypeTemplate.TEventArgs>> interceptorClosure = null;
				Argument<EventHandler<TypeTemplate.TEventArgs>> value = null;

				decorate().OnAdd()
					.OnBefore(w => {
						value = w.Arg1<EventHandler<TypeTemplate.TEventArgs>>();
						interceptorClosure = w.Local<EventInterceptorClosure<TypeTemplate.TEventArgs>>();

						interceptorClosure.Assign(w.New<EventInterceptorClosure<TypeTemplate.TEventArgs>>());
						interceptorClosure.Field(x => x.EventName).Assign(member.Name);
						interceptorClosure.Field(x => x.Log).Assign(m_LogField);
						interceptorClosure.Field(x => x.Handler).Assign(value);

						value.Assign(
							w.MakeDelegate<EventInterceptorClosure<TypeTemplate.TEventArgs>, EventHandler<TypeTemplate.TEventArgs>, TypeTemplate.TEventHandler>(
								interceptorClosure,
								x => x.HandleEvent
							)
						);
					})
					.OnSuccess(w =>
						m_EventHandlerMapField.Add(interceptorClosure.Field(x => x.Handler), value)
					);

				decorate().OnRemove()
					.OnBefore(w => {
						value = w.Arg1<EventHandler<TypeTemplate.TEventArgs>>();
						var interceptingHandler = w.Local<EventHandler<TypeTemplate.TEventArgs>>();
						w.If(m_EventHandlerMapField.TryGetValue(value, interceptingHandler) == w.Const(true)).Then(() =>
							value.Assign(interceptingHandler)
						);
					});
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class EventInterceptorClosure<TEventArgs>
		{
			public void HandleEvent(object sender, TEventArgs args)
			{
				Log.Add("BEFORE-RAISE:" + EventName + "=" + args.ToString());

				try
				{
					Handler(sender, args);
				}
				finally
				{
					Log.Add("AFTER-RAISE:" + EventName + "=" + args.ToString());
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string EventName;
			public EventHandler<TEventArgs> Handler;
			public List<string> Log;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class PropagatingDecorator : DecorationConvention
		{
			//TODO: replace AncestorRepository.IFewMethods with TypeTemplate.TPrimary
			private Field<AncestorRepository.IFewMethods> m_Target;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public PropagatingDecorator()
				: base(Will.DecorateMethods)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnClass(ClassType classType, DecoratingClassWriter writer)
			{
				//TODO: replace AncestorRepository.IFewMethods with TypeTemplate.TPrimary
				m_Target = writer.DependencyField<AncestorRepository.IFewMethods>("m_Target");
				base.OnClass(classType, writer);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override void OnMethod(MethodMember member, Func<MethodDecorationBuilder> decorate)
			{
				decorate()
					.OnReturnVoid(w => w.PropagateCall<TypeTemplate.TReturn>(m_Target))
					.OnReturnValue((w, retVal) => retVal.Assign(w.PropagateCall<TypeTemplate.TReturn>(m_Target)));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestTarget : AncestorRepository.IFewMethods, AncestorRepository.IFewReadWriteProperties, AncestorRepository.IFewEvents
		{
			private readonly List<string> m_Log;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TestTarget(List<string> log)
			{
				m_Log = log;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IFewMethods Members

			public void One()
			{
				m_Log.Add("TEST-One");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void Two(int n)
			{
				m_Log.Add("TEST-Two");
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int Three()
			{
				m_Log.Add("TEST-Three");
				return 0;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int Four(string s)
			{
				m_Log.Add("TEST-Four=" + s);
				return 123;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string Five(int n)
			{
				m_Log.Add("TEST-Five=" + n);
				throw new ExceptionRepository.TestExceptionOne("EOne");
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IFewReadWriteProperties Members

			public int AnInt { get; set; }
			public string AString { get; set; }
			public object AnObject { get; set; }

			#endregion

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			#region IFewEvents Members

			public void RaiseOne()
			{
				if ( EventOne != null )
				{
					EventOne(this, EventArgs.Empty);
				}
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public string RaiseTwo(string input)
			{
				var eventArgs = new AncestorRepository.InOutEventArgs();
				eventArgs.InputValue = input;
				
				if ( EventTwo != null )
				{
					EventTwo(this, eventArgs);
				}
				
				return eventArgs.OutputValue;
			}

			//-----------------------------------------------------------------------------------------------------------------------------------------------------

			public event EventHandler EventOne;
			public event EventHandler<AncestorRepository.InOutEventArgs> EventTwo;

			#endregion
		}
	}
}
