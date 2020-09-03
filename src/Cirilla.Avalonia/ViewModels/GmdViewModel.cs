using Cirilla.Core.Models;
using System.Collections.Generic;
using System.IO;

namespace Cirilla.Avalonia.ViewModels
{
    public class GmdViewModel : ViewModelBase, IFileViewModel
    {
        public GmdViewModel(FileInfo fileInfo)
        {
            Info = fileInfo;
            gmd = new GMD(fileInfo.FullName);

            for (int i = 0; i < gmd.Entries.Count; i++)
                Entries.Add(new GmdEntryViewModel(i, gmd.Entries[i]));
        }

        public FileInfo Info { get; private set; }
        public bool CanClose => true;
        public bool CanSave => true;

        public List<GmdEntryViewModel> Entries { get; } = new List<GmdEntryViewModel>();

        private readonly GMD gmd;

        public bool Close()
        {
            return true;
        }

        public void Save()
        {

        }
    }
}
