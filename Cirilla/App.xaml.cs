using Cirilla.Properties;
using System.Windows;

namespace GMDEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Settings.Default.Config == null)
                Settings.Default.Config = new Cirilla.CirillaConfig();
        }
    }
}
