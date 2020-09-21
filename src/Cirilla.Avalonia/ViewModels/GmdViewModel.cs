using Cirilla.Core.Models;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;

namespace Cirilla.Avalonia.ViewModels
{
    public class GmdViewModel : ViewModelBase, IFileViewModel
    {
        public GmdViewModel(FileInfo fileInfo)
        {
            Info = fileInfo;
            gmd = new GMD(fileInfo.FullName);

            for (int i = 0; i < gmd.Entries.Count; i++)
                entries.Add(new GmdEntryViewModel(i, gmd.Entries[i]));

            var keyFilter = this.WhenValueChanged(t => t.KeySearchQuery)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(KeyFilterPredicate);

            var valueFilter = this.WhenValueChanged(t => t.ValueSearchQuery)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(ValueFilterPredicate);

            entries.Connect()
                .Filter(keyFilter)
                .Filter(valueFilter)
                .Sort(SortExpressionComparer<GmdEntryViewModel>.Ascending(x => x.Index))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out filteredEntries)
                .DisposeMany()
                .Subscribe();
        }

        [Reactive] public string KeySearchQuery { get; set; } = "";
        [Reactive] public string ValueSearchQuery { get; set; } = "";

        private readonly ReadOnlyObservableCollection<GmdEntryViewModel> filteredEntries;
        public ReadOnlyObservableCollection<GmdEntryViewModel> FilteredEntries => filteredEntries;

        public FileInfo Info { get; }
        public bool CanClose => true;
        public bool CanSave => true;

        private readonly GMD gmd;
        private readonly SourceList<GmdEntryViewModel> entries = new SourceList<GmdEntryViewModel>();

        public bool Close()
        {
            return true;
        }

        public void Save(string path)
        {

        }

        private static Func<GmdEntryViewModel, bool> KeyFilterPredicate(string key)
        {
            if (string.IsNullOrEmpty(key))
                return x => true;
            return x => x.Key?.Contains(key, StringComparison.OrdinalIgnoreCase) == true;
        }

        private static Func<GmdEntryViewModel, bool> ValueFilterPredicate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return x => true;
            return x => x.Value.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
