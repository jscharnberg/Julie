using Avalonia;
using Avalonia.Markup.Xaml.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Julie.Core.Models;
using Julie.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Julie.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private JulieSettings _settings;

        [ObservableProperty]
        private ObservableCollection<TemplateItem> templates;

        [ObservableProperty]
        private TemplateItem selectedTemplate;

        public SettingsViewModel()
        {
            _settings = JulieSettingsManager.Load();

            SelectedTheme = _settings.Theme;

            Template = _settings.SeriLogTemplate;
        }

        [ObservableProperty]
        private ObservableCollection<string> availableLanguages = new() { "Deutsch", "English" };

        [ObservableProperty]
        private string selectedLanguage = "Deutsch";

        [ObservableProperty]
        private ObservableCollection<string> availableLoggers;

        [ObservableProperty]
        private string selectedLogger;

        [ObservableProperty]
        private string template;

        [RelayCommand]
        public void ChangeLanguage()
        {
            // Implementiere Locale-Switch hier
        }

        [RelayCommand]
        public void ToggleTheme()
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (Application.Current is App app)
                {
                    // Prüfen, welches Theme aktuell ist
                    var current = app.RequestedThemeVariant;

                    // Wechseln zwischen Dark und Light
                    if (current == Avalonia.Styling.ThemeVariant.Dark)
                        app.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;
                    else
                        app.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
                }
            });
        }




        [RelayCommand]
        public void AddTemplate()
        {
            var newTemplate = new TemplateItem { Name = "Neu", Content = "" };
            Templates.Add(newTemplate);
            SelectedTemplate = newTemplate;
        }

        [RelayCommand]
        public void DeleteTemplate()
        {
            if (SelectedTemplate != null)
            {
                Templates.Remove(SelectedTemplate);
            }
        }

        [RelayCommand]
        public void SaveTemplate()
        {
            if (SelectedTemplate != null)
            {
                SelectedTemplate.Content = Template;
            }
        }

        [RelayCommand]
        public void Save()
        {
            _settings.SeriLogTemplate = Template;

            JulieSettingsManager.Save(_settings);
        }
        //public void Save()
        //{
        //    _settings.LoggerType = SelectedLogger;
        //    _settings.Template = Template;

        //    JulieSettingsManager.Save(_settings);

        //    // Session aktualisieren
        //    AppState.UpdateSettings(_settings);

        //    // Optional: Event feuern, damit andere VM reagieren können
        //    SettingsChanged?.Invoke(this, EventArgs.Empty);
        //}

        [ObservableProperty]
        private string selectedTheme = "System";

        public ObservableCollection<string> AvailableThemes { get; } = new()
    {
        "Light",
        "Dark",
        "System"
    };


        partial void OnSelectedThemeChanged(string value)
        {
            ApplyTheme(value);
        }

        private void ApplyTheme(string theme)
        {
            if (Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                var app = (App)Application.Current;
                app.RequestedThemeVariant = theme switch
                {
                    "Light" => Avalonia.Styling.ThemeVariant.Light,
                    "Dark" => Avalonia.Styling.ThemeVariant.Dark,
                    _ => Avalonia.Styling.ThemeVariant.Default
                };


                _settings.Theme = SelectedTheme; // Speichern des Themes
                JulieSettingsManager.Save(_settings);
            }
        }

        [RelayCommand]
        public void Reset()
        {
            _settings = new JulieSettings();
            Template = _settings.SeriLogTemplate;
        }

        public event EventHandler SettingsChanged;
    }
}
