using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;

namespace Happil
{
	public class ConventionObjectFactory : ObjectFactoryBase
	{
		private readonly IObjectFactoryConvention[] m_ConventionSingletonInstances;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ConventionObjectFactory(DynamicModule module, params IObjectFactoryConvention[] conventions)
			: base(module)
		{
			m_ConventionSingletonInstances = conventions;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public TContract CreateInstanceOf<TContract>()
		{
			return GetOrBuildType(new TypeKey(primaryInterface: typeof(TContract))).CreateInstance<TContract>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override ClassType DefineNewClass(TypeKey key)
		{
			var context = new ObjectFactoryContext(this, key);

			foreach ( var convention in m_ConventionSingletonInstances )
			{
				if ( convention.ShouldApply(context) )
				{
					convention.Apply(context);
				}
			}

			if ( context.ClassType == null )
			{
				throw new CodeGenerationException("Class type was not created.");
			}

			return context.ClassType;
		}
	}
}
