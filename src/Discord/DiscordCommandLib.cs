using Discord;
using Discord.Net;
using Discord.WebSocket;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BepInEx.Logging;
using SkanksAIO.Discord.Attributes;

namespace SkanksAIO.Discord;

class DiscordCommandLib
{
    public delegate Task MessageReceivedEventHandler(SocketUserMessage message);

    public event MessageReceivedEventHandler? MessageReceived;

    public delegate void ReadyEventHandler();

    public event ReadyEventHandler? Ready;

    private DiscordSocketClient client;
    
    private string token;

    private ITextChannel? broadcastChannel;
    
    private ITextChannel? AnnouncementChannel;

    private Dictionary<string, object> commandHandlers = new();
    private Dictionary<string, CommandInfo> commands = new();

    public DiscordCommandLib(string Token)
    {
        // todo fix System.MissingMethodException Void Newtonsoft.Json.JsonSerializer.add_Error(System.EventHandler`1<Newtonsoft.Json.Serialization.ErrorEventArgs
        token = Token ?? throw new ArgumentNullException(nameof(Token));

        var behaviour = Plugin.Instance?.AddComponent<DiscordBehaviour>()!;
        behaviour.MessageReceived += OnMessageReceived;
        behaviour.SlashCommandExecuted += OnSlashCommandExecuted;

        client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            UseInteractionSnowflakeDate = false,
        });

        client.Log += (log) =>
        {
            Plugin.Logger?.LogDebug("[DiscordLib] " + log.ToString());
            return Task.CompletedTask;
        };

        client.Ready += OnClientReady;
        client.SlashCommandExecuted += (arg) =>
        {
            behaviour.QueueSlashCommand(arg);
            return Task.CompletedTask;
        };

        client.MessageReceived += (message) =>
        {
            behaviour.QueueMessage(message);
            return Task.CompletedTask;
        };
    }

    public async Task Start()
    {
        try
        {
            Init();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
        }
        catch (Exception e)
        {
            Plugin.Logger?.LogError("[DiscordLib] " + e.Message);
        }
    }

    public async Task Stop()
    {
        await client.LogoutAsync();
        await client.StopAsync();
    }

    public async Task SendMessageAsync(string? text = null, bool isTTS = false, Embed? embed = null,
        RequestOptions? options = null, AllowedMentions? allowedMentions = null,
        MessageReference? messageReference = null, MessageComponent? components = null, ISticker[]? stickers = null,
        Embed[]? embeds = null, MessageFlags flags = MessageFlags.None, bool isAnnouncement = false)
    {
        switch (broadcastChannel)
        {
            case null when AnnouncementChannel == null:
                Plugin.Logger?.LogError("[DiscordLib] Broadcast and Announcement channels not set, Unable to send messages.");
                return;
            case null:
                Plugin.Logger?.LogError("[DiscordLib] Broadcast channel not set, Unable to send messages.");
                return;
        }

        if (AnnouncementChannel == null)
        {
            Plugin.Logger?.LogDebug("[DiscordLib] Announcement channel not set, sending messages via broadcastChannel.");
        }

        if (isAnnouncement)
        {
            await AnnouncementChannel!.SendMessageAsync(text, isTTS, embed, options, allowedMentions, messageReference,
                components, stickers, embeds, flags);
        }
        else
        {
            await broadcastChannel!.SendMessageAsync(text, isTTS, embed, options, allowedMentions, messageReference,
                components, stickers, embeds, flags);
        }

        
    }

    private void Init()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsDefined(typeof(DiscordCommandHandlerAttribute)));

        foreach (var type in types)
        {
            Plugin.Logger?.LogDebug($"[DiscordLib] Loading Handler: {type.Name}");
            var instance = Activator.CreateInstance(type);
            if (instance == null)
            {
                Plugin.Logger?.LogError($"[DiscordLib] Unable to create instance of {type.FullName}");
                continue;
            }

            commandHandlers.Add(type.FullName ?? throw new InvalidOperationException(), instance);

            foreach (var method in
                     type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attribute = method.GetCustomAttribute<DiscordCommandAttribute>();
                if (attribute == null) continue;

                var commandInfo = new CommandInfo(attribute.Name, attribute.Description, method);

                commandInfo.CommandBuilder
                    .WithDescription(attribute.Description)
                    .WithName(attribute.Name.ToLower());

                // Get all method parameters
                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType == typeof(SocketSlashCommand))
                    {
                        continue;
                    }

                    commandInfo.CommandBuilder.AddOption(parameter.Name?.ToLower(),
                        TypeToApplicationCommandOptionType(parameter.ParameterType), parameter.Name,
                        !parameter.HasDefaultValue);
                }

                commands.Add(attribute.Name.ToLower(), commandInfo);
            }
        }
    }

    private async Task OnClientReady()
    {
        if (Settings.ChannelId!.Value != 0)
        {
            broadcastChannel = await client.GetChannelAsync(Settings.ChannelId!.Value) as ITextChannel;
        }

        if (Settings.AnnounceChannelId!.Value != 0)
        {
            AnnouncementChannel = await client.GetChannelAsync(Settings.AnnounceChannelId!.Value) as ITextChannel;
        }

        var globalApplicationComands = await client.GetGlobalApplicationCommandsAsync();

        // foreach (var command in commands)
        // {
        //     var existingCommand = globalApplicationComands.FirstOrDefault(x => x.Name.ToLower() == command.Key.ToLower());
        //     if (existingCommand != null) { // FIXME: This does not check if the arguments / command options changed.
        //         continue;
        //     }

        //      Plugin.Logger?.LogDebug($"[DiscordLib] Registering Command: {command.Key}");
        //     await client.CreateGlobalApplicationCommandAsync(command.Value.commandBuilder.Build());
        // }

        await client.BulkOverwriteGlobalApplicationCommandsAsync(commands.Values.Select(x => x.CommandBuilder.Build())
            .ToArray());

        Plugin.Logger?.LogDebug("[DiscordLib] Client ready");
        var _ = Task.Run(() => Ready?.Invoke());
    }

    private async Task OnSlashCommandExecuted(SocketSlashCommand arg)
    {
        var command = arg.Data.Name.ToLower();
        if (!commands.ContainsKey(command))
        {
            Plugin.Logger?.LogError($"[DiscordLib] Unknown command: {command}");
            return;
        }

        var arguments = arg.Data.Options;
        var commandInfo = commands[command];
        var method = commandInfo.Handler;

        var requiresAdmin = method.DeclaringType != null && (method.GetCustomAttribute<AdminAttribute>() != null ||
                                                             method.DeclaringType
                                                                 .GetCustomAttribute<AdminAttribute>() != null);

        if (requiresAdmin &&
            (arg.User as SocketGuildUser)!.Roles.FirstOrDefault(x => x.Id == Settings.AdminRoleId!.Value) == null)
        {
            await arg.RespondAsync("You do not have permission to use this command.");
            return;
        }

        if (method == null!) return;

        if (method.DeclaringType == null)
        {
            Plugin.Logger?.LogError($"[DiscordLib] Unable to find declaring type for method: {method.Name}");
            return;
        }

        // method.DeclaringType
        var commandHandler = commandHandlers[method.DeclaringType.FullName!];
        if (commandHandler == null!)
        {
            Plugin.Logger?.LogError(
                $"[DiscordLib] Unable to find command handler for type: {method.DeclaringType.Name}");
            return;
        }

        var parametersToPass = new List<object>();

        foreach (var parameter in method.GetParameters())
        {
            if (parameter.ParameterType == typeof(SocketSlashCommand))
            {
                parametersToPass.Add(arg);
            }
            else
            {
                parametersToPass.Add(TryGetValue(arguments, parameter.Name!, out var value)
                    ? value!
                    : parameter.DefaultValue!);
            }
        }

        try
        {
            await (Task)method.Invoke(commandHandler, parametersToPass.ToArray())!;
        }
        catch (Exception e)
        {
            Plugin.Logger?.LogError("[DiscordLib] " + e.Message);
        }
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        if (message.Channel.Id != Settings.ChannelId!.Value) return;
        if (!(message is SocketUserMessage)) return;

        await (Task)MessageReceived?.Invoke((message as SocketUserMessage)!)!;
    }

    private static bool TryGetValue(IReadOnlyCollection<SocketSlashCommandDataOption> dict, string key,
        out object? value)
    {
        var r = dict.FirstOrDefault(x => x.Name.ToLower() == key.ToLower());
        if (r == null)
        {
            value = null;
            return false;
        }

        value = r.Value;
        return true;
    }

    private ApplicationCommandOptionType TypeToApplicationCommandOptionType(Type t)
    {
        if (t == typeof(string))
        {
            return ApplicationCommandOptionType.String;
        }
        else if (t == typeof(int) || t == typeof(long))
        {
            return ApplicationCommandOptionType.Integer;
        }
        else if (t == typeof(double))
        {
            return ApplicationCommandOptionType.Number;
        }
        else if (t == typeof(bool))
        {
            return ApplicationCommandOptionType.Boolean;
        }

        return ApplicationCommandOptionType.String;
    }
}