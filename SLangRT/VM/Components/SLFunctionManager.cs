using System.Text;

namespace SLangRT.VM.Components;

/// <summary>
/// Manage the functions
/// </summary>
public class SLFunctionManager : SLComponent
{
    /// <summary>
    /// The defined functions
    /// </summary>
    public Dictionary<string, Func<object[], object>> Functions { get; set; }

    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="Parent">The parent runtime</param>
    public SLFunctionManager(Runtime? Parent)
        : base(Parent)
    {
        Functions = new();
        Functions["print"] = (args) => {
            Console.WriteLine(args[0]);
            return null;
        };
    }

    /// <summary>
    /// Check if a line is a function call
    /// </summary>
    /// <param name="line">The line</param>
    /// <returns>true if it's a function call, else false</returns>
    public bool IsFunctionCall(string line)
    {
        return line.Contains("(") && line.EndsWith(")");
    }

    /// <summary>
    /// Execute the component
    /// </summary>
    /// <param name="ln">The current line</param>
    /// <returns>Some data like the return value...</returns>
    public override SLComponentExecuteData Execute(string ln)
    {
        if (IsFunctionCall(ln))
        {
            string functionName = ln.Substring(0, ln.IndexOf('(')).Trim();
            string argsString = ln.Substring(ln.IndexOf('(') + 1, ln.LastIndexOf(')') - ln.IndexOf('(') - 1);
            List<string> argsList = new List<string>();
            StringBuilder currentArg = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(argsString))
            {
                bool insideString = false;

                for (int i = 0; i < argsString.Length; i++)
                {
                    char c = argsString[i];

                    if (c == '"' && (i == 0 || argsString[i - 1] != '\\'))
                    {
                        insideString = !insideString;
                    }

                    if (c == ',' && !insideString)
                    {
                        argsList.Add(currentArg.ToString().Trim());
                        currentArg.Clear();
                    }
                    else
                    {
                        currentArg.Append(c);
                    }
                }
                if (currentArg.Length > 0)
                {
                    argsList.Add(currentArg.ToString().Trim());
                }
            }

            object[] args = argsList.Select(arg => Parent.Parser.ParseValue(arg)).ToArray();

            if (Functions.ContainsKey(functionName))
            {
                return new() { ReturnValue = Functions[functionName](args), HasBeenManaged = true };
            }
            else
            {
                throw new Exception($"Function doesn't exist! Function name: {functionName}");
            }
        }
        else
        {
            return new();
        }
    }
}
