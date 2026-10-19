using System.Windows;
using FloatingB.Infrastructure.Windows;

namespace FloatingB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow(new WindowPositionService());
            mainWindow.Show();
        }
    }

}
