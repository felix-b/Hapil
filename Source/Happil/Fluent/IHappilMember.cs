namespace Happil.Fluent
{
	public interface IHappilMember
	{
		IHappilMember[] Flatten();
		string Name { get; }
	}
}
