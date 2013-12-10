using System.Reflection.Emit;
namespace Happil.Fluent
{
	internal interface IHappilMember
	{
		void EmitBody();
		string Name { get; }
	}
}
