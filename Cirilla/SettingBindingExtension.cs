using Cirilla.Properties;
using System.Windows.Data;

namespace Cirilla
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path) : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            Source = Settings.Default.Config;
            Mode = BindingMode.TwoWay;
        }
    }
}
