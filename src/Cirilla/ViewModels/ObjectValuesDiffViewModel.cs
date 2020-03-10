using System.ComponentModel;

namespace Cirilla.ViewModels
{
    public class ObjectValuesDiffViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObjectValuesDiffViewModel(string name, object currentValue, object newValue = null)
        {
            Name = name;
            CurrentValue = currentValue;
            NewValue = newValue ?? currentValue; // Set to currentValue if there is no newValue
        }

        public string Name { get; set; }
        public object CurrentValue { get; set; }
        public object NewValue { get; set; }

        public bool HasChanges => !CurrentValue.Equals(NewValue);
    }
}
