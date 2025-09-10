using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Julie.Core.Enums;
using Julie.Core.Models;
using Julie.Core.Services.LogReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Julie.ViewModels
{
    public partial class LogViewModel : ObservableObject
    {
        //private readonly ILogReader _logReader;
        [ObservableProperty]
        private ObservableCollection<LogReaderOption> logReaderOptions = new();

        [ObservableProperty]
        private LogReaderOption? selectedLogReader;

        [ObservableProperty]
        private ObservableCollection<LogEntry> logs = new();

        [ObservableProperty]
        private string currentFilePath = string.Empty;

        private bool _autoUpdate = false;
        public bool AutoUpdate
        {
            get => _autoUpdate;
            set
            {
                if (_isFolderLoaded)
                {
                    // AutoUpdate kann bei geladenem Ordner nicht aktiviert werden
                    _autoUpdate = false;
                }
                else
                {
                    bool wasFalse = !_autoUpdate;
                    _autoUpdate = value;

                    if (wasFalse && _autoUpdate)
                    {
                        if (!string.IsNullOrEmpty(CurrentFilePath))
                        {
                            _ = LoadLogsAsync(CurrentFilePath);
                        }
                    }
                }

                OnPropertyChanged(nameof(AutoUpdate));
            }
        }

        [ObservableProperty]
        private string searchText = string.Empty;

        public bool IsAutoUpdateEnabled => !_isFolderLoaded;

        [ObservableProperty] private bool filterInfo = true;
        [ObservableProperty] private bool filterWarn = true;
        [ObservableProperty] private bool filterError = true;
        [ObservableProperty] private bool filterDebug = true;
        //[ObservableProperty] private string searchText = "";

        private ObservableCollection<LogEntry> _filteredLogs = new ObservableCollection<LogEntry>();
        public ObservableCollection<LogEntry> FilteredLogs
        {
            get => _filteredLogs;
            set { _filteredLogs = value; OnPropertyChanged(); }
        }

        private readonly List<FileSystemWatcher> _watchers = new();

        private bool _isFolderLoaded = false;

        [ObservableProperty]
        private string footerText = string.Empty;

        public LogViewModel()
        {
            LogReaderOptions.Add(new LogReaderOption { Name = "Cotas Logger", Reader = new CotasLogReader() });

            SelectedLogReader = LogReaderOptions.FirstOrDefault();

            Logs.CollectionChanged += (s, e) => OnLogsChanged();

            // Filter/ Search automatisch anwenden, wenn sich Properties ändern
            this.PropertyChanged += (s, e) =>
            {
                if(e.PropertyName is nameof(SearchText)
        or nameof(FilterInfo)
        or nameof(FilterWarn)
        or nameof(FilterError)
        or nameof(FilterDebug))
    {
                    UpdateFilteredLogs();
                }
            };


            Logs.CollectionChanged += (s, e) => UpdateFilteredLogs();
        }

        private void SetFolderLoaded(bool isFolder)
        {
            _isFolderLoaded = isFolder;
            if (isFolder) _autoUpdate = false;
            OnPropertyChanged(nameof(AutoUpdate));
            OnPropertyChanged(nameof(IsAutoUpdateEnabled));
        }

        public void DisposeWatchers()
        {
            foreach (var w in _watchers)
            {
                w.EnableRaisingEvents = false;
                w.Dispose();
            }
            _watchers.Clear();
        }

        private void AppendNewLines(string filePath)
        {
            var lines = SelectedLogReader!.Reader.ReadFile(filePath).ToList();
            int startLine = Logs.Count + 1;

            foreach (var entry in lines.Skip(startLine - 1))
            {
                entry.Line = Logs.Count + 1;
                entry.SourceFileName = Path.GetFileName(filePath);
                Logs.Add(entry);
            }

            // Optional AutoScroll
            AutoUpdateRequested?.Invoke(this, EventArgs.Empty);
        }

        private void WatchFile(string filePath)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(filePath)!;
            watcher.Filter = Path.GetFileName(filePath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            watcher.Changed += async (s, e) =>
            {
                if (!AutoUpdate) return;

                // Dispatcher sicherstellen, dass UI-Thread genutzt wird
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    AppendNewLines(filePath);
                });
            };

            watcher.EnableRaisingEvents = true;
            _watchers.Add(watcher);
        }

        private void OnLogsChanged()
        {
            UpdateFilteredLogs();
            // Wir feuern ein Event, dass die View scrollen soll
            AutoUpdateRequested?.Invoke(this, EventArgs.Empty);
        }

        // Event für die View
        public event EventHandler? AutoUpdateRequested;

        private void UpdateFilteredLogs()
        {
            // Filter nur auf sichtbare Logs anwenden
            var filtered = Logs
                .OrderByDescending(log => log.Line)
                .Where(log =>
                    (string.IsNullOrEmpty(SearchText) || log.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                    ((log.LogType == LogLevel.Sequence && FilterInfo) ||
                     (log.LogType == LogLevel.Warning && FilterWarn) ||
                     (log.LogType == LogLevel.Error && FilterError) ||
                     (log.LogType == LogLevel.Debug && FilterDebug))
                )
                .ToList();

            // Alte Einträge nur einmal löschen und neue hinzufügen
            FilteredLogs.Clear();
            foreach (var log in filtered)
                FilteredLogs.Add(log);

            // FooterText nur einmal setzen
            FooterText = string.IsNullOrEmpty(CurrentFilePath)
                ? $"{FilteredLogs.Count} Zeilen"
                : $"{CurrentFilePath} | {FilteredLogs.Count} Zeilen";
        }


        private async Task LoadLogsFromFilesAsync(IEnumerable<string> files)
        {
            if (SelectedLogReader == null || files == null)
                return;

            Logs.Clear();

            foreach (var file in files)
            {
                int lineNumber = 1;
                foreach (var entry in SelectedLogReader.Reader.ReadFile(file))
                {
                    entry.Line = lineNumber++;
                    entry.SourceFileName = Path.GetFileName(file);
                    Logs.Add(entry);
                }
            }
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


                SetFolderLoaded(false);

                filePath = result[0];
            }

            CurrentFilePath = filePath;
            await LoadLogsFromFilesAsync(new[] { filePath });

            WatchFile(filePath);
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
            SetFolderLoaded(true);

            var logFiles = Directory.GetFiles(folderPath, "*.log")
                                    .Concat(Directory.GetFiles(folderPath, "*.tlg"));

            await LoadLogsFromFilesAsync(logFiles);
            //WatchFolder(folderPath);
        }

        
    }
}
