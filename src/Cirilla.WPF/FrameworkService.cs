using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using Microsoft.Win32;
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string[]> OpenFileDialog(bool allowMultiple = false, List<FileDialogFilter>? filters = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = allowMultiple,
                Filter = GetFilter(filters)
            };

            if (ofd.ShowDialog() == true)
                return ofd.FileNames;

            return new string[0];
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string?> SaveFileDialog(string? defaultName = null, string? extension = null, List<FileDialogFilter>? filters = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = defaultName,
                DefaultExt = extension,
                Filter = GetFilter(filters)
            };

            if (sfd.ShowDialog() == true)
                return sfd.FileName;
            return null;
        }

        private string GetFilter(List<FileDialogFilter>? filters)
        {
            // TODO: Implement this
            return "";
        }
    }
}