using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace GMDEditor
{
    // Hacky logging stuff
    class InMemorySink : ILogEventSink
    {
        private readonly ITextFormatter _textFormatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{Exception}", CultureInfo.InvariantCulture);
        private readonly TextBox _textBox = null;

        public InMemorySink(TextBox textBox)
        {
            _textBox = textBox;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);
            _textBox.AppendText(renderSpace.ToString() + Environment.NewLine);
        }
    }
}
