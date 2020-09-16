using Nito.AsyncEx;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Cirilla.Avalonia.ViewModels
{
    public enum FlashMessageButtons
    {
        Ok,
        YesNoCancel
    }

    public enum FlashMessageResult
    {
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
                FlashMessageButtons.Ok => new[] { new FlashMessageButtonViewModel("Ok", FlashMessageResult.Ok) },
                FlashMessageButtons.YesNoCancel => new[] {
                    new FlashMessageButtonViewModel("Yes", FlashMessageResult.Yes),
                    new FlashMessageButtonViewModel("No", FlashMessageResult.No),
                    new FlashMessageButtonViewModel("Cancel", FlashMessageResult.Cancel)
                },
                _ => throw new NotSupportedException("Unsupported FlashMessageButtons value!")
            };

            ButtonClickCommand = ReactiveCommand.Create<FlashMessageResult>(ButtonClick);
        }

        public string Title { get; }
        public string Message { get; }
        public FlashMessageButtonViewModel[] Buttons { get; }

        [Reactive] public bool IsVisible { get; set; }
        [Reactive] public FlashMessageResult Result { get; private set; }

        public ReactiveCommand<FlashMessageResult, Unit> ButtonClickCommand { get; }

        private AsyncManualResetEvent resetEvent = new AsyncManualResetEvent();

        private void ButtonClick(FlashMessageResult result)
        {
            Result = result;
            resetEvent.Set();
        }

        public async Task<FlashMessageResult> ShowPopup()
        {
            IsVisible = true;
            resetEvent.Reset();
            await resetEvent.WaitAsync();
            IsVisible = false;
            return Result;
        }
    }
}
