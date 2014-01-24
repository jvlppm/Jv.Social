﻿using Jv.Social.Google;
using Jv.Social.Google.Orkut;
using Jv.Web.OAuth;
using Jv.Web.OAuth.Json;
using Jv.Web.OAuth.v1;
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
            IoC.Register<IUserAuthorizer>(() => new WinFormsAuthorizer());

            KeyPair appInfo = new KeyPair(
                key: "176102147108.apps.googleusercontent.com",
                secret: "glE2FgAVRf_VEl9etaOCfuDK");
            try
            {
                var orkutClient = OrkutClient.Login(appInfo).Result;
                Console.WriteLine(orkutClient);
            }
            catch(WebException ex)
            {
                Console.WriteLine(((object)ex.ResponseData).ToJson());
            }
            catch(AggregateException ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
