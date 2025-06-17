## Music Player Native Host Loader

This directory contains the native host loader for downloading and managing a
standalone distribution of the Music Player On .NET 8 app.

When using this way you do not have to install the .NET Runtime centrally 
on your machine.

Instead , when this is used , the loader downloads the .NET Runtime in the 
directory where it is located to.

If supported by the current system and all components are usuable,
the host will use instead the appropriate system-wide .NET installation,
if possible. Otherwise, it will fall back to a standalone installation.

Be noted that this loader is very lightweight: 
It does only allocate the memory it needs for invoking the runtime. 
The actual .NET host shipped with every app allocates and keeps a couple of megabytes without these are freed until the app's termination.

### Things that are planned to be done

- Reinvent the Loader, abstract common parts of it further and make general refactorations.

- Make the Loader able to be built and with other compilers such as the LLVM toolchain, and make it able to not use Visual Studio .vcxproj project if needed.