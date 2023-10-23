using SLangRT;
using System.Reflection;
using System.Text;

namespace SLang;

public class Program
{
    static Runtime? rt;

    static void Main(string[] args)
    {
        rt = new();
        Console.WriteLine($"SLang R.E.P.L. (Version {Assembly.GetExecutingAssembly().GetName().Version}, Runtime version {typeof(Runtime).Assembly.GetName().Version}");
        ExecuteREPL();
    }

    static void ExecuteREPL()
    {
        try
        {
            StringBuilder multiLineBuffer = new StringBuilder();
            bool isMultiLine = false;

            while (true)
            {
                Console.Write(isMultiLine ? "... " : ">>> "); // Put 3 of '>' so we don't confuse the programmer.
                string? line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                    break;

                if (line.EndsWith("{"))
                {
                    isMultiLine = true;
                    multiLineBuffer.Append(line + " ");
                }
                else if (line.EndsWith("}") || line.EndsWith("};"))
                {
                    isMultiLine = false;
                    multiLineBuffer.Append(line);
                    string singleLineCode = multiLineBuffer.ToString();
                    multiLineBuffer.Clear();
                    if (rt != null)
                        rt.ExecuteLine(singleLineCode);
                }
                else
                {
                    if (isMultiLine)
                    {
                        multiLineBuffer.Append(line + " ");
                    }
                    else
                    {
                        if (rt != null)
                            rt.ExecuteLine(line);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}");
            ExecuteREPL();
        }
    }
}
