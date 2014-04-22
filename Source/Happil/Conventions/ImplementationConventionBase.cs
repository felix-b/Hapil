using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Writers;

namespace Happil.Conventions
{
	public abstract class ImplementationConventionBase : IObjectFactoryConvention
	{
		#region IObjectFactoryConvention Members

		void IObjectFactoryConvention.Apply(DynamicModule module, ref TypeKey typeKey, ref ClassType classType)
		{
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void ChangeFullName(ref string classFullName)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void ChangeBaseType(ref Type baseType)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void DefineClass(ref ClassType classType)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void ImplementBaseClass(Type baseType, ImplementationClassWriter<TypeTemplate.TBase> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void ImplementPrimaryInterface(Type interfaceType, ImplementationClassWriter<TypeTemplate.TInterface> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void ImplementSecondaryInterface(Type interfaceType, ImplementationClassWriter<TypeTemplate.TInterface> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void ImplementAnyInterface(Type interfaceType, ImplementationClassWriter<TypeTemplate.TInterface> writer)
		{
			using ( TypeTemplate.CreateScope<TypeTemplate.TBase>(interfaceType) )
			{
				ImplementAnyBase(interfaceType, writer.AsBase<TypeTemplate.TBase>());
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void ImplementAnyBase(Type baseType, ImplementationClassWriter<TypeTemplate.TBase> writer)
		{
		}
	}
}
