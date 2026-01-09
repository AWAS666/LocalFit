using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LocalFit.Lib.DataBase;
using LocalFit.ViewModels;
using LocalFit.Views;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit;

public partial class App : Application
{
    public override void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               //.WriteTo.File(
               //    logPath,
               //    rollingInterval: RollingInterval.Day,
               //    retainedFileCountLimit: 7)
               .CreateLogger();

        Log.Information("Application starting");
        SetupGlobalExceptionHandlers();
        EnsureDatabaseMigrated();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void SetupGlobalExceptionHandlers()
    {
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Log.Error(e.Exception, "Unobserved task exception occurred");
            e.SetObserved();
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Unhandled exception occurred. Application will terminate: {IsTerminating}", e.IsTerminating);
            }
            else
            {
                Log.Fatal("Unhandled exception occurred (non-Exception object): {ExceptionObject}. Application will terminate: {IsTerminating}",
                    e.ExceptionObject, e.IsTerminating);
            }
        };
    }

    private static void EnsureDatabaseMigrated()
    {
        try
        {
            using var db = new UserContext();
            db.Database.Migrate();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error applying database migrations");
        }
    }
}