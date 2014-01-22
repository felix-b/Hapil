using System;

namespace Happil.Fluent
{
	internal interface IHappilMember
	{
		void EmitBody();
		IDisposable CreateTypeTemplateScope();
		string Name { get; }
	}
}
