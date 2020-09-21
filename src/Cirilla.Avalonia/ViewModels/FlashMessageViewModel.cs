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

        public string Title { get; }
        public string Message { get; }
        public FlashMessageButtonViewModel[]? Buttons { get; }

        [Reactive] public bool IsVisible { get; private set; }
        [Reactive] public FlashMessageResult Result { get; private set; }

        public ReactiveCommand<FlashMessageResult, Unit> ButtonClickCommand { get; }

        private AsyncManualResetEvent resetEvent = new AsyncManualResetEvent();

        public void Show()
        {
            IsVisible = true;
        }

        /// <summary>
        /// Show alert and wait until a result is available.
        /// </summary>
        /// <returns></returns>
        public async Task<FlashMessageResult> ShowAsync()
        {
            IsVisible = true;
            resetEvent.Reset();
            await resetEvent.WaitAsync();
            return Result;
        }

        public void Close(FlashMessageResult result = FlashMessageResult.None)
        {
            Result = result;
            IsVisible = false;
            resetEvent.Set();
        }
    }
}
