namespace Cirilla.Core.Models
{
    public abstract class FileTypeBase
    {
        public string _path;

        public FileTypeBase(string path)
        {
            _path = path;
        }
    }
}
