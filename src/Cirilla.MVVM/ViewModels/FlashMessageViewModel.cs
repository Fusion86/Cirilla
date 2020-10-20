using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Cirilla.MVVM.ViewModels
{
    public enum FlashMessageButtons
    {
        None,
        Ok,
        YesNoCancel
    }

    public enum FlashMessageResult
    {
        None,
        Ok,
        Yes,
        No,
        Cancel
    }

    public class FlashMessageButtonViewModel : ViewModelBase
    {
        public FlashMessageButtonViewModel(string content, FlashMessageResult result)
        {
            Content = content;
            Result = result;
        }

        public string Content { get; }
        public FlashMessageResult Result { get; }
    }

    public class FlashMessageViewModel : ViewModelBase
    {
        public FlashMessageViewModel(string title = "", string message = "", FlashMessageButtons buttons = FlashMessageButtons.Ok)
        {
            Title = title;
            Message = message;

            Buttons = buttons switch
            {
                FlashMessageButtons.None => null,
                FlashMessageButtons.Ok => new[] { new FlashMessageButtonViewModel("Ok", FlashMessageResult.Ok) },
                FlashMessageButtons.YesNoCancel => new[] {
                    new FlashMessageButtonViewModel("Yes", FlashMessageResult.Yes),
                    new FlashMessageButtonViewModel("No", FlashMessageResult.No),
                    new FlashMessageButtonViewModel("Cancel", FlashMessageResult.Cancel)
                },
                _ => throw new NotSupportedException("Unsupported FlashMessageButtons value!")
            };

            ButtonClickCommand = ReactiveCommand.Create<FlashMessageResult>(Close);
        }

        public event EventHandler<FlashMessageResult>? OnClose;

        [Reactive] public string Title { get; set; }
        [Reactive] public string Message { get; set; }
        public FlashMessageButtonViewModel[]? Buttons { get; }
        public FlashMessageResult Result { get; private set; }

        public ReactiveCommand<FlashMessageResult, Unit> ButtonClickCommand { get; }

        public void Close(FlashMessageResult result = FlashMessageResult.None)
        {
            Result = result;
            OnClose?.Invoke(this, result);
        }

        // Untested
        public Task<FlashMessageResult> WaitForClose()
        {
            var tcs = new TaskCompletionSource<FlashMessageResult>();
            OnClose += (sender, e) => tcs.SetResult(e);
            return tcs.Task;
        }
    }
}
