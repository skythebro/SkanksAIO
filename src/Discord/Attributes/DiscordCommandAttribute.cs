using System;

namespace SkanksAIO.Discord.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class DiscordCommandAttribute : Attribute
{
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";

    public DiscordCommandAttribute(string Name, string Description)
    {
        this.Name = Name;
        this.Description = Description;
    }
}