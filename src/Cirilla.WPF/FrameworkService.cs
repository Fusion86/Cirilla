using Cirilla.MVVM.Common;
using Cirilla.MVVM.Interfaces;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<string[]> OpenFileDialog(bool allowMultiple = false, IList<FileDialogFilter>? filters = null, string? initialDirectory = null)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = allowMultiple,
                Filter = GetFilter(filters)
            };

            if (initialDirectory != null)
                ofd.InitialDirectory = initialDirectory;

            if (ofd.ShowDialog() == true)
                return Task.FromResult(ofd.FileNames);

            return Task.FromResult(Array.Empty<string>());
        }

        public Task<string?> OpenFolderDialog(string? initialDirectory = null)
        {
            var fbd = new VistaFolderBrowserDialog();

            if (initialDirectory != null)
                fbd.SelectedPath = initialDirectory;

            if (fbd.ShowDialog() == true && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                return Task.FromResult<string?>(fbd.SelectedPath);
            return Task.FromResult<string?>(null);
        }

        public Task<string?> SaveFileDialog(string? defaultName = null, string? extension = null, IList<FileDialogFilter>? filters = null)
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

        private static string GetFilter(IList<FileDialogFilter>? filters)
        {
            if (filters == null) return "";

            var filterStrList = new List<string>();

            foreach (var filter in filters)
            {
                var extList = filter.Extensions.Select(x => $"*.{x}").ToList();
                string extText = string.Join(", ", extList);
                string extFilter = string.Join(';', extList);
                string filterStr = $"{filter.Name} ({extText})|{extFilter}";
                filterStrList.Add(filterStr);
            }

            return string.Join("|", filterStrList);
        }
    }
}