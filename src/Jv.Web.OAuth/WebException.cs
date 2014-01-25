using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth
{
    public class WebException : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; private set; }
        public dynamic ResponseData { get; private set; }

        public WebException(HttpStatusCode statusCode, dynamic responseData)
            : base(responseData is string? (string)responseData : null)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
        }

        public WebException(HttpStatusCode statusCode, dynamic responseData, string message)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
        }

        public WebException(HttpStatusCode statusCode, dynamic responseData, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseData = responseData;
        }
    }
}
