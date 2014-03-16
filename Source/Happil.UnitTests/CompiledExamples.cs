using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using NUnit.Framework;

// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertClosureToMethodGroup

namespace Happil.UnitTests
{
	public static class CompiledExamples
	{
		public static IDisposable InputDisposable { get; set; }
		public static Exception OutputException { get; set; }
		public static string OutputString { get; set; }
		public static List<string> OutputList { get; set; }
		public static TimeSpan TimeSpanValue { get; set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ExpressionExamples
		{
			private static TimeSpan s_TimeValue;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private static bool LogicalAndExample(bool p, int y)
			{
				return (p && y == 10);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private static TimeSpan NewStruct()
			{
				s_TimeValue = new TimeSpan();
				TimeSpanValue = new TimeSpan();
				var ts1 = new TimeSpan();
				var ts2 = new TimeSpan(1, 2, 3, 4);
				var ts3 = TimeSpan.FromDays(1);
				return ts1.Add(ts2).Add(ts3).Add(s_TimeValue);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ExampleTester1 : AncestorRepository.StatementTester
		{
			public override int DoTest(int input)
			{
				if ( input == 0 )
				{
					return 111;
				}

				return 999;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int IfThenElseExample(int input)
			{
				var result = input;

				if ( result == 1 )
				{
					result = 11;
				}
				else if ( result == 11 )
				{
					result = 22;
				}
				else if ( result == 22 )
				{
					result = 33;
				}

				return result;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static void ForeachExample(IEnumerable<int> input, IList<int> output)
			{
				foreach ( var element in input )
				{
					output.Add(element);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int WhileExample(int count)
			{
				var iterationsLeft = count;
				var iterationsDone = 0;

				while ( iterationsLeft > 0 )
				{
					iterationsDone++;
					iterationsLeft--;
				}

				return iterationsDone;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int SwithcExample1(int num)
			{
				switch ( num )
				{
					case 0:
						Console.WriteLine("000");
						break;
					case 2:
						Console.WriteLine("222");
						break;
					case 4:
						Console.WriteLine("444");
						break;
					default:
						Console.WriteLine("999");
						break;
				}

				Console.WriteLine("END");
				return 0;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int SwithcExample2(int num)
			{
				switch ( num )
				{
					case 1:
						Console.WriteLine("111");
						break;
					case 3:
						Console.WriteLine("333");
						break;
					case 5:
						Console.WriteLine("555");
						break;
					case 7:
						Console.WriteLine("777");
						break;
					case 10:
						Console.WriteLine("191919");
						break;
				}

				Console.WriteLine("END");
				return 0;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int SwithcExample3(int num)
			{
				byte x = (byte)num;
				
				switch ( x )
				{
					case 0:
					case 2:
						Console.WriteLine("000222");
						break;
					case 4:
					case 6:
						Console.WriteLine("444666");
						break;
					default:
						Console.WriteLine("999");
						break;
				}

				Console.WriteLine("END");
				return 0;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int SwithcExample4(int num)
			{
				switch ( num.ToString() )
				{
					case "1":
					case "2":
						Console.WriteLine("000222");
						break;
					case "4":
					case "6":
						Console.WriteLine("444666");
						break;
					default:
						Console.WriteLine("999");
						break;
				}

				Console.WriteLine("END");
				return 0;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int SwithcExample5(int num)
			{
				var day = (DayOfWeek)num;

				switch ( day )
				{
					case DayOfWeek.Thursday:
					case DayOfWeek.Tuesday:
						Console.WriteLine("THU-TUE");
						break;
					case DayOfWeek.Monday:
					case DayOfWeek.Sunday:
						Console.WriteLine("MON-SUN");
						break;
					default:
						Console.WriteLine("999");
						break;
				}

				Console.WriteLine("END");
				return 0;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int SwithcExample7(int num)
			{
				var s = num.ToString();

				switch ( s )
				{
					case "10":
						return 1000;
					case "20":
						return 2000;
					case "30":
						return 3000;
					default:
						return 9999;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int CompareDifferentTypes()
			{
				string s1 = "A";
				string s2 = "B";

				if ( s1 == s2 )
				{
					double d1 = 123;
					double d2 = 456;

					if ( d1 != d2 )
					{
						DateTime dt1 = DateTime.Now;
						DateTime dt2 = DateTime.UtcNow;

						if ( dt1 == dt2 )
						{
							return 0;
						}
					}
				}

				return 1;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int TryCatchAllExample(int input)
			{
				while ( input-- > 0 )
				{
					try
					{
						if ( input == 888 )
						{
							throw new ExceptionRepository.TestExceptionOne("TEST888");
						}
						try
						{
							if ( input == 666 )
							{
								continue;
							}
						}
						catch
						{
							if ( input == 777 )
							{
								break;
							}
						}

						OutputException = null;
						return 111;
					}
					catch ( Exception  e )
					{
						OutputException = e;

						if ( input == 888 )
						{
							return 999;
						}
					}
				}

				Console.WriteLine("-999");
				return -999; // should never get here!
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ExampleTester2 : AncestorRepository.StatementTester
		{
			public sealed override int DoTest(int input)
			{
				List<string> outputList = OutputList;
				while (true)
				{
					if (input <= 0)
					{
						break;
					}
					outputList.Add(input.ToString());
					try
					{
						if (input == 11)
						{
							outputList.Add("BREAK");
							break;
						}
					}
					finally
					{
						outputList.Add("FINALLY");
					}
					input--;
				}
				return 0;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class ExampleRefOutArgs : AncestorRepository.IFewMethodsWithRefOutArgs
		{
			#region IFewMethodsWithRefOutArgs Members

			public string One(ref string s1, out string s2)
			{
				s2 = s1 + s1;
				s1 = default(string);//"Z";
				return s1 + s2;
			}

			public int Two(ref int n1, out int n2)
			{
				n2 = n1 + n1;
				n1 = default(int);//99;
				return n1 + n2;
			}

			public TimeSpan Three(ref TimeSpan t1, out TimeSpan t2)
			{
				t2 = t1 + t1;
				t1 = default(TimeSpan);//TimeSpan.FromHours(9.0);
				return t1 + t2;
			}

			#endregion
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class OperatorExamples
		{
			public void BitwiseNotExample()
			{
				int x1 = 0x01;
				uint x2 = 0x01;
				long x3 = 0x01;
				ulong x4 = 0x01;

				int r1 = ~x1;
				uint r2 = ~x2;
				long r3 = ~x3;
				ulong r4 = ~x4;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void UnaryPlusExample()
			{
				int x1 = -0x01;
				uint x2 = 0x01;
				long x3 = -0x01;
				ulong x4 = 0x01;
				float x5 = -0x01;
				decimal x6 = -0x01;
				double x7 = -0x01;

				int r1 = +x1;
				uint r2 = +x2;
				long r3 = +x3;
				ulong r4 = +x4;
				float r5 = +x5;
				decimal r6 = +x6;
				double r7 = +x7;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void UnaryMinusExample()
			{
				int x1 = -0x01;
				long x3 = -0x01;
				float x5 = -0x01;
				decimal x6 = -0x01;
				double x7 = -0x01;

				int r1 = -x1;
				long r3 = -x3;
				float r5 = -x5;
				decimal r6 = -x6;
				double r7 = -x7;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void PostfixIncrementExample1()
			{
				int x1 = 1;
				//long x3 = 1;
				//float x5 = 1;
				//decimal x6 = 1;
				//double x7 = 1;

				var r1 = x1++;
				//var r1 = OutputValue++;
				//var r3 = x3++;
				//var r5 = x5++;
				//var r6 = x6++;
				//var r7 = x7++;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void PostfixIncrementExample2()
			{
				//int x1 = 1;
				//long x3 = 1;
				//float x5 = 1;
				decimal x6 = 1;
				//double x7 = 1;

				//OutputValue = x1++;
				//var r1 = OutputValue++;
				//var r3 = x3++;
				//var r5 = x5++;
				var r6 = x6++;
				//var r7 = x7++;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void PostfixIncrementExampleByRef(ref int x, ref float y, ref decimal z)
			{
				x++;
				y++;
				z++;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void PrefixIncrementExample()
			{
				int x1 = 1;
				//long x3 = 1;
				//float x5 = 1;
				//decimal x6 = 1;
				//double x7 = 1;

				var r1 = ++OutputValue;
				//var r3 = ++x3;
				//var r5 = ++x5;
				//var r6 = ++x6;
				//var r7 = ++x7;

				Console.WriteLine(x1);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int ModulusExample(int x, int y)
			{
				return (x % y);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public bool LogicalXorExample(bool x, bool y)
			{
				return (x ^ y);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void CastExamples()
			{
				//OpCodes.Unbox_Any;
				//OpCodes.Isinst;

				int x0 = 1;
				object x1 = (object)x0;
				x0 = (int)x1;

				object y0 = new System.IO.MemoryStream();
				IDisposable y1 = (y0 as IDisposable);

				Console.WriteLine("---2---");
				int? nullableNum = (y0 as int?);

				Console.WriteLine("---3---");
				object obj1 = (nullableNum as object);

				Console.WriteLine("---4---");
				object obj2 = (x0 as object);

				Console.WriteLine("---5---");
				object obj3 = (nullableNum as object);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string NullCoalesceExample(string s1, string s2)
			{
				return (s1 ?? s2);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string GetOutputString()
			{
				return OutputValue.ToString();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public int OutputValue { get; set; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class StaticConstructorExample
		{
			static StaticConstructorExample()
			{
				OutputList.Add(".CCTOR");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class LambdaExample
		{
			public IEnumerable<string> SelectStrings(IEnumerable<string> source)
			{
				return source.Where(StaticLambdaMethod);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private static bool CallDelegate(string s, Func<string, bool> func)
			{
				return func(s);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private bool MakeDelegate(string s)
			{
				Console.WriteLine("---1---");
				Func<string, bool> staticFunc = StaticLambdaMethod;
				Console.WriteLine("---2---");
				Func<string, bool> instanceFunc = InstanceLambdaMethod;

				Console.WriteLine("---3---");
				var result1 = staticFunc(s);
				Console.WriteLine("---4---");
				var result2 = instanceFunc(s);

				Console.WriteLine("---5---");
				return true;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private bool InstanceLambdaMethod(string s)
			{
				return (s.Length > 2);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private static bool StaticLambdaMethod(string s)
			{
				return (s.Length > 2);
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class EventExample : AncestorRepository.IFewEvents
		{
			#region IFewEvents Members

			public void RaiseOne()
			{
				if ( EventOne != null )
				{
					EventOne(this, EventArgs.Empty);
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string RaiseTwo(string input)
			{
				var args = new AncestorRepository.InOutEventArgs();

				if ( EventTwo != null )
				{
					EventTwo(this, args);
				}

				return args.OutputValue;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public event EventHandler EventOne;
			public event EventHandler<AncestorRepository.InOutEventArgs> EventTwo;

			#endregion
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class EnumerableExample : AncestorRepository.EnumerableTester
		{
			#region Overrides of EnumerableTester

			public override IEnumerable<string> DoBinaryTest(IEnumerable<string> first, IEnumerable<string> second)
			{
				if ( first.SequenceEqual(second) )
				{
					return new[] { "EQ" };
				}
				else
				{
					return new[] { "NEQ" };
				}
			}

			#endregion
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class HashCodeExample
		{
			private int m_First;
			private string m_Second;
			private TimeSpan m_Third;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public HashCodeExample()
			{
				m_First = 123;
				m_Second = "ABC";
				m_Third = TimeSpan.FromMinutes(5);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public sealed override int GetHashCode()
			{
				int num = 0;
				num = ((num << 5) + num) ^ this.m_First.GetHashCode();
				num = ((num << 5) + num) ^ this.m_Second.GetHashCode();
				return (((num << 5) + num) ^ this.m_Third.GetHashCode());
			}
		}
	}
}
