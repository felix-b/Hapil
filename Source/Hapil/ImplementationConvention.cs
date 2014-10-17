using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Members;
using Hapil.Writers;

namespace Hapil
{
	public abstract class ImplementationConvention : IObjectFactoryConvention
	{
		private readonly Will m_Will;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected ImplementationConvention(Will will)
		{
			m_Will = will;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IObjectFactoryConvention Members

		bool IObjectFactoryConvention.ShouldApply(ObjectFactoryContext context)
		{
			return this.ShouldApply(context);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		void IObjectFactoryConvention.Apply(ObjectFactoryContext context)
		{
			if ( m_Will.HasFlag(Will.InspectDeclaration) )
			{
				OnInspectDeclaration(context);
			}

			if ( m_Will.HasFlag(Will.ImplementBaseClass) )
			{
				OnImplementBaseClass(context.CreateImplementationWriter<TypeTemplate.TBase>());
			}

			if ( m_Will.HasFlag(Will.ImplementPrimaryInterface) && 
				context.TypeKey.PrimaryInterface != null &&
				ShouldImplementInterface(context, context.TypeKey.PrimaryInterface) )
			{
				using ( TypeTemplate.CreateScope<TypeTemplate.TInterface>(context.TypeKey.PrimaryInterface) )
				{
					OnImplementPrimaryInterface(context.CreateImplementationWriter<TypeTemplate.TInterface>());
				}
			}

			if ( m_Will.HasFlag(Will.ImplementAnySecondaryInterface) )
			{
				foreach ( var secondaryInterface in context.TypeKey.SecondaryInterfaces
					.Where(t => ShouldImplementInterface(context, t)) )
				{
					using ( TypeTemplate.CreateScope<TypeTemplate.TInterface>(secondaryInterface) )
					{
						OnImplementAnySecondaryInterface(context.CreateImplementationWriter<TypeTemplate.TInterface>());
					}
				}
			}

			if ( m_Will.HasFlag(Will.ImplementAnyInterface) )
			{
				foreach ( var secondaryInterface in context.TypeKey.GetAllInterfaces()
					.Where(t => ShouldImplementInterface(context, t)) )
				{
					using ( TypeTemplate.CreateScope<TypeTemplate.TInterface>(secondaryInterface) )
					{
						OnImplementAnyInterface(context.CreateImplementationWriter<TypeTemplate.TInterface>());
					}
				}
			}

			if ( m_Will.HasFlag(Will.ImplementAnyBaseType) )
			{
				foreach ( var secondaryInterface in context.TypeKey.GetAllAncestorTypes()
					.Where(t => !t.IsInterface || ShouldImplementInterface(context, t)) )
				{
					using ( TypeTemplate.CreateScope<TypeTemplate.TBase>(secondaryInterface) )
					{
						OnImplementAnyBaseType(context.CreateImplementationWriter<TypeTemplate.TBase>());
					}
				}
			}
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual bool ShouldApply(ObjectFactoryContext context)
		{
			return true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual bool ShouldImplementInterface(ObjectFactoryContext context, Type interfaceType)
		{
			return true;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnInspectDeclaration(ObjectFactoryContext context)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnImplementBaseClass(ImplementationClassWriter<TypeTemplate.TBase> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnImplementPrimaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnImplementAnySecondaryInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnImplementAnyInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected virtual void OnImplementAnyBaseType(ImplementationClassWriter<TypeTemplate.TBase> writer)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		[Flags]
		protected enum Will
		{
			InspectDeclaration = 0x01,
			ImplementBaseClass = 0x02,
			ImplementPrimaryInterface = 0x04,
			ImplementAnySecondaryInterface = 0x08,
			ImplementAnyInterface = 0x10,
			ImplementAnyBaseType = 0x20 
		}
	}
}
