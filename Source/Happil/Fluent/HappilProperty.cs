using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Fluent
{
	public class HappilProperty<T> : HappilOperand<T>, IHappilMember
	{
		private readonly HappilClass m_HappilClass;
		private readonly PropertyInfo m_Declaration;
		private readonly PropertyBuilder m_PropertyBuilder;
		private readonly HappilMethod<T> m_GetterMethod;
		private readonly VoidHappilMethod m_SetterMethod;
		private readonly BodyBase m_Body;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal HappilProperty(HappilClass happilClass, PropertyInfo declaration)
			: base(ownerMethod: null)
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
				m_GetterMethod = new HappilMethod<T>(happilClass, getterDeclaration);
				m_PropertyBuilder.SetGetMethod(m_GetterMethod.MethodBuilder);
			}

			var setterDeclaration = declaration.GetSetMethod();

			if ( setterDeclaration != null )
			{
				m_SetterMethod = new VoidHappilMethod(happilClass, setterDeclaration);
				m_PropertyBuilder.SetSetMethod(m_SetterMethod.MethodBuilder);
			}

			m_Body = CreatePropertyBody();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilMember Members

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

		string IHappilMember.Name
		{
			get
			{
				return m_PropertyBuilder.Name;
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilField<T> BackingField
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitTarget(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitLoad(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitStore(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnEmitAddress(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal override HappilClass OwnerClass
		{
			get
			{
				return m_HappilClass;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal PropertyInfo Declaration
		{
			get
			{
				return m_Declaration;
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

		internal BodyBase Body
		{
			get
			{
				return m_Body;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Action BodyDefinitionAction 
		{
			get
			{
				return m_Body.DefineMemberBody;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private BodyBase CreatePropertyBody()
		{
			var indexArgTypes = m_Declaration.GetIndexParameters().Select(p => p.ParameterType).ToArray();

			switch ( indexArgTypes.Length )
			{
				case 0:
					return new BodyNoArgs(this);
				case 1:
					var closed1ArgType = typeof(Body1Arg<>).MakeGenericType(typeof(T), indexArgTypes[0]);
					return (BodyBase)Activator.CreateInstance(closed1ArgType, this);
				case 2:
					var closed2ArgsType = typeof(Body2Args<,>).MakeGenericType(typeof(T), indexArgTypes[0], indexArgTypes[1]);
					return (BodyBase)Activator.CreateInstance(closed2ArgsType, this);
				default:
					throw new NotSupportedException("Indexer properties with more than 2 arguments are not supported.");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private HappilMethod<T> GetValidGetterMethod()
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
			private HappilField<T> m_BackingField;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected BodyBase(HappilProperty<T> ownerProperty)
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

			public HappilField<T> BackingField
			{
				get
				{
					if ( object.ReferenceEquals(m_BackingField, null) )
					{
						m_BackingField = OwnerProperty.OwnerClass.GetBody<object>().Field<T>("m_" + OwnerProperty.PropertyBuilder.Name);
					}

					return m_BackingField;
				}
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected HappilProperty<T> OwnerProperty { get; private set; }
			protected HappilMethod<T> GetterMethod { get; set; }
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
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class BodyNoArgs : BodyBase, IHappilPropertyBody<T>
		{
			public BodyNoArgs(HappilProperty<T> ownerProperty)
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

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void DefineMemberBody()
			{
				var typedGetterDefinition = (GetterBodyDefinition as Action<IHappilMethodBody<T>>);
				var typedSetterDefinition = (SetterBodyDefinition as Action<IVoidHappilMethodBody, HappilArgument<T>>);

				if ( typedGetterDefinition != null )
				{
					using ( GetterMethod.CreateBodyScope() )
					{
						typedGetterDefinition(GetterMethod);
					}
				}

				if ( typedSetterDefinition != null )
				{
					using ( SetterMethod.CreateBodyScope() )
					{
						typedSetterDefinition(SetterMethod, new HappilArgument<T>(SetterMethod, 1));
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Body1Arg<TIndex1> : BodyBase, IHappilPropertyBody<TIndex1, T>
		{
			public Body1Arg(HappilProperty<T> ownerProperty)
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

			#endregion

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public override void DefineMemberBody()
			{
				var typedGetterDefinition = (GetterBodyDefinition as Action<IHappilMethodBody<T>, HappilArgument<TIndex1>>);
				var typedSetterDefinition = (SetterBodyDefinition as Action<IVoidHappilMethodBody, HappilArgument<TIndex1>, HappilArgument<T>>);

				if ( typedGetterDefinition != null )
				{
					using ( GetterMethod.CreateBodyScope() )
					{
						typedGetterDefinition(GetterMethod, new HappilArgument<TIndex1>(GetterMethod, 1));
					}
				}

				if ( typedSetterDefinition != null )
				{
					using ( SetterMethod.CreateBodyScope() )
					{
						typedSetterDefinition(SetterMethod, new HappilArgument<TIndex1>(SetterMethod, 1), new HappilArgument<T>(SetterMethod, 2));
					}
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class Body2Args<TIndex1, TIndex2> : BodyBase, IHappilPropertyBody<TIndex1, TIndex2, T>
		{
			public Body2Args(HappilProperty<T> ownerProperty)
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

				if ( typedGetterDefinition != null )
				{
					using ( GetterMethod.CreateBodyScope() )
					{
						typedGetterDefinition(
							GetterMethod, 
							new HappilArgument<TIndex1>(GetterMethod, 1), 
							new HappilArgument<TIndex2>(GetterMethod, 2));
					}
				}

				if ( typedSetterDefinition != null )
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
