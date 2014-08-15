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

		void AMethodWithNotNullCollectionItems([ItemsNotNull] ICollection<string> items);

		void AMethodWithNotNullCollectionItems([ItemsNotNull] System.Collections.ICollection items);
	}
}
