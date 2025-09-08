using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Luna;

public partial class LogWindow : Window
{
    public LogWindow()
    {
        AvaloniaXamlLoader.Load(this);
    }
}