using Jv.Web.OAuth.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class WinFormsAuthorizer : IUserAuthorizer
    {
        Uri _callback;

        public async System.Threading.Tasks.Task<Uri> GetCallback()
        {
            if (_callback == null)
                _callback = new Uri("http://localhost/");
            return _callback;
        }

        public System.Threading.Tasks.Task<UserAuthorizationResult> AuthorizeUser(Uri requestUri)
        {
            throw new TaskCanceledException();
            /*WebBrowser loginBrowser = new WebBrowser { Dock = DockStyle.Fill };
            loginBrowser.Navigate(buildAuthenticationUrl());
            Form window = new Form { Width = 640, Height = 480, Text = "Positivo - Login Orkut", Icon = new System.Drawing.Icon(typeof(Orkut), "orkut_icon.ico") };
            window.Controls.Add(loginBrowser);

            loginBrowser.Navigated += (s, e) =>
            {
                try
                {
                    var result = OrkutRequest.ParseParameters(e.Url.Query);

                    if (result.ContainsKey("oauth_verifier"))
                    {
                        using (var Registry = Orkut.getRegistry())
                        {
                            if ((string)Registry.ReadValue("Token") != (string)result["oauth_token"])
                                throw new Exception(string.Format("Invalid authentication token.\r\nExpected: {0}.\r\nReceived: {1}.", Registry.ReadValue("Token"), result["oauth_token"]));

                            Registry.WriteValue("Verifier", result["oauth_verifier"] as string);
                        }
                        window.DialogResult = DialogResult.OK;
                    }
                }
                catch (Exception ex)
                {
                    loginError = ex;
                    window.DialogResult = DialogResult.Abort;
                }
            };

            if (window.ShowDialog() == DialogResult.Abort && loginError != null)
                throw loginError;*/
        }

        public void Dispose()
        {
        }
    }
}
