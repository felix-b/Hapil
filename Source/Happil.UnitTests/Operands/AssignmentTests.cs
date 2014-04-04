using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil;
using Happil.Operands;
using Happil.Expressions;
using Happil.Statements;
using Happil.Members;
using Happil.Writers;
using NUnit.Framework;

namespace Happil.UnitTests.Operands
{
	[TestFixture]
	public class AssignmentTests : ClassPerTestCaseFixtureBase
	{
		[Test]
		public void AssignFieldToField()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();

			var field1 = classWriter.Field<int>("f1");
			var field2 = classWriter.Field<int>("f2");

			//-- Act

			classWriter.Method(cls => cls.VirtualVoidMethod).Implement(w => {
				field1.Assign(field2);
			});

			CreateClassInstanceAs<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			var method = classWriter.OwnerClass.GetMemberByName<MethodMember>("VirtualVoidMethod");

			Assert.That(
				method.ToString(), 
				Is.EqualTo("{Expr<Int32>{Field{f1} = Field{f2}};}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AssignConstToField()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();

			var field1 = classWriter.Field<int>("f1");
			IOperand<int> const1 = new ConstantOperand<int>(123);

			//-- Act

			classWriter.Method(cls => cls.VirtualVoidMethod).Implement(m => {
				field1.Assign(const1);
			});

			CreateClassInstanceAs<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			var method = classWriter.OwnerClass.GetMemberByName<MethodMember>("VirtualVoidMethod");

			Assert.That(
				method.ToString(),
				Is.EqualTo("{Expr<Int32>{Field{f1} = Const<Int32>{123}};}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AssignExpressionToField()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();

			var field1 = classWriter.Field<int>("f1");
			var field2 = classWriter.Field<int>("f2");

			//-- Act

			classWriter.Method(cls => cls.VirtualVoidMethod).Implement(m => {
				field1.Assign(field2 + 123);
			});

			CreateClassInstanceAs<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			var method = classWriter.OwnerClass.GetMemberByName<MethodMember>("VirtualVoidMethod");

			Assert.That(
				method.ToString(),
				Is.EqualTo("{Expr<Int32>{Field{f1} = Expr<Int32>{Field{f2} + Const<Int32>{123}}};}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestClassOne1 : AncestorRepository.BaseOne
		{
			private int f1;

			#region Overrides of TestBaseOne

			public override void VirtualVoidMethod()
			{
				f1 = 123;
			}

			#endregion

			public int F1
			{
				get
				{
					return f1;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TestClassOne2 : AncestorRepository.BaseOne
		{
			private int f1 = 123;
			private int f2 = 456;

			#region Overrides of TestBaseOne

			public override void VirtualVoidMethod()
			{
				f1 = f2 + 123;
			}

			#endregion

			public int F1
			{
				get
				{
					return f1;
				}
			}
			public int F2
			{
				get
				{
					return f2;
				}
			}
		}
	}
}
