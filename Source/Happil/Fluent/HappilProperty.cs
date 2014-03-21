using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Expressions;

namespace Happil.Fluent
{
	public class HappilProperty : /*HappilOperand<T>,*/ IHappilMember, IHappilProperty
	{
		private readonly HappilClass m_HappilClass;
		private readonly PropertyInfo m_Declaration;
		private readonly PropertyBuilder m_PropertyBuilder;
		private readonly HappilMethod m_GetterMethod;
		private readonly VoidHappilMethod m_SetterMethod;
		private HappilField m_BackingField;
		private Action m_BodyDefinition;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilProperty(HappilClass happilClass, PropertyInfo declaration)
		{
			m_HappilClass = happilClass;
			m_Declaration = declaration;
			m_PropertyBuilder = happilClass.TypeBuilder.DefineProperty(
				happilClass.TakeMemberName(declaration.Name),
				declaration.Attributes,
				declaration.PropertyType,
				declaration.GetIndexParameters().Select(p => p.ParameterType).ToArray());

			var getterDeclaration = declaration.GetGetMethod();

			if ( getterDeclaration != null )
			{
				var closedGetterMethodType = typeof(HappilMethod<>).MakeGenericType(getterDeclaration.ReturnType);
				m_GetterMethod = (HappilMethod)Activator.CreateInstance(closedGetterMethodType, happilClass, getterDeclaration);
				m_PropertyBuilder.SetGetMethod(m_GetterMethod.MethodBuilder);
			}

			var setterDeclaration = declaration.GetSetMethod();

			if ( setterDeclaration != null )
			{
				m_SetterMethod = new VoidHappilMethod(happilClass, setterDeclaration);
				m_PropertyBuilder.SetSetMethod(m_SetterMethod.MethodBuilder);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public override bool HasTarget
		//{
		//	get
		//	{
		//		return !(m_Declaration.GetGetMethod() ?? m_Declaration.GetSetMethod()).IsStatic;
		//	}
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

		void IHappilMember.DefineBody()
		{
			m_BodyDefinition();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IHappilMember.EmitBody()
		{
			if ( m_GetterMethod != null )
			{
				((IHappilMember)m_GetterMethod).EmitBody();
			}
			
			if ( m_SetterMethod != null )
			{
				((IHappilMember)m_SetterMethod).EmitBody();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IDisposable CreateTypeTemplateScope()
		{
			return TypeTemplate.CreateScope(typeof(TypeTemplate.TProperty), m_Declaration.PropertyType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		MemberInfo IHappilMember.Declaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string Name
		{
			get
			{
				return m_PropertyBuilder.Name;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilProperty Set<TAttribute>(Action<IHappilAttributeBuilder<TAttribute>> values = null)
			where TAttribute : Attribute
		{
			var builder = new HappilAttributeBuilder<TAttribute>(values);
			m_PropertyBuilder.SetCustomAttribute(builder.GetAttributeBuilder());
			return this;
		}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilField BackingField
		{
			get
			{
				if ( object.ReferenceEquals(m_BackingField, null) )
				{
					m_BackingField = OwnerClass.GetBody<object>().DefineField(
						"m_" + m_PropertyBuilder.Name,
						m_PropertyBuilder.PropertyType,
						isStatic: false);
				}

				return m_BackingField;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public PropertyInfo Declaration
		{
			get
			{
				return m_Declaration;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type OperandType
		{
			get
			{
				return m_Declaration.PropertyType;
			}
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//protected override void OnEmitTarget(ILGenerator il)
		//{
		//	throw new NotSupportedException();
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//protected override void OnEmitLoad(ILGenerator il)
		//{
		//	throw new NotSupportedException();
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//protected override void OnEmitStore(ILGenerator il)
		//{
		//	throw new NotSupportedException();
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//protected override void OnEmitAddress(ILGenerator il)
		//{
		//	throw new NotSupportedException();
		//}

		//-------------------------------------------------------------------------------------------------------------------------------------------------

		internal void SetAttributes(Func<HappilAttributes> attributes)
		{
			if ( attributes != null )
			{
				foreach ( var attribute in attributes().GetAttributes() )
				{
					m_PropertyBuilder.SetCustomAttribute(attribute);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilClass OwnerClass
		{
			get
			{
				return m_HappilClass;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal PropertyBuilder PropertyBuilder
		{
			get
			{
				return m_PropertyBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal BodyBase GetPropertyBody<T>()
		{
			var indexArgTypes = m_Declaration.GetIndexParameters().Select(p => p.ParameterType).ToArray();

			switch ( indexArgTypes.Length )
			{
				case 0:
					var closedNoArgsType = typeof(BodyNoArgs<>).MakeGenericType(typeof(T));
					return (BodyBase)Activator.CreateInstance(closedNoArgsType, this);
				case 1:
					var closed1ArgType = typeof(Body1Arg<,>).MakeGenericType(indexArgTypes[0], typeof(T));
					return (BodyBase)Activator.CreateInstance(closed1ArgType, this);
				case 2:
					var closed2ArgsType = typeof(Body2Args<,,>).MakeGenericType(indexArgTypes[0], indexArgTypes[1], typeof(T));
					return (BodyBase)Activator.CreateInstance(closed2ArgsType, this);
				default:
					throw new NotSupportedException("Indexer properties with more than 2 arguments are not supported.");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Action BodyDefinitionAction 
		{
			get
			{
				return m_BodyDefinition;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private HappilMethod GetValidGetterMethod()
		{
			if ( m_GetterMethod == null )
			{
				throw new InvalidOperationException("The property does not define a getter");
			}

			return m_GetterMethod;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private VoidHappilMethod GetValidSetterMethod()
		{
			if ( m_SetterMethod == null )
			{
				throw new InvalidOperationException("The property does not define a setter");
			}

			return m_SetterMethod;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal abstract class BodyBase : IHappilPropertyBody
		{
			private Delegate m_GetterBodyDefinition;
			private Delegate m_SetterBodyDefinition;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected BodyBase(HappilProperty ownerProperty)
			{
				this.OwnerProperty = ownerProperty;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilPropertyBody Members

			public PropertyInfo Declaration
			{
				get
				{
					return OwnerProperty.m_Declaration;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public string Name
			{
				get
				{
					return OwnerProperty.m_PropertyBuilder.Name;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public Type Type
			{
				get
				{
					return OwnerProperty.m_Declaration.PropertyType;
				}
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public abstract void DefineMemberBody();

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public HappilField BackingField
			{
				get
				{
					return OwnerProperty.BackingField;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected HappilProperty OwnerProperty { get; private set; }
			protected HappilMethod GetterMethod { get; set; }
			protected VoidHappilMethod SetterMethod { get; set; }

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected Delegate GetterBodyDefinition
			{
				get
				{
					return m_GetterBodyDefinition;
				}
				set
				{
					GetterMethod = OwnerProperty.GetValidGetterMethod();
					m_GetterBodyDefinition = value;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected Delegate SetterBodyDefinition
			{
				get
				{
					return m_SetterBodyDefinition;
				}
				set
				{
					SetterMethod = OwnerProperty.GetValidSetterMethod();
					m_SetterBodyDefinition = value;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			internal void SetAttributes(HappilAttributes attributes)
			{
				if ( attributes != null )
				{
					foreach ( var attribute in attributes.GetAttributes() )
					{
						OwnerProperty.PropertyBuilder.SetCustomAttribute(attribute);
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BodyNoArgs<T> : BodyBase, IHappilPropertyBody<T>
		{
			public BodyNoArgs(HappilProperty ownerProperty)
				: base(ownerProperty)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilPropertyBody<T> Members

			public IHappilPropertyGetter Get(Action<IHappilMethodBody<T>> body)
			{
				GetterBodyDefinition = body;
				return null;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<T>> body)
			{
				SetterBodyDefinition = body;
				return null;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			FieldAccessOperand<T> IHappilPropertyBody<T>.BackingField
			{
				get
				{
					return BackingField.AsOperand<T>();
				}
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void DefineMemberBody()
			{
				var typedGetterDefinition = (GetterBodyDefinition as Action<IHappilMethodBody<T>>);
				var typedSetterDefinition = (SetterBodyDefinition as Action<IVoidHappilMethodBody, HappilArgument<T>>);
				
				if ( typedGetterDefinition != null )
				{
					using ( (GetterMethod as IHappilMember).CreateTypeTemplateScope() )
					{
						using ( GetterMethod.CreateBodyScope() )
						{
							typedGetterDefinition(GetterMethod.GetMethodBody<T>());
						}
					}
				}

				if ( typedSetterDefinition != null )
				{
					using ( (SetterMethod as IHappilMember).CreateTypeTemplateScope() )
					{
						using ( SetterMethod.CreateBodyScope() )
						{
							typedSetterDefinition(SetterMethod, new HappilArgument<T>(SetterMethod, 1));
						}
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Body1Arg<TIndex1, T> : BodyBase, IHappilPropertyBody<TIndex1, T>
		{
			public Body1Arg(HappilProperty ownerProperty)
				: base(ownerProperty)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilPropertyBody<T,TIndex1> Members

			public IHappilPropertyGetter Get(Action<IHappilMethodBody<T>, HappilArgument<TIndex1>> body)
			{
				GetterBodyDefinition = body;
				return null;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TIndex1>, HappilArgument<T>> body)
			{
				SetterBodyDefinition = body;
				return null;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			FieldAccessOperand<T> IHappilPropertyBody<TIndex1, T>.BackingField
			{
				get
				{
					return BackingField.AsOperand<T>();
				}
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void DefineMemberBody()
			{
				var typedGetterDefinition = (GetterBodyDefinition as Action<IHappilMethodBody<T>, HappilArgument<TIndex1>>);
				var typedSetterDefinition = (SetterBodyDefinition as Action<IVoidHappilMethodBody, HappilArgument<TIndex1>, HappilArgument<T>>);

				using ( TypeTemplate.CreateScope(
					typeof(TypeTemplate.TIndex1), OwnerProperty.Declaration.GetIndexParameters()[0].ParameterType) )
				{
					if ( typedGetterDefinition != null )
					{
						using ( (GetterMethod as IHappilMember).CreateTypeTemplateScope() )
						{
							using ( GetterMethod.CreateBodyScope() )
							{
								typedGetterDefinition(GetterMethod.GetMethodBody<T>(), new HappilArgument<TIndex1>(GetterMethod, 1));
							}
						}
					}

					if ( typedSetterDefinition != null )
					{
						using ( (SetterMethod as IHappilMember).CreateTypeTemplateScope() )
						{
							using ( SetterMethod.CreateBodyScope() )
							{
								typedSetterDefinition(SetterMethod, new HappilArgument<TIndex1>(SetterMethod, 1), new HappilArgument<T>(SetterMethod, 2));
							}
						}
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Body2Args<TIndex1, TIndex2, T> : BodyBase, IHappilPropertyBody<TIndex1, TIndex2, T>
		{
			public Body2Args(HappilProperty ownerProperty)
				: base(ownerProperty)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			#region IHappilPropertyBody<T,TIndex1,TIndex2> Members

			public IHappilPropertyGetter Get(Action<IHappilMethodBody<T>, HappilArgument<TIndex1>, HappilArgument<TIndex2>> body)
			{
				GetterBodyDefinition = body;
				return null;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public IHappilPropertySetter Set(Action<IVoidHappilMethodBody, HappilArgument<TIndex1>, HappilArgument<TIndex2>, HappilArgument<T>> body)
			{
				SetterBodyDefinition = body;
				return null;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			FieldAccessOperand<T> IHappilPropertyBody<TIndex1, TIndex2, T>.BackingField
			{
				get
				{
					return BackingField.AsOperand<T>();
				}
			}

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void DefineMemberBody()
			{
				var typedGetterDefinition = (
					GetterBodyDefinition 
					as Action<IHappilMethodBody<T>, HappilArgument<TIndex1>, HappilArgument<TIndex2>>); 
				var typedSetterDefinition = (
					SetterBodyDefinition 
					as Action<IVoidHappilMethodBody, HappilArgument<TIndex1>, HappilArgument<TIndex2>, HappilArgument<T>>);

				var indexParameters = OwnerProperty.Declaration.GetIndexParameters();

				using ( TypeTemplate.CreateScope(
					typeof(TypeTemplate.TIndex1), indexParameters[0].ParameterType,
					typeof(TypeTemplate.TIndex2), indexParameters[1].ParameterType) )
				{
					if ( typedGetterDefinition != null )
					{
						using ( (GetterMethod as IHappilMember).CreateTypeTemplateScope() )
						{
							using ( GetterMethod.CreateBodyScope() )
							{
								typedGetterDefinition(
									GetterMethod.GetMethodBody<T>(), 
									new HappilArgument<TIndex1>(GetterMethod, 1), 
									new HappilArgument<TIndex2>(GetterMethod, 2));
							}
						}			
					}

					if ( typedSetterDefinition != null )
					{
						using ( (SetterMethod as IHappilMember).CreateTypeTemplateScope() )
						{
							using ( SetterMethod.CreateBodyScope() )
							{
								typedSetterDefinition(
									SetterMethod,
									new HappilArgument<TIndex1>(SetterMethod, 1),
									new HappilArgument<TIndex2>(SetterMethod, 2),
									new HappilArgument<T>(SetterMethod, 3));
							}
						}
					}
				}
			}
		}
	}
}
