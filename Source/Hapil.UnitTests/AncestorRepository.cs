using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace Hapil.UnitTests
{
	public static class AncestorRepository
	{
		public interface IMakerInterfaceOne
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMakerInterfaceTwo
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class BaseZero
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

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

		public interface ITester
		{
			void TestAction();
			string TestFunc();
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

		public interface IFewMethodsWithRefOutArgs
		{
			string One(ref string s1, out string s2);
			int Two(ref int n1, out int n2);
			TimeSpan Three(ref TimeSpan t1, out TimeSpan t2);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMethodsWithRefOutArgsOverloads
		{
			void One(ref string s, out int n);
			void One(string s, ref int n);
			void One(out string s, int n);
			void One(string s, int n);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IFewReadWriteProperties
		{
			int AnInt { get; set; }
			string AString { get; set; }
			object AnObject { get; set; }
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IMoreReadWriteProperties
        {
            int AnotherInt { get; set; }
            string AnotherString { get; set; }
            object AnotherObject { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IReadWritePropertiesContainer
        {
            IFewReadWriteProperties Few { get; set; }
            IMoreReadWriteProperties More { get; set; }
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

		public interface IFewEvents
		{
			void RaiseOne();
			string RaiseTwo(string input);
			event EventHandler EventOne;
			event EventHandler<InOutEventArgs> EventTwo;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class InOutEventArgs : EventArgs
		{
			public override string ToString()
			{
				return "IN{" + (InputValue ?? "") + "}/OUT{" + (OutputValue ?? "") + "}";
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string InputValue { get; set; }
			public string OutputValue { get; set; }
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class ObjectCreationTester
		{
			public abstract object DoTest();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StructCreationTester
		{
			public abstract void DoTest(TimeSpan value);
			public TimeSpan TimeValue { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IVersionControlled<T>
		{
			T RemoteVersion { get; }
			T LocalVersion { get; }
			T WorkingVersion { get; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IOperatorTester
		{
			OperatorInputOutput Unary(OperatorInputOutput input);
			OperatorInputOutput Binary(OperatorInputOutput left, OperatorInputOutput right);
			void NumbersByRef(ref int x, ref float y, ref decimal z);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IComparisonTester
		{
			bool CompareInt(int x, int y);
			bool CompareFloat(float x, float y);
			bool CompareDecimal(decimal x, decimal y);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class OperatorInputOutput
		{
			public bool BooleanValue { get; set; }
			public byte ByteValue { get; set; }
			public short ShortValue { get; set; }
			public int IntValue { get; set; }
			public uint UIntValue { get; set; }
			public long LongValue { get; set; }
			public ulong ULongValue { get; set; }
			public float FloatValue { get; set; }
			public decimal DecimalValue { get; set; }
			public double DoubleValue { get; set; }
			public string StringValue { get; set; }
			public TimeSpan TimeSpanValue { get; set; }
			public SqlInt32 SqlIntValue { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ICastTester
		{
			System.IO.Stream CastToStream(object input);
			int? CastToNullableInt(object input);
			object CastToObject(int input);
			object CastNullableToObject(int? input);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class EnumerableTester
		{
			public virtual IEnumerable<string> DoTest(IEnumerable<string> source)
			{
				throw new NotImplementedException();
			}
			public virtual IEnumerable<string> DoOutBooleanTest(IEnumerable<string> source, out bool result)
			{
				throw new NotImplementedException();
			}
			public virtual bool DoBooleanBinaryTest(IEnumerable<string> first, IEnumerable<string> second)
			{
				throw new NotImplementedException();
			}
			public virtual IEnumerable<int> DoCastingTest(IEnumerable<object> source)
			{
				throw new NotImplementedException();
			}
			public virtual IEnumerable<FirstLetterGroup> DoGroupingTest(IEnumerable<string> source)
			{
				throw new NotImplementedException();
			}
			public virtual bool DoBooleanTest(IEnumerable<string> source)
			{
				throw new NotImplementedException();
			}
			public virtual int DoIntTest(IEnumerable<string> source)
			{
				throw new NotImplementedException();
			}
			public virtual string DoStringTest(IEnumerable<string> source)
			{
				throw new NotImplementedException();
			}
			public virtual IEnumerable<string> DoBinaryTest(IEnumerable<string> first, IEnumerable<string> second)
			{
				throw new NotImplementedException();
			}
		}
		public class FirstLetterGroup
		{
			public string FirstLetter { get; set; }
			public string[] Values { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class StringTester
		{
			public virtual string DoTest(string input)
			{
				throw new NotImplementedException();
			}
			public virtual int DoIntTest(string input)
			{
				throw new NotImplementedException();
			}
			public virtual bool DoBooleanTest(string input)
			{
				throw new NotImplementedException();
			}
			public virtual char DoCharTest(string input)
			{
				throw new NotImplementedException();
			}
			public virtual char[] DoCharArrayTest(string input)
			{
				throw new NotImplementedException();
			}
			public virtual string DoBinaryTest(string first, string second)
			{
				throw new NotImplementedException();
			}
			public virtual int DoIntBinaryTest(string first, string second)
			{
				throw new NotImplementedException();
			}
			public virtual string[] DoSplitTest(string input)
			{
				throw new NotImplementedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract class DictionaryTester
		{
			public virtual void DoTest(IDictionary<int, string> dictionary)
			{
				throw new NotImplementedException();
			}
			public virtual int DoIntTest(IDictionary<int, string> dictionary)
			{
				throw new NotImplementedException();
			}
			public virtual string DoStringTest(IDictionary<int, string> dictionary)
			{
				throw new NotImplementedException();
			}
			public virtual bool DoBooleanTest(IDictionary<int, string> dictionary)
			{
				throw new NotImplementedException();
			}
			public virtual int[] DoKeysTest(IDictionary<int, string> dictionary)
			{
				throw new NotImplementedException();
			}
			public virtual string[] DoValuesTest(IDictionary<int, string> dictionary)
			{
				throw new NotImplementedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IFewGenericMethods
		{
			void One<T>();
			T Two<T>(T value);
			TOut Three<TIn, TOut>(TIn input);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IGenericIterfaceWithFewMethods<T>
		{
			void One();
			T Two(T value);
			TOut Three<TOut>(T input);
			TOut Four<TAux, TOut>(T input, TAux aux);
		}
    
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public abstract class LoggingBase
	    {
	        private readonly List<string> m_Log = new List<string>();

            //-------------------------------------------------------------------------------------------------------------------------------------------------

	        public string[] GetLog()
	        {
	            return m_Log.ToArray();
	        }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public string[] TakeLog()
            {
                var result = m_Log.ToArray();
                m_Log.Clear();
                return result;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

	        public void AddLog(string message)
	        {
	            m_Log.Add(message);
	        }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public interface IOneFuncLeft
	    {
	        string One();
	    }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IOneFuncRight
        {
            string One();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IOnePropertyLeft
        {
            string One { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IOnePropertyRight
        {
            string One { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public abstract class GenericBaseWithConstraints<TAbstract, TConcrete>
            where TAbstract : class
            where TConcrete : class, TAbstract
        {
            public abstract TAbstract AsAbstract { get; set; }
            public abstract TConcrete AsConcrete { get; set; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public abstract class GenericBaseWithStructConstraint<T> 
            where T : struct
        {
            public abstract string FormatValue(T value);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public interface IHaveType
        {
            Type TheType { get; }
        }
    }
}
