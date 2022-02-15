using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Xune.Backend;
using Xune.ViewModels;
using Xune.Views;

namespace Xune
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            Name = "Xune";            
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dc = new DiscChanger();
                MainViewModel.Instance = new MainViewModel(dc, new LibraryManager());
                desktop.MainWindow = new MainWindow
                {
                    DataContext = MainViewModel.Instance
                };

                desktop.MainWindow.Closing += (sender, e) =>
                {
                    dc.Dispose();
                };
            }

            base.OnFrameworkInitializationCompleted();
        }        
    }
}