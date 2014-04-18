Happil
======

Hi there! Here a **.NET library** is being built, aimed to **support and facilitate** the following programming methodologies for **.NET developers**:

* **Contract-Only Programming**: allow developers author interfaces of application components, while automatically generating the implementations, based on conventions appropriate for every specific kind of components (e.g., services, data transfer objects, configuration, etc etc).
* **Aspect-Oriented Programming**: allow dynamic implementation of aspect decorators for compiled application components. 
* **Subject-Oriented Programming**: provide .NET developers with a mechanism similar to **Scala's traits** or **Ruby's modules**. 
* **Dynamic Programming**: bring some **advantages** of the **dynamic languages**, by providing .NET developers with the **Duck Typing** mechanism.

More technical highlights:

* Implementation and decoration conventions are **arranged in pipelines**, so that the **code generation** is by itself **inherently aspect-oriented**.
* The code model is exposed through **fluent API**, which covers **most of C#** language constructs. Both **imperative and functional** programming constructs as **first-class citizens** in the code model.
* **Pre-generation** is supported for massive amounts of dynamic types. With pre-generation, dynamic types can be **generated in advance**, saved to an assembly, then loaded during application start-up, speeding it up.
* **Second-level generics** allows implementation and decoration conventions be authored as **templates**, while providing the authors with **type safety** mechanism.

The library also aims to:

* Provide very **short and easy happy path** for the above use cases.
* Allow writing **clean, maintainable**, code generators, which are also **type-safe** and **refactor-safe**.
* Be **lightweight**, even in **large applications** with **thousands** of dynamic types.
* Make it **easy** to create a **valid program**, and makes it **hard** to create an **invalid** one.
* **Just work**, and keep its **users happy**.

### More Information

The [**Wiki**](https://github.com/felix-b/Happil/wiki) is the ultimate source of information on the **Happil** project.

### Why?

For comparison of **Happil** with other similar libraries, see [**Alternatives**](https://github.com/felix-b/Happil/wiki/Alternatives).

### Licenses

**Happil** library is developed uner the [**MIT License**](https://github.com/felix-b/Happil/blob/master/LICENSE). 

[**NUnit**](http://www.nunit.org/) is licensed under [**zlib/libpng License (Zlib)**](http://nunit.org/index.php?p=license&r=2.5.10).

[**Moq**](https://code.google.com/p/moq/) is licensed under the [**New BSD License**](http://opensource.org/licenses/BSD-3-Clause).
