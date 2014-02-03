﻿using Jv.Web.OAuth.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Web.OAuth.v2
{
    public class OAuthClient : WebClient
    {
        public KeyPair ApplicationInfo { get; private set; }
        public OAuthAccessToken AccessToken { get; private set; }

        public OAuthClient(KeyPair applicationInfo,
                                OAuthAccessToken accessToken,
                                HttpClient httpClient = null)
            : base(httpClient)
        {
            if (applicationInfo == null)
                throw new ArgumentNullException("applicationInfo");
            if (accessToken == null)
                throw new ArgumentNullException("accessToken");
            

            ApplicationInfo = applicationInfo;
            AccessToken = accessToken;
        }

        protected override HttpRequestMessage CreateRequest(Uri url, HttpMethod httpMethod, HttpParameters parameters, WebRequestFormat requestFormat)
        {
            var signedParams = AccessToken.Sign(ApplicationInfo, url, httpMethod, parameters);
            return base.CreateRequest(url, httpMethod, parameters, requestFormat);
        }
    }
}
