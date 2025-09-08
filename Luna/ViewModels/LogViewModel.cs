using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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

        [ObservableProperty]
        private string currentFilePath = string.Empty;

        public string FooterText => string.IsNullOrEmpty(CurrentFilePath) ? $"0 Zeilen" : $"{CurrentFilePath} | {Logs.Count} Zeilen";

        [RelayCommand]
        public async Task LoadLogsAsync(string? filePath = null)
        {
            // Wenn kein Pfad übergeben wird, Dialog öffnen
            if (string.IsNullOrEmpty(filePath))
            {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter() { Name = "Log Files", Extensions = { "tlg", "log" } });
                dlg.AllowMultiple = false;

                var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
                var window = lifetime.MainWindow;
                var result = await dlg.ShowAsync(window);

                if (result == null || result.Length == 0)
                    return;

                filePath = result[0];
            }

            CurrentFilePath = filePath;

            // Logs laden
            Logs.Clear();

            int lineNumber = 1;
            foreach (var entry in _logReader.ReadFile(filePath))
            {
                entry.Line = lineNumber++;
                Logs.Add(entry);
            }

            OnPropertyChanged(nameof(FooterText)); // Footer aktualisieren
        }
    }
}
