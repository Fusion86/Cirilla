using Cirilla.Avalonia.Models;
using DynamicData;
using ReactiveUI;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Cirilla.Avalonia.Services
{
    public class LogCollector : ILogEventSink
    {
        public ReadOnlyObservableCollection<LogEventModel> Events => eventsBinding;

        private readonly ReadOnlyObservableCollection<LogEventModel> eventsBinding;
        private readonly SourceList<LogEventModel> eventsList = new SourceList<LogEventModel>();

        public LogCollector()
        {
            eventsList.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out eventsBinding)
                .Subscribe();
        }

        public void Emit(LogEvent logEvent)
        {
            eventsList.Add(new LogEventModel(logEvent));
        }
    }
}
