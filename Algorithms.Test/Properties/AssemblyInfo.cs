using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Algorithms.Properties;

class AssemblyInfo
{
    public static string RootNamespace = "Algorithms";

    static string _rootDirectory;

    public static string RootDirectory {
        get
        {
            if (_rootDirectory != null) return _rootDirectory;
            string file = GetFileName();
            string dir = Path.GetDirectoryName(file) ?? file;
            _rootDirectory = Path.GetDirectoryName(dir) ?? dir;
            return _rootDirectory;
        }
    }

    static string GetFileName([CallerFilePath] string file = "") => file;

    public static Stream? LoadEmbeddedResource(string path, [CallerFilePath] string callerPath = "")
    {
        string name = MapResourceNameCallerPath(RootDirectory, path, RootNamespace, callerPath);
        return LoadEmbeddedResource(name);
    }

    static Assembly GetAssembly() => typeof(AssemblyInfo).Assembly;

    public static TextReader LoadEmbeddedText(string path, [CallerFilePath] string callerPath = "")
    {
        string name = MapResourceNameCallerPath(RootDirectory, path, RootNamespace, callerPath);
        UnmanagedMemoryStream? stream = LoadEmbeddedResource(name);
        return stream != null ? new StreamReader(stream) : null;
    }

    public static UnmanagedMemoryStream? LoadEmbeddedResource(string resourcePath)
    {
        string path = resourcePath;
        if (path.IndexOf('\\') >= 0)
            path = path.Replace('\\', '.');
        if (path.IndexOf('/') >= 0)
            path = path.Replace('/', '.');

        Assembly assembly = GetAssembly();
        return (UnmanagedMemoryStream?)assembly.GetManifestResourceStream(path);
    }

    public static string MapResourceName(string root, string path, string namespc)
    {
        string resourcePath = path.Length < 1 || (path[0] != '/' && path[0] != '\\')
            ? Path.Combine(root, path)
            : path.Substring(1);

        var sb = new StringBuilder(resourcePath.Length + namespc.Length);
        if (!string.IsNullOrEmpty(namespc)) {
            sb.Append(namespc);
            sb.Append('.');
        }

        sb.Append(resourcePath);
        sb.Replace('/', '.');
        sb.Replace('\\', '.');
        resourcePath = sb.ToString();
        return resourcePath;
    }

    public static string MapResourceNameCallerPath(string root, string path, string namespc,
        [CallerFilePath] string callerPath = null)
    {
        callerPath = Path.GetDirectoryName(callerPath);
        string relCallerPath = MakeFilePathRelative(root, callerPath);
        return MapResourceName(relCallerPath, path, namespc);
    }

    public static UnmanagedMemoryStream? LoadEmbeddedResource(string root, string path,
        [CallerFilePath] string callerPath = null)
    {
        string resourcePath = MapResourceNameCallerPath(root, path, "", callerPath);
        return LoadEmbeddedResource(resourcePath);
    }

    public static ResourceManager LoadDefaultResource()
    {
        Assembly assembly = GetAssembly();
        var manager = new ResourceManager(assembly.GetName().Name + ".g", assembly);
        return manager;
    }

    public static IEnumerable<string> ListResources()
    {
        ResourceManager rm = LoadDefaultResource();
        ResourceSet? rs = rm.GetResourceSet(CultureInfo.CurrentCulture, false, false);
        foreach (DictionaryEntry pair in rs) yield return pair.ToString();
    }

    public static string PathDifference(string path1, string path2)
    {
        int lastSep = -1;
        int lastSame;

        for (lastSame = 0; lastSame < path1.Length && lastSame < path2.Length; lastSame++) {
            if (char.ToLower(path1[lastSame], CultureInfo.InvariantCulture)
                != char.ToLower(path2[lastSame], CultureInfo.InvariantCulture))
                break;

            if (path1[lastSame] == Path.DirectorySeparatorChar)
                lastSep = lastSame;
        }

        if (lastSame == 0)
            return path2;

        if (lastSame == path1.Length && lastSame == path2.Length)
            return string.Empty;

        var builder = new StringBuilder();
        while (lastSame < path1.Length) {
            if (path1[lastSame] == Path.DirectorySeparatorChar)
                builder.Append(".." + Path.DirectorySeparatorChar);
            lastSame++;
        }

        return builder + path2.Substring(lastSep + 1);
    }

    public static string MakeFilePathRelative(string basePath, string fileName)
    {
        if (!string.IsNullOrEmpty(basePath)) {
            if (!basePath.EndsWith("\\"))
                basePath += "\\";
            return PathDifference(basePath, fileName);
        }

        return fileName;
    }
}