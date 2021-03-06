﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Operands;
using NUnit.Framework;

namespace Hapil.UnitTests.Writers
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
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
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
					"[Loc0 = [Enumerable::Select(Arg1[source],Func<String,String>(DoTest<AnonymousMethod>))]];" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
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
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
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
					"[Loc0 = [Enumerable::Select(Arg1[source],Func<String,String>(this.DoTest<AnonymousMethod>))]];" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
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
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
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
				    "[Loc2 = NewObj[DoTest<Closure>]()];" + 
					"[Loc2.Field[<hoisted>Loc0_String] = Const[prefix]];" +
					"[Loc1 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc2.DoTest<AnonymousMethod>))]];" +
					"Return[Loc1];" + 
				"}"
			));
			
			Assert.That(ListClassFields(anonymousMethod.Closure.ClosureClass), Is.EqualTo(new[] {
				"InstanceField[<hoisted>Loc0_String]"
			}));
			
			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
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
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
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
					"[Loc1 = NewObj[DoTest<Closure>]()];" +
					"[Loc1.Field[<hoisted>Arg_source] = Arg1[arg0]];" +
					"[Loc0 = [Enumerable::Select(Loc1.Field[<hoisted>Arg_source],Func<String,String>(Loc1.DoTest<AnonymousMethod>))]];" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(ListClassFields(anonymousMethod.Closure.ClosureClass), Is.EqualTo(new[] {
				"InstanceField[<hoisted>Arg_source]"
			}));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
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
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
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
					"[Loc2 = NewObj[DoTest<Closure>]()];" +
					"[Loc2.Field[<hoisted>Arg_source] = Arg1[arg0]];" + 
					"[Loc2.Field[<hoisted>This] = this];" + 
					"[Loc2.Field[<hoisted>Loc0_String] = Const[prefix]];" + 
					"[Loc1 = [Enumerable::Select(Loc2.Field[<hoisted>Arg_source],Func<String,String>(Loc2.DoTest<AnonymousMethod>))]];" +
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
				"DoTest<AnonymousMethod>(String):String{" +
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
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) =>
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
					"[Loc2 = NewObj[DoTest<Closure>]()];" + 
					"[Loc2.Field[<hoisted>Arg_source] = Arg1[source]];" + 
					"[Loc2.Field[<hoisted>This] = this];" +
					"IF ([Loc2.Field[<hoisted>Arg_source] != Const[null]]) THEN {" +
						"[Loc3 = NewObj[DoTest<Closure>1]()];" +
						"[Loc3.Field[Parent] = Loc2];" +
						"[Loc3.Field[<hoisted>Loc0_String] = Const[prefix]];" +
						"[Loc1 = [Enumerable::Select(Loc3.Field[Parent].Field[<hoisted>Arg_source],Func<String,String>(Loc3.DoTest<AnonymousMethod>))]];" + 
						"Return[Loc1];" +
					"};" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
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
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
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
					"[Loc3 = NewObj[DoTest<Closure>]()];" +
					"[Loc3.Field[<hoisted>Loc1_Int32] = Const[123]];" +
					"IF ([Arg1[source] != Const[null]]) THEN {" +
						"WHILE ([Enumerable::Any(Arg1[source])]) {" +
							"[Loc4 = NewObj[DoTest<Closure>1]()];" +
							"[Loc4.Field[Parent] = Loc3];" +
							"[Loc4.Field[<hoisted>Loc2_String] = Const[suffix]];" +
							"[Loc0 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc4.DoTest<AnonymousMethod>))]];" +
						"};" +
					"};" +
					"Return[Loc0];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
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

		[Test]
		public void SameCapturesForTwoAnonymousMethod_WriteOneClosure_FromClosureTests()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var prefix = w.Local("PFX");
					var input = w.Local(w.NewArray<int>(100, 101));
					var output = w.Local(w.New<List<string>>());

					n.Assign(99);

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						n.FuncToString() + w.Const(":") +
						item.FuncToString())));

					output.AddRange(input.Select(w.Lambda<int, string>(item =>
						prefix + w.Const(":") +
						n.FuncToString() + w.Const(":") +
						item.FuncToString())));

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var methodFive = WriteMethod("Five");

			//-- Assert

			Assert.That(methodFive.GetMethodText(), Is.EqualTo(
				"Five(Int32):String{" +
					"[Loc4 = NewObj[Five<Closure>]()];" +
					"[Loc4.Field[<hoisted>Arg_n] = Arg1[arg0]];" +
					"[Loc4.Field[<hoisted>Loc0_String] = Const[PFX]];" +
					"[Loc1 = [new Int32[]Const[2]]];[Loc1[Const[0]] = Const[100]];[Loc1[Const[1]] = Const[101]];" +
					"[Loc2 = Loc1];" +
					"[Loc3 = NewObj[List<String>]()];" +
					"[Loc4.Field[<hoisted>Arg_n] = Const[99]];" +
					"Loc3.AddRange([Enumerable::Select(Loc2,Func<Int32,String>(Loc4.Five<AnonymousMethod>))]);" +
					"Loc3.AddRange([Enumerable::Select(Loc2,Func<Int32,String>(Loc4.Five<AnonymousMethod>1))]);" +
					"Return[[String::Join(Const[;],Loc3)]];" +
				"}"
			));

			var anonymousMethods = base.FindAnonymousMethods();
			Assert.That(anonymousMethods.Length, Is.EqualTo(2));

			Assert.That(anonymousMethods[0].GetMethodText(), Is.EqualTo(
				"Five<AnonymousMethod>(Int32):String{" +
					"Return[[" +
						"[[[this.Field[<hoisted>Arg_n] cast-to Type[Object]].ToString()] + " +
						"Const[:]] + " +
						"[[Arg1[arg1] cast-to Type[Object]].ToString()]" +
					"]];" +
				"}"
			));

			Assert.That(anonymousMethods[1].GetMethodText(), Is.EqualTo(
				"Five<AnonymousMethod>1(Int32):String{" +
					"Return[[" +
						"[[[this.Field[<hoisted>Loc0_String] + " +
						"Const[:]] + " +
						"[[this.Field[<hoisted>Arg_n] cast-to Type[Object]].ToString()]] + " +
						"Const[:]] + " +
						"[[Arg1[arg1] cast-to Type[Object]].ToString()]" +
					"]];" +
				"}"
			));

			var closure = base.Class.GetNestedClasses().Single();

			Assert.That(anonymousMethods[0].OwnerClass, Is.SameAs(closure));
			Assert.That(anonymousMethods[1].OwnerClass, Is.SameAs(closure));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Arg_n]", "InstanceField[<hoisted>Loc0_String]" },
				ListClassFields(closure));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void SameCapturesForTwoAnonymousMethod_WriteOneClosure()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
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

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc5 = NewObj[DoTest<Closure>]()];" +
					"[Loc5.Field[<hoisted>Loc1_String] = Const[prefix]];" +
					"[Loc5.Field[<hoisted>Loc2_String] = Const[suffix]];" +
					"[Loc3 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc5.DoTest<AnonymousMethod>))]];" +
					"[Loc4 = [Enumerable::Reverse([Enumerable::Select(Arg1[source],Func<String,String>(Loc5.DoTest<AnonymousMethod>1))])]];" +
					"[Loc0 = [Enumerable::Concat(Loc3,Loc4)]];" +
					"Return[Loc0];" +
				"}"
			));

			var anonymousMethods = base.FindAnonymousMethods();
			Assert.That(anonymousMethods.Length, Is.EqualTo(2));

			Assert.That(anonymousMethods[0].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
					"Return[" +
						"[[this.Field[<hoisted>Loc1_String] + Arg1[arg1]] + this.Field[<hoisted>Loc2_String]]" +
					"];" +
				"}"
			));

			Assert.That(anonymousMethods[1].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>1(String):String{" +
					"Return[" +
						"[[this.Field[<hoisted>Loc2_String] + Arg1[arg1]] + this.Field[<hoisted>Loc1_String]]" +
					"];" +
				"}"
			));

			var closure = base.Class.GetNestedClasses().Single();

			Assert.That(anonymousMethods[0].OwnerClass, Is.SameAs(closure));
			Assert.That(anonymousMethods[1].OwnerClass, Is.SameAs(closure));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Loc1_String]", "InstanceField[<hoisted>Loc2_String]" },
				ListClassFields(closure));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void OverlappingCapturesForTwoAnonymousMethod_WriteOneUnionClosure()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
					var output = w.Local<IEnumerable<string>>();
					var prefix = w.Local("prefix");
					var suffix = w.Local("suffix");
					var prefixed = w.Local(initialValue: source.Select(s => prefix + s));
					var suffixed = w.Local(initialValue: source.Select(s => s + suffix));
					output.Assign(prefixed.Concat(suffixed));
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					"[Loc5 = NewObj[DoTest<Closure>]()];" +
					"[Loc5.Field[<hoisted>Loc1_String] = Const[prefix]];" +
					"[Loc5.Field[<hoisted>Loc2_String] = Const[suffix]];" +
					"[Loc3 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc5.DoTest<AnonymousMethod>))]];" +
					"[Loc4 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc5.DoTest<AnonymousMethod>1))]];" +
					"[Loc0 = [Enumerable::Concat(Loc3,Loc4)]];" +
					"Return[Loc0];" +
				"}"
			));

			var anonymousMethods = base.FindAnonymousMethods();
			Assert.That(anonymousMethods.Length, Is.EqualTo(2));

			Assert.That(anonymousMethods[0].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
					"Return[" +
						"[this.Field[<hoisted>Loc1_String] + Arg1[arg1]]" +
					"];" +
				"}"
			));

			Assert.That(anonymousMethods[1].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>1(String):String{" +
					"Return[" +
						"[Arg1[arg1] + this.Field[<hoisted>Loc2_String]]" +
					"];" +
				"}"
			));

			var closure = base.Class.GetNestedClasses().Single();

			Assert.That(anonymousMethods[0].OwnerClass, Is.SameAs(closure));
			Assert.That(anonymousMethods[1].OwnerClass, Is.SameAs(closure));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Loc1_String]", "InstanceField[<hoisted>Loc2_String]" },
				ListClassFields(closure));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NestedAnonymousMethods_SameClosure()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var input = w.Local(w.NewArray<int>(100, 101, 102, 103));
					var output = w.Local(w.New<List<string>>());

					output.AddRange(
						input.Select(
							w.Lambda<int, string>(item =>
								(item + n).FuncToString().ToCharArray().Select(c => 
									c.CastTo<int>() + n
								)
								.Sum()
								.FuncToString()
							)
						)
					);

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var methodFive = WriteMethod("Five");

			//-- Assert

			Assert.That(methodFive.GetMethodText(), Is.EqualTo(
				"Five(Int32):String{" +
					"[Loc3 = NewObj[Five<Closure>]()];" +
					"[Loc3.Field[<hoisted>Arg_n] = Arg1[arg0]];" +
					"[Loc0 = [new Int32[]Const[4]]];" +
					"[Loc0[Const[0]] = Const[100]];[Loc0[Const[1]] = Const[101]];[Loc0[Const[2]] = Const[102]];[Loc0[Const[3]] = Const[103]];" +
					"[Loc1 = Loc0];" +
					"[Loc2 = NewObj[List<String>]()];" +
					"Loc2.AddRange([Enumerable::Select(Loc1,Func<Int32,String>(Loc3.Five<AnonymousMethod>))]);" +
					"Return[[String::Join(Const[;],Loc2)]];" + 
				"}"
			));

			var anonymousMethods = base.FindAnonymousMethods();
			Assert.That(anonymousMethods.Length, Is.EqualTo(2));

			Assert.That(anonymousMethods[0].GetMethodText(), Is.EqualTo(
				"Five<AnonymousMethod>(Int32):String{" +
					"Return[[[[" +
						"Enumerable::Sum([Enumerable::Select(" +
							"[[[[Arg1[arg1] + this.Field[<hoisted>Arg_n]] cast-to Type[Object]].ToString()].ToCharArray()]," +
							"Func<Char,Int32>(this.Five<AnonymousMethod>N)" +
						")])] cast-to Type[Object]].ToString()" +
					"]];" + 
				"}"
			));

			Assert.That(anonymousMethods[1].GetMethodText(), Is.EqualTo(
				"Five<AnonymousMethod>N(Char):Int32{" +
					"Return[[[Arg1[arg1] cast-to Type[Int32]] + this.Field[<hoisted>Arg_n]]];" +
				"}"
			));

			var closure = base.Class.GetNestedClasses(recursive: true).Single();

			Assert.That(anonymousMethods[0].OwnerClass, Is.SameAs(closure));
			Assert.That(anonymousMethods[1].OwnerClass, Is.SameAs(closure));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Arg_n]" },
				ListClassFields(closure));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NestedAnonymousMethods_NestedNonLinkedClosures()
		{
			//-- Arrange

			DeriveClassFrom<object>()
				.DefaultConstructor()
				.ImplementInterface<AncestorRepository.IFewMethods>()
				.Method<int, string>(intf => intf.Five).Implement((w, n) => {
					var input = w.Local(w.NewArray<int>(100, 101, 102, 103));
					var output = w.Local(w.New<List<string>>());

					output.AddRange(
						input.Select(
							w.Delegate<int, string>((ww, item) => {
								var number = ww.Local(initialValue: item + n);
								var digits = ww.Local(initialValue: number.FuncToString().ToCharArray());
								var sum = ww.Local(initialValue: digits.Select(c => c.CastTo<int>() + number + n).Sum());
								ww.Return(sum.FuncToString());
							})
						)
					);

					w.Return(Static.Func(String.Join, w.Const(";"), output));
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var methodFive = WriteMethod("Five");

			//-- Assert

			Assert.That(methodFive.GetMethodText(), Is.EqualTo(
				"Five(Int32):String{" +
					"[Loc3 = NewObj[Five<Closure>]()];" +
					"[Loc3.Field[<hoisted>Arg_n] = Arg1[arg0]];" +
					"[Loc0 = [new Int32[]Const[4]]];" +
					"[Loc0[Const[0]] = Const[100]];[Loc0[Const[1]] = Const[101]];[Loc0[Const[2]] = Const[102]];[Loc0[Const[3]] = Const[103]];" + 
					"[Loc1 = Loc0];" +
					"[Loc2 = NewObj[List<String>]()];" +
					"Loc2.AddRange([Enumerable::Select(Loc1,Func<Int32,String>(Loc3.Five<AnonymousMethod>))]);" +
					"Return[[String::Join(Const[;],Loc2)]];" +
				"}"
			));

			var anonymousMethods = base.FindAnonymousMethods();
			Assert.That(anonymousMethods.Length, Is.EqualTo(2));

			Assert.That(anonymousMethods[0].GetMethodText(), Is.EqualTo(
				"Five<AnonymousMethod>(Int32):String{" +
					"[Loc3 = NewObj[Five<AnonymousMethod><Closure>]()];" +
					"[Loc3.Field[<hoisted>This] = this];" +
					"[Loc3.Field[<hoisted>Loc0_Int32] = [Arg1[arg1] + this.Field[<hoisted>Arg_n]]];" +
					"[Loc1 = [[[Loc3.Field[<hoisted>Loc0_Int32] cast-to Type[Object]].ToString()].ToCharArray()]];" +
					"[Loc2 = [Enumerable::Sum([Enumerable::Select(Loc1,Func<Char,Int32>(Loc3.Five<AnonymousMethod>N))])]];" +
					"Return[[[Loc2 cast-to Type[Object]].ToString()]];" +
				"}"
			));

			Assert.That(anonymousMethods[1].GetMethodText(), Is.EqualTo(
				"Five<AnonymousMethod>N(Char):Int32{" +
					"Return[[" +
						"[[Arg1[arg1] cast-to Type[Int32]] + this.Field[<hoisted>Loc0_Int32]] + this.Field[<hoisted>This].Field[<hoisted>Arg_n]" +
					"]];" +
				"}"
			));

			var closure1 = base.Class.GetNestedClasses(recursive: false).Single();
			var closure2 = closure1.GetNestedClasses(recursive: false).Single();

			Assert.That(closure2.GetNestedClasses(), Is.Empty);
			Assert.That(anonymousMethods[0].OwnerClass, Is.SameAs(closure1));
			Assert.That(anonymousMethods[1].OwnerClass, Is.SameAs(closure2));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>Arg_n]" },
				ListClassFields(closure1));

			CollectionAssert.AreEquivalent(
				new[] { "InstanceField[<hoisted>This]", "InstanceField[<hoisted>Loc0_Int32]" },
				ListClassFields(closure2));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AnonymousMethodInLoopWithNoClosure_CacheDelegate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
					var output = w.Local<IEnumerable<string>>();
					w.For(from: 0, to: 10).Do((loop, i) => {
						output = w.Local(initialValue: source.Select(s => s.ToUpper()));
					});
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
					"[Loc3 = Const[null]];" +
					"FOR ({[Loc1 = Const[0]];} ; [Loc1 < Const[10]] ; {[Loc1 = [Loc1 + Const[1]]];}) {" +
						"IF ([Loc3 == Const[null]]) THEN {" +
							"[Loc3 = NewObj[Func<String,String>](Const[null],Method[DoTest<AnonymousMethod>])];" +
						"};" +
						"[Loc2 = [Enumerable::Select(Arg1[source],Loc3)]];" +
					"};" +
					"Return[Loc2];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
					"Return[[Arg0[arg1].ToUpper()]];" +
				"}"
			));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AnonymousMethodInLoopWithOutsideClosure_CacheDelegate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
					var output = w.Local<IEnumerable<string>>();
					var suffix = w.Local<string>("zz");
					w.For(from: 0, to: 10).Do((loop, i) => {
						output = w.Local(initialValue: source.Select(s => s.ToUpper() + suffix));
					});
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
					"[Loc5 = Const[null]];" +
					"[Loc4 = NewObj[DoTest<Closure>]()];" +
					"[Loc4.Field[<hoisted>Loc1_String] = Const[zz]];" +
					"FOR ({[Loc2 = Const[0]];} ; [Loc2 < Const[10]] ; {[Loc2 = [Loc2 + Const[1]]];}) {" +
						"IF ([Loc5 == Const[null]]) THEN {" +
							"[Loc5 = NewObj[Func<String,String>](Loc4,Method[DoTest<AnonymousMethod>])];" +
						"};" +
						"[Loc3 = [Enumerable::Select(Arg1[source],Loc5)]];" +
					"};" +
					"Return[Loc3];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
					"Return[[[Arg1[arg1].ToUpper()] + this.Field[<hoisted>Loc1_String]]];" +
				"}"
			));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AnonymousMethodInLoopWithInsideClosure_DoNotCacheDelegate()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
					var output = w.Local<IEnumerable<string>>();
					w.For(from: 0, to: 10).Do((loop, i) => {
						var suffix = w.Local<string>("z" + i.FuncToString());
						output = w.Local(initialValue: source.Select(s => s.ToUpper() + suffix));
					});
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
					"FOR ({[Loc1 = Const[0]];} ; [Loc1 < Const[10]] ; {[Loc1 = [Loc1 + Const[1]]];}) {" +
						"[Loc4 = NewObj[DoTest<Closure>]()];" +
						"[Loc4.Field[<hoisted>Loc2_String] = [Const[z] + [[Loc1 cast-to Type[Object]].ToString()]]];" +
						"[Loc3 = [Enumerable::Select(Arg1[source],Func<String,String>(Loc4.DoTest<AnonymousMethod>))]];" +
					"};" +
					"Return[Loc3];" +
				"}"
			));

			Assert.That(anonymousMethod.GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{" +
					"Return[[[Arg1[arg1].ToUpper()] + this.Field[<hoisted>Loc2_String]]];" +
				"}"
			));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AnonymousMethodsInLoopWithNoOrOutsideClosures_AllDelegatesCached()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.EnumerableTester>()
				.DefaultConstructor()
				.Method<IEnumerable<string>, IEnumerable<string>>(cls => cls.DoTest).Implement((w, source) => {
					var output = w.Local<List<string>>(initialValue: w.New<List<string>>());
					var prefix = w.Local<string>("y");
					w.If(source != w.Const<IEnumerable<string>>(null)).Then(() => {
						var suffix = w.Local<string>("z");
						w.For(from: 0, to: 3).Do((loop, i) => 
							output.AddRange(source
								.Select(s => i.FuncToString() + s)
								.Select(s => prefix + s)
								.Select(s => s + suffix)
								.Select(s => s.ToUpper()))
						);
					});
					w.Return(output);
				})
				.AllMethods().Throw<NotImplementedException>()
				.Flush();

			//-- Act

			var doTestMethod = WriteMethod("DoTest");

			//-- Assert

			Assert.That(doTestMethod.GetMethodText(), Is.EqualTo(
				"DoTest(IEnumerable<String>):IEnumerable<String>{" +
					#region Full Method Text
					"[Loc4 = NewObj[DoTest<Closure>]()];" +
					"[Loc0 = NewObj[List<String>]()];" +
					"[Loc4.Field[<hoisted>Loc1_String] = Const[y]];" +
					"IF ([Arg1[source] != Const[null]]) THEN {" +
						"[Loc9 = Const[null]];" +
						"[Loc8 = Const[null]];" +
						"[Loc7 = Const[null]];" +
						"[Loc6 = Const[null]];" +
						"[Loc5 = NewObj[DoTest<Closure>1]()];" +
						"[Loc5.Field[Parent] = Loc4];" +
						"[Loc5.Field[<hoisted>Loc2_String] = Const[z]];" +
						"FOR ({[Loc5.Field[<hoisted>Loc3_Int32] = Const[0]];} ; " +
							"[Loc5.Field[<hoisted>Loc3_Int32] < Const[3]] ; " +
							"{[Loc5.Field[<hoisted>Loc3_Int32] = [Loc5.Field[<hoisted>Loc3_Int32] + Const[1]]];}) " +
						"{" +
							"IF ([Loc9 == Const[null]]) THEN {" +
								"[Loc9 = NewObj[Func<String,String>](Const[null],Method[DoTest<AnonymousMethod>])];" +
							"};" +
							"IF ([Loc8 == Const[null]]) THEN {" +
								"[Loc8 = NewObj[Func<String,String>](Loc5,Method[DoTest<AnonymousMethod>2])];" +
							"};" +
							"IF ([Loc7 == Const[null]]) THEN {" +
								"[Loc7 = NewObj[Func<String,String>](Loc5,Method[DoTest<AnonymousMethod>1])];" +
							"};" +
							"IF ([Loc6 == Const[null]]) THEN {" +
								"[Loc6 = NewObj[Func<String,String>](Loc5,Method[DoTest<AnonymousMethod>])];" +
							"};" +
							"Loc0.AddRange(" + 
								"[Enumerable::Select(" +
									"[Enumerable::Select(" +
										"[Enumerable::Select(" +
											"[Enumerable::Select(Arg1[source],Loc6)]," + 
											"Loc8)]," + 
										"Loc7)]," + 
									"Loc9)]" +
							");" +	//end AddRange
						"};" +		//end FOR
					"};" +			//end IF
					"Return[Loc0];" +
					#endregion
				"}"
			));

			var anonymousMethods = base.FindAnonymousMethods();
			Assert.That(anonymousMethods.Length, Is.EqualTo(4));

			Assert.That(anonymousMethods[0].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{Return[[Arg0[arg1].ToUpper()]];}"
			));
			Assert.That(anonymousMethods[1].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>(String):String{Return[[[[this.Field[<hoisted>Loc3_Int32] cast-to Type[Object]].ToString()] + Arg1[arg1]]];}"
			));
			Assert.That(anonymousMethods[2].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>1(String):String{Return[[Arg1[arg1] + this.Field[<hoisted>Loc2_String]]];}"
			));
			Assert.That(anonymousMethods[3].GetMethodText(), Is.EqualTo(
				"DoTest<AnonymousMethod>2(String):String{Return[[this.Field[Parent].Field[<hoisted>Loc1_String] + Arg1[arg1]]];}"
			));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private string[] ListClassFields(ClassType classType)
		{
			return classType.GetAllMembers().OfType<FieldMember>().Select(m => m.ToString()).ToArray();
		}
	}
}
	