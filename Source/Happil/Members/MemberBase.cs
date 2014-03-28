using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Happil.Members
{
	public abstract class MemberBase
	{
		private readonly ClassType m_OwnerClass;
		private readonly string m_Name;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MemberBase(ClassType ownerClass, string name)
		{
			m_OwnerClass = ownerClass;
			m_Name = name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string Name
		{
			get { return m_Name; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract MemberInfo MemberDeclaration { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType OwnerClass
		{
			get { return m_OwnerClass; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal abstract void Write();
		internal abstract void Compile();
	
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal virtual IDisposable CreateTypeTemplateScope()
		{
			return null;
		}
	}
}
