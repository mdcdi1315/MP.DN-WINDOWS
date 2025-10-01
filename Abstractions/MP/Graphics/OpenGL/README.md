

## The OpenGL API Layer.

This namespace and the child ones define the platform-agnostic OpenGL graphics API and usages of it.

This layer is OpenGL 3.3.

This will be used finally by the app as an replacement for WPF and WinForms frameworks.

This will finally allow me to make the app mutli-platform (in some time).


### Loading the layer

To use the layer API, you must first load the API itself.

OpenGL is a collection of function pointers that describe all the graphics operations.

To load these, you create an implementation of the `IOpenGLFunctionLoader` interface.

It has a single method called `GetFunction` and accepts the name of the function to load.

This function returns a function pointer (that's why is `void*`) which represents the corresponding OpenGL function.

Then, you feed that interface instance to the `GL.LoadFunctions` static method, which does accept that. 
It will load ALL the functions present in the API layer. 

All the functions to use are exported as unmanaged C call declaration .NET function pointers (thus you need an `unsafe` context to access these).

> [!NOTE]
You cannot avoid using the `unsafe` context for using the OpenGL functions.
Using ordinary function delegates returned through `Marshal` allocate a 
lot of memory, are error-prone and the calls are CPU-consuming due to that .NET must perform transition to the native function. 
We need also performance because OpenGL is a raw, fast-forward graphics API.

When you no longer need the API, you can unload it as well by using the `GL.UnloadFunctions` static method.

### Error handling-checking

The `GL` class does also provide a handy static method called `AssertError`.

This does call in the `glGetError` function, and if an error is found, the error is translated to a convenient .NET exception and it is then thrown.

You can use this after calling an OpenGL function to learn if there are any errors regarding it.

> [!WARNING]
The method call is removed in your release builds for performance reasons,
so anything that you must test must be done in a debug build instead.

