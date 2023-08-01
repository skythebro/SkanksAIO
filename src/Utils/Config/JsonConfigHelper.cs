using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SkanksAIO;

public static class JsonConfigHelper
{
    private static Dictionary<string, string> DefaultMessagesDictionary => new()
    {
        {  "online" , "%user% is online!" },
        {  "offline" , "%user% has gone offline!" },
        {  "newUserCreatedCharacter" , "%user% has finished character creation!" },
        {  "newUserOnline" , "A new vampire just joined" },
        {  "newUserOffline" , "A vampire left before finishing character creation" }
    };

    private static Dictionary<string, string> DefaultOnlineMessages => new()
    {
        {  "CharacterName" , "%user% is back baby!" },
        {  "CharacterName2" , "The best V rising player: %user% is here!" }
    };


    private static Dictionary<string, string> DefaultOfflineMessages => new()
    {
        {  "CharacterName" , "%user% went offline..." },
        {  "CharacterName2" , "Where did %user% go? They went offline..." }
    };

    private static List<string> DefaultAnnouncements => new()
    {
        {  "Announcement 1" },
        {  "Announcement 2" },
        {  "Announcement 3" }
    };
    
    private static List<ulong> DefaultVips => new()
    {
        {  76561197960287930 },
        {  76561197960287931 },
        {  76561197960287932 }
    };
    
    private static Dictionary<string, string> DefaultMessage { get; set; } = new Dictionary<string, string>();

    private static Dictionary<string, string> UsersOnlineMessages { get; set; } = new Dictionary<string, string>();

    private static Dictionary<string, string> UsersOfflineMessages { get; set; } = new Dictionary<string, string>();
    
    private static List<string> Announcements = new();
    
    private static List<ulong> Vips = new();
    
    private static float AnnouncementInterval { get; set; }
    
    public static void CreateJsonConfigs()
    {
        
        if (!Directory.Exists(Plugin.ConfigPath)) Directory.CreateDirectory(Plugin.ConfigPath);
            
        string jsonOutPut;
        if (!File.Exists(Path.Combine(Plugin.ConfigPath, "Messages.json")))
        {
            jsonOutPut = JsonSerializer.Serialize(DefaultMessagesDictionary);
            File.WriteAllText(Path.Combine(Plugin.ConfigPath, "Messages.json"), jsonOutPut);
        }

        if (!File.Exists(Path.Combine(Plugin.ConfigPath, "OnlineMessage.json")))
        {
            jsonOutPut = JsonSerializer.Serialize(DefaultOnlineMessages);
            File.WriteAllText(Path.Combine(Plugin.ConfigPath, "OnlineMessage.json"), jsonOutPut);
        }
            
        if (!File.Exists(Path.Combine(Plugin.ConfigPath, "OfflineMessage.json")))
        {
            jsonOutPut = JsonSerializer.Serialize(DefaultOfflineMessages);
            File.WriteAllText(Path.Combine(Plugin.ConfigPath, "OfflineMessage.json"), jsonOutPut);
        }
            
        if (!File.Exists(Path.Combine(Plugin.ConfigPath, "Announcements.json")))
        {
            jsonOutPut = JsonSerializer.Serialize(DefaultAnnouncements);
            File.WriteAllText(Path.Combine(Plugin.ConfigPath, "Announcements.json"), jsonOutPut);
        }
        
        if (!File.Exists(Path.Combine(Plugin.ConfigPath, "Vips.json")))
        {
            jsonOutPut = JsonSerializer.Serialize(DefaultVips);
            File.WriteAllText(Path.Combine(Plugin.ConfigPath, "Vips.json"), jsonOutPut);
        }
    }
    
    public static void LoadAllConfigs()
    {
        string json;
        json = File.ReadAllText(Path.Combine(Plugin.ConfigPath, "Messages.json"));
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        SetDefaultMessage(dict!);
        
        json = File.ReadAllText(Path.Combine(Plugin.ConfigPath, "OnlineMessage.json"));
        dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        SetOnlineMessage(dict!);
        
        json = File.ReadAllText(Path.Combine(Plugin.ConfigPath, "OfflineMessage.json"));
        dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        SetOfflineMessage(dict!);
        
        json = File.ReadAllText(Path.Combine(Plugin.ConfigPath, "Announcements.json"));
        var list = JsonSerializer.Deserialize<List<string>>(json);
        SetAnnouncements(list!);

        json = File.ReadAllText(Path.Combine(Plugin.ConfigPath, "Vips.json"));
        var vips = JsonSerializer.Deserialize<List<ulong>>(json);
        SetVips(vips!);
        
    }

    

    public static string GetDefaultMessage(string value)
    {
        
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        
        return DefaultMessage.ContainsKey(value) ? DefaultMessage[value] : value;
    }
    private static bool SetDefaultMessage(Dictionary<string, string> value)
    {
        if (value.Count == 0)
        {
            return false;
        }

        DefaultMessage = value;
        return true;
    }
    
    public static string GetOnlineMessage(string user)
    {
        if (string.IsNullOrEmpty(user))
        {
            return GetDefaultMessage("newUserOnline");
        }

        return UsersOnlineMessages.ContainsKey(user) ? UsersOnlineMessages[user] : GetDefaultMessage("online");
    }

    private static bool SetOnlineMessage(Dictionary<string, string> value)
    {
        if (value.Count == 0)
        {
            return false;
        }

        UsersOnlineMessages = value;
        return true;
    }
    
    public static string GetOfflineMessage(string user)
    {
        if (string.IsNullOrEmpty(user))
        {
            return GetDefaultMessage("newUserOffline");
        }

        if (UsersOfflineMessages.ContainsKey(user))
        {
            return UsersOfflineMessages[user];
        }
        else
        {
            return GetDefaultMessage("offline");
        }
    }
    
    public static bool SetOfflineMessage(Dictionary<string, string> value)
    {
        if (value.Count == 0)
        {
            return false;
        }

        UsersOfflineMessages = value;
        return true;
    }

    private static void SetAnnouncements(List<string> announcements)
    {
        Announcements = announcements;
    }

    public static List<string> GetAnnouncements()
    {
        return Announcements;
    }
    
    public static List<ulong> GetVips()
    {
        return Vips;
    }
    private static void SetVips(List<ulong> vips)
    {
        Vips = vips;
    }

    public static float GetAnnouncementInterval()
    {
        return AnnouncementInterval;
    }

    public static void SetAnnouncementInterval(float announcementInterval)
    {
        AnnouncementInterval = announcementInterval;
    }
}