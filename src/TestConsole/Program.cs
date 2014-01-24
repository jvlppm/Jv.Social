using Jv.Social.Google;
using Jv.Social.Google.Orkut;
using Jv.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            KeyPair appInfo = new KeyPair(
                key: "176102147108.apps.googleusercontent.com",
                secret: "glE2FgAVRf_VEl9etaOCfuDK");

            var orkutClient = OrkutClient.Login(appInfo).Result;
            Console.WriteLine(orkutClient);
        }
    }
}
