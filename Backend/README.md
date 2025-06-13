## Music Player On .NET 8 backend - Windows implementation. 

This directory contains all these files to build the base functionality of the
Music Player app , providing safe-typed native interop and has basic building
blocks for all the features that are currently part of the app.

The backend is built as a seperate library and it is packaged with the final implementation.

This project was created so that all the memory generated is known and completely controlled -
that's why the custom implementations for many common namespaces , like the `System.IO` or the `System.Security.Cryptography`.

