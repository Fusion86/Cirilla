using Cirilla.MVVM.ViewModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;

namespace Cirilla.MVVM
{
    class ExceptionHandler : IObserver<Exception>
    {
        public ExceptionHandler(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        private readonly MainWindowViewModel mainWindowViewModel;

        public void OnCompleted()
        {
            if (Debugger.IsAttached) Debugger.Break();
            RxApp.MainThreadScheduler.Schedule(() => { throw new NotImplementedException(); });
        }

        public void OnError(Exception error)
        {
            if (Debugger.IsAttached) Debugger.Break();
            mainWindowViewModel.ShowFlashAlert("Unhandled Error", $"Type: {error.GetType()}\nMessage: {error.Message}");
            RxApp.MainThreadScheduler.Schedule(() => { throw error; });
        }

        public void OnNext(Exception value)
        {
            if (Debugger.IsAttached) Debugger.Break();
            mainWindowViewModel.ShowFlashAlert("Unhandled Event", $"Type: {value.GetType()}\nMessage: {value.Message}");
            //RxApp.MainThreadScheduler.Schedule(() => { throw value; });
        }
    }
}
