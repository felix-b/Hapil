using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
	}
}
