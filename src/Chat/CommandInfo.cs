using System.Reflection;

namespace SkanksAIO.Chat;

public class CommandInfo
{
    public string Name { get; }
    public string Description { get; }
    public MethodInfo Handler { get; }

    public CommandInfo(string name, string description, MethodInfo handler)
    {
        Name = name;
        Description = description;
        Handler = handler;
    }
}
