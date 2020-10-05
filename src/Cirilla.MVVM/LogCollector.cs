using Cirilla.MVVM.ViewModels;
using DynamicData;
using ReactiveUI;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Cirilla.MVVM.Services
{
    public class LogCollector : ILogEventSink
    {
        public ReadOnlyObservableCollection<LogEventViewModel> Events => eventsBinding;

        private readonly ReadOnlyObservableCollection<LogEventViewModel> eventsBinding;
        private readonly SourceList<LogEventViewModel> eventsList = new SourceList<LogEventViewModel>();

        public LogCollector()
        {
            eventsList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out eventsBinding)
                .Subscribe();
        }

        public void Emit(LogEvent logEvent)
        {
            eventsList.Add(new LogEventViewModel(logEvent));
        }
    }
}
