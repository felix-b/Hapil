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
		private readonly TransientConventionFactoryCallback m_TransientConventionFactory;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ConventionObjectFactory(DynamicModule module, params IObjectFactoryConvention[] conventionSingletonInstances)
			: base(module)
		{
			m_TransientConventionFactory = null;
			m_ConventionSingletonInstances = conventionSingletonInstances;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public ConventionObjectFactory(DynamicModule module, TransientConventionFactoryCallback transientConventionFactory)
			: base(module)
		{
			m_TransientConventionFactory = transientConventionFactory;
			m_ConventionSingletonInstances = null;
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IConstructors<TContract> CreateInstanceOf<TContract>()
		{
			var type = GetOrBuildType(new TypeKey(primaryInterface: typeof(TContract)));
			return new Constructors<TContract>(type);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IConstructors<TContract> CreateInstanceOf<TContract, TSecondary>()
		{
			var type = GetOrBuildType(new TypeKey(
				primaryInterface: typeof(TContract), 
				secondaryInterfaces: typeof(TSecondary)));
			
			return new Constructors<TContract>(type);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public IConstructors<TContract> CreateInstanceOf<TContract, TSecondary1, TSecondary2>()
		{
			var type = GetOrBuildType(new TypeKey(
				primaryInterface: typeof(TContract), 
				secondaryInterfaces: new[] { typeof(TSecondary1), typeof(TSecondary2) }));

			return new Constructors<TContract>(type);
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------

		public IConstructors<object> CreateInstanceOf(Type contractType, params Type[] secondaryInterfaceTypes)
		{
			var type = GetOrBuildType(new TypeKey(
				primaryInterface: contractType,
				secondaryInterfaces: secondaryInterfaceTypes));

			return new Constructors<object>(type);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override ClassType DefineNewClass(TypeKey key)
		{
			var context = new ObjectFactoryContext(this, key);
			var conventions = GetConventions(context);

			foreach ( var convention in conventions )
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

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private IObjectFactoryConvention[] GetConventions(ObjectFactoryContext context)
		{
			if ( m_ConventionSingletonInstances != null )
			{
				return m_ConventionSingletonInstances;
			}
			else
			{
				return m_TransientConventionFactory(context).ToArray();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public delegate IEnumerable<IObjectFactoryConvention> TransientConventionFactoryCallback(ObjectFactoryContext context);
	}
}
