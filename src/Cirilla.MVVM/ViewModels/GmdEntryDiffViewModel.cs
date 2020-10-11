using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace Cirilla.MVVM.ViewModels
{
    public class GmdEntryDiffViewModel : ViewModelBase
    {
        public GmdEntryDiffViewModel(GmdEntryViewModel gmdEntry, string? newValue = null)
        {
            if (gmdEntry.Key == null)
                throw new Exception("Can't create a GmdEntryDiffViewModel for a GmdEntryViewModel where the 'Key' is null.");

            Entry = gmdEntry;
            NewValue = newValue ?? gmdEntry.Value;

            //this.WhenAnyValue(x => x.Entry.Value, x => x.NewValue,
            //        (currentValue, newValue) => !currentValue.Equals(newValue))
            //    .Do(Console.WriteLine)
            //    .ToPropertyEx(this, x => x.HasChanges);
        }

        public GmdEntryViewModel Entry { get; }
        public bool HasChanges => !Entry.Value.Equals(NewValue);

        [Reactive] public object NewValue { get; set; }
        //[ObservableAsProperty] public bool HasChanges { get; }
    }
}
