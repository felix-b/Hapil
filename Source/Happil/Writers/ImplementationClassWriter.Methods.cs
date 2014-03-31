﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public partial class ImplementationClassWriter<TBase> : ClassWriterBase
	{
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ITemplateMethodSelector AllMethods(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, NA, NA, NA, NA, NA, NA, NA, NA>(this, TypeMemberCache.Of<TBase>().ImplementableMethods.SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IVoidMethodSelector Method(Expression<Func<TBase, Action>> method)
		{
			return new MethodSelector<NA, NA, NA, NA, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1> Method<TA1>(Expression<Func<TBase, Action<TA1>>> method)
		{
			return new MethodSelector<NA, TA1, NA, NA, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1, TA2> Method<TA1, TA2>(Expression<Func<TBase, Action<TA1, TA2>>> method)
		{
			return new MethodSelector<NA, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1, TA2, TA3> Method<TA1, TA2, TA3>(Expression<Func<TBase, Action<TA1, TA2, TA3>>> method)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4> Method<TA1, TA2, TA3, TA4>(Expression<Func<TBase, Action<TA1, TA2, TA3, TA4>>> method)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5> Method<TA1, TA2, TA3, TA4, TA5>(Expression<Func<TBase, Action<TA1, TA2, TA3, TA4, TA5>>> method)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6> Method<TA1, TA2, TA3, TA4, TA5, TA6>(Expression<Func<TBase, Action<TA1, TA2, TA3, TA4, TA5, TA6>>> method)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7> Method<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(Expression<Func<TBase, Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7>>> method)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, TA7, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> Method<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(Expression<Func<TBase, Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>>> method)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(this, Helpers.ResolveMethodFromLambda(method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IVoidMethodSelector Method(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, NA, NA, NA, NA, NA, NA, NA, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1> Method<TA1>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, NA, NA, NA, NA, NA, NA, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1, TA2> Method<TA1, TA2>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1, TA2, TA3> Method<TA1, TA2, TA3>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, NA, NA, NA, NA, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4> Method<TA1, TA2, TA3, TA4>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, NA, NA, NA, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5> Method<TA1, TA2, TA3, TA4, TA5>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, NA, NA, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6> Method<TA1, TA2, TA3, TA4, TA5, TA6>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, NA, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7> Method<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, TA7, NA>(this, methods);
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> Method<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(params MethodInfo[] methods)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(this, methods);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IFunctionMethodSelector<TReturn> Method<TReturn>(Expression<Func<TBase, Func<TReturn>>> method)
		{
			return new MethodSelector<TReturn, NA, NA, NA, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TReturn> Method<TA1, TReturn>(Expression<Func<TBase, Func<TA1, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, NA, NA, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TA2, TReturn> Method<TA1, TA2, TReturn>(Expression<Func<TBase, Func<TA1, TA2, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TReturn> Method<TA1, TA2, TA3, TReturn>(Expression<Func<TBase, Func<TA1, TA2, TA3, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, NA, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TReturn> Method<TA1, TA2, TA3, TA4, TReturn>(Expression<Func<TBase, Func<TA1, TA2, TA3, TA4, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, NA, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TReturn> Method<TA1, TA2, TA3, TA4, TA5, TReturn>(Expression<Func<TBase, Func<TA1, TA2, TA3, TA4, TA5, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, NA, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TReturn> Method<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>(Expression<Func<TBase, Func<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, NA, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn> Method<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>(Expression<Func<TBase, Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, NA>(this, Helpers.ResolveMethodFromLambda(method));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn> Method<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>(Expression<Func<TBase, Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>>> method)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(this, Helpers.ResolveMethodFromLambda(method));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IFunctionMethodSelector<TReturn> Function<TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, NA, NA, NA, NA, NA, NA, NA, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TReturn> Function<TA1, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, NA, NA, NA, NA, NA, NA, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TA2, TReturn> Function<TA1, TA2, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TReturn> Function<TA1, TA2, TA3, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, NA, NA, NA, NA, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TReturn> Function<TA1, TA2, TA3, TA4, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, NA, NA, NA, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TReturn> Function<TA1, TA2, TA3, TA4, TA5, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, NA, NA, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TReturn> Function<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, NA, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn> Function<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, NA>(this, methods);
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn> Function<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>(params MethodInfo[] methods)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(this, methods);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IVoidMethodSelector VoidMethods(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, NA, NA, NA, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1> VoidMethods<TA1>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, NA, NA, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1, TA2> VoidMethods<TA1, TA2>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1), typeof(TA2)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1, TA2, TA3> VoidMethods<TA1, TA2, TA3>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1), typeof(TA2), typeof(TA3)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4> VoidMethods<TA1, TA2, TA3, TA4>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5> VoidMethods<TA1, TA2, TA3, TA4, TA5>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6> VoidMethods<TA1, TA2, TA3, TA4, TA5, TA6>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7> VoidMethods<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, TA7, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7)).SelectIf(where));
		}
		public IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> VoidMethods<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<NA, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(this, m_Members.ImplementableMethods.OfSignature(typeof(void), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IFunctionMethodSelector<TReturn> NonVoidMethods<TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, NA, NA, NA, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TReturn> NonVoidMethods<TA1, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, NA, NA, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TA2, TReturn> NonVoidMethods<TA1, TA2, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, TA2, NA, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1), typeof(TA2)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TReturn> NonVoidMethods<TA1, TA2, TA3, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, NA, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1), typeof(TA2), typeof(TA3)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TReturn> NonVoidMethods<TA1, TA2, TA3, TA4, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, NA, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TReturn> NonVoidMethods<TA1, TA2, TA3, TA4, TA5, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, NA, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TReturn> NonVoidMethods<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, NA, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn> NonVoidMethods<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, NA>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7)).SelectIf(where));
		}
		public IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn> NonVoidMethods<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>(Func<MethodInfo, bool> where = null)
		{
			return new MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(this, m_Members.ImplementableMethods.OfSignature(typeof(TReturn), typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8)).SelectIf(where));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> ImplementEmpty();
			ImplementationClassWriter<TBase> Throw<TException>(string message);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface ITemplateMethodSelector : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<TemplateMethodWriter> body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IVoidMethodSelector : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter> body);
		}
		public interface IVoidMethodSelector<TA1> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>> body);
		}
		public interface IVoidMethodSelector<TA1, TA2> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body);
		}
		public interface IVoidMethodSelector<TA1, TA2, TA3> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body);
		}
		public interface IVoidMethodSelector<TA1, TA2, TA3, TA4> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body);
		}
		public interface IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body);
		}
		public interface IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body);
		}
		public interface IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body);
		}
		public interface IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public interface IFunctionMethodSelector<TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>> body);
		}
		public interface IFunctionMethodSelector<TA1, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body);
		}
		public interface IFunctionMethodSelector<TA1, TA2, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body);
		}
		public interface IFunctionMethodSelector<TA1, TA2, TA3, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body);
		}
		public interface IFunctionMethodSelector<TA1, TA2, TA3, TA4, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body);
		}
		public interface IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body);
		}
		public interface IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body);
		}
		public interface IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body);
		}
		public interface IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn> : IMethodSelectorBase
		{
			ImplementationClassWriter<TBase> Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class MethodSelector<TReturn, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> :
			IMethodSelectorBase,
			ITemplateMethodSelector,
			IVoidMethodSelector,
			IVoidMethodSelector<TA1>,
			IVoidMethodSelector<TA1, TA2>,
			IVoidMethodSelector<TA1, TA2, TA3>,
			IVoidMethodSelector<TA1, TA2, TA3, TA4>,
			IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5>,
			IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6>,
			IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7>,
			IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>,
			IFunctionMethodSelector<TReturn>,
			IFunctionMethodSelector<TA1, TReturn>,
			IFunctionMethodSelector<TA1, TA2, TReturn>,
			IFunctionMethodSelector<TA1, TA2, TA3, TReturn>,
			IFunctionMethodSelector<TA1, TA2, TA3, TA4, TReturn>,
			IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TReturn>,
			IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>,
			IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>,
			IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>
		{
			private readonly ClassType m_OwnerClass;
			private readonly ImplementationClassWriter<TBase> m_ClassWriter;
			private readonly MethodInfo[] m_SelectedMethods;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodSelector(ImplementationClassWriter<TBase> classWriter, IEnumerable<MethodInfo> selectedMethods)
				: this(classWriter, selectedMethods.ToArray())
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public MethodSelector(ImplementationClassWriter<TBase> classWriter, params MethodInfo[] selectedMethods)
			{
				m_OwnerClass = classWriter.OwnerClass;
				m_ClassWriter = classWriter;
				m_SelectedMethods = selectedMethods;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IMethodSelectorBase Members

			ImplementationClassWriter<TBase> IMethodSelectorBase.ImplementEmpty()
			{
				return DefineMethodImplementations(
					method => new EmptyMethodWriter(method), 
					writer => {});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			ImplementationClassWriter<TBase> IMethodSelectorBase.Throw<TException>(string message)
			{
				//TODO:redesign
				throw new NotImplementedException();
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region ITemplateMethodSelector Members

			ImplementationClassWriter<TBase> ITemplateMethodSelector.Implement(Action<TemplateMethodWriter> body)
			{
				return DefineMethodImplementations(
					method => new TemplateMethodWriter(method),
					body);
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IVoidMethodSelector<*> Members

			ImplementationClassWriter<TBase> IVoidMethodSelector.Implement(Action<VoidMethodWriter> body)
			{
				return DefineMethodImplementations(
					method => new VoidMethodWriter(method),
					body);
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1>.Implement(Action<VoidMethodWriter, Argument<TA1>> body)
			{
				return DefineMethodImplementations(
					method => new VoidMethodWriter(method),
					writer => body(writer, writer.Arg1<TA1>()));
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1, TA2>.Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>> body)
			{
				return DefineMethodImplementations(
					m => new VoidMethodWriter(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1, TA2, TA3>.Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
			{
				return DefineMethodImplementations(
					m => new VoidMethodWriter(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1, TA2, TA3, TA4>.Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
			{
				return DefineMethodImplementations(
					m => new VoidMethodWriter(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5>.Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
			{
				return DefineMethodImplementations(
					m => new VoidMethodWriter(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6>.Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
			{
				return DefineMethodImplementations(
					m => new VoidMethodWriter(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7>.Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
			{
				return DefineMethodImplementations(
					m => new VoidMethodWriter(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
			}
			ImplementationClassWriter<TBase> IVoidMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>.Implement(Action<VoidMethodWriter, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
			{
				return DefineMethodImplementations(
					m => new VoidMethodWriter(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IFunctionMethodSelector<*, TReturn> Members

			ImplementationClassWriter<TBase> IFunctionMethodSelector<TReturn>.Implement(Action<FunctionMethodWriter<TReturn>> body)
			{
				return DefineMethodImplementations(
					method => new FunctionMethodWriter<TReturn>(method),
					body);
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>> body)
			{
				return DefineMethodImplementations(
					method => new FunctionMethodWriter<TReturn>(method),
					writer => body(writer, writer.Arg1<TA1>()));
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TA2, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>> body)
			{
				return DefineMethodImplementations(
					m => new FunctionMethodWriter<TReturn>(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>()));
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TA2, TA3, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>> body)
			{
				return DefineMethodImplementations(
					m => new FunctionMethodWriter<TReturn>(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>()));
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TA2, TA3, TA4, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>> body)
			{
				return DefineMethodImplementations(
					m => new FunctionMethodWriter<TReturn>(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>()));
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>> body)
			{
				return DefineMethodImplementations(
					m => new FunctionMethodWriter<TReturn>(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>()));
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>> body)
			{
				return DefineMethodImplementations(
					m => new FunctionMethodWriter<TReturn>(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>()));
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>> body)
			{
				return DefineMethodImplementations(
					m => new FunctionMethodWriter<TReturn>(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>()));
			}
			ImplementationClassWriter<TBase> IFunctionMethodSelector<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TReturn>.Implement(Action<FunctionMethodWriter<TReturn>, Argument<TA1>, Argument<TA2>, Argument<TA3>, Argument<TA4>, Argument<TA5>, Argument<TA6>, Argument<TA7>, Argument<TA8>> body)
			{
				return DefineMethodImplementations(
					m => new FunctionMethodWriter<TReturn>(m),
					w => body(w, w.Arg1<TA1>(), w.Arg2<TA2>(), w.Arg3<TA3>(), w.Arg4<TA4>(), w.Arg5<TA5>(), w.Arg6<TA6>(), w.Arg7<TA7>(), w.Arg8<TA8>()));
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private ImplementationClassWriter<TBase> DefineMethodImplementations<TWriter>(
				Func<MethodMember, TWriter> writerFactory,
				Action<TWriter> implementation)
				where TWriter : MethodWriterBase
			{
				var methodsToImplement = m_OwnerClass.TakeNotImplementedMembers(m_SelectedMethods);

				foreach ( var method in methodsToImplement )
				{
					var methodFactory = new VirtualMethodFactory(m_OwnerClass, method);
					var methodMember = new MethodMember(m_OwnerClass, methodFactory);
					
					m_OwnerClass.AddMember(methodMember);
					
					using ( methodMember.CreateTypeTemplateScope() )
					{
						var writer = writerFactory(methodMember);
						implementation(writer);
					}
				}

				return m_ClassWriter;
			}
		}
	}
}
