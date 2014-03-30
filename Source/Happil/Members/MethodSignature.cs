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

			ArgumentName = new string[parameters.Length];
			ArgumentType = new Type[parameters.Length];
			ArgumentUnderlyingType = new Type[parameters.Length];
			ArgumentIsByRef = new bool[parameters.Length];
			ArgumentIsOut = new bool[parameters.Length];

			for ( int i = 0 ; i < parameters.Length ; i++ )
			{
				ArgumentName[i] = parameters[i].Name;
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

		public MethodSignature(bool isStatic, Type[] argumentTypes = null, Type returnType = null)
		{
			var safeArgumentTypes = (argumentTypes ?? Type.EmptyTypes);

			ArgumentName = safeArgumentTypes.Select((type, index) => "arg" + index).ToArray();
			ArgumentType = new Type[safeArgumentTypes.Length];
			ArgumentUnderlyingType = new Type[safeArgumentTypes.Length];
			ArgumentIsByRef = new bool[safeArgumentTypes.Length];
			ArgumentIsOut = new bool[safeArgumentTypes.Length];

			for ( int i = 0 ; i < safeArgumentTypes.Length ; i++ )
			{
				ArgumentType[i] = safeArgumentTypes[i];
				ArgumentIsByRef[i] = false;
				ArgumentIsOut[i] = false;
				ArgumentUnderlyingType[i] = safeArgumentTypes[i];
			}

			ReturnType = returnType;
			IsVoid = (returnType == null || returnType == typeof(void));
			IsStatic = isStatic;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public Type[] BuildTemplateTypePairs()
		{
			var pairs = new Type[(1 + ArgumentCount) * 2];

			pairs[0] = typeof(TypeTemplate.TReturn);
			pairs[1] = ReturnType;

			TypeTemplate.BuildArgumentsTypePairs(ArgumentType, pairs, arrayStartIndex: 2);

			return pairs;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string[] ArgumentName { get; private set; }
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
