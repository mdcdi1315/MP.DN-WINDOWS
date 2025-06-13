

## What does `CGI` anyways mean?

`CGI Settings` stands for '**C**ompact-**G**enerated **I**nformation Settings'.

This because the provided settings must be defined with as less space as possible,
plus the information provided must be typed and generated so that can be become independent
of .NET semantics (such as type information and metadata).

In fact, CGI Settings could be also implemented roughly and into other
object-oriented environments in the same way.

## About the CGI Settings model

The low-level interfaces (`ICGISettingsReader` and `ICGISettingsWriter` respectively) 
are designed in that way so that an implementing settings reader and writer can as 
just simply focus on the implementation details rather on how implement the base settings interface logic.

The CGI settings model does not care how you will write and get the data from a data source; 
It does care though how it will 'provide' the data into the app instance.

For example, you could create a CGI Settings reader and writer that transmit and recieve CGI Settings over the network.

This allows the entire model to be built on and create the entire 'CGI Settings Infrastracture'.

The infrastructure is then split up into 4 parts:

-> The low-level abstractions.

Here the entire base and logic of CGI settings is defined;
the entire model depends on these abstractions in order 
everything can communicate and cooperate.

-> The CGI Interchargeable Binary Format.

Here is defined a base implementation based on the low-level abstractions where settings in the 
CGI settings logic can be permanently saved on a stream. 
It defines a pseudo-wire format describing how CGI settings are saved and retrieved back.

-> CGI Settings Extensions.

Here is the extensibility part of the CGI settings logic.

You can retrieve and save custom data (such as, typed classes) via simply registering 
a CGI Setting Extension during read and write time.

Practically, you can entirely cover all your needs that you expect
from a settings management system.

-> CGI Settings Loader logic.

A CGI Settings Loader Reader and Writer implementation is defined that is capable of 
easily auto-filling in custom, typed classes that usually hold settings data. 
No more tedious actions and complex typecasts!

Through a class object that implements specific behavior you can load and write settings by
just using two lines of code. Behind, these classes use reflection to communicate over 
with those objects and that's where all the hard work is done.

Most important, these communicate with the low-level abstractions, so factually
you do not have any restrictions from where you will read from and write settings to.
