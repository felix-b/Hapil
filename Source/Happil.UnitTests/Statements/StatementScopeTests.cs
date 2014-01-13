using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Happil.Expressions;
using Happil.Fluent;
using Happil.Statements;
using NUnit.Framework;

namespace Happil.UnitTests.Statements
{
	[TestFixture]
	public class StatementScopeTests
	{
		private HappilModule m_Module;
		private HappilClass m_Class;
		private HappilMethod m_Method;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			m_Module = new HappilModule(
				"Happil.UnitTests.EmittedBy" + this.GetType().Name,
				allowSave: false);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[SetUp]
		public void SetUp()
		{
			StatementScope.Cleanup();
			
			var classBody = m_Module.DeriveClassFrom<object>(Guid.NewGuid().ToString());
			m_Class = ((HappilClassBody<object>)classBody).HappilClass;

			classBody.Method<string>(x => x.ToString).Implement(m => { });
			m_Method = m_Class.FindMember<HappilMethod>("ToString");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TearDown]
		public void TearDown()
		{
			StatementScope.Cleanup();
			m_Class = null;
			m_Method = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void RootScope_InitialState()
		{
			//-- Act
			
			var scope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());

			//-- Assert

			Assert.That(scope.OwnerClass, Is.SameAs(m_Class));
			Assert.That(scope.OwnerMethod, Is.SameAs(m_Method));
			Assert.That(scope.Depth, Is.EqualTo(0));
			Assert.That(scope.Previous, Is.Null);
			Assert.That(scope.Root, Is.SameAs(scope));
			Assert.That(scope.ThisExceptionBlockType, Is.EqualTo(ExceptionBlockType.None));
			Assert.That(scope.ThisExceptionStatement, Is.Null);
			Assert.That(scope.InheritedExceptionBlockType, Is.EqualTo(ExceptionBlockType.None));
			Assert.That(scope.InheritedExceptionStatement, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Current_NoScope()
		{
			//-- Assert
			Assert.That(StatementScope.Exists, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void Current_NoScope_CurrentThrows()
		{
			//-- Act
			var scope = StatementScope.Current;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Current_RootScope_CreateAndDispose()
		{
			//-- Act

			var scope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());

			var existsInside = StatementScope.Exists;
			var currentInside = StatementScope.Current;

			scope.Dispose();

			var existsAfter = StatementScope.Exists;

			//-- Assert

			Assert.That(existsInside, Is.True);
			Assert.That(currentInside, Is.SameAs(scope));
			Assert.That(existsAfter, Is.False);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void RootScope_AttemptCreateNestedRoot_Throws()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());

			//-- Act
			
			var nestedRootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void Current_NestedScope_CreateAndDispose()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());

			//-- Act

			var nestedScope = new StatementScope(new List<IHappilStatement>());

			var existsInsideNested = StatementScope.Exists;
			var currentInsideNested = StatementScope.Current;
			var previousInsideNested = StatementScope.Current.Previous;
			var rootInsideNested = StatementScope.Current.Root;
			var depthInsideNested = StatementScope.Current.Depth;

			nestedScope.Dispose();

			var existsAfterNested = StatementScope.Exists;
			var currentAfterNested = StatementScope.Current;
			var previousAfterNested = StatementScope.Current.Previous;
			var rootAfterNested = StatementScope.Current.Root;
			var depthAfterNested = StatementScope.Current.Depth;

			//-- Assert

			Assert.That(existsInsideNested, Is.True);
			Assert.That(currentInsideNested, Is.SameAs(nestedScope));
			Assert.That(previousInsideNested, Is.SameAs(rootScope));
			Assert.That(rootInsideNested, Is.SameAs(rootScope));
			Assert.That(depthInsideNested, Is.EqualTo(1));

			Assert.That(existsAfterNested, Is.True);
			Assert.That(currentAfterNested, Is.SameAs(rootScope));
			Assert.That(previousAfterNested, Is.Null);
			Assert.That(rootAfterNested, Is.SameAs(rootScope));
			Assert.That(depthAfterNested, Is.EqualTo(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void RegisterExpressionStatement_AddedToStatementList()
		{
			//-- Arrange

			var statements = new List<IHappilStatement>();
			var rootScope = new StatementScope(m_Class, m_Method, statements);

			//-- Act

			var expression = new HappilUnaryExpression<int, int>(m_Method, new UnaryOperators.OperatorMinus<int>(), m_Method.Const(111)); 

			//-- Assert

			Assert.That(statements.Count, Is.EqualTo(1));
			Assert.That(statements[0], Is.InstanceOf<ExpressionStatement>());
			Assert.That(((ExpressionStatement)statements[0]).Expression, Is.SameAs(expression));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void RegisterNonExpressionOperand_NothingHappens()
		{
			//-- Arrange

			var statements = new List<IHappilStatement>();
			var rootScope = new StatementScope(m_Class, m_Method, statements);

			//-- Act

			rootScope.RegisterExpressionStatement(expression: null);

			//-- Assert

			Assert.That(statements.Count, Is.EqualTo(0));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void RegisterExpressionAndContainingExpression_ContainedExpressionConsumed()
		{
			//-- Arrange

			var statements = new List<IHappilStatement>();
			var rootScope = new StatementScope(m_Class, m_Method, statements);

			//-- Act

			var innerExpression = new HappilUnaryExpression<int, int>(m_Method, new UnaryOperators.OperatorMinus<int>(), m_Method.Const(111));
			var outerExpression = new HappilUnaryExpression<int, int>(m_Method, new UnaryOperators.OperatorPlus<int>(), innerExpression);

			//-- Assert

			Assert.That(statements.Count, Is.EqualTo(1));
			Assert.That(statements[0], Is.InstanceOf<ExpressionStatement>());
			Assert.That(((ExpressionStatement)statements[0]).Expression, Is.SameAs(outerExpression));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void NoExceptionScope_ThisAndInheritedHaveNoValues()
		{
			//-- Act

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());

			//-- Assert

			Assert.That(StatementScope.Current.ThisExceptionBlockType, Is.EqualTo(ExceptionBlockType.None));
			Assert.That(StatementScope.Current.ThisExceptionStatement, Is.Null);
			Assert.That(StatementScope.Current.InheritedExceptionBlockType, Is.EqualTo(ExceptionBlockType.None));
			Assert.That(StatementScope.Current.InheritedExceptionStatement, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ExceptionScope_ThisAndInheritedHaveSameValues()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var tryStatement = new TryStatement(() => { });
			
			//-- Act

			var tryScope = new StatementScope(new List<IHappilStatement>(), tryStatement, ExceptionBlockType.Try);

			//-- Assert

			Assert.That(StatementScope.Current.ThisExceptionBlockType, Is.EqualTo(ExceptionBlockType.Try));
			Assert.That(StatementScope.Current.ThisExceptionStatement, Is.SameAs(tryStatement));
			Assert.That(StatementScope.Current.InheritedExceptionBlockType, Is.EqualTo(ExceptionBlockType.Try));
			Assert.That(StatementScope.Current.InheritedExceptionStatement, Is.SameAs(tryStatement));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void ScopeNestedInExceptionScope_ThisHasNoValue_InheritedHasValue()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var tryStatement = new TryStatement(() => { });
			var tryScope = new StatementScope(new List<IHappilStatement>(), tryStatement, ExceptionBlockType.Try);

			//-- Act

			var innerScope = new StatementScope(new List<IHappilStatement>());

			//-- Assert

			Assert.That(StatementScope.Current.ThisExceptionBlockType, Is.EqualTo(ExceptionBlockType.None));
			Assert.That(StatementScope.Current.ThisExceptionStatement, Is.Null);
			Assert.That(StatementScope.Current.InheritedExceptionBlockType, Is.EqualTo(ExceptionBlockType.Try));
			Assert.That(StatementScope.Current.InheritedExceptionStatement, Is.SameAs(tryStatement));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_RootScope_NotFound()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(rootScope);

			//-- Assert

			Assert.That(result, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_NestedScopesNoException_NotFound()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var innerScope1 = new StatementScope(new List<IHappilStatement>());
			var innerScope2 = new StatementScope(new List<IHappilStatement>());

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(rootScope);

			//-- Assert

			Assert.That(StatementScope.Current.Depth, Is.EqualTo(2));
			Assert.That(result, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_RootExceptionHomeInner_NotFound()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var tryScope = new StatementScope(new List<IHappilStatement>(), new TryStatement(() => { }), ExceptionBlockType.Try);
			var homeScope = new StatementScope(new List<IHappilStatement>());
			var innerScope = new StatementScope(new List<IHappilStatement>());

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);

			//-- Assert

			Assert.That(StatementScope.Current.Depth, Is.EqualTo(3));
			Assert.That(result, Is.Null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_RootHomeExceptionInner_Found()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var homeScope = new StatementScope(new List<IHappilStatement>());
			
			var tryStatement = new TryStatement(() => { });
			var tryScope = new StatementScope(new List<IHappilStatement>(), tryStatement, ExceptionBlockType.Try);
			
			var innerScope = new StatementScope(new List<IHappilStatement>());

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);

			//-- Assert

			Assert.That(StatementScope.Current.Depth, Is.EqualTo(3));
			Assert.That(result, Is.SameAs(tryStatement));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_RootHomeException_Found()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var homeScope = new StatementScope(new List<IHappilStatement>());
			
			var tryStatement = new TryStatement(() => { });
			var tryScope = new StatementScope(new List<IHappilStatement>(), tryStatement, ExceptionBlockType.Try);

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);

			//-- Assert

			Assert.That(StatementScope.Current.Depth, Is.EqualTo(2));
			Assert.That(result, Is.SameAs(tryStatement));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_RootHomeNestedExceptions_FoundOuter()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var homeScope = new StatementScope(new List<IHappilStatement>());

			var outerTryStatement = new TryStatement(() => { });
			var outerTryScope = new StatementScope(new List<IHappilStatement>(), outerTryStatement, ExceptionBlockType.Try);

			var innerTryStatement = new TryStatement(() => { });
			var innerTryScope = new StatementScope(new List<IHappilStatement>(), innerTryStatement, ExceptionBlockType.Try);

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);

			//-- Assert

			Assert.That(StatementScope.Current.Depth, Is.EqualTo(3));
			Assert.That(result, Is.SameAs(outerTryStatement));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_MultipleLevelsOfNesting_FoundOuter()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var homeScope = new StatementScope(new List<IHappilStatement>());
			
			var nestedScope1 = new StatementScope(new List<IHappilStatement>());

			var outerTryStatement = new TryStatement(() => { });
			var outerTryScope = new StatementScope(new List<IHappilStatement>(), outerTryStatement, ExceptionBlockType.Try);

			var innerTryStatement = new TryStatement(() => { });
			var innerTryScope = new StatementScope(new List<IHappilStatement>(), innerTryStatement, ExceptionBlockType.Try);

			var nestedScope2 = new StatementScope(new List<IHappilStatement>());

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);

			//-- Assert

			Assert.That(StatementScope.Current.Depth, Is.EqualTo(5));
			Assert.That(result, Is.SameAs(outerTryStatement));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void FindOutermostExceptionScopeWithin_InsideFinally_Throw()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var homeScope = new StatementScope(new List<IHappilStatement>());
			
			var tryStatement = new TryStatement(() => { });
			var finallyScope = new StatementScope(new List<IHappilStatement>(), tryStatement, ExceptionBlockType.Finally);

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void FindOutermostExceptionScopeWithin_TryNestedInsideFinally_Throw()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());
			var homeScope = new StatementScope(new List<IHappilStatement>());

			var outerTryStatement = new TryStatement(() => { });
			var outerFinallyScope = new StatementScope(new List<IHappilStatement>(), outerTryStatement, ExceptionBlockType.Finally);

			var innerTryStatement = new TryStatement(() => { });
			var innerTryScope = new StatementScope(new List<IHappilStatement>(), innerTryStatement, ExceptionBlockType.Try);

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void FindOutermostExceptionScopeWithin_RootFinallyHomeCatch_DoNotThrow_Found()
		{
			//-- Arrange

			var rootScope = new StatementScope(m_Class, m_Method, new List<IHappilStatement>());

			var outerTryStatement = new TryStatement(() => { });
			var outerFinallyScope = new StatementScope(new List<IHappilStatement>(), outerTryStatement, ExceptionBlockType.Finally);

			var homeScope = new StatementScope(new List<IHappilStatement>());

			var innerTryStatement = new TryStatement(() => { });
			var innerCatchScope = new StatementScope(new List<IHappilStatement>(), innerTryStatement, ExceptionBlockType.Catch);

			//-- Act

			var result = StatementScope.Current.FindOutermostTryStatementWithin(homeScope);

			//-- Assert

			Assert.That(result, Is.SameAs(innerTryStatement));
		}
	}
}
