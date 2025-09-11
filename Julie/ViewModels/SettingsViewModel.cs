using Avalonia.Markup.Xaml.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Julie.Core.Models;
using Julie.Core.Services;
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

            AvailableLoggers = new ObservableCollection<string>(_settings.AvailableLoggers);

            SelectedLogger = _settings.LoggerType;
            Template = _settings.Template;
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
            _settings.LoggerType = SelectedLogger;
            _settings.Template = Template;

            JulieSettingsManager.Save(_settings);
        }

        [RelayCommand]
        public void Reset()
        {
            _settings = new JulieSettings();
            SelectedLogger = _settings.LoggerType;
            Template = _settings.Template;
        }
    }
}
