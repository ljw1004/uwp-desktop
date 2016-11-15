# UWP for Desktop

This NuGet package lets you use UWP WinRT APIs from your desktop apps.

* [UwpDesktop](https://www.nuget.org/packages/UwpDesktop) NuGet package

To use it, right-click on your project, *Manage NuGet Packages*, and install the package.


## Explanation for how it works

MSDN provides this [list of WinRT APIs that can be called from desktop apps](https://msdn.microsoft.com/en-us/library/windows/desktop/dn554295(v=vs.85).aspx).

**To find the UWP WinRT APIs themselves**, look for example at this file:
```
C:\Program Files (x86)\Windows Kits\10\Platforms\UAP\10.0.10240.0\Platform.xml
```
This Platform.xml file conveys which WinRT APIs are available in "version 10240" of the UWP SDK. (This was the version that came out at VS2015 RTM. Newer versions will come out periodically). The Platform.xml file works by being an index into a set of other winmd files. It looks like this:
```
<ApiContract name="Windows.Foundation.FoundationContract" version="1.0.0.0" />
<ApiContract name="Windows.Foundation.UniversalApiContract" version="1.0.0.0" />
<ApiContract name="Windows.Networking.Connectivity.WwanContract" version="1.0.0.0" />
```
Each line denote a .winmd file in the `C:\Program Files (x86)\Windows Kits\10\References directory`. That's how I picked up exact filenames and pathnames to add as references. (I should really have added a reference to the WwanContract.winmd file as well, but couldn't be bothered)

These three winmd files make up the WinRT APIs available across all Win10 devices. You might additionally want to use the additional WinRT APIs that are part of Desktop. Look at this file:
```
C:\Program Files (x86)\Windows Kits\10\Extension SDKs\WindowsDesktop\10.0.10240.0\SDKManifest.xml
```
It has a load more `<ApiContract>` directives, pointing to additional .winmd files you should reference from the `C:\Program Files (x86)\Windows Kits\10\References` directory. I haven't bothered to do that.

 

**To interop with DLLs or WINMDs built for Win8.1 or WindowsPhone8.1**, the issue is that those old DLLs and WinMDs used to think that all WinRT types were in a file called "windows.winmd". But they're not anymore! Instead they're in files called Windows.Foundation.FoundationContract.winmd and the like. The solution is a *facade winmd*, which is basically a load of type-forwarders from windows.winmd to the new winmd files.
```
c:\Program Files (x86)\Windows Kits\10\UnionMetadata\Facade\Windows.WinMD
```
 

**To use "await" on WinRT types**, this and other interop between .NET and WinRT are provided by the following DLL. Note that it's an 8.1-era DLL, and therefore only works in conjunction with "facade\windows.winmd" above. (There are several other interop assemblies but they're not so crucial).
```
c:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll
```
If you fail to add both these two assemblies, then you'll get error messages like this:

* BC36930: 'Await' requires that the type 'IAsyncAction' have a suitable GetAwaiter method
* CS0012: The type 'IAsyncAction' is defined in an assembly that is not referenced. You must add a reference to assembly 'Windows, Version=255.255.255.0, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime'.
* CS4036: 'IAsyncAction' does not contain a definition for 'GetAwaiter' and no extension method 'GetAwaiter' accept a first argument of type 'IAsyncAction' could be found (are you missing a using directive for 'System'?)
    

## Example code (VB)

Here's an example console app in VB which uses UWP WinRT APIs:

```vb
Imports System.IO

Module Module1
    Sub Main()
        MainAsync().GetAwaiter().GetResult()
    End Sub

    Async Function MainAsync() As Task
        ' basics
        Dim folder = Windows.Storage.KnownFolders.DocumentsLibrary
        Dim opts As New Windows.Storage.Search.QueryOptions(
                              Windows.Storage.Search.CommonFileQuery.OrderByName, {".txt"})
        Dim files = Await folder.CreateFileQueryWithOptions(opts).GetFilesAsync(0, 20)
        For Each file In files
            Console.WriteLine(IO.Path.GetFileName(file.Path))
        Next

        ' streams
        Using reader = New IO.StreamReader(Await files.First.OpenStreamForReadAsync())
            Dim txt = Await reader.ReadToEndAsync()
            Console.WriteLine(txt.Substring(0, 100))
        End Using

        ' pictures
        Dim pics = Await Windows.Storage.KnownFolders.PicturesLibrary.GetFilesAsync(
                         Windows.Storage.Search.CommonFileQuery.OrderBySearchRank, 0, 1)
        Using stream = Await pics.First.OpenReadAsync()
            Dim decoder = Await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream)
            Console.WriteLine(decoder.OrientedPixelWidth & "x" & decoder.OrientedPixelHeight)
        End Using

        ' httpclient
        Dim client As New Net.Http.HttpClient()
        Dim html = Await client.GetStringAsync("http://www.microsoft.com")
        Console.WriteLine(html.Substring(0, 100))

    End Function
End Module
```


## Example code (C#)

Here's an example console app in C# which uses UWP WinRT APIs:


```cs
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;

class Program
{
    static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        // events
        var sm = new StringMap();
        sm.MapChanged += (s, e) => Console.WriteLine($"EVENT {e.Key}");
        sm.Add("key", "value");
        var c = new C();
        c.Canceled += (s, e) => Console.WriteLine($"EVENT {e}");
        c.Trigger(BackgroundTaskCancellationReason.IdleTask);

        // await
        var folder = KnownFolders.DocumentsLibrary;
        var opts = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".txt" });
        var files = await folder.CreateFileQueryWithOptions(opts).GetFilesAsync(0, 10);
        foreach (var file in files) Console.WriteLine($"FILE {Path.GetFileName(file.Path)}");

        // streams
        using (var reader = new StreamReader(await files.First().OpenStreamForReadAsync()))
        {
            var txt = await reader.ReadToEndAsync();
            Console.WriteLine($"STREAM {txt.Substring(0, 100)}");
        }

        // xaml
        CornerRadius cr = new CornerRadius(1.5);
        Console.WriteLine($"XAML {cr.BottomLeft} - {cr.BottomRight}");

        // pictures
        var pics = await KnownFolders.PicturesLibrary.GetFilesAsync(CommonFileQuery.OrderBySearchRank, 0, 1);
        using (var stream = await pics.First().OpenReadAsync())
        {
            var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
            Console.WriteLine($"PICTURE {decoder.OrientedPixelWidth} x {decoder.OrientedPixelHeight}");
        }

        // httpclient
        var client = new HttpClient();
        var html = await client.GetStringAsync("http://www.microsoft.com");
        Console.WriteLine($"HTTP {html.Substring(0, 100)}");
    }
}


public sealed class C : IBackgroundTaskInstance
{
    public BackgroundTaskDeferral GetDeferral() => null;
    public Guid InstanceId { get; set; }
    public uint Progress { get; set; }
    public uint SuspendedCount { get; set; }
    public BackgroundTaskRegistration Task { get; set; }
    public object TriggerDetails { get; set; }

    private EventRegistrationTokenTable<BackgroundTaskCanceledEventHandler> CanceledEvent;

    public event BackgroundTaskCanceledEventHandler Canceled
    {
        add
        {
            return EventRegistrationTokenTable<BackgroundTaskCanceledEventHandler>
                .GetOrCreateEventRegistrationTokenTable(ref CanceledEvent)
                .AddEventHandler(value);
        }
        remove
        {
            CanceledEvent?.RemoveEventHandler(value);
        }
    }

    public void Trigger(BackgroundTaskCancellationReason reason)
    {
        CanceledEvent?.InvocationList.Invoke(this, reason);
    }
}
```


# How to contribute

## Contribute by adding new WinRT APIs

Look on nuget.org at the [uwp-desktop packages already released](https://www.nuget.org/packages/UwpDesktop/):

```
UWP for Desktop 10.0.14393.2
UWP for Desktop 10.0.10586.2
UWP for Desktop 10.0.10586.1
UWP for Desktop 10.0.10240.2
UWP for Desktop 10.0.10240.1
```

The pattern is that if someone wants to write a desktop app that targets 10240, they'd reference the latest 10240 uwp-desktop package. If someone wants to write a desktop app that targets 10586, they'll reference the latest 10586 package.

*Each time a new WinRT release comes out, we make a corresponding new package of UwpDesktop*. The new package has /r: references to the winmds that contain the new APIs.

The minor versions are there because I made bugfixes or feature improvements to UwpDesktop itself, and wanted those fixes to be available to everyone, no matter what their target.

When a new WinRT release comes out, ...

1. Follow the instructions "how it works" above. That's a dull mechanical process that ends up with a list of .winmd files.
2. Look inside the nupkg directory and duplicate a .nuspec + .targets pair of files, renaming them appropriately. Edit the version number inside your new .nuspec file. Edit the .targets file according to the list of .winmd files you came up with. I use Word search+replace to manually generate a new .targets file.
3. Edit UwpDesktopAnalyzer.csproj in a text editor. At the bottom in the AfterBuild target, you'll see that it builds all the .nupkg files as part of its build. Duplicate one of these to build your new .nupkg file. I guarantee you'll make some typos as you do this...!
4. Open the UwDesktopAnalyzer project into VS and build it. The AfterBuild target will run and will generate your new .nupkg. It generates the .nupkg files in the bin\ directory.
5. Test your new .nupkg. Do this by creating a new desktop app (I use a console app myself), manage NuGet references, click the Settings icon to add a new package source, point it to the directory where your .nupkg file was created. (I find this dialog really counterintuitive but you'll get the hang of it eventually). Add the reference. Verify that you can use the new APIs.
6. Currently the NuGet package is owned by Lucian Wischik, lu@wischik.com, so you'll have email me a copy of your new .nupkg to upload to the NuGet site. (Please do this with a Release build of UwpDesktopAnalyzer, under bin\Release\...). But if someone from Microsoft wants to take ownership, please do!


## Contribute by improving the analyzer

I'll explain how the analyzer works. When someone is working on a desktop project and they add a reference to the UwpDesktop nuget package, then (1) if they're using packages.config then it obeys the install.ps1 script which adds a reference to the analyzer binary, (2) if they're using project.json then it obeys the convention that the analyzer binary is in an appropriate directory of the nuget package. Regardless, the result is that Visual Studio and msbuild know to pass a /r: reference to the analyzer binary when building the user's project.

The analyzer is invoked both live (as you type in Visual Studio) and at build-time (by msbuild). It looks at every single *invocation*. What happens next is subtle...

The way the WinRT "forbidden list" works is that, basically, some WinRT types aren't allowed to be used by desktop apps. (Some are allowed to be used in Centennial apps but not regular desktop apps, but the dividing line wasn't clear enough to me, so I just simplified it down to a single "forbidden list").

**How might the user get hold of one of these types?** The only way is if they use "new" operator to *invoke the constructor* of the type, or if the *invoke a static method* on the type. So if we cut things off at the source (constructors and static methods) then we guarantee the user's code will never get hold of a forbidden type. (and so implicitly will never call any instance methods on it!)

The method UwpDesktopAnalyzer.cs:AnalyzeSyntaxNode() is called on-demand for every syntax node. It only pays attention to invocations and "new" operators. It figures out the fully-qualified name of the target method/constructor. And it looks to see if this name is in the list of forbidden constructors / static methods. (Technical tip: the current implementation is considered inefficient by the Roslyn team because it involves lots of string allocations. They recommend comparing symbols directly. Kevin Pilch-Bission would be a good point-of-contact to see how to improve this.)

How do we get hold of the complete list of forbidden constructors / static methods? I pre-compute that manually. I wrote a separate command-line utility called "Preprocess". It looks at the file "ForbiddenClasses.txt" which merely contains the list of types. It cross-references this with the actual official 10586 SDK to find all constructors and static methods in those types. (TODO: this should be updated. It should be looking in the very latest 14393 SDK). It spits out a list. You have to copy+paste this list into UwpDesktopAnalyzer.cs, in the getter for ForbiddenStaticMethods. Then rebuild.

Why do we only have a single ForbiddenClass.txt list? I made some simplifying assumptions. It is technically possible that the Windows team might have made some WinRT types that were existed in 10240, and were disallowed in desktop apps in 10240, but are allowed in desktop apps in 10586. Or some other convoluted variation. But honestly what's the point? The accuracy of the ForbiddenClass list isn't high enough, and the value-add to developers isn't high enough, to justify any more complexity here.

When you want to improve any part of the analyzer...

1. You should read the instructions about "how it works" above, and also "how to contribute by adding APIs" above.
2. If you're augmenting ForbiddenClasses.txt, then you have to edit this file, run the Preprocess project, copy+paste the output into UwpDesktopAnalyzer.cs, then rebuild the UwpDesktopAnalyzer project.
3. If you're editing the analyzer then just rebuild it.
4. If you're making a change that affects developers on downlevel release of WinRT then you should make a new release of UwpDesktop with a bumped minor version number (e.g. 10.0.10586.3, or .4, or .5, or whatever comes next). And do this for EVERY downlevel version of the package that you touch. You'll do this by editing the nupkg\*.nuspec files of every applicable release.
5. As before, when you build the UwpDesktopAnalyzer project, it will build all the .nupkg files
6. As before, test it by adding a custom "Manage NuGet Packages" source
7. As before, email me the binaries that you want uploaded to nuget.org.
