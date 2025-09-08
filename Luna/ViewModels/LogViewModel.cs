using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Luna.Core.Models;
using Luna.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Luna.ViewModels
{
    public partial class LogViewModel : ObservableObject
    {
        private readonly LogReader _logReader = new();

        [ObservableProperty]
        private ObservableCollection<LogEntry> logs = new();

        [RelayCommand]
        private void LoadLogs(string filePath)
        {
            Logs.Clear();
            foreach (var entry in _logReader.ReadFile(filePath))
            {
                Logs.Add(entry); // funktioniert, wenn beides Luna.Core.Models.LogEntry ist
            }
        }
    }
}
