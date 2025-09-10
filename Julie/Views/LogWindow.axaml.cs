using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree; // <- WICHTIG
using Julie.ViewModels;

namespace Julie;

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
    }
}
