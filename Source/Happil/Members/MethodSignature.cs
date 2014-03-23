using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Happil.Members
{
	public class MethodSignature
	{
		public MethodSignature(MethodInfo declaration)
		{
			var parameters = declaration.GetParameters();

			ArgumentType = new Type[parameters.Length];
			ArgumentUnderlyingType = new Type[parameters.Length];
			ArgumentIsByRef = new bool[parameters.Length];
			ArgumentIsOut = new bool[parameters.Length];

			for ( int i = 0 ; i < parameters.Length ; i++ )
			{
				ArgumentType[i] = parameters[i].ParameterType;
				ArgumentIsByRef[i] = parameters[i].ParameterType.IsByRef;
				ArgumentIsOut[i] = parameters[i].IsOut;
				ArgumentUnderlyingType[i] = (
					parameters[i].ParameterType.IsByRef ? 
					parameters[i].ParameterType.GetElementType() : 
					parameters[i].ParameterType);
			}

			ArgumentCount = parameters.Length;
			ReturnType = declaration.ReturnType;
			IsVoid = (declaration.ReturnType == null || declaration.ReturnType == typeof(void));
			IsStatic = declaration.IsStatic;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSignature(bool isStatic, Type returnType, Type[] argumentTypes)
		{
			ArgumentType = new Type[argumentTypes.Length];
			ArgumentUnderlyingType = new Type[argumentTypes.Length];
			ArgumentIsByRef = new bool[argumentTypes.Length];
			ArgumentIsOut = new bool[argumentTypes.Length];

			for ( int i = 0 ; i < argumentTypes.Length ; i++ )
			{
				ArgumentType[i] = argumentTypes[i];
				ArgumentIsByRef[i] = false;
				ArgumentIsOut[i] = false;
				ArgumentUnderlyingType[i] = argumentTypes[i];
			}

			ReturnType = returnType;
			IsVoid = (returnType == null || returnType == typeof(void));
			IsStatic = isStatic;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type[] ArgumentType { get; private set; }
		public Type[] ArgumentUnderlyingType { get; private set; }
		public bool[] ArgumentIsByRef { get; private set; }
		public bool[] ArgumentIsOut { get; private set; }
		public int ArgumentCount { get; private set; }
		public bool IsStatic { get; private set; }
		public bool IsVoid { get; private set; }
		public Type ReturnType { get; private set; }
	}
}
