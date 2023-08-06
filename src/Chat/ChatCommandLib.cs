using ProjectM;
using ProjectM.Network;
using SkanksAIO.Chat.Attributes;
using SkanksAIO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bloodstone.API;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;
using Unity.Entities;

namespace SkanksAIO.Chat;

internal class ChatCommandLib
{
    private static Dictionary<string, CommandInfo> commands { get; set; } = new Dictionary<string, CommandInfo>();

    public static Dictionary<string, CommandInfo> GetCommands => commands;

    internal void Init()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsSubclassOf(typeof(AbstractChatCommandsHandler)));

        foreach (var type in types)
        {
            foreach (var method in
                     type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var attribute = method.GetCustomAttribute<ChatCommandAttribute>();
                if (attribute == null) continue;

                Plugin.Logger?.LogDebug(
                    $"[ChatCommandLib] Registering Command: {attribute.Name.ToLower()} from {type.Name}");
                commands.Add(attribute.Name.ToLower(), new CommandInfo(attribute.Name, attribute.Description, method));
            }
        }
    }

    internal bool Handle(Entity chatMessageEntity)
    {
        try
        {
            var em = VWorld.Server.EntityManager;
            var chatMessageEvent = em.GetComponentDataAOT<ChatMessageEvent>(chatMessageEntity);
            var rawMessage = chatMessageEvent.MessageText.ToString();
            if (!rawMessage.StartsWith(Settings.ChatCommandPrefix.Value)) return true;

            var fromCharacter = em.GetComponentDataAOT<FromCharacter>(chatMessageEntity);
            em.TryGetComponentData<User>(fromCharacter.User, out var user);
            em.TryGetComponentData<PlayerCharacter>(fromCharacter.Character, out var character);
            var arguments = rawMessage.Split(' ').ToList();
            
            var command = arguments.Shift()[Settings.ChatCommandPrefix.Value.Length..];

            if (!commands.ContainsKey(command))
            {
                ServerChatUtils.SendSystemMessageToClient(em, user, $"Unknown command: {command}");
                Plugin.Logger?.LogError($"[ChatCommandLib] Unknown command: {command}");
                return true;
            }

            
            var method = commands[command].Handler;
            if (method.DeclaringType == null)
            {
                Plugin.Logger?.LogError($"[ChatCommandLib] Unable to find declaring type for method: {method.Name}");
                return true;
            }
            
            var instance = (AbstractChatCommandsHandler)Activator.CreateInstance(method.DeclaringType,
                new object[] { fromCharacter, rawMessage, user })!;
            
            if (method.GetCustomAttribute<AdminAttribute>() != null && !user.IsAdmin)
            {
                Plugin.Logger?.LogError(
                    $"[ChatCommandLib] User {user.CharacterName} attempted to run [{command}] but is not an admin.");
                instance.Reply($"You do not have permission to use this command.");
                return true;
            }
            
            var parameters = new List<object>();
            
            var getParameters = method.GetParameters().ToList();
            
            for (var i = 0; i < getParameters.Count; i++)
            {
                var parameter = getParameters[i];
                
                if (arguments.Count == 0 && parameter.HasDefaultValue)
                {
                    parameters.Add(parameter.DefaultValue!);
                    continue;
                }
                
                if (arguments.Count == 0)
                {
                    Plugin.Logger?.LogError($"[ChatCommandLib] Not enough arguments for command: {command}");
                    instance.Reply($"Not enough arguments for command: {command}");
                    break;
                }
                
                var argument = arguments.Shift();
                var type = parameter.ParameterType;
                parameters.Add(SmartCast(type, argument));
            }

            if (arguments.Count > 0)
            {
                var remainder = string.Join(" ", arguments);
                
                if (parameters.Count == method.GetParameters().Length)
                {
                    Plugin.Logger?.LogError($"[ChatCommandLib] Too many arguments for command: {command}");
                    instance.Reply($"Too many arguments for command: {command}");
                    return true;
                }
                
                // add remainder to the last parameter if the type is string
                if (parameters.Last() is string)
                {
                    parameters[parameters.Count - 1] = $"{parameters.Last()} {remainder}";
                }
                else
                {
                    Plugin.Logger?.LogError($"[ChatCommandLib] Too many arguments for command: {command}");
                    instance.Reply($"Too many arguments for command: {command}");
                }
            }

            method.Invoke(instance, parameters.ToArray());
            Plugin.Logger?.LogDebug($"[ChatCommandLib] {user.CharacterName} used command: {command}");
            return false;
        }
        catch (Exception e)
        {
            Plugin.Logger?.LogError($"[ChatCommandLib] An error occured: {e.Message}");
            Plugin.Logger?.LogError($"[ChatCommandLib] StackTrace: {e.StackTrace}");
            return true;

        }
    }

    private static object SmartCast(Type t, string input)
    {
        try
        {
            if (t == typeof(string))
                return input;
            else if (t == typeof(int))
                return int.Parse(input);
            else if (t == typeof(float))
                return float.Parse(input);
            else if (t == typeof(bool))
                return bool.Parse(input);
            else if (t == typeof(double))
                return double.Parse(input);
            else if (t == typeof(long))
                return long.Parse(input);
            else if (t == typeof(ulong))
                return ulong.Parse(input);
            else if (t == typeof(short))
                return short.Parse(input);
            else if (t == typeof(ushort))
                return ushort.Parse(input);
            else if (t == typeof(byte))
                return byte.Parse(input);
            else if (t == typeof(sbyte))
                return sbyte.Parse(input);
            else if (t == typeof(char))
                return char.Parse(input);
            else if (t == typeof(decimal))
                return decimal.Parse(input);
            else
            {
                Plugin.Logger?.LogWarning("Unable to auto-cast " + input + " to " + t.Name);
                return input;
            }
        }
        catch (Exception)
        {
            Plugin.Logger?.LogError("Unable to auto-cast " + input + " to " + t.Name);
            return input;
        }
    }

    public void OnchatMessage()
    {
    }
}