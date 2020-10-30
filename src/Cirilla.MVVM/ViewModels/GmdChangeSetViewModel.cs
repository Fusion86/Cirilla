using Cirilla.MVVM.Common;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public class GmdChangeSetViewModel : ViewModelBase
    {
        public GmdChangeSetViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            var canPopulateImportEntries = this.WhenAnyValue(x => x.SourceGmd, (GmdViewModel? x) => x != null);

            ApplyAndSaveCommand = ReactiveCommand.CreateFromTask(ApplyChangeSetAndSave);
            UpdateDiffEntriesCommand = ReactiveCommand.Create(UpdateDiffEntries, canPopulateImportEntries);

            valuesToUpdate.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out valuesToUpdateBinding)
                .DisposeMany()
                .Subscribe();

            changeSet.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out changeSetBinding)
                .DisposeMany()
                .Subscribe();

            this.WhenAnyValue(x => x.SourceGmd, x => x.HideUnchangedEntries)
                .Where(x => x.Item1 != null)
                .Select(_ => Unit.Default)
                .InvokeCommand(UpdateDiffEntriesCommand);
        }

        public ReactiveCommand<Unit, Unit> ApplyAndSaveCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateDiffEntriesCommand { get; }

        [Reactive] public bool HideUnchangedEntries { get; set; }
        [Reactive] public GmdViewModel? SourceGmd { get; set; }

        public ReadOnlyObservableCollection<StringKeyValuePair> ValuesToUpdate => valuesToUpdateBinding;
        public ReadOnlyObservableCollection<GmdEntryDiffViewModel> ChangeSet => changeSetBinding;

        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly ReadOnlyObservableCollection<StringKeyValuePair> valuesToUpdateBinding;
        private readonly ReadOnlyObservableCollection<GmdEntryDiffViewModel> changeSetBinding;
        private readonly SourceCache<StringKeyValuePair, string> valuesToUpdate = new SourceCache<StringKeyValuePair, string>(x => x.Key);
        private readonly SourceCache<GmdEntryDiffViewModel, string> changeSet = new SourceCache<GmdEntryDiffViewModel, string>(x => x.Entry.Key!);
        private static readonly ILogger log = Log.ForContext<GmdChangeSetViewModel>();

        public void SetNewValues(IEnumerable<StringKeyValuePair> values)
        {
            valuesToUpdate.Edit(x => x.Load(values));
            UpdateDiffEntries();
        }

        private void UpdateDiffEntries()
        {
            // TODO: This function might be pretty slow. Maybe run on background thread?
            if (SourceGmd == null)
            {
                log.Warning("Can't execute 'PopulateImportEntries' when 'SourceGmd' is null.");
                return;
            }

            var sw = Stopwatch.StartNew();

            changeSet.Edit(lst =>
            {
                lst.Clear();

                // Not sure what this code does, but it seems to work...
                var entries = SourceGmd.Connect().AsObservableList().Items.ToList();

                foreach (var entry in entries.Where(x => x.Key != null))
                {
                    // TODO: Allow user to deselect CsvEntry (to not import that entry)
                    var newValue = ValuesToUpdate.FirstOrDefault(x => x.Key == entry.Key);

                    if (newValue != null)
                    {
                        var diff = new GmdEntryDiffViewModel(entry, newValue.Value);

                        if (!HideUnchangedEntries || diff.HasChanges)
                            lst.AddOrUpdate(new GmdEntryDiffViewModel(entry, newValue.Value));
                    }
                }
            });

            sw.Stop();

            log.Verbose("PopulateImportEntries took {@ElapsedMilliseconds} ms.", sw.ElapsedMilliseconds);
        }

        private async Task ApplyChangeSetAndSave()
        {
            if (SourceGmd == null) return;

            int updatedEntriesCount = 0;

            foreach (var entry in ChangeSet)
            {
                if (entry.ApplyChanges())
                    updatedEntriesCount++;
            }

            if (updatedEntriesCount > 0)
            {
                await mainWindowViewModel.SaveFilesCommand.Execute(new[] { SourceGmd });
                mainWindowViewModel.ShowFlashAlert("Updated entries", $"Changed {updatedEntriesCount} entries in {SourceGmd.Info.FullName}");
            }
            else
            {
                mainWindowViewModel.ShowFlashAlert("No entries changed", $"No entries have been modified in {SourceGmd.Info.FullName}");
            }

            log.Information($"Applied {updatedEntriesCount} changes to '{SourceGmd.Info.FullName}'");
        }
    }
}
