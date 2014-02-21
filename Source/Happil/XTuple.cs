//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Happil.Fluent;

//namespace Happil
//{
//	public static class XTuple
//	{
//		public static T New<T>() where T : class
//		{

//		}

//		//-----------------------------------------------------------------------------------------------------------------------------------------------------

//		public class Factory : HappilFactoryBase
//		{
//			public Factory(HappilModule module)
//				: base(module)
//			{
//			}

//			//-------------------------------------------------------------------------------------------------------------------------------------------------

//			protected override IHappilClassDefinition DefineNewClass(HappilModule module, HappilTypeKey key)
//			{
//				var tupleProperties = TypeMembers.Of(key.PrimaryInterface).ImplementableProperties;
//				var classDefinition = Module.DeriveClassFrom<object>(MakeClassNameFrom(key.PrimaryInterface, prefix: "XTupleOf"));

//				classDefinition.ImplementInterface<TypeTemplate.TPrimary>()
//					.AllProperties().ImplementAutomatic()
//					.AllMethods(m => m.Name == "Init").Implement(m => {
//						for ( int i = 0 ; i < tupleProperties.Length ; i++ )
//						{
							
//						}
//					});

//				return classDefinition;
//			}

//			//-------------------------------------------------------------------------------------------------------------------------------------------------

//			private string MakeClassNameFrom(Type type, string prefix = null, string suffix = null)
//			{
//				return type.Namespace + "." + (prefix ?? "") + type.Name + (suffix ?? "");
//			}
//		}
//	}
//}
