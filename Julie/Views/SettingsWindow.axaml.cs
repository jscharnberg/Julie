using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Julie.ViewModels;

namespace Julie.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = new SettingsViewModel();
        }
    }
}