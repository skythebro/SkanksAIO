using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Discord;

namespace SkanksAIO.Discord;

public class CommandInfo
{
    private string Name { get; }
    private string Description { get; }
    public MethodInfo Handler { get; }
    public SlashCommandBuilder CommandBuilder {get;}

    public CommandInfo(string name, string description, MethodInfo handler)
    {
        Name = name;
        Description = description;
        Handler = handler;

        CommandBuilder = new SlashCommandBuilder();
    }
}
