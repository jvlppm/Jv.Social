using Jv.Web.OAuth;
using Jv.Web.OAuth.Authentication;
using Jv.Web.OAuth.Json;
using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Jv.Social.Google.Orkut
{
    public class OrkutClient
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

        public static async Task<OrkutClient> Login(KeyPair applicationInfo, IWebAuthenticator authenticator)
        {
            var login = new GoogleLogin(applicationInfo, new Uri("http://orkut.gmodules.com/social"));
            var oAuthClient = await login.Login(authenticator);

            return new OrkutClient(oAuthClient);
        }
        #endregion

        #region Core
        public async Task Ajax(IEnumerable<IRpc> rpcs)
        {
            int id = 0;
            var rpcsDic = rpcs.ToDictionary(i => { return ("request_" + id++).ToString(); });
            if (rpcsDic.Count == 0)
                return;

            var parameters = new HttpParameters {
                { "request", rpcsDic.Select(rpc => new {
                    id = rpc.Key,
                    method = rpc.Value.Method,
                    @params = rpc.Value.Parameters.Fields
                }).ToJson() }
            };
            parameters.AddRange(rpcsDic.Values.SelectMany(r => r.Parameters.Files));

            var data = await OAuthClient.Ajax("http://www.orkut.com/social/rpc",
                method: "POST",
                parameters: parameters);

            foreach (var result in data)
            {
                var rid = ((string)result.id);
                if (result.data != null)
                    rpcsDic[rid].SetResult(result.data);
                if (result.error != null)
                    rpcsDic[rid].SetError((HttpStatusCode)result.error.code, result);
                rpcsDic.Remove(rid);
            }

            foreach (var rid in rpcsDic.Values)
                rid.SetResult(null);

            await Task.WhenAll(rpcs.Select(r => r.Task));
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

        public async Task<T> Ajax<T>(IRpc<T> rpc)
        {
            await Ajax((IRpc)rpc);
            return await rpc.Task;
        }
        #endregion

        #region API
        public Task<Person> GetCurrentUser()
        {
            return Ajax(new PersonGet(UserIds.Me));
        }

        public Task<Page<Person>> GetFriends(string userId, int startIndex, int count)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");

            return Ajax(new PeopleGet(userId, GroupIds.Friends, count, startIndex));
        }
        #endregion
    }
}
