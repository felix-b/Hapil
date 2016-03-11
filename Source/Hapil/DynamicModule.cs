using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Hapil.Members;

namespace Hapil
{
	public class DynamicModule
	{
	    private readonly AssemblyBuilder m_AssemblyBuilder;
		private readonly ModuleBuilder m_ModuleBuilder;
		private readonly AssemblyName m_AssemblyName;
		private readonly UniqueNameSet m_ClassNames;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DynamicModule(
			string simpleName = "Hapil.EmittedTypes", 
			bool allowSave = false, 
			string saveDirectory = null)
	    {
			m_AssemblyName = new AssemblyName(simpleName);
			m_ClassNames = new UniqueNameSet();

		    if ( allowSave )
		    {
				m_AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					m_AssemblyName, 
					AssemblyBuilderAccess.RunAndSave,
					dir: saveDirectory ?? Path.GetDirectoryName(this.GetType().Assembly.Location));
				
				m_ModuleBuilder = m_AssemblyBuilder.DefineDynamicModule(
					name: simpleName + ".dll", 
					fileName: simpleName + ".dll", 
					emitSymbolInfo: true);
		    }
		    else
		    {
				m_AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					m_AssemblyName, 
					AssemblyBuilderAccess.Run);
				
				m_ModuleBuilder = m_AssemblyBuilder.DefineDynamicModule(
					name: simpleName + ".dll", 
					emitSymbolInfo: false);
			}

			this.CanSave = allowSave;
	    }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType DefineClass(TypeKey key, string classFullName)
		{
			var uniqueClassName = m_ClassNames.TakeUniqueName(classFullName);
			return new ClassType(this, key, uniqueClassName, key.BaseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ClassType DefineClass(Type baseType, TypeKey key, string classFullName)
		{
			var uniqueClassName = m_ClassNames.TakeUniqueName(classFullName);
			return new ClassType(this, key, uniqueClassName, baseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void SaveAssembly()
		{
			m_AssemblyBuilder.Save(m_AssemblyName.Name + ".dll");
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string SimpleName
		{
			get
			{
				return m_AssemblyBuilder.GetName().Name;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public bool CanSave { get; private set; }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ModuleBuilder ModuleBuilder
		{
			get
			{
				return m_ModuleBuilder;
			}
		}
	}
}
