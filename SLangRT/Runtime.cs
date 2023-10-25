using SLangRT.VM;
using SLangRT.VM.Components;
using System;

namespace SLangRT;

/// <summary>
/// The <see cref="Runtime"/> class, used to run SLang code
/// </summary>
public class Runtime
{
    /// <summary>
    /// The component list
    /// </summary>
    public List<SLComponent> Components { get; private set; }

    /// <summary>
    /// The parser instance
    /// </summary>
    public Parser Parser {get; private set;}

    /// <summary>
    /// Boolean used to say to the runtime it belongs to a REPL.
    /// </summary>
    public bool IsForREPL = false;

    /// <summary>
    /// The constructor
    /// </summary>
    public Runtime()
    {
        Parser = new();
        Parser.parentRuntime = this;
        Components = new();
        Components.Add(new SLVariableManager(this));
        Components.Add(new SLFunctionManager(this));
        Components.Add(new SLLoader(this));
    }

    /// <summary>
    /// Get a component by it's type
    /// </summary>
    /// <param name="t">The component's type</param>
    /// <returns>The component if found, else NULL</returns>
    public SLComponent GetComponentByType(Type t)
    {
        foreach (SLComponent slc in Components)
        {
            if (slc.GetType() == t)
            {
                return slc;
            }
        }
        return null;
    }

    /// <summary>
    /// Execute a line of code
    /// </summary>
    /// <param name="ln">The line to be executed</param>
    public void ExecuteLine(string ln)
    {
        if (!ln.EndsWith(';'))
            throw new("Lines must finish with a semicolon (';')");

        ln = ln.Remove(ln.Length - 1, 1); // Remove the ending semicolon

        bool HasBeenManaged = false;

        foreach (SLComponent slc in Components)
        {
            HasBeenManaged = slc.Execute(ln).HasBeenManaged;
            if (HasBeenManaged)
                break;
        }

        if (!HasBeenManaged)
            throw new("No components of SLang managed to execute the line. Maybe what you are trying to do is unsupported?");
    }
}
