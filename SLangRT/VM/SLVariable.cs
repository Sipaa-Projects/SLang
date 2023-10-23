namespace SLangRT.VM;

/// <summary>
/// Represents a variable in SLang.
/// </summary>
public class SLVariable
{
    public string Name { get; set; } = "";
    public object? Value { get; set; } = null;
    public Type? Type { get; set; } = null;
}
