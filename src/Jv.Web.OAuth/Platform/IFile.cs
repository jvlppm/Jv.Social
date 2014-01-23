using System;
using System.IO;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.Platform
{
    public interface IFile
    {
        string Name { get; }
        Stream Content { get; }
        string ContentType { get; }
    }
}
