
## Contributing Guidelines for the Music Player For Windows Repository

You want to make a PR to one of the Music Player repositories? 

That's great!

This guide will teach you how to contribute code for the Music Player app.

Rules:

**Rule No.1**: Always contribute '*clean* code', as long as possible.
This helps me to understand what you have written on source, and in the end of the day, 
you will understand what you have written too.

Try additionally to break code into additional classes if you
notice that you have very common functionality of several 
statically-defined methods shared among classes in your code.

You may also add common functionality that your code is using 
as a utililty functionality, that's the reason why the `Utilities` namespace is provided.

Since the app does not come with unit tests, this rule is maybe the most important one.

**Rule No.2**: Always place new sources appropriately and with a clean and nice manner.

When you add new source files, make sure their parent namespaces are folders 
into the project you are contributing code for.

In this way you and anyone else contributing code understands where to search for issues,
even if debugging information is limited.

There are some notable exceptions to this rule that must be taken into account however:

- Global classes defined in any project except the actual implementation and the build tasks
should be placed along the `.csproj` project file, to indicate that the file contains or is part of a global class.

- The root project folder of the implementation has implicitly the namespace `MP`, 
so all sources in the `MP` namespace are placed among the project file, 
and any source in child namespaces are placed into folders.

- The assembly information must always be located along with the project file.

**Rule No.3**: All DLLImport and COM Interop code must *NOT* contain any reference to a 
class or a structure that cannot be treated as `unmanaged`.

- If you need a managed reference to retrieve from or send to native code, 
prefer to make the DLL Import completely unmanaged and private and instead 
create a public and static method containing the desired managed reference 
to export or pass.

For example, you need to pass a `System.String` to unmanaged code,
instead create a private method containing the P/Invoke, use the `fixed` keyword and mark 
the parameter that you would put there the `System.String` parameter,
as a `System.Char*` pointer retrieved from the `fixed` keyword.

A common guideline for the private unmanaged P/Invoke signatures is to add also the suffix `_Native`,
which indicates that the specified P/Invoke is the actual signature as the OS expects it to be.

- Always set the `ExactSpelling` option of the DllImport attribute to `true`.

- Enumeration types are also blittable types, and thus they can be passed as is to P/Invokes.
You cannot pass, however, the `System.Enum` type.

- You are free to use `System.Span`-related API's on any of the interop methods you define. 
However, you should note again that even using `System.Span`, you still need to create a private method,
and passing the unmanaged pointer representation of the memory block.

- For the P/Invoke definitions, do not use the shorthand type names that C# defines, such as `int`, `uint` or `ulong`.
Instead use the .NET type name for them, which are for the aforementioned types: `System.Int32` , `System.UInt32` and `System.UInt64`.

- Try not to use the `System.IntPtr` and `System.UIntPtr` types to generically use them instead of `void*` types. 
For such cases, just directly use the `void*` type. The exception is when the parameter, 
return value or structure field does represent a valid Windows Handle, which then you are free to use it if you want.

- Any structures that are needed to be passed to interop code must be written in explicit field positions,
except if you have fields that are pointer types and their size varies from platform to platform,
which in that case you must expose the fields sequentially.

For COM interop, all .NET interop interface methods must also be written fully unmanaged,
and always marked with the `HRESULT` type as return type if applicable and the `PreserveSig` attribute.

- If you commonly need to pass references, non-unmanaged structures or marshalled .NET COM interfaces
, prefer to create an extension method instead.

- Additionally, do *NOT*  use the `System.Object` type to get a marshalled COM interface. Instead, you mark it as a `void*`
and if the object returned is an instance of a specific interface, use the `IsPointerToCOMInterfaceType` attribute
to mark which interface type is. The in question attribute is defined in the Annotations namespace.

- If you need no matter how a `System.Object` for the interface, just use the 
`ComMarshalling.CreateInteropObject` method, on the `void*` that you have.

Finally note this: NO `LibraryImport` and `GeneratedComInterface` attributes. Use classic .NET interop patterns only.

If you cannot enforce any of the above guidelines of this rule due to CLR bugs, 
you should contact privately to one of the contributors so as to open together an issue in the .NET Runtime repo. 

**Rule No.4**: No interop code ends up in the implementation assembly.

All interactivity with the OS must be done with interop classes defined in the Backend project.

The implementation assembly is only focused on the actual Music 
Player app and it's UI, and this should remain that way.

**Rule No.5**: Consider in the end whether your code can be OS-free and is written in an abstract manner.

In such case, you must contribute that code in the `Abstractions` library so that every developer using the library can access your code.

**Rule No.6**: Try to use the unsafe methods and the `System.IO.Stream` extension methods defined in the Abstractions library.

They are fast enough and are implemented in pure IL.

They also help to avoid common `checked` and `unchecked` context issues when converting numbers.

**Rule No.7**: Do your best. Do not put it down. If you have additional questions about contributing you can always ask!!!
