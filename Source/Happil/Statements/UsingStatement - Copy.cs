using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Happil.Fluent;

namespace Happil.Statements
{
	internal class UsingStatement<TDisposable> : IHappilStatement, IHappilUsingSyntax
		where TDisposable : IDisposable 
	{
		private readonly HappilOperand<TDisposable> m_Disposable;
		private readonly List<IHappilStatement> m_BodyBlock;

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		public UsingStatement(IHappilOperand<TDisposable> disposable)
		{
			m_Disposable = (HappilOperand<TDisposable>)disposable;
			m_BodyBlock = new List<IHappilStatement>();
		}

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilStatement Members

		public void Emit(ILGenerator il)
		{
			throw new NotImplementedException();
		}

		#endregion

		//-----------------------------------------------------------------------------------------------------------------------------------------------------

		#region IHappilUsingSyntax Members

		public void Do(Action body)
		{
			using ( var scope = new StatementScope(m_BodyBlock) )
			{
				var method = scope.OwnerMethod;

				method.Try(() => {
					body();
				})
				.Finally(() => {
					method.If(m_Disposable != method.Default<TDisposable>()).Then(() => {
						m_Disposable.Void(x => x.Dispose);
					});
				});
			}
		}

		#endregion
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	public interface IHappilUsingSyntax
	{
		void Do(Action body);
	}
}
