using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Algorithms.Properties;

internal class AssemblyInfo
{
    public static string RootNamespace = "Algorithms";

    private static string _rootDirectory;

    public static string RootDirectory
    {
        get
        {
            if (_rootDirectory != null) return _rootDirectory;
            var file = GetFileName();
            var dir = Path.GetDirectoryName(file) ?? file;
            _rootDirectory = Path.GetDirectoryName(dir) ?? dir;
            return _rootDirectory;
        }
    }

    static string GetFileName([CallerFilePath] string file = "")
    {
        return file;
    }

    public static Stream? LoadEmbeddedResource(string path, [CallerFilePath] string callerPath = "")
    {
        var name = MapResourceNameCallerPath(RootDirectory, path, RootNamespace, callerPath);
        return LoadEmbeddedResource(name);
    }

    private static Assembly GetAssembly()
    {
        return typeof(AssemblyInfo).Assembly;
    }

    public static TextReader LoadEmbeddedText(string path, [CallerFilePath] string callerPath = "")
    {
        var name = MapResourceNameCallerPath(RootDirectory, path, RootNamespace, callerPath);
        var stream = LoadEmbeddedResource(name);
        return stream != null ? new StreamReader(stream) : null;
    }

    public static UnmanagedMemoryStream? LoadEmbeddedResource(string resourcePath)
    {
        var path = resourcePath;
        if (path.IndexOf('\\') >= 0)
            path = path.Replace('\\', '.');
        if (path.IndexOf('/') >= 0)
            path = path.Replace('/', '.');

        var assembly = GetAssembly();
        return (UnmanagedMemoryStream?)assembly.GetManifestResourceStream(path);
    }

    public static string MapResourceName(string root, string path, string namespc)
    {
        var resourcePath = path.Length < 1 || path[0] != '/' && path[0] != '\\'
            ? Path.Combine(root, path)
            : path.Substring(1);

        var sb = new StringBuilder(resourcePath.Length + namespc.Length);
        if (!string.IsNullOrEmpty(namespc))
        {
            sb.Append(namespc);
            sb.Append('.');
        }
        sb.Append(resourcePath);
        sb.Replace('/', '.');
        sb.Replace('\\', '.');
        resourcePath = sb.ToString();
        return resourcePath;
    }

    public static string MapResourceNameCallerPath(string root, string path, string namespc, [CallerFilePath] string callerPath = null)
    {
        callerPath = Path.GetDirectoryName(callerPath);
        var relCallerPath = MakeFilePathRelative(root, callerPath);
        return MapResourceName(relCallerPath, path, namespc);
    }

    public static UnmanagedMemoryStream? LoadEmbeddedResource(string root, string path, [CallerFilePath] string callerPath = null)
    {
        var resourcePath = MapResourceNameCallerPath(root, path, "", callerPath);
        return LoadEmbeddedResource(resourcePath);
    }

    public static ResourceManager LoadDefaultResource()
    {
        var assembly = GetAssembly();
        var manager = new ResourceManager(assembly.GetName().Name + ".g", assembly);
        return manager;
    }

    public static IEnumerable<string> ListResources()
    {
        var rm = LoadDefaultResource();
        var rs = rm.GetResourceSet(CultureInfo.CurrentCulture, false, false);
        foreach (DictionaryEntry pair in rs)
        {
            yield return pair.ToString();
        }
    }

    public static string PathDifference(string path1, string path2)
    {
        var lastSep = -1;
        int lastSame;

        for (lastSame = 0; lastSame < path1.Length && lastSame < path2.Length; lastSame++)
        {
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
        while (lastSame < path1.Length)
        {
            if (path1[lastSame] == Path.DirectorySeparatorChar)
                builder.Append(".." + Path.DirectorySeparatorChar);
            lastSame++;
        }
        return builder + path2.Substring(lastSep + 1);
    }

    public static string MakeFilePathRelative(string basePath, string fileName)
    {
        if (!string.IsNullOrEmpty(basePath))
        {
            if (!basePath.EndsWith("\\"))
                basePath += "\\";
            return PathDifference(basePath, fileName);
        }
        return fileName;
    }
}
