namespace SLangRT.VM.Components;

public class SLComponentExecuteData
{
    /// <summary>
    /// A boolean to check if the line has been managed by the component
    /// </summary>
    public bool HasBeenManaged { get; set; } = false;

    /// <summary>
    /// The returned value. Null if unmanaged, or if the line didn't returned anything
    /// </summary>
    public object? ReturnValue { get; set; } = null;
}

/// <summary>
/// Represents a component of SLang (ex: variable manager, function manager...)
/// </summary>
public class SLComponent
{
    /// <summary>
    /// The paent <see cref="Runtime"/> instance
    /// </summary>
    public Runtime? Parent { get; internal set; } = null;

    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="Parent">The parent <see cref="Runtime"/></param>
    public SLComponent(Runtime? Parent)
    {
        this.Parent = Parent;
    }

    /// <summary>
    /// Execute the component
    /// </summary>
    /// <param name="ln">The current line</param>
    /// <returns>Some data like the return value...</returns>
    public virtual SLComponentExecuteData Execute(string ln)
    {
        return new();
    }
}
