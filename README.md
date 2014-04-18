Happil
======

Hi there! Here a **.NET library** is being built, aimed to **support and facilitate** the following programming techniques for **.NET developers**:

* **CoI - Convention over Implementation**: let developers only author interfaces of application components, then  let the framework dynamically generate corresponding implementations, based on conventions associated with every specific kind of the component (e.g., data transfer objects, data access layer, configuration, logging, etc etc).
* **Aspect-Oriented Programming**: transparently support black-box aspects, implemented as dynamically generated CoI decorators, which intercept the calls to the components. The aspects can be connected to both hard-coded and CoI components. 
* **Subject-Oriented Programming**: extend single inheritance in CLR with a horizontal behavior composition technique similar to traits/mixins/modules in such languages as Scala, PHP, and Ruby.
* **Duck Typing**: provide the Duck Typing mechanism in order to bring some of dynamic languages advantages to static programming. 

#### Technical highlights:

* Implementation and decoration conventions are **arranged in pipelines**, so that the **code generation** is by itself **inherently aspect-oriented**.
* The code model is exposed through **fluent API**, which covers **most of C#** language constructs. Both **imperative and functional** programming constructs are **first-class citizens** in the code model.
* **Pre-compilation** is supported for massive amounts of dynamic types. Dynamic types can be **generated in advance**, saved to an assembly, then **loaded** during application **start-up, speeding it up**.
* **Second-level generics** allows implementation and decoration conventions be authored as **templates**, while providing the authors with **type safety**.

#### The library also aims to:

* Provide a very **short and easy happy path** for the above use cases.
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
