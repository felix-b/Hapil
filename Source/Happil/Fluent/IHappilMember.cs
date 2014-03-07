using System;
using System.Reflection;

namespace Happil.Fluent
{
	internal interface IHappilMember
	{
		IDisposable CreateTypeTemplateScope();
		void EmitBody();
		MemberInfo Declaration { get; }
		string Name { get; }
	}
}
