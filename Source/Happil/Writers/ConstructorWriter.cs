using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Expressions;
using Happil.Members;
using Happil.Operands;

namespace Happil.Writers
{
	public class ConstructorWriter : MethodWriterBase
	{
		private readonly ConstructorMember m_OwnerConstructor;
		private readonly Action<ConstructorWriter> m_Script;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ConstructorWriter(ConstructorMember ownerConstructor, Action<ConstructorWriter> script)
			: base(ownerConstructor)
		{
			m_OwnerConstructor = ownerConstructor;
			m_Script = script;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base()
		{
			var baseConstructor = FindBaseConstructor();
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1>(IOperand<TA1> arg1)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1, TA2>(IOperand<TA1> arg1, IOperand<TA2> arg2)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1), typeof(TA2));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1, arg2),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1, TA2, TA3>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1), typeof(TA2), typeof(TA3));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1, arg2, arg3),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1, TA2, TA3, TA4>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1, arg2, arg3, arg4),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1, TA2, TA3, TA4, TA5>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1, arg2, arg3, arg4, arg5),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1, TA2, TA3, TA4, TA5, TA6>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5, IOperand<TA6> arg6)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1, arg2, arg3, arg4, arg5, arg6),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5, IOperand<TA6> arg6, IOperand<TA7> arg7)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1, arg2, arg3, arg4, arg5, arg6, arg7),
				@operand: new ThisOperand<object>(OwnerMethod));
		}
		public void Base<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5, IOperand<TA6> arg6, IOperand<TA7> arg7, IOperand<TA8> arg8)
		{
			var baseConstructor = FindBaseConstructor(typeof(TA1), typeof(TA2), typeof(TA3), typeof(TA4), typeof(TA5), typeof(TA6), typeof(TA7), typeof(TA8));
			new UnaryExpressionOperand<object, object>(
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8),
				@operand: new ThisOperand<object>(OwnerMethod));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This()
		{
			throw new NotImplementedException();
		}
		public void This<TA1>(IOperand<TA1> arg1)
		{
			throw new NotImplementedException();
		}
		public void This<TA1, TA2>(IOperand<TA1> arg1, IOperand<TA2> arg2)
		{
			throw new NotImplementedException();
		}
		public void This<TA1, TA2, TA3>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3)
		{
			throw new NotImplementedException();
		}
		public void This<TA1, TA2, TA3, TA4>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4)
		{
			throw new NotImplementedException();
		}
		public void This<TA1, TA2, TA3, TA4, TA5>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5)
		{
			throw new NotImplementedException();
		}
		public void This<TA1, TA2, TA3, TA4, TA5, TA6>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5, IOperand<TA6> arg6)
		{
			throw new NotImplementedException();
		}
		public void This<TA1, TA2, TA3, TA4, TA5, TA6, TA7>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5, IOperand<TA6> arg6, IOperand<TA7> arg7)
		{
			throw new NotImplementedException();
		}
		public void This<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8>(IOperand<TA1> arg1, IOperand<TA2> arg2, IOperand<TA3> arg3, IOperand<TA4> arg4, IOperand<TA5> arg5, IOperand<TA6> arg6, IOperand<TA7> arg7, IOperand<TA8> arg8)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
		
		protected internal override void Flush()
		{
			if ( m_OwnerConstructor.HasDependencyInjection )
			{
				WriteDependencyInjection();
			}

			if ( m_Script != null )
			{
				m_Script(this);
			}
			
			base.Flush();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void WriteDependencyInjection()
		{
			var dependencyFieldByArgumentIndex = m_OwnerConstructor.GetDependencyFieldArgumentIndex().ToDictionary(
				keySelector: kvp => kvp.Value,     // argument index is the key
				elementSelector: kvp => kvp.Key);  // field member is the value

			ForEachArgument((arg, index) => {
				FieldMember dependencyField;
				if ( dependencyFieldByArgumentIndex.TryGetValue(index, out dependencyField) )
				{
					dependencyField.AsOperand<TypeTemplate.TArgument>().Assign(arg);
				}
			});
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private ConstructorInfo FindBaseConstructor(params Type[] argumentTypes)
		{
			var constructor = OwnerMethod.OwnerClass.BaseType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				binder: null,
				types: argumentTypes,
				modifiers: null);

			if ( constructor != null )
			{
				return constructor;
			}

			throw new InvalidOperationException("Base constructor not found.");
		}
	}
}
