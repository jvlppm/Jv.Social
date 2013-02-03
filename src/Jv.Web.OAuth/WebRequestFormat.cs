using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth
{
    public enum WebRequestFormat
    {
        /// <summary>
        /// Use parameters and files as multipart/form-data.
        /// </summary>
        MultiPart,
        /// <summary>
        /// Send parameters and files as an application/x-www-form-urlencoded content type.
        /// </summary>
        //UrlEncoded,

        /// <summary>
        /// Use fields inside the request Url, and files as multipart/form-data.
        /// </summary>
        MixedUrlMultipart
    }
}
