using System.IO;
using System.Reflection;
using System.Windows;
using Serilog;
using Serilog.Sinks.RollingFileAlternate;

namespace ECS
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var logdir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                  .WriteTo.RollingFileAlternate(logdir, fileSizeLimitBytes: 1048576)
                                                  .CreateLogger();
            Log.Information("Hello world!");
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
