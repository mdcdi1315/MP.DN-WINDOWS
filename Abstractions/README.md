## Music Player abstractions library.

This project defines the bare and stripped out building blocks for the Music Player app.

Additionally it defines the unsafe byte and array manipulation methods that the app does heavily make use of.

This will be referenced by any Music Player platform flavor to try to generalize some parts 
of it , like the RCU Engine.

The home of the library will be here into the Windows implementation, since it is the first implementation of the Music Player app ever created.

It is also loadable as a .NET 7 + assembly and it ships with full documentation over every class it defines.

My additional goal is you (yes you developers out there) to use this library to define music or 
in general, playback functionality to any possible app.

Note that the library defines and abstractions required for the app in general.

You should really use those classes that you really need, for example, you need the RCU Engine abstraction only, use only that.

As an additional point, the library guarantees that it will not allocate more memory in the GC heap other than is required,
for example, you need an instance of the RCU Engine, only the required bytes to create this abstraction will be used.

- .NET Standard 2.0? Why not?

The answer is relatively simple: .NET Framework is now somewhat deprecated and there 
is no need to use this assembly from .NET Framework. (Even if I enabled such support 
you would encounter issues because you will need the .NET Core SDK and C# 10 features to gain full functionality).

I am also doing this to force developers moving to .NET Core - it is much faster, cross-platform and much more code-performant and less error-prone.