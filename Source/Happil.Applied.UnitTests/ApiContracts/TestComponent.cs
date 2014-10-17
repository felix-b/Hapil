using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happil.Applied.UnitTests.ApiContracts
{
	internal class TestComponent : ITestComponent
	{
		public TestComponent()
		{
			Log = new List<string>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region ITestComponent Members

		public void AMethodWithNotNullString(string str)
		{
			LogCall("AMethodWithNotNullString", str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotEmptyString(string str)
		{
			LogCall("AMethodWithNotEmptyString", str);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Stream ANotNullFunction(int x)
		{
			LogCall("ANotNullFunction", x);

			if ( x != 0 )
			{
				return new MemoryStream(Encoding.Unicode.GetBytes(x.ToString()));
			}
			else
			{
				return null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string ANotEmptyStringFunction(int x)
		{
			LogCall("ANotEmptyStringFunction", x);

			if ( x > 0 )
			{
				return x.ToString();
			}
			else if ( x == 0 )
			{
				return null;
			}
			else
			{
				return string.Empty;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotNullOutParam(int size, out Stream data)
		{
			LogCall("AMethodWithNotNullOutParam", size);

			if ( size > 0 )
			{
				data = new MemoryStream(new byte[size]);
			}
			else
			{
				data = null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotNullRefParam(int size, ref Stream data)
		{
			LogCall(
				"AMethodWithNotNullRefParam", 
				size, 
				data != null ? "Stream[" + data.Length + "]" : "NULL");

			if ( size > 0 )
			{
				data = new MemoryStream(new byte[size]);
			}
			else if ( size < 0 )
			{
				data = null;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotEmptyCollection(string[] items)
		{
			LogCall("AMethodWithNotEmptyCollection", items.Cast<object>().ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotNullListItems(IList<string> items)
		{
			LogCall("AMethodWithNotNullListItems", items.Cast<object>().ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotEmptyListItems(IList<string> items)
		{
			LogCall("AMethodWithNotEmptyListItems", items.Cast<object>().ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotNullCollectionItems(ICollection<string> items)
		{
			LogCall("AMethodWithNotNullCollectionItems", items.Cast<object>().ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithNotNullCollectionItems(System.Collections.ICollection items)
		{
			LogCall("AMethodWithNotNullCollectionItems", items.Cast<object>().ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithMultipleChecksOnParameter(IList<string> items)
		{
			LogCall("AMethodWithMultipleChecksOnParameter", items.Cast<object>().ToArray());
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithIntRanges(int number1, int number2, int number3, int number4)
		{
			LogCall("AMethodWithIntRanges", number1, number2, number3, number4);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithDoubleRanges(double number1, double number2, double number3, double number4)
		{
			LogCall("AMethodWithDoubleRanges", number1, number2, number3, number4);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void AMethodWithStringLength(string str1, string str2, string str3)
		{
			LogCall("AMethodWithStringLength", str1, str2, str3);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public List<string> Log { get; private set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void LogCall(string methodName, params object[] args)
		{
			var text = new StringBuilder(methodName + '(');

			for ( int i = 0 ; i < args.Length ; i++ )
			{
				text.Append(args[i]);

				if ( i < args.Length - 1 )
				{
					text.Append(',');
				}
			}

			text.Append(')');

			Log.Add(text.ToString());
		}
	}
}
