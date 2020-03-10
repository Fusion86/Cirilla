namespace Cirilla.Core.Models
{
    public abstract class FileTypeBase
    {
        public string Filepath { get; private set; }

        public FileTypeBase(string path)
        {
            Filepath = path;
        }
    }
}
