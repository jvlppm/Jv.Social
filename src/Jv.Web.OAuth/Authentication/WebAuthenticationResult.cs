using System.Net;
namespace Jv.Web.OAuth.Authentication
{
    public sealed class WebAuthenticationResult
    {
        /// <summary>
        /// Contains the protocol data when the operation successfully completes.
        /// </summary>
        public string ResponseData { get; private set; }

        /// <summary>
        /// Returns the HTTP error code when ResponseStatus is equal to WebAuthenticationStatus.ErrorHttp.
        /// This is only available if there is an error.
        /// </summary>
        public HttpStatusCode ResponseErrorDetail { get; private set; }

        /// <summary>
        /// Contains the status of the asynchronous operation when it completes.
        /// </summary>
        public WebAuthenticationStatus ResponseStatus { get; private set; }

        #region Custom API

        public readonly static WebAuthenticationResult UserCancel;

        static WebAuthenticationResult()
        {
            UserCancel = new WebAuthenticationResult
            {
                ResponseStatus = WebAuthenticationStatus.UserCancel
            };
        }

        public static WebAuthenticationResult FromHttpError(HttpStatusCode code)
        {
            return new WebAuthenticationResult
            {
                ResponseErrorDetail = code,
                ResponseStatus = WebAuthenticationStatus.ErrorHttp
            };
        }

        public static WebAuthenticationResult FromResponseData(string responseData)
        {
            return new WebAuthenticationResult
            {
                ResponseData = responseData,
                ResponseStatus = WebAuthenticationStatus.Success
            };
        }
        #endregion
    }
}