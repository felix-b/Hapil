using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Hapil.Members
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
			IsPublic = declaration.IsPublic;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSignature(bool isStatic, bool isPublic, Type[] argumentTypes = null, string[] argumentNames = null, Type returnType = null)
		{
			var safeArgumentTypes = (argumentTypes ?? Type.EmptyTypes).Select(TypeTemplate.Resolve).ToArray();

			ArgumentName = (argumentNames ?? safeArgumentTypes.Select((type, index) => "arg" + index).ToArray());
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

			ArgumentCount = safeArgumentTypes.Length;
			ReturnType = TypeTemplate.Resolve(returnType);
			IsVoid = (returnType == null || returnType == typeof(void));
			IsStatic = isStatic;
			IsPublic = isPublic;
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private MethodSignature(MethodSignature source, bool? isStatic, bool? isPublic)
        {
            ArgumentName = source.ArgumentName;
            ArgumentType = source.ArgumentType;
            ArgumentUnderlyingType = source.ArgumentUnderlyingType;
            ArgumentIsByRef = source.ArgumentIsByRef;
            ArgumentIsOut = source.ArgumentIsOut;
            ArgumentCount = source.ArgumentCount;
            IsVoid = source.IsVoid;
            ReturnType = source.ReturnType;

            IsPublic = isPublic ?? source.IsPublic;
            IsStatic = isStatic ?? source.IsStatic;
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

		public MethodSignature Extend(Type[] newArgumentTypes, string[] newArgumentNames)
		{
			return new MethodSignature(
				IsStatic,
				IsPublic,
				argumentTypes: this.ArgumentType.Concat(newArgumentTypes).ToArray(),
				argumentNames: this.ArgumentName.Concat(newArgumentNames).ToArray(),
				returnType: this.ReturnType);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodSignature ChangeModifiers(bool? isPublic, bool? isStatic)
		{
		    return new MethodSignature(this, isStatic, isPublic);
		}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

	    public bool HasArgumentTypes(params Type[] argumentTypesToMatch)
	    {
	        if ( argumentTypesToMatch.Length != ArgumentType.Length )
	        {
	            return false;
	        }

	        for ( int i = 0 ; i < argumentTypesToMatch.Length ; i++ )
	        {
	            if ( ArgumentType[i] != TypeTemplate.Resolve(argumentTypesToMatch[i]) )
	            {
	                return false;
	            }
	        }

	        return true;
	    }

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of Object

		public override string ToString()
		{
			return string.Format("({0}):{1}", string.Join(",", ArgumentType.Select(t => t.FriendlyName())), IsVoid ? "void" : ReturnType.FriendlyName());
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public string[] ArgumentName { get; private set; }
		public Type[] ArgumentType { get; private set; }
		public Type[] ArgumentUnderlyingType { get; private set; }
		public bool[] ArgumentIsByRef { get; private set; }
		public bool[] ArgumentIsOut { get; private set; }
		public int ArgumentCount { get; private set; }
		public bool IsPublic { get; private set; }
		public bool IsStatic { get; private set; }
		public bool IsVoid { get; private set; }
		public Type ReturnType { get; private set; }
	}
}
