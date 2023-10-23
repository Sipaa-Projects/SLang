using SLangRT.VM;
using SLangRT.VM.Components;
using System;

namespace SLangRT;

public class Runtime
{
    public List<SLComponent> Components;
    public bool IsForREPL = false;

    public Runtime()
    {
        Components = new();
        Components.Add(new SLVariableManager(this));
    }

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
