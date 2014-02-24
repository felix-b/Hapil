using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Happil.Fluent;
using TT = Happil.TypeTemplate;

// ReSharper disable ConvertToLambdaExpression

namespace Happil
{
	public static class XTuple
	{
		private static readonly object s_FactoryInstanceSyncRoot = new object();
		private static Factory s_FactoryInstance = null;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static T New<T>() where T : class
		{
			return FactoryInstance.CreateTuple<T>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static Factory FactoryInstance
		{
			get
			{
				var instance = s_FactoryInstance;

				if ( instance == null )
				{
					lock ( s_FactoryInstanceSyncRoot )
					{
						if ( s_FactoryInstance == null )
						{
							instance = new Factory(new HappilModule());
							s_FactoryInstance = instance;
						}
					}
				}

				return instance;
			}
			set
			{
				lock ( s_FactoryInstanceSyncRoot )
				{
					s_FactoryInstance = value;
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public class Factory : HappilFactoryBase
		{
			public Factory(HappilModule module)
				: base(module)
			{
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public T CreateTuple<T>() where T : class
			{
				var key = new HappilTypeKey(primaryInterface: typeof(T));
				var type = base.GetOrBuildType(key);
				return type.CreateInstance<T>();
			}
	
			//-------------------------------------------------------------------------------------------------------------------------------------------------

			protected override IHappilClassDefinition DefineNewClass(HappilModule module, HappilTypeKey key)
			{
				var classDefinition = Module.DeriveClassFrom<object>(MakeClassNameFrom(key.PrimaryInterface, prefix: "XTupleOf"));
				var builder = new TupleClassBuilder(key, classDefinition);

				builder.BuildClass();

				return classDefinition;
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private string MakeClassNameFrom(Type type, string prefix = null, string suffix = null)
			{
				return type.Namespace + "." + (prefix ?? "") + type.Name + (suffix ?? "");
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private class TupleClassBuilder
		{
			private readonly HappilTypeKey m_Key;
			private readonly IHappilClassBody<object> m_ClassBody;
			private readonly Dictionary<string, PropertyInfo> m_TupleProperties;

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public TupleClassBuilder(HappilTypeKey key, IHappilClassBody<object> classBody)
			{
				m_Key = key;
				m_ClassBody = classBody;
				m_TupleProperties = TypeMembers.Of(key.PrimaryInterface).ImplementableProperties.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			public void BuildClass()
			{
				m_ClassBody.DefaultConstructor();

				ImplementTupleInterface();
				ImplementObjectOverrides();
				ImplementIEquatable();
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementTupleInterface()
			{
				m_ClassBody
					.ImplementInterface<TT.TPrimary>()
					.AllProperties()
						.ImplementAutomatic()
					.AllMethods(m => m.Name == "Init")
						.Implement(m => {
							m.ForEachArgument(arg => {
								m.This<TT.TPrimary>().BackingFieldOf<TT.TArgument>(m_TupleProperties[arg.Name]).Assign(arg);
							});
							m.Return(m.This<TT.TReturn>());
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementObjectOverrides()
			{
				m_ClassBody
					.Method<object, bool>(cls => cls.Equals)
						.Implement((m, other) => {
							m.Return(
								m.This<IEquatable<TT.TPrimary>>().Func<TT.TPrimary, bool>(intf => intf.Equals, other.CastTo<TT.TPrimary>())
							);		
						})
					.Method<int>(cls => cls.GetHashCode)
						.Implement(m => {
							var resultHashCode = m.Local<int>(0);
						
							m.This<TT.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								var fieldHashCode = m.This<TT.TPrimary>().BackingFieldOf<TT.TProperty>(prop).Func<int>(x => x.GetHashCode);
								resultHashCode.Assign(((resultHashCode << 5) + resultHashCode) ^ fieldHashCode);
							});
						
							m.Return(resultHashCode);
						});
			}

			//-------------------------------------------------------------------------------------------------------------------------------------------------

			private void ImplementIEquatable()
			{
				m_ClassBody
					.ImplementInterface<IEquatable<TT.TPrimary>>()
					.Method<TT.TPrimary, bool>(intf => intf.Equals)
						.Implement((m, other) => {
							m.This<TT.TPrimary>().Members.SelectAllProperties().ForEach(prop => {
								m.If(m.This<TT.TPrimary>().BackingFieldOf<TT.TProperty>(prop) != other.Prop<TT.TProperty>(prop)).Then(() => 
									m.ReturnConst(false)
								);
							});
							m.ReturnConst(true);
						});
			}
		}
	}
}

// ReSharper restore ConvertToLambdaExpression

