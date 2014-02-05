using Jv.Social.Google;
using Jv.Social.Google.Orkut;
using Jv.Social.Google.Plus;
using Jv.Social.Twitter;
using Jv.Web.OAuth;
using Jv.Web.OAuth.Authentication;
using Jv.Web.OAuth.Json;
using Jv.Web.OAuth.v1;
using Jv.Web.OAuth.v2;
using Jv.Web.OAuth.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            KeyPair googleAppInfo = new KeyPair(
                key: "176102147108.apps.googleusercontent.com",
                secret: "glE2FgAVRf_VEl9etaOCfuDK");

            KeyPair twitterAppInfo = new KeyPair(
                key: "vS4UjhTai41mgJteeC3eQ",
                secret: "I2ATAsnTxrFC9UpTlRDuYKareUznje35vnoZXcayq4");

            try
            {
                using (var authenticator = new WinFormsAuthenticator())
                {
                    //var gPlus = await GooglePlusClient.Login(googleAppInfo, authenticator);
                    var gPlus = new GooglePlusClient(new Jv.Web.OAuth.v2.OAuthClient(googleAppInfo, new BearerAccessToken("ya29.1.AADtN_VCY-BgfkcuIag5FEQ_2n1BL8YPd2Q7uGM76wJxI8Z8BoNRcATsR2Emcc55nt1q", null, "https://www.googleapis.com/auth/plus.login", "1/UNEHbS6OTPxYZXNZiwwy0g9sXET1qb7EPUuG09HiWJY")));

                    var user = await gPlus.Ajax("people/me", HttpMethod.Get);

                    Console.WriteLine(gPlus.OAuthClient.AccessToken);

                    /*var orkutClient = await OrkutClient.Login(googleAppInfo, authenticator);
                    Console.WriteLine("Orkut: " + orkutClient.OAuthClient.AccessToken);
                    //var user = await orkutClient.GetCurrentUser();

                    var friends = await orkutClient.GetFriends(UserIds.Me, 0, 20);

                    //Console.WriteLine("Orkut: {0} {1}", user.Name.GivenName, user.Name.FamilyName);

                    //var twitterClient = await TwitterClient.Login(twitterAppInfo, authenticator);
                    //Console.WriteLine("Twitter: " + twitterClient.OAuthClient.AccessToken);*/
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
