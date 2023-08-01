using System;
using System.IO;
using System.Threading.Tasks;
using BepInEx.Logging;
using Bloodstone;

namespace SkanksAIO.Logger.Handler;

internal class FileLogHandler : ILogHandler
{
    private const string LogFilePath = $"logs/{MyPluginInfo.PLUGIN_GUID}.log";

    public FileLogHandler()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);
    }

    public bool Accept(LogLevel level)
    {
        return level <= LogLevel.All;
    }

    public Task Handle(LogLevel level, string message)
    {
        using (var fs = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.None))
        {
            fs.Write(System.Text.Encoding.UTF8.GetBytes(FormatLog(level, message)));
        }
        return Task.CompletedTask;
    }

    private static string FormatLog(LogLevel level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return $"[{timestamp}] [{level}] {message}\r\n";
    }
}
