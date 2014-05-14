using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Members;
using Happil.Writers;

namespace Happil.Operands
{
	internal class AnonymousMethodWriter : MethodWriterBase
	{
		private IAnonymousMethodIdentification m_Identification;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public AnonymousMethodWriter(MethodMember ownerMethod)
			: base(ownerMethod, mode: MethodWriterModes.Normal, attachToOwner: false)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( OwnerMethod.IdentifyAnonymousMethods(out m_Identification) )
			{
				if ( m_Identification.ClosuresRequired )
				{
					WriteClosures();
				}

				WriteAnonymousMethodsThatHaveNoClosure();
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void WriteClosures()
		{
			m_Identification.DefineClosures();

			foreach ( var closure in m_Identification.ClosuresOuterToInner )
			{
				closure.ImplementClosure();
			}
			
			var hostMethodRewriter = new ClosureHostMethodRewritingVisitor(m_Identification);
			OwnerMethod.AcceptVisitor(hostMethodRewriter);
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		private void WriteAnonymousMethodsThatHaveNoClosure()
		{
			foreach ( var anonymousMethod in m_Identification.AnonymousMethods.Where(m => m.AnonymousMethod == null) )
			{
				var scope = m_Identification.GetAnonymousMethodScope(anonymousMethod);
				Debug.Assert(scope != AnonymousMethodScope.Closure, "Anonymous method was not implemented by its closure.");

				anonymousMethod.CreateAnonymousMethod(
					OwnerMethod.OwnerClass, 
					isStatic: scope == AnonymousMethodScope.Static,
					isPublic: false);
			}
		}
	}
}
