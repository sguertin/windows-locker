# Windows Locker

## How to Build Windows Locker

You'll need [dotnet sdk
10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) for windows.

## IDE - Visual Studio / Rider

If you have an IDE, building should be as simple as launching the sln file and
running the build command and/or the publish command to get a file ready to use.

When publishing, look for the option to make it self-contained/single file, the
file will be a bit bigger (~150MB).

## CLI - Terminal

If you don't have an IDE, the alternative is to use the terminal. Certain free
code editors such as VSCodium, PyCharm Community Edition, Notepad++, Atom, and
many more all have built in terminals, and if you don't have those, `cmd`,
`powershell` are both available. If you're already familiar with the command
line feel free to jump ahead to [Commands](### Commands).

To quickly get to a terminal in windows, hit the WinKey + R, a small window
should open over the start menu, type `powershell` into it and hit `enter`.
It should open a terminal prompt, left click in the terminal and you'll be able
to enter commands. First, you'll need to set the folder for the terminal to the
project folder. A quick way to get that path is to go to the file explorer,
navigate to the project folder, then hold `shift` and right-click on the
folder. This will open an expanded context menu that will have a `Copy as Path`
option, which will set it on your clipboard. Then you simply go back to the
terminal, type `cd`, add a space, and then hit `ctrl+shift+v` to paste the path.

It will end up looking something like this:

```powershell
> cd "C:\Path\To\Your\Project"
```

Hit `enter` and your terminal will move to that folder. From there you're ready
to [Build and Publish](#### Build and Publish).

### Commands

#### Build and Publish

This will build and publish the project into a publish folder.

```powershell
> dotnet build WindowsLocker.sln;
> dotnet publish -p:PublishProfile=File;
```

### Configuration

Included with the published executable is a file called appSettings.json
