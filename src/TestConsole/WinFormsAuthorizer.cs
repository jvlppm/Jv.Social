using Jv.Web.OAuth.v1;
using Jv.Web.OAuth.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            var tcs = new TaskCompletionSource<UserAuthorizationResult>();

			var t = new Thread((ThreadStart)delegate
			{
				// TODO: Create a Dialog/Window class.
				var backWin = new Form
				{
					BackColor = global::System.Drawing.Color.Black,
					Opacity = 0.55,
					WindowState = FormWindowState.Maximized,
					FormBorderStyle = FormBorderStyle.None,
					TopMost = true
				};

				var win = new Form
				{
					FormBorderStyle = FormBorderStyle.None,
					TopMost = true
				};

				bool ignoreWindowSizeChange = false;
				EventHandler winSizeChanged = delegate
				{
					if (ignoreWindowSizeChange)
						return;
					win.Top = (backWin.Height - win.Height) / 2;
					win.Left = backWin.Left;
				};

				win.SizeChanged += winSizeChanged;
				backWin.SizeChanged += delegate
				{
					ignoreWindowSizeChange = true;
					win.Width = backWin.Width;
					win.Height = backWin.Height - 230;
					ignoreWindowSizeChange = false;
					winSizeChanged(backWin, EventArgs.Empty);
				};

				var headerPanel = new Panel { Width = 566, Height = 80 };
				var bodyPanel = new Panel
				{
					Top = 80,
					BackColor = global::System.Drawing.Color.White,
				};

				/*var backButton = new BackButton
				{
					Location = new global::System.Drawing.Point(0, 35),
					TabStop = false
				};
				headerPanel.Controls.Add(backButton);
				backButton.Click += (s, e) => win.Close();*/

				var lbl = new Label
				{
					Location = new global::System.Drawing.Point(35, 30),
					Font = new global::System.Drawing.Font("Segoe UI", 19.5f),
					Text = "Connecting to a service",//WinRT.NET.Forms.Properties.Resources.AuthenticationBrokerTitle,
					AutoSize = true
				};
				headerPanel.Controls.Add(lbl);
				win.Controls.Add(headerPanel);
				win.Controls.Add(bodyPanel);

				var browser = new WebBrowser { Width = 566 };
				win.SizeChanged += delegate
				{
					bodyPanel.Width = win.Width;
					bodyPanel.Height = win.Height - bodyPanel.Top;
					browser.Height = win.Height - 80;
					browser.Left = (win.Width - browser.Width) / 2;
					headerPanel.Left = browser.Left;
				};

				browser.PreviewKeyDown += (sender, e) =>
				{
					if (e.KeyCode == Keys.Escape)
						win.DialogResult = DialogResult.Abort;
				};
				bodyPanel.Controls.Add(browser);
				win.Deactivate += (sender, e) =>
				{
					if (win.CanFocus)
						win.DialogResult = DialogResult.Abort;
				};

				browser.Navigated += (sender, e) =>
				{
					if (e.Url.GetLeftPart(UriPartial.Path).EndsWith(_callback.ToString()))
					{
                        dynamic authData = e.Url.ToString().ParseUrlParameters().ToExpandoObject();

						tcs.TrySetResult(new UserAuthorizationResult(authData.oauth_token, authData.oauth_verifier));
						win.DialogResult = DialogResult.OK;
					}
				};

				browser.Navigate(requestUri);

				backWin.Show();
				win.ShowDialog();
				backWin.Close();

                tcs.TrySetCanceled();
			});

			t.SetApartmentState(ApartmentState.STA);
			t.Start();

			return tcs.Task;
        }

        public void Dispose()
        {
        }
    }
}
