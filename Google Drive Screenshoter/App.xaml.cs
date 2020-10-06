using Google_Drive_Screenshoter.Properties;
using System.Windows;

namespace Google_Drive_Screenshoter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
