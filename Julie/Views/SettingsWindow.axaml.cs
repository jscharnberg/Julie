using Avalonia.Controls;
using Avalonia.Input;
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

        private void SettingsWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is not SettingsViewModel vm)
                return;

            var isCtrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
            var isShift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

            if (isCtrl && !isShift)
            {
                switch (e.Key)
                {
                    case Key.S:
                        vm.Save();
                        e.Handled = true;
                        break;
                }
            }
        }

    }
}