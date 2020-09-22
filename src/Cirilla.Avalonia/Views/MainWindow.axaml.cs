using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Cirilla.Avalonia.ViewModels;
using System;

namespace Cirilla.Avalonia.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private TreeView treeFileBrowser => this.FindControl<TreeView>("treeFileBrowser");

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            treeFileBrowser.AddHandler(DoubleTappedEvent, OnFileBrowserDoubleTapped);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void MenuExit_Click(object? sender, RoutedEventArgs e)
        {
            // TODO: Check for unsaved changes
            Close();
        }

        private void OnFileBrowserDoubleTapped(object? sender, RoutedEventArgs e)
        {
            ViewModel.OpenSelectedFileBrowserItemsCommand.Execute().Subscribe();
        }
    }
}
