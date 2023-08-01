using System;

namespace SkanksAIO.Discord.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class DiscordCommandAttribute : Attribute
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public DiscordCommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}