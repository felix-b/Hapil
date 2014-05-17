using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Operands;
using NUnit.Framework;

namespace Happil.UnitTests.Writers
{
	[TestFixture]
	public class AnonymousMethodWriterTests : ClosureTestFixtureBase
	{
		[Test]
		public void CaptureNothing_WriteStaticAnonymousMethod()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) => {
					var output = w.Local(initialValue: source.Select(s => s.ToUpper()));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc0 = [Enumerable::Select(Arg1[source],Func<String,String>(AnonymousMethod))]];" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[Arg0[arg1].ToUpper()]];" +
				"}"
			));

			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(base.Class));
			Assert.That(base.Class.GetNestedClasses(), Is.Empty);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureThis_WriteInstanceAnonymousMethod()
		{
			//-- Arrange

			Field<string> prefixField;

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Field("m_Prefix", out prefixField)
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) => {
					var output = w.Local(initialValue: source.Select(s => prefixField + s));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc0 = [Enumerable::Select(Arg1[source],Func<String,String>(this.AnonymousMethod))]];" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[this.Field[m_Prefix] + Arg1[arg1]]];" +
				"}"
			));

			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(base.Class));
			Assert.That(base.Class.GetNestedClasses(), Is.Empty);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureLocal_WriteClosure()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) => {
					var prefix = w.Local("prefix");
					var output = w.Local(initialValue: source.Select(s => prefix + s));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" + 
				    "[Loc2 = NewObj[DoTestClosure]()];" + 
					"[Loc2.Field[<hoisted>Loc0_String] = Const[prefix]];" +
					"[Loc1 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc2.AnonymousMethod))]];" +
					"Return[Loc1];" + 
				"}"
			));
			
			Assert.That(ListClassFields(anonymousMethod.Closure.ClosureClass), Is.EqualTo(new[] {
				"InstanceField[<hoisted>Loc0_String]"
			}));
			
			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
				    "Return[[this.Field[<hoisted>Loc0_String] + Arg1[arg1]]];" +
				"}"
			));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureArgument_WriteClosure()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) => {
					var output = w.Local(initialValue: source.Select(s => source.First() + s));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc1 = NewObj[DoTestClosure]()];" +
					"[Loc1.Field[<hoisted>Arg_source] = Arg1[source]];" +
					"[Loc0 = [Enumerable::Select(Loc1.Field[<hoisted>Arg_source],Func<String,String>(Loc1.AnonymousMethod))]];" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(ListClassFields(anonymousMethod.Closure.ClosureClass), Is.EqualTo(new[] {
				"InstanceField[<hoisted>Arg_source]"
			}));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[[Enumerable::First(this.Field[<hoisted>Arg_source])] + Arg1[arg1]]];" +
				"}"
			));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CaptureLocalArgumentAndField_WriteClosure()
		{
			//-- Arrange

			Field<string> suffixField;

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Field("m_Suffix", out suffixField)
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) => {
					var prefix = w.Local("prefix");
					var output = w.Local(initialValue: source.Select(s => prefix + source.First() + s + suffixField));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" + 
					"[Loc2 = NewObj[DoTestClosure]()];" +
					"[Loc2.Field[<hoisted>Arg_source] = Arg1[source]];" + 
					"[Loc2.Field[<hoisted>This] = this];" + 
					"[Loc2.Field[<hoisted>Loc0_String] = Const[prefix]];" + 
					"[Loc1 = [Enumerable::Select(Loc2.Field[<hoisted>Arg_source],Func<String,String>(Loc2.AnonymousMethod))]];" +
					"Return[Loc1];" +
				"}"
			));

			CollectionAssert.AreEquivalent(
				new[] {
					"InstanceField[<hoisted>Arg_source]",
					"InstanceField[<hoisted>This]",
					"InstanceField[<hoisted>Loc0_String]"
				}, 
				ListClassFields(anonymousMethod.Closure.ClosureClass));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[" + 
						"[[this.Field[<hoisted>Loc0_String] + [Enumerable::First(this.Field[<hoisted>Arg_source])]] + " +
						"Arg1[arg1]] + " +
						"this.Field[<hoisted>This].Field[m_Suffix]" + 
					"]];" +
				"}"
			));
		}


		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CapturesFromTwoScopes_WriteTwoLinkedClosures()
		{
			//-- Arrange

			Field<string> suffixField;

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Field("m_Suffix", out suffixField)
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) =>
					w.If(source != w.Const<IEnumerable<string>>(null)).Then(() => {
						var prefix = w.Local("prefix");
						var output = w.Local(initialValue: source.Select(s => prefix + source.First() + s + suffixField));
						w.Return(output);
					})
				)
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc2 = NewObj[DoTestClosure]()];" + 
					"[Loc2.Field[<hoisted>Arg_source] = Arg1[source]];" + 
					"[Loc2.Field[<hoisted>This] = this];" +
					"IF ([Loc2.Field[<hoisted>Arg_source] != Const[null]]) THEN {" +
						"[Loc3 = NewObj[DoTestClosure1]()];" +
						"[Loc3.Field[Parent] = Loc2];" +
						"[Loc3.Field[<hoisted>Loc0_String] = Const[prefix]];" +
						"[Loc1 = [Enumerable::Select(Loc3.Field[Parent].Field[<hoisted>Arg_source],Func<String,String>(Loc3.AnonymousMethod))]];" + 
						"Return[Loc1];" +
					"};" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[" +
						"[[this.Field[<hoisted>Loc0_String] + " +
						"[Enumerable::First(this.Field[Parent].Field[<hoisted>Arg_source])]] + " +
						"Arg1[arg1]] + " +
						"this.Field[Parent].Field[<hoisted>This].Field[m_Suffix]" +
					"]];" +
				"}"
			));

			var closures = base.Class.GetNestedClasses();

			Assert.That(closures.Length, Is.EqualTo(2));
			Assert.That(closures.All(c => c.IsClosure));
			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(closures[1]));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Arg_source]", "InstanceField[<hoisted>This]" },
				ListClassFields(closures[0]));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[Parent]", "InstanceField[<hoisted>Loc0_String]" },
				ListClassFields(closures[1]));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void CapturesFromTwoNonConsequtiveScopes_WriteTwoLinkedClosures()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) => {
					var output = w.Local<IEnumerable<string>>();
					var prefix = w.Local(123);
					w.If(source != w.Const<IEnumerable<string>>(null)).Then(() => 
						w.While(source.Any()).Do(loop => {
							var suffix = w.Local("suffix");
							output.Assign(source.Select(s => prefix.FuncToString() + s + suffix));
						})
					);
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethod = base.FindAnonymousMethods().Single();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc3 = NewObj[DoTestClosure]()];" +
					"[Loc3.Field[<hoisted>Loc1_Int32] = Const[123]];" +
					"IF ([Arg1[source] != Const[null]]) THEN {" +
						"WHILE ([Enumerable::Any(Arg1[source])]) {" +
							"[Loc4 = NewObj[DoTestClosure1]()];" +
							"[Loc4.Field[Parent] = Loc3];" +
							"[Loc4.Field[<hoisted>Loc2_String] = Const[suffix]];" +
							"[Loc0 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc4.AnonymousMethod))]];" +
						"};" +
					"};" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[[[[" +
						"this.Field[Parent].Field[<hoisted>Loc1_Int32] cast-to Type[Object]].ToString()] + " +
						"Arg1[arg1]] + " +
						"this.Field[<hoisted>Loc2_String]" +
					"]];" +
				"}"
			));

			var closures = base.Class.GetNestedClasses();

			Assert.That(closures.Length, Is.EqualTo(2));
			Assert.That(closures.All(c => c.IsClosure));
			Assert.That(anonymousMethod.OwnerClass, Is.SameAs(closures[1]));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Loc1_Int32]" },
				ListClassFields(closures[0]));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[Parent]", "InstanceField[<hoisted>Loc2_String]" },
				ListClassFields(closures[1]));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, Ignore("this test is not yet ready.")]
		public void SameCapturesForTwoAnonymousMethod_WriteOneClosure()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(intf => intf.DoTest).Implement((w, source) => {
					var output = w.Local<IEnumerable<string>>();
					var prefix = w.Local("prefix");
					var suffix = w.Local("suffix");
					var forward = w.Local(initialValue: source.Select(s => prefix + s + suffix));
					var reverse = w.Local(initialValue: source.Select(s => suffix + s + prefix).Reverse());
					output.Assign(forward.Concat(reverse));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			var anonymousMethods = base.FindAnonymousMethods();

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc3 = NewObj[DoTestClosure]()];" +
					"[Loc3.Field[<hoisted>Loc1_Int32] = Const[123]];" +
					"IF ([Arg1[source] != Const[null]]) THEN {" +
						"WHILE ([Enumerable::Any(Arg1[source])]) {" +
							"[Loc4 = NewObj[DoTestClosure1]()];" +
							"[Loc4.Field[Parent] = Loc3];" +
							"[Loc4.Field[<hoisted>Loc2_String] = Const[suffix]];" +
							"[Loc0 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc4.AnonymousMethod))]];" +
						"};" +
					"};" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(anonymousMethods[0].GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[[[[" +
						"this.Field[Parent].Field[<hoisted>Loc1_Int32] cast-to Type[Object]].ToString()] + " +
						"Arg1[arg1]] + " +
						"this.Field[<hoisted>Loc2_String]" +
					"]];" +
				"}"
			));

			Assert.That(anonymousMethods[1].GetMethodText(), Is.EqualTo(
				"AnonymousMethod(String):String{" +
					"Return[[[[[" +
						"this.Field[Parent].Field[<hoisted>Loc1_Int32] cast-to Type[Object]].ToString()] + " +
						"Arg1[arg1]] + " +
						"this.Field[<hoisted>Loc2_String]" +
					"]];" +
				"}"
			));

			var closures = base.Class.GetNestedClasses();

			Assert.That(closures.Length, Is.EqualTo(2));
			Assert.That(closures.All(c => c.IsClosure));
			Assert.That(anonymousMethods[0].OwnerClass, Is.SameAs(closures[1]));
			Assert.That(anonymousMethods[1].OwnerClass, Is.SameAs(closures[1]));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Loc1_Int32]" },
				ListClassFields(closures[0]));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[Parent]", "InstanceField[<hoisted>Loc2_String]" },
				ListClassFields(closures[1]));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private string[] ListClassFields(ClassType classType)
		{
			return classType.GetAllMembers().OfType<FieldMember>().Select(m => m.ToString()).ToArray();
		}
	}
}
	