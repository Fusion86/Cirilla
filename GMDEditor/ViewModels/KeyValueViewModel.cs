using System;
using System.ComponentModel;

namespace GMDEditor.ViewModels
{
    public enum EditCondition
    {
        Always,
        UnsafeOnly,
        Never,
    }

    public class KeyValueViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string OriginalKey { get; }
        public string OriginalValue { get; }
        public bool ValueFrozen { get; set; }

        private string _key;
        private string _value;

        public EditCondition CanEditKey { get; }
        public EditCondition CanEditValue { get; }

        public KeyValueViewModel(
            string key,
            string value,
            EditCondition canEditKey = EditCondition.Never,
            EditCondition canEditValue = EditCondition.Always)
        {
            _key = key;
            OriginalKey = key;

            _value = value;
            OriginalValue = value;

            CanEditKey = canEditKey;
            CanEditValue = canEditValue;
        }

        public string Key
        {
            get => _key;

            set
            {
                if (CanEditKey == EditCondition.Always)
                    _key = value;
                else if (CanEditKey == EditCondition.UnsafeOnly)
                    throw new NotImplementedException();
            }
        }

        public string Value
        {
            get => _value;

            set
            {
                if (CanEditValue == EditCondition.Always)
                {
                    _value = value;
                }
                else if (CanEditValue == EditCondition.UnsafeOnly)
                    throw new NotImplementedException();
            }
        }
    }
}
