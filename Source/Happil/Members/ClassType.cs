using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Happil.Writers;

namespace Happil.Members
{
	public class ClassType
	{
		private const TypeAttributes DefaultTypeAtributes =
			TypeAttributes.Public |
			TypeAttributes.Class |
			TypeAttributes.Sealed |
			TypeAttributes.AutoClass |
			TypeAttributes.AnsiClass;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private readonly TypeKey m_Key;
		private readonly TypeBuilder m_TypeBuilder;
		private readonly List<ClassWriterBase> m_Writers;
		private readonly List<MemberBase> m_Members;
		private readonly List<MethodInfo> m_FactoryMethods;
		private readonly UniqueNameSet m_MemberNames;
		private Type m_CreatedType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ClassType(ModuleBuilder module, TypeKey key, string classFullName, Type baseType)
		{
			m_Key = key;
			m_Writers = new List<ClassWriterBase>();
			m_Members = new List<MemberBase>();
			m_FactoryMethods = new List<MethodInfo>();
			m_MemberNames = new UniqueNameSet();
			m_CreatedType = null;
			m_TypeBuilder = module.DefineType(classFullName, DefaultTypeAtributes, TypeTemplate.Resolve(baseType));
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TypeKey Key
		{
			get { return m_Key; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type BaseType
		{
			get { return m_TypeBuilder.BaseType; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal string TakeMemberName(string proposedName)
		{
			return m_MemberNames.TakeUniqueName(proposedName);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddWriter(ClassWriterBase writer)
		{
			m_Writers.Add(writer);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void Compile()
		{
			foreach ( var writer in m_Writers )
			{
				writer.Flush();
			}

			foreach ( var member in m_Members )
			{
				member.Write();
			}

			foreach ( var member in m_Members )
			{
				member.Compile();
			}

			m_CreatedType = m_TypeBuilder.CreateType();
			FixupFactoryMethods();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void DefineFactoryMethod(ConstructorInfo constructor, Type[] parameterTypes)
		{
			var resolvedParameterTypes = parameterTypes.Select(TypeTemplate.Resolve).ToArray();
			var factoryMethodName = TakeMemberName("FactoryMethod" + (m_FactoryMethods.Count + 1).ToString());
			var factoryMethod = m_TypeBuilder.DefineMethod(
				factoryMethodName,
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static,
				CallingConventions.Standard,
				typeof(object),
				resolvedParameterTypes);

			var il = factoryMethod.GetILGenerator();
			il.DeclareLocal(typeof(object));

			for ( int i = 0 ; i < parameterTypes.Length ; i++ )
			{
				il.Emit(OpCodes.Ldarg, i);
			}

			il.Emit(OpCodes.Newobj, constructor);
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ret);

			m_FactoryMethods.Add(factoryMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Delegate[] GetFactoryMethods()
		{
			throw new NotImplementedException();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TypeBuilder TypeBuilder
		{
			get { return m_TypeBuilder; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Type CreatedType
		{
			get { return m_CreatedType; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void FixupFactoryMethods()
		{
			for ( int i = 0 ; i < m_FactoryMethods.Count ; i++ )
			{
				var runtimeMethod = m_CreatedType.GetMethod(m_FactoryMethods[i].Name, BindingFlags.Public | BindingFlags.Static);
				m_FactoryMethods[i] = runtimeMethod;
			}
		}
	}
}
