using System;
using Windows.Security.Authentication.Web;

namespace Pictua.XFUI.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            var redirectURI = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

            LoadApplication(new XFUI.App(redirectURI.AbsoluteUri));
        }
    }
}
