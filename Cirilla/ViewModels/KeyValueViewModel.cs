using System.ComponentModel;
using System.Diagnostics;

namespace Cirilla.ViewModels
{
    [DebuggerDisplay("{Key,nq} = {Value,nq}")]
    public class KeyValueViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Key { get; }
        public string Value { get; }

        public KeyValueViewModel(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
