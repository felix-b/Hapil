using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Happil.UnitTests
{
	public static class AncestorRepository
	{
		public class BaseOne
		{
			public void VoidMethod()
			{
			}
			public void VoidMethodWithOneArg(int number)
			{
			}
			public void VoidMethodWithManyArgs(int number, string text)
			{
			}
			public void VoidMethodWithManyArgs(int number, DateTime date)
			{
			}
			public void VoidMethodWithManyArgs(int number, string text, DateTime date)
			{
			}
			public int FuncWithNoArgs()
			{
				return 123;
			}
			public int FuncWithOneArg(int number)
			{
				return 123;
			}
			public int FuncWithManyArgs(int number, string text)
			{
				return 123;
			}
			public int FuncWithManyArgs(int number, string text, DateTime date)
			{
				return 123;
			}
			public virtual void VirtualVoidMethod()
			{
			}
			public virtual int VirtualFuncWithNoArgs()
			{
				return Int32.MaxValue;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class BaseTwo
		{
			public abstract int FirstValue { get; set; }
			public abstract string SecondValue { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class BaseThree
		{
			public abstract int Add(int x, int y);
			public abstract int TakeNextCounter();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IFewMethods
		{
			void One();
			void Two(int n);
			int Three();
			int Four(string s);
			string Five(int n);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMoreMethods
		{
			void One();
			void Two();
			void Three(int n);
			void Four(string s);
			void Five(int n, string s);
			void Six(string s, int n);
			void Seven(TimeSpan t, string s, int n);
			void Eight(string s, int n, TimeSpan t);
			int Eleven();
			string Twelwe();
			int Thirteen(string s);
			string Fourteen(int n);
			int Fifteen(TimeSpan t, string s);
			string Sixteen(int n, TimeSpan t);
			object Seventeen(TimeSpan t, string s, int n);
			IEnumerable<int> Eighteen(string s, int n, TimeSpan t);
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IFewReadWriteProperties
		{
			int AnInt { get; set; }
			string AString { get; set; }
			object AnObject { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IReadOnlyAndReadWriteProperties
		{
			int AnInt { get; }
			string AString { get; }
			object AnObject { get; }
			int AnotherInt { get; set; }
			string AnotherString { get; set; }
			object AnotherObject { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IFewPropertiesWithIndexers
		{
			int AnInt { get; set; }
			string AString { get; set; }
			object AnObject { get; set; }
			int this[string  s] { get; set; }
			string this[int n, string s] { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IOneProperty
		{
			int PropertyOne { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITwoProperties : IOneProperty
		{
			int PropertyTwo { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IPropertyContainerOne
		{
			IOneProperty One { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IPropertyContainerTwo
		{
			ITwoProperties Two { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IPropertyContainersReader
		{
			int SumAll(IPropertyContainerOne container1, IPropertyContainerTwo container2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IPropertyContainersWriter
		{
			void SetAll(IPropertyContainerOne container1, IPropertyContainerTwo container2, int value);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITargetObjectCaller
		{
			object CallTheTarget(object target);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITargetValueTypeCaller
		{
			object CallTheTarget(int value);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StatementTester
		{
			public abstract int DoTest(int input);

			public virtual bool Predicate(int input)
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StatementTester2
		{
			public abstract int DoTest(int x, int y);
			
			public virtual bool Predicate(int x, int y)
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StatementTester3
		{
			public abstract void DoTest(IEnumerable<int> input, IList<int> output);

			public virtual bool Predicate(int item)
			{
				return false;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StatementTester4
		{
			public abstract long DoTest(long input);

			public virtual bool Predicate(long item)
			{
				return false;
			}
		}
	}
}
