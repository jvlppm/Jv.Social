using Jv.Json;
using Jv.Web.OAuth;
using Jv.Web.OAuth.Extensions;
using Jv.Web.OAuth.v1;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Jv.Social.Google.Orkut
{
    public sealed class OrkutClient
    {
        #region Properties
        public OAuthClient OAuthClient { get; private set; }
        #endregion

        #region Login
        public OrkutClient(OAuthClient oAuthClient)
        {
            if (oAuthClient == null)
                throw new System.ArgumentNullException("oAuthClient");

            OAuthClient = oAuthClient;
        }

        public OrkutClient(KeyPair applicationInfo, KeyPair token)
            : this(new OAuthClient(applicationInfo, token))
        {
        }

        public static async Task<OrkutClient> Login(KeyPair applicationInfo)
        {
            try
            {
                var login = new GoogleLogin(applicationInfo, "http://orkut.gmodules.com/social");
                var oAuthClient = await login.Login();

                return new OrkutClient(oAuthClient);
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Response.GetResponseString());
            }
        }
        #endregion

        #region Core
        public async Task Ajax(IEnumerable<IRpc> rpcs)
        {
            object idLock = new object();
            int id = 0;
            var rpcsDic = rpcs.ToDictionary(i => { lock (idLock) return ("request_" + id++).ToString(); });
            var rpcsIds = rpcsDic.Keys.ToList();

            var parameters = new HttpParameters {
                    { "request", rpcsDic.Select(rpc => new {
                                    id = rpc.Key,
                                    method = rpc.Value.Method,
                                    @params = rpc.Value.Parameters.Fields
                                }).ToJson() }
                };

            //TODO: Mudar id para dentro RPC (evitar colisao em nome de arquivos)
            parameters.AddRange(rpcsDic.Values.SelectMany(r => r.Parameters.Files));

            var data = await OAuthClient.Ajax("http://www.orkut.com/social/rpc",
                type: "POST",
                data: parameters);

            foreach (var result in data)
            {
                var rid = ((string)result.id);
                if (result.data != null)
                    rpcsDic[rid].SetResult(result.data);
                if (result.error != null)
                    rpcsDic[rid].SetError(result.error);
                rpcsIds.Remove(rid);
            }

            foreach (var rid in rpcsIds)
                rpcsDic[rid].SetResult(null);
        }

        public async Task Ajax(IRpc rpc)
        {
            try
            {
                await Ajax(new[] { rpc });
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<T> Ajax<T>(IRpc<T> rpc) where T : DynamicWrapper
        {
            await Ajax((IRpc)rpc);
            return await rpc.Task;
        }
        #endregion

        #region Public API
        public Task<Person> CurrentUser()
        {
            return Ajax(new PersonGet(UserIds.Me));
        }
        #endregion
    }
}
