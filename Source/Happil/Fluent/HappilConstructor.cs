using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Happil.Expressions;

namespace Happil.Fluent
{
	internal class HappilConstructor : HappilMethod, IHappilConstructorBody
	{
		private readonly Type[] m_ArgumentTypes;
		private readonly string[] m_ArgumentNames;
		private readonly ConstructorBuilder m_ConstructorBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private HappilConstructor(HappilClass happilClass)
			: base(happilClass)
		{
			m_ArgumentTypes = new Type[0];
			m_ArgumentNames = new string[0];

			m_ConstructorBuilder = happilClass.TypeBuilder.DefineConstructor(
				MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Static,
				CallingConventions.Standard,
				m_ArgumentTypes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilConstructor(HappilClass happilClass, Type[] argumentTypes)
			: base(happilClass)
		{
			m_ArgumentTypes = argumentTypes;
			m_ArgumentNames = CreateDefaultArgumentNames(argumentTypes);

			m_ConstructorBuilder = happilClass.TypeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
				CallingConventions.HasThis,
				argumentTypes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilConstructorBody Members

		public void Base()
		{
			var baseConstructor = FindBaseConstructor();

			new HappilUnaryExpression<object, object>(
				this, 
				@operator: new UnaryOperators.OperatorCall<object>(baseConstructor), 
				@operand: new HappilThis<object>(this));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1>(IHappilOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1, TArg2>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void Base<TArg1, TArg2, TArg3>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1>(IHappilOperand<TArg1> arg1)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1, TArg2>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void This<TArg1, TArg2, TArg3>(IHappilOperand<TArg1> arg1, IHappilOperand<TArg2> arg2, IHappilOperand<TArg3> arg3)
		{
			throw new NotImplementedException();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public override void SetAttributes(HappilAttributes attributes)
		{
			if ( attributes != null )
			{
				foreach ( var attribute in attributes.GetAttributes() )
				{
					m_ConstructorBuilder.SetCustomAttribute(attribute);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ConstructorInfo ConstructorInfo
		{
			get
			{
				return m_ConstructorBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of HappilMethod

		internal protected override Type[] GetArgumentTypes()
		{
			return m_ArgumentTypes;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override string[] GetArgumentNames()
		{
			return m_ArgumentNames;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal protected override Type GetReturnType()
		{
			return null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override ILGenerator GetILGenerator()
		{
			return m_ConstructorBuilder.GetILGenerator();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override Type[] BuildTemplateActualTypePairs()
		{
			var pairs = new Type[m_ArgumentTypes.Length * 2];
			TypeTemplate.BuildArgumentsTypePairs(m_ArgumentTypes, pairs, arrayStartIndex: 0);

			return pairs;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override string GetName()
		{
			return ".ctor";
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override ParameterBuilder DefineParameter(int position)
		{
			return m_ConstructorBuilder.DefineParameter(position, ParameterAttributes.None, strParamName: null);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------
	
		private ConstructorInfo FindBaseConstructor(params Type[] argumentTypes)
		{
			for ( var baseType = HappilClass.TypeBuilder.BaseType ; baseType != null ; baseType = baseType.BaseType )
			{
				var constructor = baseType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
					binder: null,
					types: argumentTypes,
					modifiers: null);

				if ( constructor != null )
				{
					return constructor;
				}
			}

			throw new InvalidOperationException("Base constructor not found.");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static HappilConstructor CreateStaticConstructor(HappilClass ownerClass)
		{
			return new HappilConstructor(ownerClass);
		}
	}
}