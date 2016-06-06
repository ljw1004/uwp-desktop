// This code reads the file "ForbiddenClasses.txt",
// cross-references it against the Windows 10 SDK v10586 that's on the hard disk,
// and prints a list of "ForbiddenStatics" - the fully qualified name of every
// static method and constructor on those forbidden classes.
// (to be copied+pasted into the source code of UwpDesktopAnalyzer...)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        var forbiddenClasses = new HashSet<string>(File.ReadAllLines("ForbiddenClasses.txt"));
        var allWinmdTypes = ReadAllWinmdTypesAndStaticMethods("10586");
        foreach (var w in allWinmdTypes)
        {
            var qualifiedName = w.Namespace + "." + w.Name;
            if (!forbiddenClasses.Contains(qualifiedName)) continue;
            foreach (var m in w.StaticMethods)
            {
                Console.WriteLine($"\"{qualifiedName}.{m}\",");
            }
        }
    }

    static List<WinmdType> ReadAllWinmdTypesAndStaticMethods(string sdkver)
    {
        var r = new List<WinmdType>();

        var fn1 = $@"C:\Program Files (x86)\Windows Kits\10\Platforms\UAP\10.0.{sdkver}.0\Platform.xml";
        var fn2 = $@"C:\Program Files (x86)\Windows Kits\10\Extension SDKs\WindowsDesktop\10.0.{sdkver}.0\SDKManifest.xml";
        var apis = XDocument.Load(fn1).Descendants("ApiContract").Concat(
            XDocument.Load(fn2).Descendants("ApiContract")).ToList();
        var winmdfns = (from api in apis
                        let name = api.Attribute("name").Value
                        let version = api.Attribute("version").Value
                        select $@"C:\Program Files (x86)\Windows Kits\10\References\{name}\{version}\{name}.winmd").ToList();

        foreach (var winmdfn in winmdfns)
        {
            using (var stream = new FileStream(winmdfn, FileMode.Open, FileAccess.Read))
            using (var pereader = new PEReader(stream))
            {
                if (!pereader.HasMetadata) continue;
                var reader = pereader.GetMetadataReader();
                foreach (var typeHandle in reader.TypeDefinitions)
                {
                    var type = reader.GetTypeDefinition(typeHandle);
                    var tname = reader.GetString(type.Name);
                    var tnamespace = reader.GetString(type.Namespace);
                    if (type.BaseType.IsNil) continue;
                    string btype;
                    switch (type.BaseType.Kind)
                    {
                        case HandleKind.TypeSpecification: throw new NotImplementedException("TypeSpecification");
                        case HandleKind.TypeReference: { var b = reader.GetTypeReference((TypeReferenceHandle)type.BaseType); btype = reader.GetString(b.Namespace) + "." + reader.GetString(b.Name); break; }
                        case HandleKind.TypeDefinition: { var b = reader.GetTypeDefinition((TypeDefinitionHandle)type.BaseType); btype = reader.GetString(b.Namespace) + "." + reader.GetString(b.Name); break; }
                        default: continue;
                    }
                    if (btype == "System.ValueType") continue;
                    if (btype == "System.Enum") continue;
                    if (btype == "System.Delegate" || btype == "System.MulticastDelegate") continue;
                    var mnames = (from methodHandle in type.GetMethods()
                                  let method = reader.GetMethodDefinition(methodHandle)
                                  let attr = method.Attributes
                                  let mname = reader.GetString(method.Name)
                                  where attr.HasFlag(MethodAttributes.Public)
                                  where (mname == ".ctor") ||
                                        (attr.HasFlag(MethodAttributes.Static) &&
                                        !attr.HasFlag(MethodAttributes.SpecialName) &&
                                        !attr.HasFlag(MethodAttributes.RTSpecialName))
                                  select reader.GetString(method.Name)).ToList();
                    var w = new WinmdType() { Name = tname, Namespace = tnamespace, StaticMethods = mnames };
                    r.Add(w);
                }
            }
        }
        return r;
    }
    
}

class WinmdType
{
    public string Namespace;
    public string Name;
    public List<string> StaticMethods;

    public override string ToString() => $"{Namespace}.{Name} [{string.Join(",",StaticMethods)}]";
}
