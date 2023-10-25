using System.Diagnostics;
using System.Reflection;

namespace SLangRT.VM.Components;

/// <summary>
/// Load libraries and import .NET types into SLang.
/// </summary>
public class SLLoader : SLComponent
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="Parent">The parent runtime</param>
    public SLLoader(Runtime? Parent)
        : base(Parent) { }

    /// <summary>
    /// Get a type from all the loaded assemblies
    /// </summary>
    /// <param name="typeName">The type name</param>
    /// <returns>The type if found, else NULL</returns>
    public static Type GetTypeInLoadedAssemblies(string typeName)
    {
        foreach (Assembly s in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in s.GetTypes())
            {
                if (t.FullName == typeName)
                {
                    return t;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Load a library with a DLL file
    /// </summary>
    public bool LoadLibrary(string path)
    {
        try
        {
            Debug.WriteLine($"slrt: Starting loading of assembly '{path}'.");
            if (!path.Contains(":\\") && !path.Contains("/"))
            {
                Debug.WriteLine("slrt: Path isn't absolute, making it absolute.");
                path = AppDomain.CurrentDomain.BaseDirectory + path;
                Debug.WriteLine($"slrt: Path became '{path}'.");
            }

            Assembly a = Assembly.LoadFile(path);
            bool IsSLLib = false;
            Type slMeta = null;

            // Check if it's a SLang library
            foreach (Type type in a.GetTypes())
            {
                if (type.Name == "SLMetadata")
                {
                    IsSLLib = true;
                    slMeta = type;
                    Debug.WriteLine(
                        $"slrt: SLang library detected! Library metadata type: {slMeta.FullName}"
                    );
                }
            }

            // If it's a SLang library, then try running the function to load it
            if (IsSLLib && slMeta != null)
            {
                object instance = null;
                MethodInfo methodInfo = slMeta.GetMethod("LibLoad");
                if (methodInfo != null)
                {
                    if (!methodInfo.IsStatic)
                        instance = Activator.CreateInstance(slMeta);

                    object[] parameters = { Parent }; // Put the runtime as parameter so the library can interact with the runtime

                    // Finally, invoke 'LibLoad' and check the result
                    bool result = (bool)methodInfo.Invoke(instance, parameters);
                    if (!result)
                        throw new($"The SLang library '{a.GetName().Name}' failed to load.");
                }
                else
                {
                    throw new($"Function 'LibLoad' isn't found in type '{slMeta.FullName}'");
                }
            }

            // Say the library has been loaded, then exit this method.
            Debug.WriteLine(
                $"slrt: Loaded '{a.GetName().Name}', version {a.GetName().Version.ToString()}"
            );
            return true;
        }
        catch (Exception e)
        {
            throw new($"Failed assembly loading: {e.GetType().Name}: {e.Message}");
        }
    }
}
