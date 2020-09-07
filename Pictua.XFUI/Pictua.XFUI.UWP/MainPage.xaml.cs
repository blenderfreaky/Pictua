using FFImageLoading.Forms.Platform;

namespace Pictua.XFUI.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            //var redirectURI = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

            LoadApplication(new XFUI.App("https://sso/"));
            Pictua.XFUI.App.AuthParentWindow = this;
            CachedImageRenderer.Init();
        }
    }
}
