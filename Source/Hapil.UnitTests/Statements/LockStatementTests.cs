using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Hapil.UnitTests.Statements
{
	[TestFixture]
	public class LockStatementTests : NUnitEmittedTypesTestBase
	{
		[SetUp]
		public void SetUp()
		{
			ReadyToLockEvent = new ManualResetEvent(initialState: false);
			LockAcquiredEvent = new ManualResetEvent(initialState: false);
			ReadyToUnlockEvent = new ManualResetEvent(initialState: false);
			LockReleasedEvent = new ManualResetEvent(initialState: false);
			SyncRoot = new object();
			OutputException = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[TearDown]
		public void TearDown()
		{
			ReadyToLockEvent.Dispose();
			LockAcquiredEvent.Dispose();
			ReadyToUnlockEvent.Dispose();
			LockReleasedEvent.Dispose();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLockAndRelease()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var syncRoot = m.Local(initialValue: Static.Prop(() => SyncRoot));
					var readyToLock = m.Local(initialValue: Static.Prop(() => ReadyToLockEvent));
					var lockAcquired = m.Local(initialValue: Static.Prop(() => LockAcquiredEvent));
					var readyToUnlock = m.Local(initialValue: Static.Prop(() => ReadyToUnlockEvent));
					var lockReleased = m.Local(initialValue: Static.Prop(() => LockReleasedEvent));

					m.If(!readyToLock.Func<int, bool>(x => x.WaitOne, m.Const(1000)))
						.Then(() => m.Throw<Exception>("readyToLock wait failed"));

					m.Lock(syncRoot, 100).Do(() => {
						lockAcquired.Func<bool>(x => x.Set);
						
						m.If(!readyToUnlock.Func<int, bool>(x => x.WaitOne, m.Const(1000)))
							.Then(() => m.Throw<Exception>("readyToUnlock wait failed"));
					});

					lockReleased.Func<bool>(x => x.Set);
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			var testerTask = Task.Factory.StartNew(() => {
				tester.DoTest(0);
			});

			//-- Assert

			ReadyToLockEvent.Set();
			
			if ( !LockAcquiredEvent.WaitOne(1000) )
			{
				Assert.Fail("LockAcquiredEvent wait failed.");
			}

			if ( Monitor.TryEnter(SyncRoot) )
			{
				Assert.Fail("SyncRoot must have been locked!");
			}

			ReadyToUnlockEvent.Set();

			if ( !LockReleasedEvent.WaitOne(1000) )
			{
				Assert.Fail("LockReleasedEvent wait failed.");
			}

			if ( !Monitor.TryEnter(SyncRoot) )
			{
				Assert.Fail("SyncRoot must have been released!");
			}

			if ( !testerTask.Wait(1000) )
			{
				Assert.Fail("Tester task is stuck");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void TestLockAttemptTimedOut()
		{
			//-- Arrange

			DeriveClassFrom<AncestorRepository.StatementTester>()
				.DefaultConstructor()
				.Method<int, int>(cls => cls.DoTest).Implement((m, input) => {
					var syncRoot = m.Local(initialValue: Static.Prop(() => SyncRoot));

					m.Try(() => {
						m.Lock(syncRoot, 100).Do(() => { });
					})
					.Catch<Exception>(e => {
						Static.Prop(() => OutputException).Assign(e);
					});
				});

			//-- Act

			var tester = CreateClassInstanceAs<AncestorRepository.StatementTester>().UsingDefaultConstructor();
			
			Monitor.Enter(SyncRoot);
			
			var testerTask = Task.Factory.StartNew(() => {
				tester.DoTest(0);
			});

			//-- Assert

			if ( !testerTask.Wait(3000) )
			{
				Assert.Fail("Tester task is stuck");
			}

			Assert.That(OutputException, Is.Not.Null);
			Assert.That(OutputException, Is.InstanceOf<TimeoutException>());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static object SyncRoot { get; set; }
		public static ManualResetEvent ReadyToLockEvent { get; set; }
		public static ManualResetEvent LockAcquiredEvent { get; set; }
		public static ManualResetEvent ReadyToUnlockEvent { get; set; }
		public static ManualResetEvent LockReleasedEvent { get; set; }
		public static Exception OutputException { get; set; }
	}
}
