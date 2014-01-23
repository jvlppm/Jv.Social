using System;
using System.IO;

namespace Jv.Web.OAuth
{
    public class File
    {
        public string FileName { get; set; }
        public Stream ContentStream { get; set; }
        public string ContentType { get; set; }
    }
}
