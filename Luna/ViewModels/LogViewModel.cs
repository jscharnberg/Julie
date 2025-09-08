using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Luna.Core.Enums;
using Luna.Core.Models;
using Luna.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Luna.ViewModels
{
    public partial class LogViewModel : ObservableObject
    {
        private readonly LogReader _logReader = new();

        [ObservableProperty]
        private ObservableCollection<LogEntry> logs = new();

        [ObservableProperty]
        private string currentFilePath = string.Empty;

        [ObservableProperty] private bool filterInfo = true;
        [ObservableProperty] private bool filterWarn = true;
        [ObservableProperty] private bool filterError = true;
        [ObservableProperty] private bool filterDebug = true;
        [ObservableProperty] private string searchText = "";

        public ObservableCollection<LogEntry> FilteredLogs { get; } = new();

        public string FooterText => string.IsNullOrEmpty(CurrentFilePath)
            ? $"{Logs.Count} Zeilen"
            : $"{CurrentFilePath} | {Logs.Count} Zeilen";

        public LogViewModel()
        {
            Logs.CollectionChanged += (s, e) => UpdateFilteredLogs();

            // Filter/ Search automatisch anwenden, wenn sich Properties ändern
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName is nameof(SearchText) or nameof(FilterInfo)
                    or nameof(FilterWarn) or nameof(FilterError) or nameof(FilterDebug))
                {
                    UpdateFilteredLogs();
                }
            };
        }

        private void UpdateFilteredLogs()
        {
            FilteredLogs.Clear();

            foreach (var log in Logs)
            {
                if (!string.IsNullOrEmpty(SearchText) &&
                    !log.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (log.LogType == LogLevel.Sequence && !FilterInfo) continue;
                if (log.LogType == LogLevel.Warning && !FilterWarn) continue;
                if (log.LogType == LogLevel.Error && !FilterError) continue;
                if (log.LogType == LogLevel.Debug && !FilterDebug) continue;

                FilteredLogs.Add(log);
            }

            OnPropertyChanged(nameof(FooterText));
        }

        [RelayCommand]
        public async Task LoadLogsAsync(string? filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                var dlg = new OpenFileDialog
                {
                    AllowMultiple = false
                };
                dlg.Filters.Add(new FileDialogFilter { Name = "Log Files", Extensions = { "tlg", "log" } });

                var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
                var window = lifetime.MainWindow;
                var result = await dlg.ShowAsync(window);

                if (result == null || result.Length == 0)
                    return;

                filePath = result[0];
            }

            CurrentFilePath = filePath;
            Logs.Clear();

            int lineNumber = 1;
            foreach (var entry in _logReader.ReadFile(filePath))
            {
                entry.Line = lineNumber++;
                Logs.Add(entry);
            }
        }

        [RelayCommand]
        public async Task LoadLogFolderAsync()
        {
            var dlg = new OpenFolderDialog { Title = "Ordner mit Log-Dateien auswählen" };
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
            var window = lifetime.MainWindow;

            var folderPath = await dlg.ShowAsync(window);
            if (string.IsNullOrEmpty(folderPath))
                return;

            CurrentFilePath = folderPath;

            var logFiles = Directory.GetFiles(folderPath, "*.log")
                                    .Concat(Directory.GetFiles(folderPath, "*.tlg"));

            Logs.Clear();
            int lineNumber = 1;

            foreach (var file in logFiles)
            {
                foreach (var entry in _logReader.ReadFile(file))
                {
                    entry.Line = lineNumber++;
                    Logs.Add(entry);
                }
            }
        }
    }
}
