using Cirilla.Core.Models;
using Cirilla.Steam;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShoutoutReset.WPF
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OpenSaveDataCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveSaveDataCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectAllNoneShoutoutsCommand { get; }
        public ReactiveCommand<IList<ShoutoutViewModel>, Unit> ResetSelectedShoutouts { get; }

        [Reactive] public SaveSlotViewModel? SelectedSaveSlot { get; set; }

        public ObservableCollection<SaveSlotViewModel> SaveSlots { get; } = new();

        // Needs to be reactive for the `hasSaveData` observable.
        [Reactive] private SaveData? SaveData { get; set; }

        private List<SteamAccount>? steamUsersWithMhw;

        public MainWindowViewModel()
        {
            var hasSaveData = this.WhenAnyValue(x => x.SaveData).Select(x => x != null);
            var hasSaveSlotSelected = this.WhenAnyValue(x => x.SelectedSaveSlot).Select(x => x != null);

            OpenSaveDataCommand = ReactiveCommand.Create(OpenSaveData);
            SaveSaveDataCommand = ReactiveCommand.Create(SaveSaveData, hasSaveData);

            // This command is handled in the MainWindow code-behind, because we can't change the ListBox selection from the ViewModel.
            SelectAllNoneShoutoutsCommand = ReactiveCommand.Create(() => { }, hasSaveSlotSelected);

            ResetSelectedShoutouts = ReactiveCommand.Create<IList<ShoutoutViewModel>>(ResetShoutouts);

            _ = Task.Run(() => steamUsersWithMhw = SteamUtility.GetSteamUsersWithMhw());
        }

        private void OpenSaveData()
        {
            // Copied from MHWSaveTransfer/MainWindowViewModel.OpenSaveData() with small tweaks.

            var ofd = new OpenFileDialog()
            {
                Filter = "SAVEDATA1000|SAVEDATA1000|All files (*.*)|*.*"
            };

            SteamAccount? steamAccount = null;

            if (steamUsersWithMhw != null)
            {
                if (steamUsersWithMhw.Count > 1)
                {
                    // TODO: Ask user to select a steam account.
                    steamAccount = steamUsersWithMhw[0];
                }
                else if (steamUsersWithMhw.Count == 1)
                {
                    steamAccount = steamUsersWithMhw[0];
                }

                if (steamAccount != null)
                {
                    string? saveDir = SteamUtility.GetMhwSaveDir(steamUsersWithMhw[0]);
                    if (saveDir != null)
                        ofd.InitialDirectory = saveDir;
                }
            }

            if (ofd.ShowDialog() == true)
            {
                SaveSlots.Clear();
                SaveData = null;

                try
                {
                    SaveData = new SaveData(ofd.FileName);

                    foreach (var saveSlot in SaveData.SaveSlots)
                        SaveSlots.Add(new SaveSlotViewModel(saveSlot));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Couldn't open " + Path.GetFileName(ofd.FileName), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveSaveData()
        {
            if (SaveData == null) return;

            var sfd = new SaveFileDialog()
            {
                FileName = "SAVEDATA1000",
                Filter = "SAVEDATA1000|SAVEDATA1000|All files (*.*)|*.*"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    SaveData.Save(sfd.FileName);
                    MessageBox.Show("Done!", "Saved file");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Couldn't save to " + Path.GetFileName(sfd.FileName), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ResetShoutouts(IList<ShoutoutViewModel> shoutouts)
        {
            foreach (var shoutout in shoutouts)
                shoutout.Reset();

            MessageBox.Show($"Done! Reset {shoutouts.Count} shoutouts to their default value.", "Reset shoutouts");
        }
    }
}
