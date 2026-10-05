using Avalonia;
using System;

namespace Dareu.LM113.App;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            try { System.IO.File.AppendAllText("unhandled.log", "Unhandled: " + e.ExceptionObject + Environment.NewLine); } catch { }
        };
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            try { System.IO.File.AppendAllText("unhandled.log", "TaskException: " + e.Exception + Environment.NewLine); } catch { }
            e.SetObserved();
        };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            try { System.IO.File.WriteAllText("crash.log", ex.ToString()); } catch { }
            Console.Error.WriteLine(ex);
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
