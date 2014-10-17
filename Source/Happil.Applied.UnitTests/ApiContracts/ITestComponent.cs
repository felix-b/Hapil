using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Applied.ApiContracts;

namespace Happil.Applied.UnitTests.ApiContracts
{
	public interface ITestComponent
	{
		void AMethodWithNotNullString([NotNull] string str);
		
		void AMethodWithNotEmptyString([NotEmpty] string str);
		
		[return: NotNull] Stream ANotNullFunction(int x);
		
		[return: NotEmpty] string ANotEmptyStringFunction(int x);

		void AMethodWithNotNullOutParam(int size, [NotNull] out Stream data);

		void AMethodWithNotNullRefParam(int size, [NotNull] ref Stream data);

		void AMethodWithNotEmptyCollection([NotEmpty] string[] items);

		void AMethodWithNotNullListItems([ItemsNotNull] IList<string> items);

		void AMethodWithNotEmptyListItems([ItemsNotEmpty] IList<string> items);

		void AMethodWithNotNullCollectionItems([ItemsNotNull] ICollection<string> items);

		void AMethodWithNotNullCollectionItems([ItemsNotNull] System.Collections.ICollection items);

		//TODO: verify that checks are invoked in declaration order for input parameters
		void AMethodWithMultipleChecksOnParameter([ItemsNotNull, ItemsNotEmpty] IList<string> items);

		void AMethodWithIntRanges(
			[InRange(100, 200, Exclusive = true)] int number1,
			[InRange(100, 200)] int number2,
			[InRange(Min = 100)] int number3,
			[InRange(Max = 200)] int number4);

		void AMethodWithDoubleRanges(
			[InRange(0, 1, Exclusive = true)] double number1,
			[InRange(0, 1)] double number2,
			[InRange(Min = 0)] double number3,
			[InRange(Max = 1)] double number4);

		void AMethodWithStringLength(
			[Length(3, 10)] string str1,
			[MinLength(3)] string str2,
			[MaxLength(10)] string str3);

		void AMethodWithArrayOfPositiveInts([Positive] int[] values);
	}
}
