using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Happil.Statements;

namespace Happil.Fluent
{
	internal class HappilClass
	{
		private readonly TypeBuilder m_TypeBuilder;
		private readonly List<IHappilMember> m_Members;
		private readonly List<Tuple<IHappilMember, Action>> m_MemberBodyDefinitions;
		private readonly Stack<StatementScope> m_StatementScopeStack;
		private Type m_BuiltType = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public HappilClass(TypeBuilder typeBuilder)
        {
            m_TypeBuilder = typeBuilder;
			m_Members = new List<IHappilMember>();
			m_MemberBodyDefinitions = new List<Tuple<IHappilMember, Action>>();
			m_StatementScopeStack = new Stack<StatementScope>();
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
				m_MemberBodyDefinitions.Add(new Tuple<IHappilMember, Action>(member, bodyDefinition));
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
			foreach ( var tuple in m_MemberBodyDefinitions )
			{
				var member = tuple.Item1;
				var bodyDefinitionAction = tuple.Item2;

				bodyDefinitionAction();

				if ( m_StatementScopeStack.Count > 0 )
				{
					throw new InvalidOperationException(string.Format(
						"Scope stack is not empty after body definition of member '{0}' ({1}).",
						member.Name, member.GetType().Name));
				}
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

		public void PushScope(StatementScope scope)
		{
			m_StatementScopeStack.Push(scope);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void PopScope(StatementScope scope)
		{
			if ( m_StatementScopeStack.Count == 0 || m_StatementScopeStack.Peek() != scope )
			{
				throw new InvalidOperationException("Specified scope is not the current scope.");
			}

			m_StatementScopeStack.Pop();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public StatementScope CurrentScope
		{
			get
			{
				if ( m_StatementScopeStack.Count == 0 )
				{
					throw new InvalidOperationException("There is no active scope at the moment.");
				}
				
				return m_StatementScopeStack.Peek();
			}
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
