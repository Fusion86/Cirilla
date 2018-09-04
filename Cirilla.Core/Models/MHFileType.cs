using Cirilla.Core.Helpers;
using System;

namespace Cirilla.Core.Models
{
    public class MHFileType : Enumeration
    {
        private static int incr = 0;

        public readonly string[] Magics;
        public readonly Type Handler;

        public static MHFileType GMD = new MHFileType(typeof(GMD), "Text files.", "GMD");

        public MHFileType() { }

        public MHFileType(Type handler, string desc, params string[] magics) : base(incr++, nameof(handler))
        {
            Handler = handler;
            Magics = magics;
        }
    }
}
