﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;
using System.Reflection.Emit;

namespace Happil
{
	public class HappilModule
	{
	    private readonly AssemblyBuilder m_AssemblyBuilder;
		private readonly ModuleBuilder m_ModuleBuilder;
		private readonly AssemblyName m_AssemblyName;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public HappilModule(
			string assemblySimpleName = "Happil.EmittedTypes", 
			bool allowSave = false, 
			string saveDirectory = null)
	    {
			m_AssemblyName = new AssemblyName(assemblySimpleName);

		    if ( allowSave )
		    {
				m_AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					m_AssemblyName, 
					AssemblyBuilderAccess.RunAndSave,
					dir: saveDirectory ?? Environment.CurrentDirectory);
				
				m_ModuleBuilder = m_AssemblyBuilder.DefineDynamicModule(
					name: assemblySimpleName + ".dll", 
					fileName: assemblySimpleName + ".dll", 
					emitSymbolInfo: true);
		    }
		    else
		    {
				m_AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					m_AssemblyName, 
					AssemblyBuilderAccess.Run);
				
				m_ModuleBuilder = m_AssemblyBuilder.DefineDynamicModule(
					name: assemblySimpleName + ".dll", 
					emitSymbolInfo: false);
			}
	    }

	    //-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<object> DefineClass(string classFullName)
		{
			return DefineClass(classFullName, baseType: typeof(object));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<object> DefineClass(string classFullName, Type baseType)
		{
			var typeAtributes =
				TypeAttributes.Public |
				TypeAttributes.Class |
				TypeAttributes.BeforeFieldInit |
				TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass;

			TypeBuilder typeBuilder = m_ModuleBuilder.DefineType(classFullName, typeAtributes, baseType);
			return new HappilClass(typeBuilder).GetBody<object>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<TBase> DeriveClassFrom<TBase>(string classFullName)
		{
			var typeAtributes =
				TypeAttributes.Public |
				TypeAttributes.Class |
				TypeAttributes.BeforeFieldInit |
				TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass;

			TypeBuilder typeBuilder = m_ModuleBuilder.DefineType(classFullName, typeAtributes, parent: typeof(TBase));
			return new HappilClass(typeBuilder).GetBody<TBase>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IHappilClassBody<object> DefineClass(HappilTypeKey key, string namespaceName)
		{
			return DefineClass(namespaceName + ".ImplOf" + key.PrimaryInterface.Name, key.BaseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public void SaveAssembly()
		{
			m_AssemblyBuilder.Save(m_AssemblyName.Name + ".dll");
		}
	}
}
