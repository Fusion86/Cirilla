﻿using Serilog.Events;
using System;

namespace Cirilla.MVVM.Common
{
    public class LogEventModel
    {
        public DateTimeOffset Timestamp { get; }
        public LogEventLevel Level { get; }
        public string Message { get; }
        public string? MessageDetails { get; }
        public string? SourceContext { get; }
        public string? ShortSourceContext { get; }

        public LogEventModel(LogEvent logEvent)
        {
            Timestamp = logEvent.Timestamp;
            Level = logEvent.Level;
            Message = logEvent.RenderMessage();
            MessageDetails = logEvent.Exception?.ToString();

            if (logEvent.Properties.TryGetValue("SourceContext", out var prop) && prop is ScalarValue sourceContext)
            {
                SourceContext = sourceContext.Value.ToString();
                ShortSourceContext = SourceContext?.Substring(SourceContext.LastIndexOf('.') + 1)
                    .Replace("ViewModel", "").Replace("Service", "");
            }
        }
    }
}
