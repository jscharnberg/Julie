using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Julie.ViewModels;

namespace Julie.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnLoadLogClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Log Files", Extensions = { "log", "txt" } });
            var result = await dialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                var vm = DataContext as LogViewModel2;
                vm?.LoadLogFile(result[0]);
            }
        }
    }
}