using System.Collections.Generic;

namespace Cirilla.MVVM.Common
{
    public class FileDialogFilter
    {
        public FileDialogFilter(string name, List<string>? extensions = null)
        {
            Name = name;
            Extensions = extensions ?? new List<string>();
        }

        public string Name { get; set; }
        public List<string> Extensions { get; set; }
    }
}
