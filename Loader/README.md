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