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
		private readonly DynamicModule m_Module;
		private readonly TypeBuilder m_TypeBuilder;
		private readonly List<ClassWriterBase> m_Writers;
		private readonly List<MemberBase> m_Members;
		private readonly Dictionary<MemberInfo, MemberBase> m_MembersByDeclarations;
		private readonly Dictionary<string, MemberBase> m_MembersByName;
		private readonly List<MethodInfo> m_FactoryMethods;
		private readonly UniqueNameSet m_MemberNames;
		private readonly HashSet<MemberInfo> m_NotImplementedMembers;
		private Type m_CompiledType;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal ClassType(DynamicModule module, TypeKey key, string classFullName, Type baseType)
		{
			var resolvedBaseType = TypeTemplate.Resolve(baseType);

			m_Key = key;
			m_Module = module;
			m_Writers = new List<ClassWriterBase>();
			m_Members = new List<MemberBase>();
			m_MembersByDeclarations = new Dictionary<MemberInfo, MemberBase>();
			m_MembersByName = new Dictionary<string, MemberBase>();
			m_FactoryMethods = new List<MethodInfo>();
			m_MemberNames = new UniqueNameSet();
			m_NotImplementedMembers = new HashSet<MemberInfo>();
			m_NotImplementedMembers.UnionWith(TypeMemberCache.Of(resolvedBaseType).ImplementableMembers);
			m_CompiledType = null;
			m_TypeBuilder = module.ModuleBuilder.DefineType(classFullName, DefaultTypeAtributes, resolvedBaseType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public FieldMember GetPropertyBackingField(PropertyInfo declaration)
		{
			MemberBase member;

			if ( !m_MembersByDeclarations.TryGetValue(declaration, out member) )
			{
				throw new ArgumentException(string.Format("Property '{0}' was not implemented by this class.", declaration.Name));
			}

			return ((PropertyMember)member).BackingField;
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

		public DynamicModule Module
		{
			get { return m_Module; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal string TakeMemberName(string proposedName, bool mustUseThisName = false)
		{
			return m_MemberNames.TakeUniqueName(proposedName, mustUseThisName);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TInfo[] TakeNotImplementedMembers<TInfo>(TInfo[] members) where TInfo : MemberInfo
		{
			var membersToImplement = new HashSet<MemberInfo>(members);
			membersToImplement.IntersectWith(m_NotImplementedMembers);

			m_NotImplementedMembers.ExceptWith(membersToImplement);

			return membersToImplement.Cast<TInfo>().ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddWriter(ClassWriterBase writer)
		{
			m_Writers.Add(writer);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddMember(MemberBase member)
		{
			m_Members.Add(member);

			if ( member.MemberDeclaration != null )
			{
				m_MembersByDeclarations.Add(member.MemberDeclaration, member);
				m_MembersByName[member.Name] = member;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TMember GetMemberByName<TMember>(string memberName) where TMember : MemberBase
		{
			return (TMember)m_MembersByName[memberName];
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TMember GetMemberByDeclaration<TMember>(MemberInfo declaration) where TMember : MemberBase
		{
			return (TMember)m_MembersByDeclarations[declaration];
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal IEnumerable<MemberBase> GetAllMembers()
		{
			return m_Members;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void AddInterface(Type interfaceType)
		{
			if ( !m_TypeBuilder.GetInterfaces().Contains(interfaceType) )
			{
				m_NotImplementedMembers.UnionWith(TypeMemberCache.Of(interfaceType).ImplementableMembers);
				m_TypeBuilder.AddInterfaceImplementation(interfaceType);
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal void Compile()
		{
			foreach ( var writer in m_Writers )
			{
				writer.Flush();
			}

			for ( int i = 0 ; i < m_Members.Count ; i++ )
			{
				var member = m_Members[i];

				using ( member.CreateTypeTemplateScope() )
				{
					member.Write();
				}
			}

			foreach ( var member in m_Members )
			{
				using ( member.CreateTypeTemplateScope() )
				{
					member.Compile();
				}
			}

			m_CompiledType = m_TypeBuilder.CreateType();
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
			return m_FactoryMethods.Select(CreateFactoryMethodDelegate).ToArray();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal TypeBuilder TypeBuilder
		{
			get { return m_TypeBuilder; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		internal Type CompiledType
		{
			get { return m_CompiledType; }
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void FixupFactoryMethods()
		{
			for ( int i = 0 ; i < m_FactoryMethods.Count ; i++ )
			{
				var runtimeMethod = m_CompiledType.GetMethod(m_FactoryMethods[i].Name, BindingFlags.Public | BindingFlags.Static);
				m_FactoryMethods[i] = runtimeMethod;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private Delegate CreateFactoryMethodDelegate(MethodInfo factoryMethod)
		{
			var parameters = factoryMethod.GetParameters();
			var openDelegateType = s_DelegatePrototypesByArgumentCount[parameters.Length];
			var delegateTypeParameters = parameters.Select(p => TypeTemplate.Resolve(p.ParameterType)).Concat(new[] { factoryMethod.ReturnType });
			var closedDelegateType = openDelegateType.MakeGenericType(delegateTypeParameters.ToArray());

			return Delegate.CreateDelegate(closedDelegateType, factoryMethod);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private static readonly Type[] s_DelegatePrototypesByArgumentCount = new Type[] {
			typeof(Func<>), 
			typeof(Func<,>), 
			typeof(Func<,,>), 
			typeof(Func<,,,>), 
			typeof(Func<,,,,>), 
			typeof(Func<,,,,,>), 
			typeof(Func<,,,,,,>), 
			typeof(Func<,,,,,,,>), 
			typeof(Func<,,,,,,,,>)
		};
	}
}
