Happil
======

Hi there! Here a **.NET library** is being built, aimed to **support and facilitate** the following programming techniques for **.NET developers**:

* **CoI - Convention over Implementation**: let developers only author interfaces of application components, then  let the framework dynamically generate corresponding implementations, based on conventions associated with every specific kind of the component (e.g., data transfer objects, data access layer, configuration, logging, etc etc). The conventions are authored by the users of the library. **Happil.Applied** is an additional library, which supplies a set of reusable out-of-the-box conventions.
* **Aspect-Oriented Programming**: transparently support black-box aspects, implemented as dynamically generated CoI decorators, which intercept the calls to the components. The aspects can be connected to both hard-coded and CoI components. 
* **Subject-Oriented Programming**: extend single inheritance in CLR with a horizontal behavior composition technique similar to traits/mixins/modules in such languages as Scala, PHP, and Ruby.
* **Duck Typing**: provide the Duck Typing mechanism in order to bring some of dynamic languages advantages to static programming. 

#### Highlights

* The library is a **wrapper** around the .NET **Reflection.Emit** API. The library aims to provide a convenient, intuitive, **high-level code model**, which it **translates into MSIL** bytecode.
* The code model is exposed through **type-safe and refactor-safe fluent API**, which covers **most of C#** language constructs. Both **imperative and functional** programming constructs are **first-class citizens** in the code model.
* Implementation and decoration conventions are **arranged in pipelines**, so that the **code generation** is by itself **inherently aspect-oriented**.
* **Second-level generics** allows implementation and decoration conventions be authored as **templates**, yet providing the authors with **type safety**.
* The generated MSIL bytecode is **statically bound**, **reflection-free**, and is **completely decoupled** from **runtime state** accumulated by code generators (fine print: unless explicitly coded the other way by the authors).
* Such decoupling allows **pre-compilation**. This is useful when there are massive amounts of dynamic types. Instead of being generated during application initialization (or lazily later on), dynamic types can be **pre-compiled** and **saved to an assembly**, which is then **loaded** during application **start-up**, thus **speeding up** application initialization.
* **Performance overhead** introduced by aspect decorators is **kept at minimum** by **avoiding** the "**Invocation Object**" pattern. All aspects that apply to a component are combined together in the members of the decorator class. Moreover, when a CoI component is being decorated, the aspects are implemented inside the members of the component, with no separate decorator class.

#### The library also aims to:

* Provide a **short and easy happy path** for the discussed use cases.
* Allow writing **clean and maintainable** code generators, which are also **type-safe** and **refactor-safe**.
* Be **lightweight**, even in **large applications** with **thousands** of dynamic types.
* Make it **easy** to create a **valid program**, and make it **hard** to create an **invalid** one.
* **Just work**, and keep the **users happy**.

### Now Show Me The Money!

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

### More Information

The [**Wiki**](https://github.com/felix-b/Happil/wiki) is the ultimate source of information on the **Happil** project.

### Why?

For comparison of **Happil** with other similar libraries, see [**Alternatives**](https://github.com/felix-b/Happil/wiki/Alternatives).

### Licenses

**Happil** library is developed uner the [**MIT License**](https://github.com/felix-b/Happil/blob/master/LICENSE). 

[**NUnit**](http://www.nunit.org/) is licensed under [**zlib/libpng License (Zlib)**](http://nunit.org/index.php?p=license&r=2.5.10).

[**Moq**](https://code.google.com/p/moq/) is licensed under the [**New BSD License**](http://opensource.org/licenses/BSD-3-Clause).
