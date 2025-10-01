

## The Serialization API's documentation

This series of documents provides insight and information about the Music Player's Serialization API,
which is an abstracted API for getting .NET object graphs and transforming them to a format that can be permanently saved.

Unlike the `BinaryFormatter` API, this does not make use of private reflection except when creating nested class objects.

This was created due to the fact that there were created many JSON parsers doing almost the same things,
so abstracting this does make that a bit easier and there is only the need to define the data themselves.

Most of the de/serialization process is done into a class 
called [`SerializationManager<T>`](../../MP/Serialization/SerializationManagerOfT.cs) 
that effectively manages such sessions, mostly comprising the features that this powerful API provides.

> [!CAUTION]
Be noted, using such an API for your app poses a severe security risk, 
even if the data that you will save are safe locations in a PC file system.
Only use it for simple information that will not corrupt the state of your app.
You have otherwise been advised to move away from such patterns. 
Such example was the `BinaryFormatter` API.

Table of Contents:

| Entry                           | Link                            |
|---------------------------------|---------------------------------|
| Understanding how the API works | [Link](ser-generalprocess.md)   |
| Creating a serializable class   | [Link](ser-createclass.md)      |