using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using TT = Happil.TypeTemplate;

namespace Happil.UnitTests
{
	[TestFixture]
	public class HelpersTests
	{
		[Test]
		public void GetMethodInfoFromLambda_InstanceNonGenericOfNonGenericType()
		{
			//-- Arrange

			Expression<Func<AncestorRepository.IFewMethods, Func<int, string>>> lambda = ((x) => x.Five);
			var expectedMethod = typeof(AncestorRepository.IFewMethods).GetMethod("Five");

			//-- Act

			var actualMethod = Helpers.ResolveMethodFromLambda(lambda);

			//-- Assert

			Assert.That(expectedMethod, Is.Not.Null);
			Assert.That(actualMethod, Is.SameAs(expectedMethod));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void GetMethodInfoFromLambda_StaticNonGenericOfNonGenericType()
		{
			//-- Arrange

			Expression<Func<int, string, string>> lambda = (x, y) => NonGenericStaticClass.One(x, y);
			var expectedMethod = typeof(NonGenericStaticClass).GetMethod("One", BindingFlags.Static | BindingFlags.Public);

			//-- Act

			var actualMethod = Helpers.ResolveMethodFromLambda(lambda);

			//-- Assert

			Assert.That(expectedMethod, Is.Not.Null);
			Assert.That(actualMethod, Is.SameAs(expectedMethod));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void GetMethodInfoFromLambda_InstanceGenericOfNonGenericType()
		{
			//-- Arrange

			using ( TT.CreateScope(typeof(TT.TProperty), typeof(int)) )
			{
				Expression<Func<NonGenericClass, Func<TT.TProperty, TT.TProperty, TT.TProperty>>> lambda = x => x.GenericOne;
				Expression<Func<NonGenericClass, Func<int, int, int>>> lambdaNoTemplate = x => x.GenericOne;

				var expectedMethod = typeof(NonGenericClass).GetMethod("GenericOne").MakeGenericMethod(typeof(int));

				//-- Act

				var actualMethod = Helpers.ResolveMethodFromLambda(lambda);
				var actualMethodNoTemplate = Helpers.ResolveMethodFromLambda(lambdaNoTemplate);

				//-- Assert

				Assert.That(expectedMethod, Is.Not.Null);
				Assert.That(actualMethod, Is.SameAs(expectedMethod));
				Assert.That(actualMethodNoTemplate, Is.SameAs(expectedMethod));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void GetMethodInfoFromLambda_StaticGenericOfNonGenericType()
		{
			//-- Arrange

			using ( TT.CreateScope(typeof(TT.TIndex1), typeof(int), typeof(TT.TIndex2), typeof(string)) )
			{
				Expression<Func<TT.TIndex1, TT.TIndex1, TT.TIndex2>> lambda = (x, y) => NonGenericStaticClass.Two<TT.TIndex1, TT.TIndex2>(x, y);
				Expression<Func<int, int, string>> lambdaNoTemplate = (x, y) => NonGenericStaticClass.Two<int, string>(x, y);
				
				var expectedMethod = typeof(NonGenericStaticClass)
					.GetMethod("Two", BindingFlags.Public | BindingFlags.Static)
					.MakeGenericMethod(typeof(int), typeof(string));

				//-- Act

				var actualMethod = Helpers.ResolveMethodFromLambda(lambda);
				var actualMethodNoTemplate = Helpers.ResolveMethodFromLambda(lambdaNoTemplate);

				//-- Assert

				Assert.That(expectedMethod, Is.Not.Null);
				Assert.That(actualMethod, Is.SameAs(expectedMethod));
				Assert.That(actualMethodNoTemplate, Is.SameAs(expectedMethod));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		[Test]
		public void GetMethodInfoFromLambda_InstanceNonGenericOfGenericType()
		{
			//-- Arrange

			using ( TT.CreateScope(typeof(TT.TProperty), typeof(string)) )
			{
				Expression<Func<GenericClass<TT.TProperty>, Func<TT.TProperty, TT.TProperty, TT.TProperty>>> lambda = ((x) => x.One);
				Expression<Func<GenericClass<string>, Func<string, string, string>>> lambdaNoTemplate = ((x) => x.One);
				
				var expectedMethod = typeof(GenericClass<string>).GetMethod("One");

				//-- Act

				var actualMethod = Helpers.ResolveMethodFromLambda(lambda);
				var actualMethodNoTemplate = Helpers.ResolveMethodFromLambda(lambdaNoTemplate);

				//-- Assert

				Assert.That(expectedMethod, Is.Not.Null);
				Assert.That(actualMethod, Is.SameAs(expectedMethod));
				Assert.That(actualMethodNoTemplate, Is.SameAs(expectedMethod));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void GetMethodInfoFromLambda_StaticNonGenericOfGenericType()
		{
			//-- Arrange

			using ( TT.CreateScope(typeof(TT.TProperty), typeof(string)) )
			{
				Expression<Func<TT.TProperty, TT.TProperty, int>> lambda1 = (x, y) => GenericStaticClass<TT.TProperty>.One(x, y);
				Expression<Func<string, string, int>> lambda1NoTemplate = (x, y) => GenericStaticClass<string>.One(x, y);
				var expectedMethod1 = typeof(GenericStaticClass<string>).GetMethod("One", BindingFlags.Public | BindingFlags.Static);

				Expression<Func<TT.TProperty, TT.TProperty, int>> lambda2 = (x, y) => GenericStaticClass<TT.TProperty>.Two(x, y);
				Expression<Func<string, string, int>> lambda2NoTemplate = (x, y) => GenericStaticClass<string>.Two(x, y);
				var expectedMethod2 = typeof(GenericStaticClass<string>).GetMethod("Two", BindingFlags.Public | BindingFlags.Static);

				//-- Act

				var actualMethod1 = Helpers.ResolveMethodFromLambda(lambda1);
				var actualMethod1NoTemplate = Helpers.ResolveMethodFromLambda(lambda1NoTemplate);

				var actualMethod2 = Helpers.ResolveMethodFromLambda(lambda2);
				var actualMethod2NoTemplate = Helpers.ResolveMethodFromLambda(lambda2NoTemplate);

				//-- Assert

				Assert.That(expectedMethod1, Is.Not.Null);
				Assert.That(actualMethod1, Is.SameAs(expectedMethod1));
				Assert.That(actualMethod1NoTemplate, Is.SameAs(expectedMethod1));

				Assert.That(expectedMethod2, Is.Not.Null);
				Assert.That(actualMethod2, Is.SameAs(expectedMethod2));
				Assert.That(actualMethod2NoTemplate, Is.SameAs(expectedMethod2));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void GetMethodInfoFromLambda_InstanceGenericOfGenericType()
		{
			using ( TT.CreateScope(typeof(TT.TArg1), typeof(string), typeof(TT.TReturn), typeof(TimeSpan)) )
			{
				//-- Arrange

				Expression<Func<GenericClass<TT.TArg1>, Func<TT.TArg1, TT.TArg1, TT.TReturn>>> lambda2 = ((x) => x.Two<TT.TReturn>);
				Expression<Func<GenericClass<string>, Func<string, string, TimeSpan>>> lambda2NoTemplate = ((x) => x.Two<TimeSpan>);
				var expectedMethod2 = typeof(GenericClass<string>).GetMethod("Two").MakeGenericMethod(typeof(TimeSpan));

				Expression<Func<GenericClass<TT.TArg1>, Func<TT.TArg1, TT.TArg1, TT.TReturn>>> lambda3 = ((x) => x.Three<TT.TReturn>);
				Expression<Func<GenericClass<string>, Func<string, string, TimeSpan>>> lambda3NoTemplate = ((x) => x.Three<TimeSpan>);
				var expectedMethod3 = typeof(GenericClass<string>).GetMethod("Three").MakeGenericMethod(typeof(TimeSpan));

				//-- Act

				var actualMethod2 = Helpers.ResolveMethodFromLambda(lambda2);
				var actualMethod2NoTemplate = Helpers.ResolveMethodFromLambda(lambda2NoTemplate);

				var actualMethod3 = Helpers.ResolveMethodFromLambda(lambda3);
				var actualMethod3NoTemplate = Helpers.ResolveMethodFromLambda(lambda3NoTemplate);

				//-- Assert

				Assert.That(expectedMethod2, Is.Not.Null);
				Assert.That(actualMethod2, Is.SameAs(expectedMethod2));
				Assert.That(actualMethod2NoTemplate, Is.SameAs(expectedMethod2));

				Assert.That(expectedMethod3, Is.Not.Null);
				Assert.That(actualMethod3, Is.SameAs(expectedMethod3));
				Assert.That(actualMethod3NoTemplate, Is.SameAs(expectedMethod3));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Test]
		public void GetMethodInfoFromLambda_StaticGenericOfGenericType()
		{
			using ( TT.CreateScope(typeof(TT.TArg1), typeof(int), typeof(TT.TReturn), typeof(string)) )
			{
				//-- Arrange

				Expression<Func<TT.TArg1, TT.TArg1, TT.TReturn>> lambda3 = (x, y) => GenericStaticClass<TT.TArg1>.Three<TT.TReturn>(x, y);
				Expression<Func<int, int, string>> lambda3NoTemplate = (x, y) => GenericStaticClass<int>.Three<string>(x, y);
				var expectedMethod3 = typeof(GenericStaticClass<int>)
					.GetMethod("Three", BindingFlags.Public | BindingFlags.Static)
					.MakeGenericMethod(typeof(string));

				Expression<Func<TT.TArg1, TT.TArg1, TT.TReturn>> lambda4 = (x, y) => GenericStaticClass<TT.TArg1>.Four<TT.TReturn>(x, y);
				Expression<Func<int, int, string>> lambda4NoTemplate = (x, y) => GenericStaticClass<int>.Four<string>(x, y);
				var expectedMethod4 = typeof(GenericStaticClass<int>)
					.GetMethod("Four", BindingFlags.Public | BindingFlags.Static)
					.MakeGenericMethod(typeof(string));

				//-- Act

				var actualMethod3 = Helpers.ResolveMethodFromLambda(lambda3);
				var actualMethod3NoTemplate = Helpers.ResolveMethodFromLambda(lambda3NoTemplate);
				
				var actualMethod4 = Helpers.ResolveMethodFromLambda(lambda4);
				var actualMethod4NoTemplate = Helpers.ResolveMethodFromLambda(lambda4NoTemplate);

				//-- Assert

				Assert.That(expectedMethod3, Is.Not.Null);
				Assert.That(actualMethod3, Is.SameAs(expectedMethod3));

				Assert.That(expectedMethod4, Is.Not.Null);
				Assert.That(actualMethod4, Is.SameAs(expectedMethod4));
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class NonGenericClass
		{
			public T GenericOne<T>(T x, T y)
			{
				throw new NotImplementedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class NonGenericStaticClass
		{
			public static string One(int x, string y)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static TOut Two<TIn, TOut>(TIn x, TIn y)
			{
				throw new NotImplementedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class GenericClass<T>
		{
			public T One(T x, T y)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TResult Two<TResult>(T x, T y)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TResult Three<TResult>(T x, T y)
			{
				throw new NotImplementedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class GenericStaticClass<TIn>
		{
			public static int One(TIn x, TIn y)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static int Two(TIn x, TIn y)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static TOut Three<TOut>(TIn x, TIn y)
			{
				throw new NotImplementedException();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public static TOut Four<TOut>(TIn x, TIn y)
			{
				throw new NotImplementedException();
			}
		}
	}
}
