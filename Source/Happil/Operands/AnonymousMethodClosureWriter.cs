using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Writers;

namespace Happil.Operands
{
	internal class AnonymousMethodClosureWriter : ClassWriterBase
	{
		private readonly Dictionary<MethodMember, IClosureIdentification> m_ClosuresByHostMethod;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AnonymousMethodClosureWriter(ClassType ownerClass)
			: base(ownerClass)
		{
			m_ClosuresByHostMethod = new Dictionary<MethodMember, IClosureIdentification>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			OwnerClass.ForEachMember<MethodMember>(IdentifyNeedForClosures, predicate: m => m.IsAnonymous);
			ImplementClosuresIfAny();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void IdentifyNeedForClosures(MethodMember anonymousMethod)
		{
			IClosureIdentification newIdentification;
			var needsClosures = anonymousMethod.NeedsClosures(out newIdentification);

			if ( newIdentification != null && newIdentification.Captures.Length > 0 )
			{
				anonymousMethod.MakeInstanceMethod();
			}

			if ( needsClosures )
			{
				IClosureIdentification existingIdentification;

				if ( m_ClosuresByHostMethod.TryGetValue(anonymousMethod.HostMethod, out existingIdentification) )
				{
					existingIdentification.Merge(newIdentification);
				}
				else
				{
					m_ClosuresByHostMethod.Add(anonymousMethod.HostMethod, newIdentification);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void ImplementClosuresIfAny()
		{
			foreach ( var hostMethodClosures in m_ClosuresByHostMethod )
			{
				ImplementClosuresInHostMethod(
					hostMethod: hostMethodClosures.Key,
					closureIdentification: hostMethodClosures.Value);
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void ImplementClosuresInHostMethod(MethodMember hostMethod, IClosureIdentification closureIdentification)
		{
			closureIdentification.DefineClosures();

			foreach ( var closure in closureIdentification.ClosuresOuterToInner )
			{
				closure.ImplementClosure();
			}

			var hostMethodRewriter = new ClosureHostMethodRewritingVisitor(closureIdentification);
			hostMethod.AcceptVisitor(hostMethodRewriter);
		}
	}
}
