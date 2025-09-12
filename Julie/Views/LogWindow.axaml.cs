using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree; // <- WICHTIG
using Julie.ViewModels;

namespace Julie.Views;

public partial class LogWindow : Window
{
    public LogWindow()
    {
        AvaloniaXamlLoader.Load(this);

        this.AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is LogViewModel vm)
            {
                vm.AutoUpdateRequested += (s, e) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        var scroll = LogList.FindDescendantOfType<ScrollViewer>();
                        scroll?.ScrollToEnd();
                    });
                };
            }
        };

        this.KeyDown += LogWindow_KeyDown;
    }

    private void LogWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not LogViewModel vm)
            return;

        var isCtrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        var isShift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

        if (isCtrl && !isShift)
        {
            switch (e.Key)
            {
                case Key.F: // Strg+F → Suche fokussieren
                    FocusSearchBox();
                    e.Handled = true;
                    break;
                case Key.O: // Strg+O → Datei öffnen
                    vm.LoadLogsCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.OemPeriod: // Strg+. → Settings
                    vm.OpenSettingsCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.U:
                    vm.AutoUpdate = !vm.AutoUpdate;
                    e.Handled = true;
                    break;
            }
        }
        else if (isCtrl && isShift)
        {
            if (e.Key == Key.O) // Strg+Shift+O → Ordner öffnen
            {
                vm.LoadLogFolderCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void FocusSearchBox()
    {
        var searchBox = this.FindControl<TextBox>("SearchBox"); // XAML: x:Name="SearchBox"
        searchBox?.Focus();
    }
}
