using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using System;

namespace Cirilla.Core.Enums
{
    public class MHFileType : Enumeration
    {
        private static int incr = 0;

        public readonly Type Handler;
        public readonly string[] Magics;
        public readonly string[] FileExtensions;

        public static MHFileType GMD = new MHFileType(typeof(GMD), "Text files.", magics: new[] { "GMD" });
        public static MHFileType ITM = new MHFileType(typeof(ITM), "Text files.", fileExtensions: new[] { ".itm" });

        public MHFileType() { }

        public MHFileType(Type handler, string desc, string[] magics = null, string[] fileExtensions = null) : base(incr++, nameof(handler))
        {
            Handler = handler;
            Magics = magics;
            FileExtensions = fileExtensions;
        }
    }
}
