using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Happil.Fluent
{
	internal class HappilClass
	{
		private readonly TypeBuilder m_TypeBuilder;
		private readonly List<IHappilMember> m_Members;
		private readonly List<Action> m_MemberBodyDefinitions;
		private Type m_BuiltType = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;
			m_Members = new List<IHappilMember>();
			m_MemberBodyDefinitions = new List<Action>();
        }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> GetBody<TBase>()
		{
			return new HappilClassBody<TBase>(happilClass: this);
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilClass Inherit<TBase>(params Func<IHappilClassBody<TBase>, IHappilMember>[] members)
		//{
		//	throw new NotImplementedException();
		//}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilClass Inherit(object baseType, params Func<IHappilClassBody<object>, IHappilMember>[] members)
		//{
		//	return this;
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void ImplementInterface(Type interfaceType)
		{
			m_TypeBuilder.AddInterfaceImplementation(interfaceType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string TakeMemberName(string proposedName)
		{
			//TODO: check for duplicate names and add suffix if necessary
			return proposedName;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void RegisterMember(IHappilMember member, Action bodyDefinition = null)
		{
			m_Members.Add(member);

			if ( bodyDefinition != null )
			{
				m_MemberBodyDefinitions.Add(bodyDefinition);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TMember FindMember<TMember>(string name) where TMember : IHappilMember
		{
			return m_Members.OfType<TMember>().Where(m => m.Name == name).First();
		}

		////-----------------------------------------------------------------------------------------------------------------------------------------------------

		//public HappilClass Implement(Type interfaceType, params Func<IHappilClassBody<object>, IHappilMember>[] members)
		//{
		//	m_TypeBuilder.AddInterfaceImplementation(interfaceType);
		//	//TODO: add members to member list
		//	return this;
		//}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Called by HappilFactoryBase.TypeEntry constructor.
		/// </summary>
		public Type CreateType()
		{
			foreach ( var definition in m_MemberBodyDefinitions )
			{
				definition();
			}

			foreach ( var member in m_Members )
			{
				member.EmitBody();
			}

			m_BuiltType = m_TypeBuilder.CreateType();
			//TODO: add members to member list
			return m_BuiltType;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Called by HappilFactoryBase.TypeEntry constructor.
		/// </summary>
		public Delegate[] GetFactoryMethods()
		{
			//TODO:TASK#5 this is a temporary implementation; the real implementation must be Reflection-free as explained here:
			//Factory method should be delegate to a static method in the generated type
			//The static method should invoke correct constructor and return the created instance.
			return new Delegate[] {
				new Func<object>(() => Activator.CreateInstance(m_BuiltType))
			};
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeBuilder TypeBuilder
		{
			get
			{
				return m_TypeBuilder;
			}
		}
	}
}
