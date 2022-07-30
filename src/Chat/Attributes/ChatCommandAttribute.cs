using System;

namespace SkanksAIO.Chat.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ChatCommandAttribute : Attribute
{
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";

    public ChatCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}