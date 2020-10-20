namespace Cirilla.MVVM.ViewModels
{
    public interface IFlashMessageContainer
    {
        FlashMessageViewModel ShowFlashAlert(string title = "", string message = "", FlashMessageButtons buttons = FlashMessageButtons.Ok);
    }
}