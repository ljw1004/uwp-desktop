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
