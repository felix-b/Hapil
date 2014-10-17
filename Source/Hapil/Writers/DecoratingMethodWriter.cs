using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil.Decorators;
using Hapil.Members;

namespace Hapil.Writers
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
					m_DecorationBuilder = CreateDecorationBuilder();
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
			else
			{
				foreach ( var writer in InnerWriters )
				{
					writer.Flush();
				}
			}

			base.Flush();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		protected internal virtual MethodDecorationBuilder CreateDecorationBuilder()
		{
			return new MethodDecorationBuilder(this);
		}
	}
}
