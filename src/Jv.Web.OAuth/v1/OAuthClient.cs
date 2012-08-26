﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Jv.Web.OAuth.Extensions;
using System.Net;

namespace Jv.Web.OAuth.v1
{
    public class OAuthClient
    {
        #region Properties
        public KeyPair ApplicationInfo { get; private set; }
        public KeyPair Token { get; set; }
        #endregion

        #region Constructors
        public OAuthClient(KeyPair applicationInfo, KeyPair token = null)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");

            ApplicationInfo = applicationInfo;
            Token = token;
        }
        #endregion

        #region Private
        public IDictionary<string, string> GetOauthParameters(string requestType, string url, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            parameters = parameters ?? Enumerable.Empty<KeyValuePair<string, string>>();

            var oauthParameters = new Dictionary<string, string>();

            oauthParameters["oauth_version"] = "1.0";
            oauthParameters["oauth_nonce"] = CreateNonce();
            oauthParameters["oauth_timestamp"] = CreateTimeStamp();

            oauthParameters["oauth_consumer_key"] = ApplicationInfo.Key;

            if (Token != null && !string.IsNullOrEmpty(Token.Key))
                oauthParameters["oauth_token"] = Token.Key;

            oauthParameters["oauth_signature_method"] = "HMAC-SHA1";
            oauthParameters["oauth_signature"] = BuildSignature(requestType, url, oauthParameters.Union(parameters));

            return oauthParameters;
        }

        static Random Random = new Random(Environment.TickCount);

        static string CreateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        static string CreateNonce()
        {
            return Random.Next(123400, 9999999).ToString();
        }

        string BuildSignature(string type, string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return GetSignature(string.Format("{0}&{1}&{2}", type, Uri.EscapeDataString(url), parameters.AsUrlParameters().UriDataEscape()));
        }

        string GetSignature(string signatureBase)
        {
            string consumerSecret = Uri.EscapeDataString(ApplicationInfo.Secret);
            string tokenSecret = string.Empty;
            if (Token != null && Token.Secret != null)
                tokenSecret = Uri.EscapeDataString(Token.Secret);

            IBuffer keyMaterial = (consumerSecret + "&" + tokenSecret).AsBufferUTF8();
            MacAlgorithmProvider hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey macKey = hmacSha1Provider.CreateKey(keyMaterial);
            IBuffer dataToBeSigned = signatureBase.AsBufferUTF8();
            IBuffer signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            return CryptographicBuffer.EncodeToBase64String(signatureBuffer);
        }
        #endregion
    }
}
