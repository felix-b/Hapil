Happil
======

Hi there! I'm building a **Reflection.Emit wrapper** library in **C#.NET**, which focuses on two major use cases: 

* **Contract-First (and only) Development**: let developers author contracts as C# interfaces or base classes, then at run-time, emit classes that implement functionality behind the contracts.
* **Aspect-Oriented Programming**: allow dynamic implementation of aspects as decorators for compiled components.

I want a library which:

* Provides very **short and easy happy path** for the above use cases.
* Allows writing **clean, maintainable**, compiler- and **refactor-safe** code generators.
* Is **lightweight**, even in **large applications** with **thousands** of dynamic types.
* Makes it **easy** to create a **valid program**, and makes it **hard** to create an **invalid** one.
* **Just works**, and keeps its **users happy**.

### More Information

The [**Wiki**](https://github.com/felix-b/Happil/wiki) is the ultimate source of information on the **Happil** project.

### Why?

For comparison of **Happil** with other similar libraries, see [**Alternatives**](https://github.com/felix-b/Happil/wiki/Alternatives).

### Licenses

**Happil** library is developed uner the [**MIT License**](https://github.com/felix-b/Happil/blob/master/LICENSE). 

[**NUnit**](http://www.nunit.org/) is licensed under [**zlib/libpng License (Zlib)**](http://nunit.org/index.php?p=license&r=2.5.10).

[**Moq**](https://code.google.com/p/moq/) is licensed under the [**New BSD License**](http://opensource.org/licenses/BSD-3-Clause).
