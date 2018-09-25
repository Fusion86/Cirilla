using System;
using System.ComponentModel;

namespace Cirilla
{
    [Serializable]
    public class CirillaConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string WorkingDirectoryPath { get; set; }
        public string ExtractedFilesPath { get; set; }
        public bool UnsafeModusEnabled { get; set; }
    }
}
