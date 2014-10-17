using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hapil.Members
{
	public abstract class MemberBase
	{
		private readonly string m_Name;
		private ClassType m_OwnerClass;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected MemberBase(ClassType ownerClass, string name)
		{
			m_OwnerClass = ownerClass;
			m_Name = name;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format("{0}[{1}]", this.Kind, this.Name);
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public virtual string Name
		{
			get { return m_Name; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract MemberKind Kind { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public abstract MemberInfo MemberDeclaration { get; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType OwnerClass
		{
			get
			{
				return m_OwnerClass;
			}
			protected set
			{
				m_OwnerClass = value;
			}
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
