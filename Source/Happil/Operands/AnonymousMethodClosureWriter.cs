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

				if ( m_ClosuresByHostMethod.TryGetValue(newIdentification.HostMethod, out existingIdentification) )
				{
					existingIdentification.Merge(newIdentification);
				}
				else
				{
					m_ClosuresByHostMethod.Add(newIdentification.HostMethod, newIdentification);
				}
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void ImplementClosuresIfAny()
		{
			foreach ( var hostMethodClosures in m_ClosuresByHostMethod.Values )
			{
				ImplementClosuresInHostMethod(hostMethodClosures);
			}
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void ImplementClosuresInHostMethod(IClosureIdentification hostMethodClosures)
		{
			foreach ( var closure in hostMethodClosures.ClosuresOuterToInner )
			{
				closure.ImplementClosure();
			}

			var hostMethodRewriter = new ClosureHostMethodRewritingVisitor(hostMethodClosures);
			hostMethodClosures.HostMethod.AcceptVisitor(hostMethodRewriter);
		}
	}
}
