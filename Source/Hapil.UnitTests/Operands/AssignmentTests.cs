using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hapil.Testing.NUnit;
using Hapil;
using Hapil.Operands;
using Hapil.Expressions;
using Hapil.Statements;
using Hapil.Members;
using Hapil.Writers;
using NUnit.Framework;

namespace Hapil.UnitTests.Operands
{
	[TestFixture]
	public class AssignmentTests : NUnitEmittedTypesTestBase
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
				method.GetMethodText(),
				Is.EqualTo("VirtualVoidMethod():void{[this.Field[f1] = this.Field[f2]];}"));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void AssignConstToField()
		{
			//-- Arrange

			var classWriter = DeriveClassFrom<AncestorRepository.BaseOne>().DefaultConstructor();

			var field1 = classWriter.Field<int>("f1");
			IOperand<int> const1 = new Constant<int>(123);

			//-- Act

			classWriter.Method(cls => cls.VirtualVoidMethod).Implement(m => {
				field1.Assign(const1);
			});

			CreateClassInstanceAs<AncestorRepository.BaseOne>().UsingDefaultConstructor();

			//-- Assert

			var method = classWriter.OwnerClass.GetMemberByName<MethodMember>("VirtualVoidMethod");

			Assert.That(
				method.GetMethodText(),
				Is.EqualTo("VirtualVoidMethod():void{[this.Field[f1] = Const[123]];}"));
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
				method.GetMethodText(),
				Is.EqualTo("VirtualVoidMethod():void{[this.Field[f1] = [this.Field[f2] + Const[123]]];}"));
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
