﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Happil.Decorators;
using Happil.Members;

namespace Happil.Writers
{
	public class DecoratingMethodWriter : MethodWriterBase
	{
		private MethodDecorationBuilder m_DecorationBuilder;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public DecoratingMethodWriter(MethodMember ownerMethod)
			: base(ownerMethod, mode: MethodWriterModes.Decorator, attachToOwner: true)
		{
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public MethodDecorationBuilder DecorationBuilder
		{
			get
			{
				if ( m_DecorationBuilder == null )
				{
					m_DecorationBuilder = new MethodDecorationBuilder(this);
				}

				return m_DecorationBuilder;
			}
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal override void Flush()
		{
			if ( m_DecorationBuilder != null && !m_DecorationBuilder.IsEmpty )
			{
				m_DecorationBuilder.ApplyDecoration();
			}

			base.Flush();
		}
	}
}