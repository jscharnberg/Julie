using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

[Flags]
public enum LogLevel
{
    Null = 0x0000,
    Error = 0x0001,
    Warning = 0x0002,
    RetVal = 0x0004,
    ObjState = 0x0008,
    IpcMsg = 0x0010,
    Telegram = 0x0020,
    Sequence = 0x0040,
    Function = 0x0080,
    All = 0x00FF,
    Debug = 0x0FFF
}

public class LogLevelCount : INotifyPropertyChanged
{
    public string LevelName { get; set; }
    public int Count { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class LogViewModel2 : INotifyPropertyChanged
{
    public ObservableCollection<LogLevelCount> LogLevels { get; } = new ObservableCollection<LogLevelCount>();

    public void LoadLogFile(string filePath)
    {
        var counts = new Dictionary<string, int>();
        var regex = new Regex(@"^@(?<type>\d+)");

        foreach (var line in File.ReadLines(filePath))
        {
            var match = regex.Match(line);
            if (match.Success && int.TryParse(match.Groups["type"].Value, out int typeNum))
            {
                var levelFlags = (LogLevel)typeNum;

                foreach (LogLevel flag in Enum.GetValues(typeof(LogLevel)))
                {
                    if (flag != LogLevel.All && flag != LogLevel.Debug && flag != LogLevel.Null)
                    {
                        if (levelFlags.HasFlag(flag))
                        {
                            var flagName = flag.ToString();
                            if (counts.ContainsKey(flagName))
                                counts[flagName]++;
                            else
                                counts[flagName] = 1;
                        }
                    }
                }
            }
        }

        LogLevels.Clear();
        foreach (var kvp in counts)
        {
            LogLevels.Add(new LogLevelCount { LevelName = kvp.Key, Count = kvp.Value });
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
