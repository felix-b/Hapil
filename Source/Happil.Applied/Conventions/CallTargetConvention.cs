using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Operands;
using Happil.Writers;

namespace Happil.Applied.Conventions
{
	public class CallTargetConvention : ImplementationConvention
	{
		private Field<TypeTemplate.TBase> m_TargetField;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public CallTargetConvention()
			: base(Will.ImplementBaseClass | Will.ImplementAnyInterface)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region Overrides of ImplementationConvention

		protected override void OnImplementBaseClass(ImplementationClassWriter<TypeTemplate.TBase> writer)
		{
			writer.PrimaryConstructor("Target", out m_TargetField);
			writer.AllMethods().ImplementPropagate(m_TargetField);
			writer.AllProperties().ImplementPropagate(m_TargetField);
			writer.AllEvents().ImplementPropagate(m_TargetField);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected override void OnImplementAnyInterface(ImplementationClassWriter<TypeTemplate.TInterface> writer)
		{
			writer.AllMethods().ImplementPropagate(m_TargetField.CastTo<TypeTemplate.TInterface>());
			writer.AllProperties().ImplementPropagate(m_TargetField.CastTo<TypeTemplate.TInterface>());
			writer.AllEvents().ImplementPropagate(m_TargetField.CastTo<TypeTemplate.TInterface>());
		}

		#endregion
	}
}
