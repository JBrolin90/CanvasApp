using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace avalonia.app;

public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        //        Styles.Add(new Avalonia.Themes.Fluent.FluentTheme());
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}