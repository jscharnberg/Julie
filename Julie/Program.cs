using Avalonia;
using Julie.Core.Models;
using Julie.Core.Services;
using Julie.Core.Services.LogReader;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Julie
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {

            var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Julie", "logs", "log-.log");

            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    logPath,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}) ({Method}:{CallerLineNumber}) {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day, 
                    retainedFileCountLimit: 7, 
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            try
            {
                Log.Information("Starting Application");
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application crashed");
            }
            finally
            {
                Log.Information("Application stopped");
                Log.CloseAndFlush();
            }

        }
        //=> BuildAvaloniaApp()
        //    .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
