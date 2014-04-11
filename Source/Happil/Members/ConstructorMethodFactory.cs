using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public abstract class ConstructorMethodFactory : MethodFactoryBase
	{
		public static ConstructorMethodFactory DefaultConstructor(ClassType type)
		{
			return InstanceConstructor(type, argumentTypes: Type.EmptyTypes);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static ConstructorMethodFactory InstanceConstructor(ClassType type, Type[] argumentTypes, string[] argumentNames = null)
		{
			var resolvedArgumentTypes = argumentTypes.Select(TypeTemplate.Resolve).ToArray();
			var initialSignature = new MethodSignature(isStatic: false, argumentTypes: resolvedArgumentTypes, argumentNames: argumentNames);


			return new ProxyConstructorMethodFactory(
				initialSignature,
				realFactoryFactory: finalSignature => {
					var builder = type.TypeBuilder.DefineConstructor(
						MethodAttributes.Public | 
						MethodAttributes.SpecialName | 
						MethodAttributes.RTSpecialName,
						CallingConventions.HasThis,
						finalSignature.ArgumentType);
					return new RealConstructorMethodFactory(type, builder, finalSignature);
				});
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public static ConstructorMethodFactory StaticConstructor(ClassType type)
		{
			var builder = type.TypeBuilder.DefineConstructor(
				MethodAttributes.Private | 
				MethodAttributes.SpecialName | 
				MethodAttributes.RTSpecialName | 
				MethodAttributes.HideBySig | 
				MethodAttributes.Static,
				CallingConventions.Standard,
				Type.EmptyTypes);
			var signature = new MethodSignature(isStatic: true);

			return new RealConstructorMethodFactory(type, builder, signature);
		}
	}
}
