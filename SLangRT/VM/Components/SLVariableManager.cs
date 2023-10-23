namespace SLangRT.VM.Components;

/// <summary>
/// Manage variables
/// </summary>
public class SLVariableManager : SLComponent
{
    public List<SLVariable> Variables { get; private set; }

    public SLVariableManager(Runtime? Parent)
        : base(Parent)
    {
        Variables = new();
    }

    /// <summary>
    /// Get a variable by his name
    /// </summary>
    /// <param name="name">The variable's name</param>
    /// <returns>The index & the variable. Returns 0 & NULL if not found</returns>
    public KeyValuePair<int, SLVariable?> GetVarByName(string name)
    {
        for (int i = 0; i < Variables.Count; i++)
        {
            SLVariable v = Variables[i];
            if (v.Name == name)
            {
                return new(i, v);
            }
        }
        return new(0, null);
    }

    /// <summary>
    /// Get a variable by his name
    /// </summary>
    /// <param name="name">The variable's name</param>
    /// <returns>The variable. Returns NULL if not found</returns>
    public void SetVarWithName(string name, object value)
    {
        KeyValuePair<int, SLVariable?> slv = GetVarByName(name);
        if (slv.Value != null)
        {
            Variables[slv.Key].Value = value;
            Variables[slv.Key].Type = value.GetType();
        }
        else
        {
            Variables.Add(
                new()
                {
                    Value = value,
                    Name = name,
                    Type = value?.GetType() ?? null
                }
            );
        }
    }

    /// <summary>
    /// Check if a line is a variable definition
    /// </summary>
    public bool IsVariableDefinition(string ln) => ln.Contains('=');

    /// <summary>
    /// Check if a variable should be printed in console (private)
    /// </summary>
    private bool ShouldPrintInConsole(SLVariable? v) =>
        v != null && Parent != null && Parent.IsForREPL;

    public override SLComponentExecuteData Execute(string ln)
    {
        if (IsVariableDefinition(ln))
        {
            var split = ln.Split('=', StringSplitOptions.TrimEntries);

            SetVarWithName(split[0], split[1]);

            return new() { HasBeenManaged = true };
        }
        else
        {
            var v = GetVarByName(ln);
            if (ShouldPrintInConsole(v.Value))
            {
                if (v.Value != null) // Just to say to C# to don't show a warning
                {
                    Console.WriteLine(v.Value.Value);
                    return new() { HasBeenManaged = true };
                }
                else
                {
                    return new();
                }
            }
            else
                return new();
        }
    }
}
