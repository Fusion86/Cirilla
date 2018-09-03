namespace Cirilla.ViewModels
{
    public abstract class FileTypeTabItemViewModelBase
    {
        public abstract string Title { get; }

        public virtual void Save() { }
        public virtual bool CanSave() => true;

        public virtual void Close() { }
        public virtual bool CanClose() => true;
    }
}
