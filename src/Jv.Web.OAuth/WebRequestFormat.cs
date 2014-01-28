namespace Jv.Web.OAuth
{
    public enum WebRequestFormat
    {
        /// <summary>
        /// Use parameters and files as multipart/form-data.
        /// </summary>
        MultiPart,
        /// <summary>
        /// Use fields inside the request Url, and files as multipart/form-data.
        /// </summary>
        MixedUrlMultipart
    }
}
