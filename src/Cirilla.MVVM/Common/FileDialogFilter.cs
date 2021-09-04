using System.Collections.Generic;

namespace Cirilla.MVVM.Common
{
    public class FileDialogFilter
    {
        public FileDialogFilter(string name, IList<string>? extensions = null)
        {
            Name = name;
            Extensions = extensions ?? new string[0];
        }

        public string Name { get; set; }
        public IList<string> Extensions { get; set; }

        public readonly static FileDialogFilter AllFiles = new FileDialogFilter("All Files", new[] { "*" });
        public readonly static FileDialogFilter GMD = new FileDialogFilter("Game Message Data", new[] { "gmd" });
        public readonly static FileDialogFilter CSV = new FileDialogFilter("CSV UTF-8 (Comma Delimited)", new[] { "csv" });
        public readonly static FileDialogFilter SaveData = new FileDialogFilter("SAVEDATA1000", new[] { "" });
    }
}
