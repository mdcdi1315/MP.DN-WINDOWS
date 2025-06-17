<img src="AppIcon.ico" /> <br />

# The `Music Player On .NET` repository for Windows!!

This repository contains all these files for building the Music Player On .NET 8 app.

The app comes with an immersive WPF UI experience , while introducing ways to 
play audio files with a very different way than a usual music player would do.

And the top of all these is that it's main base is written in C# and .NET 8 - proving that everything is possible!

Additionally , it is packed with a custom .NET host executable for launching the app
without the need to install the .NET 8 runtime globally - this is managed by that executable.

This repository is also the official place to file issues that you may find during it's usage.

## Why another music player?

My perspective on music players is that many of the very known ones are very good alternatives
and are very usuable as well, just for my needs these were insufficient.

I needed something that it will manage everything for me efficiently, without being worrying about 
how the data are managed, or changing information when not requested to. Additionally, I needed a way
to export a playlist along it's consisting files.

I also dislike players that do weird stuff in the background, or do spurious actions without requesting even a consent from the user.

So, I decided to write bit by a bit my own music player, and this repository is the result.

I had this idea around 2022, and since I had some free time back then, I decided to learn C# and the core digital audio aspects.

Around a year later I started writing this app originally in .NET 7, now is running on 8 and it is what you see today.

As of June 2025, I believe that the features comprising the app have been standardized and thus the app is 'closed on the feature side'.

Some things have only left to be implemented, and more specifically for the Music Player Extensions:

- Allow extensions to intercept and provide custom UI views

- Allow extensions to provide customly-created playlists

- Extensions versioning

However, I would like to hear more ideas and features from you!

## Credits

I would like to thank Mark Heath for his awesome work on the [NAudio](https://github.com/naudio/naudio) project.

Without it this project would have never even been implemented.

Additionally , the project uses parts from the famous [SharpZipLib](https://github.com/icsharpcode/sharpziplib) library
for using some of it's functionality to provide archived playlists.

### Some of the technologies that are defined here:

#### RCU Engine

The engine and the heart of the Music Player.

It defines everything: from how the player should load to how
a playlist should be managed.

Additionally , it defines a model of how data should be submitted and retrieved back from a GUI.

**RCU** stands for '**R**un-time **C**ompiled **U**nits' Engine.

#### Music Player Binary Playlists

Defines readers and writers on the `BPL` format , a format designed for accurately saving full playlist
information , making such files to save data beyond of that being saved by usual music players.

Fully extendable by the user.

#### CGI Settings system

Defines a simple reader and writer for saving fast & 
accurately your app settings!

Under this format all your preferences are saved safely , even on app failure.

**CGI Setting** stands for '**C**ompact-**G**enerated **I**nformation Setting'.

### Building/Running the app

You can `git clone` this repository directly into your development environment, without additional requirements.

To at least have a working version of the app you just need to install the latest .NET 8 SDK , which it can be retrieved from [here](https://dotnet.microsoft.com/en-us/download).

Then, just use `dotnet build` on the `MusicPlayer.csproj` file located into the `Implementation` directory.

The above will create a framework-dependent flavor of the app, suitable for testing, usual modification and development of the app's features.

After the `dotnet build` command succeeds, you can just open the `MusicPlayer.exe` file produced under the binary output folder.

#### Building the app - framework independent installation 

Building a framework-independent installation is a more complicated process, since the Native Loader must be built.

To build that installation, you will need Visual Studio 2022. Community Edition should do the work too. 
If you do not know what to do, an installer for it is provided [here](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false).

Next, you will need the C/C++ toolset. 
This is selected through the workloads of the Visual Studio setup. 
If I remember it correctly, you select the `Desktop Development` workload.
The Loader is currently built against the version 143, so download the build tools for that version only.

You will also need the .NET SDK and it's WPF part to be installed along the C/C++ toolset, so also select the `.NET Desktop Development` workload.

After Visual Studio completes it's installation, close the installer, go to Windows Search.

Search for something called `Developer Command Prompt for VS2022`. 
Open that, and execute `cd /d <YOUR-REPO-DIR>`.
Where **YOUR-REPO-DIR** the directory where you cloned this repository.

Now, execute the command `msbuild ReleaseBuilds.proj`.

The above builds a framework-independent installation of the app. 
The results will be produced under a directory called `MP_DOTNET_RELEASE`.

You can now close the command prompt.

From there, you execute the `MP.exe` which it is the loader of the app, and the gateway that makes this installation framework-independent.

© mdcdi1315 (2023-2025). The project has been published under the MIT License.

