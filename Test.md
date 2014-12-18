---
title: Welcome to Hapil framework
---

Hapil, the CoI Framework

Hello and thank you for your interest in the **Hapil** library.

## What is Hapil

Hapil is a back-end framework for the [Convention over Implementation](ConventionOverImplementation) principle (or shortly, **```CoI```**).

The principle states that:

* Whenever  multiple contracts are required to be implemented in a unified way, the implementation must be automated
* Implementation must be applied to a contract as a set of conventions.
    * A single convention can provide the complete implementation
    * Or, different aspects of implementation can be captured by multiple conventions; a combination of conventions can be chosen to produce the complete result.
* One should not give up a good design just because it requires repetitive implementation of user-defined contracts; there is now **Hapil** to rescue.

First Header  | Second Header
------------- | -------------
Content Cell  | Content Cell
Content Cell  | Content Cell

## Show Me The Money

**Three** - given an interface of a data transfer object:

```csharp
public interface ICustomer
{
  int Id { get; set; }
  string FullName { get; set; }
  string EmailAddress { get; set; }
}
```

**Two** - plus an implementation convention:

```csharp
DeriveClassFrom<object>()
  .ImplementInterface<ICustomer>()
  .AllProperties().ImplementAutomatic();
```

**One** - plus a decorator convention:

```csharp
public class DataContractDecorator : ClassDecoratorBase
{
  public override void OnClassType(ClassType classType, ClassWriterBase writer)
  {
    writer.Attribute<DataContractAttribute>(values => values.Named(a => a.Namespace, "http://mydto/customer"));
  }
  public override void OnProperty(PropertyMember member, Func<PropertyDecorationBuilder> decorate)
  {
    decorate().Attribute<DataMemberAttribute>();
  }
}
```

**GO!**

```csharp
[DataContract(Namespace = "http://mydto/customer")]
public sealed class CustomerDto : ICustomer
{
	private int m_Id;
	private string m_FullName;
	private string m_EmailAddress;

	[DataMember]
	public int Id
	{
		get { return this.m_Id; }
		set { this.m_Id = value; }
	}

	[DataMember]
	public string FullName
	{
		get { return this.m_FullName; }
		set { this.m_FullName = value; }
	}

	[DataMember]
	public string EmailAddress
	{
		get { return this.m_EmailAddress; }
		set { this.m_EmailAddress = value; }
	}
}
```
