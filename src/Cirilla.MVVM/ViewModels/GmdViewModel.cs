﻿using Cirilla.Core.Extensions;
using Cirilla.Core.Models;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class GmdViewModel : ViewModelBase, IOpenFileViewModel
    {
        public GmdViewModel(FileInfo fileInfo)
        {
            Info = fileInfo;
            gmd = new GMD(fileInfo.FullName);

            for (int i = 0; i < gmd.Entries.Count; i++)
                entries.Add(new GmdEntryViewModel(i, gmd.Entries[i]));

            DeleteRowsCommand = ReactiveCommand.Create<IList<GmdEntryViewModel>>(DeleteRowsHandler);

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

        public ReactiveCommand<IList<GmdEntryViewModel>, Unit> DeleteRowsCommand { get; }

        [Reactive] public string KeySearchQuery { get; set; } = "";
        [Reactive] public string ValueSearchQuery { get; set; } = "";
        [Reactive] public GmdEntryViewModel? SelectedEntry { get; set; }

        private readonly ReadOnlyObservableCollection<GmdEntryViewModel> filteredEntries;
        public ReadOnlyObservableCollection<GmdEntryViewModel> FilteredEntries => filteredEntries;

        public FileInfo Info { get; }
        public bool CanClose => true;
        public bool CanSave => true;
        public string Title => Info.Name;

        private readonly GMD gmd;
        private readonly SourceList<GmdEntryViewModel> entries = new SourceList<GmdEntryViewModel>();
        private static readonly ILogger log = Log.ForContext<GmdViewModel>();

        public bool Close()
        {
            return true;
        }

        public async Task Save(string path)
        {
            await Task.Run(() => gmd.Save(path));
        }

        private void DeleteRowsHandler(IList<GmdEntryViewModel> rowsToDelete)
        {
            entries.Edit(x =>
            {
                foreach (var row in rowsToDelete)
                {
                    if (!x.Remove(row) || !gmd.Entries.Remove(row.entry))
                        log.Error($"Could not remove row '{row.Key}'.");
                }
            });
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
