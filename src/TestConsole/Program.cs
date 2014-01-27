using Jv.Social.Google;
using Jv.Social.Google.Orkut;
using Jv.Social.Twitter;
using Jv.Web.OAuth;
using Jv.Web.OAuth.Authentication;
using Jv.Web.OAuth.Json;
using Jv.Web.OAuth.v1;
using Jv.Web.OAuth.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //IoC.Register<IWebAuthenticator>(() => new WinFormsAuthenticator());
            //IoC.Register<WinFormsAuthenticator>().As<IWebAuthenticator>();

            Test().Wait();
        }

        private static async Task Test()
        {
            KeyPair orkutAppInfo = new KeyPair(
                key: "176102147108.apps.googleusercontent.com",
                secret: "glE2FgAVRf_VEl9etaOCfuDK");

            KeyPair twitterAppInfo = new KeyPair(
                key: "vS4UjhTai41mgJteeC3eQ",
                secret: "I2ATAsnTxrFC9UpTlRDuYKareUznje35vnoZXcayq4");

            try
            {
                using (var authenticator = new WinFormsAuthenticator())
                {
                    var orkutClient = await OrkutClient.Login(orkutAppInfo, authenticator);
                    Console.WriteLine("Orkut: " + orkutClient.OAuthClient.AccessToken);

                    //var twitterClient = await TwitterClient.Login(twitterAppInfo, authenticator);
                    //Console.WriteLine("Twitter: " + twitterClient.OAuthClient.AccessToken);
                }
            }
            catch (Jv.Web.OAuth.WebException ex)
            {
                Console.WriteLine(((object)ex.ResponseData).ToJson());
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
