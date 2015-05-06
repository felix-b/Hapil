using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using Hapil;
using Hapil.Operands;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Hapil.UnitTests.Statements
{
	[TestFixture]
	public class ForStatementTests : NUnitEmittedTypesTestBase
	{
		[Test]
		public void TestShortSyntaxAscending()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					m.For(0, count).Do((loop, i) => {
						Static.Prop(() => OutputList1).Add(i);
					});
                    m.Return(m.Const(0));
				});

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			OutputList1 = new List<int>();

			//-- Act

			tester.DoTest(5);

			//-- Assert

			Assert.That(OutputList1, Is.EqualTo(new[] { 0, 1, 2, 3, 4 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestShortSyntaxDescending()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					m.For(count, 0, increment: -1).Do((loop, i) => {
						Static.Prop(() => OutputList1).Add(i);
					});
                    m.Return(m.Const(0));
				});

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			OutputList1 = new List<int>();

			//-- Act

			tester.DoTest(5);

			//-- Assert

			Assert.That(OutputList1, Is.EqualTo(new[] { 4, 3, 2, 1, 0 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestShortSyntaxWithBreak()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					m.For(0, count).Do((loop, i) => {
						Static.Prop(() => OutputList1).Add(i);
						m.If(i == 5).Then(loop.Break);
					});
                    m.Return(m.Const(0));
                });

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			//-- Act

			OutputList1 = new List<int>();
			tester.DoTest(3);
			var output1 = OutputList1;

			OutputList1 = new List<int>();
			tester.DoTest(10);
			var output2 = OutputList1;

			//-- Assert

			Assert.That(output1, Is.EqualTo(new[] { 0, 1, 2 }));
			Assert.That(output2, Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestShortSyntaxWithContinue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					m.For(0, count).Do((loop, i) => {
						m.If(i == 2).Then(loop.Continue);
						Static.Prop(() => OutputList1).Add(i);
					});
                    m.Return(m.Const(0));
                });

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			OutputList1 = new List<int>();

			//-- Act

			tester.DoTest(5);

			//-- Assert

			Assert.That(OutputList1, Is.EqualTo(new[] { 0, 1, 3, 4 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFullSyntax()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					var sum = m.Local<int>(initialValueConst: 0);
					var item1 = m.Local<LinkedListNode<int>>();
					var item2 = m.Local<LinkedListNode<int>>();

					m.For(() => {
						item1.Assign(Static.Prop(() => InputLinkedList1).Prop(x => x.First));
						item2.Assign(Static.Prop(() => InputLinkedList2).Prop(x => x.Last));
					})
					.While(
						item1 != null && item2 != null
					)
					.Next(() => {
						item1.Assign(item1.Prop(x => x.Next));
						item2.Assign(item2.Prop(x => x.Previous));
					}).
					Do(loop => {
						sum.Assign(sum + item1.Prop(x => x.Value));
						Static.Prop(() => OutputList1).Add(item1.Prop(x => x.Value));
						Static.Prop(() => OutputList2).Add(item2.Prop(x => x.Value));
					});

					m.Return(sum);
				});

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			
			InputLinkedList1 = new LinkedList<int>(new[] { 100000, 20000, 3000, 400, 50, 6 });
			InputLinkedList2 = new LinkedList<int>(new[] { 1, 2, 3, 4 });
			OutputList1 = new List<int>();
			OutputList2 = new List<int>();

			//-- Act

			var result = tester.DoTest(0);

			//-- Assert

			Assert.That(result, Is.EqualTo(123400));
			Assert.That(OutputList1, Is.EqualTo(new[] { 100000, 20000, 3000, 400 }));
			Assert.That(OutputList2, Is.EqualTo(new[] { 4, 3, 2, 1 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFullSyntaxWithBreak()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					var item = m.Local<LinkedListNode<int>>();
					
					m.For(() => item.Assign(Static.Prop(() => InputLinkedList1).Prop(x => x.First)))
					.While(item != null)
					.Next(() => item.Assign(item.Prop(x => x.Next)))
					.Do(loop => {
						var value = m.Local<int>(initialValue: item.Prop(x => x.Value));
						Static.Prop(() => OutputList1).Add(value);
						m.If(value == 3).Then(loop.Break);
					});

					m.Return(0);
				});

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			InputLinkedList1 = new LinkedList<int>(new[] { 1, 2, 3, 4, 5, 6 });
			OutputList1 = new List<int>();

			//-- Act

			var result = tester.DoTest(0);

			//-- Assert

			Assert.That(OutputList1, Is.EqualTo(new[] { 1, 2, 3 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestFullSyntaxWithContinue()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(x => x.DoTest).Implement((m, count) => {
					var item = m.Local<LinkedListNode<int>>();

					m.For(() => item.Assign(Static.Prop(() => InputLinkedList1).Prop(x => x.First)))
					.While(item != null)
					.Next(() => item.Assign(item.Prop(x => x.Next)))
					.Do(loop => {
						var value = m.Local<int>(initialValue: item.Prop(x => x.Value));
						m.If(value == 3).Then(loop.Continue);
						Static.Prop(() => OutputList1).Add(value);
					});

					m.Return(0);
				});

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();

			InputLinkedList1 = new LinkedList<int>(new[] { 1, 2, 3, 4, 5, 6 });
			OutputList1 = new List<int>();

			//-- Act

			var result = tester.DoTest(0);

			//-- Assert

			Assert.That(OutputList1, Is.EqualTo(new[] { 1, 2, 4, 5, 6 }));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static List<int> OutputList1 { get; set; }
		public static List<int> OutputList2 { get; set; }
		public static LinkedList<int> InputLinkedList1 { get; set; }
		public static LinkedList<int> InputLinkedList2 { get; set; }
	}
}
