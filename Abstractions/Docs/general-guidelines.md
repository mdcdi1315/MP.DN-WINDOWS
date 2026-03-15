
## Using correctly the Abstractions Library

This is a small guide in order for you to learn how to use the 
library components properly and what you should expect from using it.

You can also use this as a cheat-sheet for solving common issues that you may find in your code involving these API's.

All the components that the library exports are covered with documentation explaining what each single API does.

### Platform Layering - What it is?

The platform layering API's abstract and wrap out common and handy API's 
that must be normally be provided through native P/Invoke calls.

For most of the developers that are using this library outside the Music Player repository,
there is no need to write P/Invokes or boilerplate code to create the platform layering.

Instead, there is provided a class called `DefaultPlatformLayer` and provides the default
services that are elsewise provided inherently by the .NET API's.

### Correctly using API's that require platform layering

There are some API's requiring the `SystemInfo` class to have been provided with a platform layer.

The most recommended way to do this is to provide a new instance of the `DefaultPlatformLayer` class
with the `RegisterPlatformLayer` method of the `SystemInfo` class in your Main method of your program,
possibly before calling these API's or just registering it before anything else runs in your Main method. 

> [!WARNING] 
Not correctly providing a platform layer instance or if the provided 
instance is poorly coded it may lead from unexpected exceptions to severe crashes.
Unless you have very good reasons to write your own platform layer, use the 
elsewise provided `DefaultPlatformLayer` class.

### Notices about the native memory access API's 

The native memory API's are completely unsafe to be used by consumer code directly.

It is only provided for some specific aspects of the library such as the imaging abstractions.

When you need to access or use though memory pointers, it is much more recommended to use the extension methods provided
for the `IMemoryHandle` interface, which do most of the .NET side operations using .NET arrays, which are memory-safe.

### Debugging Services

The debugging services provided by the library are only used by the Music Player app itself.

Normally, anyone outside of the Music Player application should continue to use their own debugging services.

Note, however, that some library components report messages through this mechanism. 
If you deem necessary that you need to view these messages, the best practice is to create a class 
deriving from the `DebugSink` class wrapping your own loggers and declaring it to the `DebugProvider` sinks
at your app's startup.

### Collections library usage

The entire Collections library is safe to be used from any app.

There may be in the future some dependencies on the platform layering, but nothing else far than that.

### Unsafe and stream static extension methods 

Both the `UnsafeMethods` and the `StreamMethods` classes do expose a bunch of methods for manipulating information 
through fast and unsafe API's wrapped in a safe manner.

Unless for any new API's marked with the preliminary attribute, 
you are free to use all the existing ones without any restrictions on memory safety.

### About the ITagReader interface and the SavedDataTag class

The `ITagReader` interface and it's derivants are safe to be used as well as the `SavedDataTag` class.

In case that you need to save `ITagReader` properties somewhere outside the app's lifecycle,
you should instead use the `NativeProperties` property of the `SavedDataTag` class.

The class elsewise can manage all the currently provided `ITagReader` derivants.

> [!CAUTION]
Do not modify the names of the properties returned by the `NativeProperties` property.

They are subject to change at any time, and the user should only provide what it was returned by the
`NativeProperties` property. However, if changes happens to the property names returned, the class will
be still compatible with the old names, for specific versions of the library.

