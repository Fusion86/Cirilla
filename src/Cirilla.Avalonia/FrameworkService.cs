using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Cirilla.MVVM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirilla.Avalonia
{
    class FrameworkService : IFrameworkService
    {
        public async Task<string[]> OpenFileDialog(bool allowMultiple = false, List<MVVM.Common.FileDialogFilter>? filters = null)
        {
            OpenFileDialog ofd = new OpenFileDialog { AllowMultiple = allowMultiple };
            ofd.Filters = GetFilters(filters).ToList();
            return await ofd.ShowAsync(GetMainWindow());
        }

        public async Task<string?> SaveFileDialog(string? defaultName = null, string? extension = null, List<MVVM.Common.FileDialogFilter>? filters = null)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialFileName = defaultName,
                DefaultExtension = extension
            };
            sfd.Filters = GetFilters(filters).ToList();
            return await sfd.ShowAsync(GetMainWindow());
        }

        public object? FindResource(string key)
        {
            return Application.Current.FindResource(key);
        }

        private Window GetMainWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                return lifetime.MainWindow;
            }

            throw new PlatformNotSupportedException("No MainWindow found for this platform.");
        }

        private IEnumerable<FileDialogFilter> GetFilters(List<MVVM.Common.FileDialogFilter>? filters)
        {
            if (filters == null) yield break;

            foreach (var filter in filters)
            {
                yield return new FileDialogFilter
                {
                    Name = filter.Name,
                    Extensions = filter.Extensions
                };
            }
        }
    }
}
