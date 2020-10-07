using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Cirilla.WPF
{
    internal class FrameworkService : IFrameworkService
    {
        public object? FindResource(string key)
        {
            return Application.Current.FindResource(key);
        }

        public Task<string[]> OpenFileDialog(bool allowMultiple = false, List<FileDialogFilter>? filters = null)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = allowMultiple,
                Filter = GetFilter(filters)
            };

            if (ofd.ShowDialog() == true)
                return Task.FromResult(ofd.FileNames);

            return Task.FromResult(new string[0]);
        }

        public Task<string?> OpenFolderDialog()
        {
            var fbd = new VistaFolderBrowserDialog();
            if (fbd.ShowDialog() == true && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                return Task.FromResult<string?>(fbd.SelectedPath);
            return Task.FromResult<string?>(null);
        }

        public Task<string?> SaveFileDialog(string? defaultName = null, string? extension = null, List<FileDialogFilter>? filters = null)
        {
            var sfd = new SaveFileDialog
            {
                FileName = defaultName,
                DefaultExt = extension,
                Filter = GetFilter(filters)
            };

            if (sfd.ShowDialog() == true)
                return Task.FromResult(sfd.FileName);
            return Task.FromResult<string?>(null);
        }

        private string GetFilter(List<FileDialogFilter>? filters)
        {
            // TODO: Implement this
            return "";
        }
    }
}